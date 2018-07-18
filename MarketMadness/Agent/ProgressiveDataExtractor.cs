using System.Text;

namespace MarketMadness.Agent
{
	public class ProgressiveDataExtractor
	{
		private char[] HTML;
		private int Pos;
		public bool NotFound;

		public ProgressiveDataExtractor(string html) : this(html, null) { }

		public ProgressiveDataExtractor(string html, string startingKeyword)
		{
			HTML = html.ToCharArray();

			if (startingKeyword != null)
				RunForward(startingKeyword);
		}

		public string ExtractString(string label)
		{
			return ExtractString(label, "<td");
		}

		public string ExtractString(string label, string tagStart)
		{
			RunForward(label);

			if (NotFound)
				return null;

			RunForward(tagStart);

			if (NotFound)
				return null;

			RunForward(">");

			if (NotFound)
				return null;

			// sometimes there's other elements to skip
			while (HTML[Pos] == '<')
				RunForward(">");

			int startIdx = Pos;
			RunForward("<");

			if (NotFound)
				return null;

			StringBuilder builder = new StringBuilder();

			for (int i = startIdx; i < Pos - 1; i++)
				builder.Append(HTML[i]);

			string str = builder.ToString();

			str = str.Trim().Replace("\r", "").Replace("\n", "");
			
			return str;
		}

		public long? ExtractLong(string label)
		{
			return ExtractLong(label, "<td");
		}

		public long? ExtractLong(string label, string tagStart)
		{
			string data = ExtractString(label, tagStart);

			if (data == null || data == "N/A" || data == "n/a")
				return null;

			long multiplier = 	HTML[Pos - 2] == 'B' || HTML[Pos - 2] == 'b'  ? 1000000 :
								HTML[Pos - 2] == 'M' || HTML[Pos - 2] == 'm' ? 1000 :
								HTML[Pos - 2] == 'K' || HTML[Pos - 2] == 'k' ? 1 :
								HTML[Pos - 2] == 'T' || HTML[Pos - 2] == 't' ? 1 : 0;
			
			// UGHH - takes the whole point of char arrays out of the mix!
			data = data.Replace("B", "").Replace("M", "").Replace("K", "").
						Replace("b", "").Replace("m", "").Replace("k", "").
						Replace("$", "").Replace("%", "").Trim();

			try
			{
				decimal decValue = decimal.Parse(data);

				if (multiplier == 0)
					return (long)decValue;

				long longValue = (long)(decValue * 1000);

				return longValue * multiplier;
			}
			catch
			{
				return null;
			}
		}

		public int? ExtractInt(string label)
		{
			return ExtractInt(label, "<td");
		}

		public int? ExtractInt(string label, string tagStart)
		{
			string data = ExtractString(label, tagStart);

			if (data == null || data == "N/A" || data == "n/a")
				return null;

			data = data.Replace("B", "000M").Replace("M", "000K").Replace("K", "000");
			data = data.Replace("b", "000m").Replace("m", "000k").Replace("k", "000");
			data = data.Replace(",", "").Replace("$", "").Replace("%", "").Trim();

			int? intValue = null;

			try
			{
				intValue = int.Parse(data);
			}
			catch
			{
			}

			return intValue;
		}

		public decimal? ExtractDecimal(string label)
		{
			return ExtractDecimal(label, "<td");
		}

		public decimal? ExtractDecimal(string label, string tagStart)
		{
			string data = ExtractString(label, tagStart);

			if (data == null || data == "N/A" || data == "n/a")
				return null;

			data = data.Replace("B", "000M").Replace("M", "000K").Replace("K", "000");
			data = data.Replace("b", "000M").Replace("m", "000K").Replace("k", "000");
			data = data.Replace("$", "").Replace("%", "");

			decimal? decValue = null;

			try
			{
				decValue = decimal.Parse(data);
			}
			catch
			{
			}

			return decValue;
		}

		private void RunForward(string phrase)
		{
			NotFound = false;
			int startPos = Pos;

			char[] phraseChars = phrase.ToCharArray();
			int phrasePos = 0;

			while (Pos < HTML.Length)
			{
				if (HTML[Pos++] == phraseChars[phrasePos])
				{
					phrasePos++;

					if (phrasePos == phraseChars.Length)
						return;
				}
				else
					phrasePos = 0;
			}

			if (Pos == HTML.Length)
			{
				Pos = startPos;
				NotFound = true;
			}
		}
	}
}
