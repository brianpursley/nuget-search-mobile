namespace NuGetSearch.Common
{
	/// <summary>
	/// Search Result Item
	/// </summary>
	public class SearchResultItem
	{
		public string Id { get; set; }
		
		public string IconUrl { get; set; }
		
		public string Title { get; set; }
		
		public string DisplayTitle { get; set; }
		
		public string Description { get; set; }
		
		public bool UseDefaultIcon { get; set; }
	}
}
