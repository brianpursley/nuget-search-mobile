namespace NuGetSearch.Common
{
	/// <summary>
	/// Utility - Miscellaneous Helper Functions
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Returns a string that is not null or empty from the two strings that are passed in
		/// </summary>
		/// <returns>If s1 is not null or empty, returns s1, otherwise returns s2</returns>
		/// <param name="s1">First string to check</param>
		/// <param name="s2">Second string to check</param>
		public static string ChooseNotNullOrEmpty(string s1, string s2)
		{
			if (string.IsNullOrEmpty(s1))
			{
				return s2;
			}
			else 
			{
				return s1;
			}
		}
		
		/// <summary>
		/// Truncates the specified string at the specified length if it exceeds that length, and adds ellipses if truncated
		/// </summary>
		/// <returns>The truncated string</returns>
		/// <param name="s">The string to truncate</param>
		/// <param name="length">The length after which the string will be truncated</param>
		public static string Ellispize(string s, int length)
		{
			if (s.Length < length)
			{
				return s;
			}
			else
			{
				return s.Substring(0, length) + "...";
			}
		}
	}
}
