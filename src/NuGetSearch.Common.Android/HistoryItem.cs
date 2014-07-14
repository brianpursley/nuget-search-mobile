using System;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Represents a specific version of a package
	/// </summary>
	public class HistoryItem
	{
		public string Id { get; set; }
		
		public string Title { get; set; }
		
		public string DisplayTitle { get; set; }
		
		public string Version { get; set; }
		
		public int VersionDownloadCount { get; set; }
		
		public DateTime Created { get; set; }
		
		public DateTime Published { get; set; }
	}
}
