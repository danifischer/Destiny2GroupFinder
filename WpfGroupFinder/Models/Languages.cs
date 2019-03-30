namespace WpfGroupFinder.Models
{
	public class Languages
	{
		public Languages(string languageLong, string languageShort, string languageCode)
		{
			LanguageLong = languageLong;
			LanguageShort = languageShort;
			LanguageCode = languageCode;
		}

		public string LanguageLong { get; }
		public string LanguageShort { get; }
		public string LanguageCode { get; }
	}
}