using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sharp.EL9300 {

	public static class Symbols {

		static readonly Dictionary<string, string> PlainToFancy;

		static Symbols() {

			PlainToFancy = new Dictionary<string, string>();
			var symbolsXml = new XmlDocument();
			symbolsXml.LoadXml(Properties.Resources.Symbols);
			foreach (XmlNode symbol in symbolsXml.GetElementsByTagName("symbol")) {
				var key = symbol.Attributes["text"].Value;
				var value = symbol.InnerText;
				PlainToFancy.Add(key, value);
			}

		}

		public static string FromPlainText(string plainText) {
			var result = new StringBuilder(plainText.Length);
			var inSymbol = false;
			var search = new StringBuilder(4);

			foreach (var c in plainText) {
				if (c == '@') {
					if (inSymbol) {
						// Finished a symbol, so look it up and output it if found
						if (PlainToFancy.TryGetValue(search.ToString(), out string symbolText)) {
							result.Append(symbolText);
						} else {
							result.Append(string.Format("@{0}@", search.ToString()));
						}
					} else {
						// Starting a new symbol
						search.Length = 0;
					}
					inSymbol ^= true;
				} else {
					(inSymbol ? search : result).Append(c);
				}
			}
			// If we're in the middle of a symbol, output the search term (minus missing closing @).
			if (inSymbol) {
				result.Append(string.Format("@{0}", search.ToString()));
			}
			return result.ToString();
		}


	}
}
