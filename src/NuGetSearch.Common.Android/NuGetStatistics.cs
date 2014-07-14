namespace NuGetSearch.Common
{
	/// <summary>
	/// NuGet Statistics (Package and Download Counts)
	/// </summary>
	public class NuGetStatistics
	{
		public string UniquePackages { get; set; }
		
		public string TotalPackages { get; set; }
		
		public string TotalPackageDownloads { get; set; }
	}
}
