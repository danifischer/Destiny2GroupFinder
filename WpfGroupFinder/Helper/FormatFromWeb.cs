using System.Text;

namespace WpfGroupFinder.Helper
{
	public static class FormatFromWeb
	{
		public static string Format(string input)
		{
			if (input == null)
				return input;

			input = input.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
			byte[] bytes = Encoding.GetEncoding(1252).GetBytes(input);
			return HttpUtility.HtmlDecode(Encoding.UTF8.GetString(bytes));
		}
	}
}