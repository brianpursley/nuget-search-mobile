using System.Collections.Generic;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Search Result
	/// </summary>
	public class SearchResult
	{
		public int Count { get; set; }
		
		public IEnumerable<SearchResultItem> Items { get; set; }
	}
}
