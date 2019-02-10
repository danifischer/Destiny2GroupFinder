namespace WpfGroupFinder.Models
{
	public class Languages
	{
		public Languages(string languageLong, string languageShort)
		{
			LanguageLong = languageLong;
			LanguageShort = languageShort;
		}

		public string LanguageLong { get; }
		public string LanguageShort { get; }
	}
}