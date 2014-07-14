using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGetSearch.Common
{
	/// <summary>
	/// NuGet Gallery Service Interface
	/// </summary>
	public interface INuGetGalleryClient
	{
		Task<NuGetStatistics> GetNuGetStatisticsAsync();
		
		Task<SearchResult> SearchAsync(string searchTerm, string orderBy, bool includePrerelease, int skipCount, int rowCount, bool includeCount);
		
		Task<PackageDetail> GetPackageDetailAsync(string packageUrl);
		
		Task<string> GetPackageLatestIdAsync(string title);
		
		Task<IEnumerable<HistoryItem>> GetPackageHistoryAsync(string title);
	}
}
