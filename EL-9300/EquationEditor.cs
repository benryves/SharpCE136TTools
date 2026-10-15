using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class EquationEditor : UserControl {

		private byte[] equation = new byte[] { 0xFF };
		public byte[] Equation {
			get {
				var value = new byte[equation.Length];
				Array.Copy(equation, value, equation.Length);
				return value;
			}
			set {
				if (value.Length < 1) {
					throw new ArgumentException();
				} else {
					equation = new byte[value.Length];
					Array.Copy(value, equation, value.Length);
					Invalidate();
				}
			}
		}

		public EquationEditor() {
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
		}

		private RectangleF Draw(Graphics graphics, PointF origin, ref int index, bool draw) {

			var bounds = new RectangleF(origin, SizeF.Empty);
			var lastBounds = bounds;

			var ex = graphics.MeasureString("X", Font, 0, StringFormat.GenericTypographic);

			while (index < equation.Length) {
				var token = equation[index++];
				switch (token) {
					case 0x3C: // Begin sub-equation
						var beforeEquationIndex = index;
						var beforeEquation = Draw(graphics, origin, ref index, false);
						var nextToken = equation[index++];
						switch (nextToken) {
							case 0xB1: // Fraction
								if (equation[index++] != 0x3C) {
									throw new InvalidOperationException();
								} else {

									// Measure the denominator
									var afterEquationIndex = index;
									var afterEquation = Draw(graphics, origin, ref index, false);

									// What's the total bounding size of the fraction?
									var fractionSize = new SizeF(Math.Max(beforeEquation.Width, afterEquation.Width) + ex.Width * 2, beforeEquation.Height + afterEquation.Height);
									var fractionBounds = new RectangleF(origin.X, origin.Y - beforeEquation.Height + ex.Height / 2, fractionSize.Width, fractionSize.Height);

									// Redraw the numerator
									index = beforeEquationIndex;
									Draw(graphics, new PointF(origin.X + (fractionSize.Width - beforeEquation.Width) / 2, origin.Y - beforeEquation.Height / 2), ref index, draw);

									// Redraw the denominator
									index = afterEquationIndex;
									Draw(graphics, new PointF(origin.X + (fractionSize.Width - afterEquation.Width) / 2, origin.Y + afterEquation.Height / 2), ref index, draw);

									// Draw the fraction line
									if (draw) {
										graphics.DrawLine(Pens.Black, origin.X + ex.Width / 4, origin.Y + ex.Height / 2, origin.X + fractionSize.Width - ex.Width / 2, origin.Y + ex.Height / 2);
									}

									// Modify the total bounds and advance the origin
									lastBounds = fractionBounds;
									bounds = RectangleF.Union(bounds, fractionBounds);
									origin.X += fractionSize.Width;
								}
								break;
							case 0x8D: // Root
								if (equation[index++] != 0x3C) {
									throw new InvalidOperationException();
								} else {

									// Measure the root's contents
									var afterEquationIndex = index;
									var afterEquation = Draw(graphics, new PointF(origin.X + beforeEquation.Width + ex.Width / 2, origin.Y), ref index, false);

									// What's the total bounding size of the root?
									var rootSize = new SizeF(beforeEquation.Width + ex.Width + afterEquation.Width, Math.Max(beforeEquation.Height, afterEquation.Height) + ex.Height / 2);
									var rootBounds = new RectangleF(origin.X, Math.Min(beforeEquation.Top, afterEquation.Top) - ex.Height / 4, rootSize.Width, rootSize.Height);

									if (draw) {
										graphics.DrawLines(Pens.Black, new[] {
											new PointF(afterEquation.Left - ex.Width / 2, afterEquation.Bottom - ex.Width / 4),
											new PointF(afterEquation.Left - ex.Width / 4, afterEquation.Bottom),
											new PointF(afterEquation.Left - ex.Width / 4, afterEquation.Y),
											new PointF(afterEquation.Right, afterEquation.Y),
										});
									}

									// Redraw before the root
									index = beforeEquationIndex;
									Draw(graphics, new PointF(origin.X, origin.Y), ref index, draw);

									// Redraw after the root
									index = afterEquationIndex;
									Draw(graphics, new PointF(origin.X + beforeEquation.Width + ex.Width / 2, origin.Y), ref index, draw);

									// Modify the total bounds and advance the origin
									lastBounds = rootBounds;
									bounds = RectangleF.Union(bounds, rootBounds);
									origin.X += rootBounds.Width;
								}
								break;
							default:
								throw new NotImplementedException();
						}
						break;
					case 0x3D: // End sub-equation
						goto finished;
					case 0xFF: // End of equation
						goto finished;
					case 0x89: // ^power
						if (equation[index] != 0x3C) {
							goto default;
						} else {
							// Moved into the subexpression
							++index;

							// Measure the power argument
							var powerEquationIndex = index;
							var powerBounds = Draw(graphics, origin, ref index, false);

							// Align the bottom edge of the power with the top edge of the previous bounds, plus an overlap of half an x-height.
							var powerOverlap = ex.Height / 2;
							var powerOffset = (lastBounds.Top - powerBounds.Bottom) + powerOverlap;
							powerBounds.Y += powerOffset;

							// Reset and draw
							index = powerEquationIndex;
							Draw(graphics, new PointF(origin.X, origin.Y + powerOffset), ref index, draw);

							// Advance bounds
							lastBounds = powerBounds;
							bounds = RectangleF.Union(bounds, powerBounds);
							origin.X += powerBounds.Width;

						}
						break;
					case 0xA1: // root
						if (equation[index] != 0x3C) {
							goto default;
						} else {
							// Moved into the subexpression
							++index;

							// Measure the abs argument
							var rootEquationIndex = index;
							var rootBounds = Draw(graphics, origin, ref index, false);

							// Allow an extra ex width for the root symbol
							rootBounds.Width += ex.Width;
							origin.X += ex.Width / 2;

							// Reset and draw
							index = rootEquationIndex;
							Draw(graphics, origin, ref index, draw);

							// Draw lines around expression
							if (draw) {
								graphics.DrawLine(Pens.Black, rootBounds.Left, rootBounds.Bottom - ex.Width / 4, rootBounds.Left + ex.Width / 4, rootBounds.Bottom);
								graphics.DrawLine(Pens.Black, rootBounds.Left + ex.Width / 4, rootBounds.Top, rootBounds.Left + ex.Width / 4, rootBounds.Bottom);
								graphics.DrawLine(Pens.Black, rootBounds.Left + ex.Width / 4, rootBounds.Top, rootBounds.Right - ex.Width / 4, rootBounds.Top);
							}

							// Advance bounds
							lastBounds = rootBounds;
							bounds = RectangleF.Union(bounds, rootBounds);
							origin.X += rootBounds.Width;
						}
						break;
					case 0xA3: // |abs|
						if (equation[index] != 0x3C) {
							goto default;
						} else {
							// Moved into the subexpression
							++index;

							// Measure the abs argument
							var absEquationIndex = index;
							var absBounds = Draw(graphics, origin, ref index, false);

							// Allow an extra ex width for |lines| either side
							absBounds.Width += ex.Width;
							origin.X += ex.Width / 2;

							// Reset and draw
							index = absEquationIndex;
							Draw(graphics, origin, ref index, draw);

							// Draw |lines| around expression
							if (draw) {
								graphics.DrawLine(Pens.Black, absBounds.Left + ex.Width / 4, absBounds.Top, absBounds.Left + ex.Width / 4, absBounds.Bottom);
								graphics.DrawLine(Pens.Black, absBounds.Right - ex.Width / 4, absBounds.Top, absBounds.Right - ex.Width / 4, absBounds.Bottom);
							}

							// Advance bounds
							lastBounds = absBounds;
							bounds = RectangleF.Union(bounds, absBounds);
							origin.X += absBounds.Width;
						}
						break;
					default:

						var detokenised = Symbols.FromPlainText(Tokeniser.GetString(equation, index - 1, 1));
						var detokenisedSize = graphics.MeasureString(detokenised, Font, 0, StringFormat.GenericTypographic);
						var detokenisedBounds = new RectangleF(origin, detokenisedSize);

						if (draw) {
							graphics.DrawString(detokenised, Font, Brushes.Black, origin, StringFormat.GenericTypographic);
						}

						lastBounds = detokenisedBounds;
						bounds = RectangleF.Union(bounds, detokenisedBounds);
						origin.X += detokenisedSize.Width;
						break;
				}
			}
			finished:;
			return bounds;
		}

		private void Draw(Graphics graphics) {
			
			// Set graphics drawing mode
			graphics.SmoothingMode = SmoothingMode.HighQuality;

			// Initial draw to get bounds
			int index = 0;
			var bounds = Draw(graphics, new PointF(0, 0), ref index, false);
			
			// Reset and redraw for real
			index = 0;
			Draw(graphics, new PointF(-bounds.X, -bounds.Y), ref index, true);
		}


		protected override void OnPaint(PaintEventArgs e) {
			try {
				e.Graphics.Clear(Color.White);
				Draw(e.Graphics);
			} catch {
				e.Graphics.Clear(Color.Red);
			}
		}

	}
}
