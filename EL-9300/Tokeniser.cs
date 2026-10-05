using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Sharp.EL9300 {

	public static class Tokeniser {

		static readonly Dictionary<byte, string> OneByteTokens;
		static readonly SortedDictionary<string, byte[]>[] AllTokens;

		static readonly ProgramMode[] TokenProgramModes;

		class TokeniserTokenSorter : IComparer<string> {
			public int Compare(string x, string y) {
				if (x.Length != y.Length) {
					return y.Length.CompareTo(x.Length);
				} else {
					return string.CompareOrdinal(x, y);
				}
			}
		}

		const int ProgramModeCount = 5;

		static int ProgramModeIndex(ProgramMode mode) {
			for (int i = 0, m = 1; i < ProgramModeCount; ++i, m <<= 1) {
				if (mode == (ProgramMode)m) return i;
			}
			throw new InvalidEnumArgumentException();
		}

		public static bool CanUseTokenInMode(ProgramMode mode, byte token) {
			// Check the mode
			ProgramModeIndex(mode);
			return (TokenProgramModes[token] & mode) != 0;
		}

		static Tokeniser() {

			// Load tokens
			TokenProgramModes = new ProgramMode[256];
			OneByteTokens = new Dictionary<byte, string>();
			AllTokens = new SortedDictionary<string, byte[]>[ProgramModeCount];
			for (int i = 0; i < AllTokens.Length; ++i) {
				AllTokens[i] = new SortedDictionary<string, byte[]>(new TokeniserTokenSorter());
			}

			var tokensXml = new XmlDocument();
			tokensXml.LoadXml(Properties.Resources.Tokens);
			foreach (XmlNode tokenNode in tokensXml.GetElementsByTagName("token")) {

				var key = tokenNode.Attributes["value"].Value;
				var value = tokenNode.InnerText;

				byte numKey = byte.Parse(key, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				OneByteTokens.Add(numKey, value);
				
				var modes = (ProgramMode)0x1F;
				var modesAttribute = tokenNode.Attributes["mode"];
				if (modesAttribute != null) {
					modes = (ProgramMode)byte.Parse(modesAttribute.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				}
				TokenProgramModes[numKey] = modes;

				for (int i = 0, m = 1; i < ProgramModeCount; ++i, m <<= 1) {
					if (((int)modes & m) != 0) {
						if (!AllTokens[i].ContainsKey(value)) {
							AllTokens[i].Add(value, new byte[] { numKey });
						}
					}
				}
				

			}

			// Convert plain text tokens to symbol text tokens for matching
			for (var i = 0; i < ProgramModeCount; ++i) {
				var symbolTokens = new Dictionary<string, byte[]>(AllTokens[i].Count / 2);
				foreach (var plainToken in AllTokens[i]) {
					var symbolToken = Symbols.FromPlainText(plainToken.Key);
					if (symbolToken != plainToken.Key && !symbolTokens.ContainsKey(symbolToken)) {
						symbolTokens.Add(symbolToken, plainToken.Value);
					}
				}
				foreach (var symbolToken in symbolTokens) {
					AllTokens[i].Add(symbolToken.Key, symbolToken.Value);
				}
			}

		}

		static string GetString(byte[] data, int index, int length) {

			StringBuilder line = null;

			for (int i = index; i < index + length; ++i) {
				if (line == null) {
					line = new StringBuilder(data.Length);
				}

				if (OneByteTokens.TryGetValue(data[i], out var token)) {
					line.Append(token);
				} else {
					switch (data[i]) {
						case 0xFD:
							line.Append(Environment.NewLine);
							break;
						case 0xFF:
							i = data.Length;
							break;
						default:
							line.Append(string.Format("[{0:X2}]", data[i]));
							break;
					}
				}
			}

			return line.ToString();
		}
		
		public static string GetString(byte[] data, int index) {
			return GetString(data, index, data.Length - index);
		}

		public static string GetString(byte[] data) {
			return GetString(data, 0);
		}
		
		public static byte[] GetBytes(ProgramMode mode, string s) {

			var modeIndex = ProgramModeIndex(mode);

			var result = new List<byte>(s.Length / 2);
			var workingLine = s.Trim() + " ";

			workingLine = workingLine.TrimStart('\\');
			while (workingLine.Trim().Length > 0) {

				var foundMatch = false;

				if (!foundMatch) {
					foreach (var possibleMatch in AllTokens[modeIndex]) {
						if (workingLine.StartsWith(possibleMatch.Key, StringComparison.Ordinal)) {
							foundMatch = true;
							result.AddRange(possibleMatch.Value);
							workingLine = workingLine.Substring(possibleMatch.Key.Length);
							break;
						}
					}
				}

				if (!foundMatch) {
					foreach (var possibleMatch in AllTokens[modeIndex]) {
						if (workingLine.StartsWith(possibleMatch.Key, StringComparison.OrdinalIgnoreCase)) {
							foundMatch = true;
							result.AddRange(possibleMatch.Value);
							workingLine = workingLine.Substring(possibleMatch.Key.Length);
							break;
						}
					}
				}

				if (!foundMatch) {
					throw new InvalidDataException("Could not tokenise line from '" + workingLine + "'.");
				}

				workingLine = workingLine.TrimStart('\\');
			}
			return result.ToArray();
		}
	}
}
