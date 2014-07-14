using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuGetSearch.Common
{
	/// <summary>
	/// NuGet Gallery Service is used to query the NuGet Gallery Server
	/// </summary>
	public class NuGetGalleryClient : INuGetGalleryClient
	{
        private const string DefaultNuGetServerUrl = "http://www.nuget.org";
        private static readonly DateTime UnpublishedDateTime = new DateTime(1900, 1, 1, 0, 0, 0);

		private string nugetServerUrl;
		private INetworkProvider networkProvider;
		
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.Common.NuGetGalleryClient"/> class.
        /// </summary>
        /// <param name="networkProvider">Network provider</param>
        public NuGetGalleryClient(INetworkProvider networkProvider) 
            : this(DefaultNuGetServerUrl, networkProvider)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="NuGetSearch.Common.NuGetGalleryClient"/> class.
		/// </summary>
		/// <param name="nugetServerUrl">NuGet server URL</param>
		/// <param name="networkProvider">Network provider</param>
		public NuGetGalleryClient(string nugetServerUrl, INetworkProvider networkProvider) 
		{
			this.nugetServerUrl = nugetServerUrl;
			this.networkProvider = new NetworkProvider();
		}
		
		/// <summary>
		/// Retrieves NuGet Stats from the NuGet Gallery Server
		/// </summary>
		/// <returns>The nu get statistics async.</returns>
		public async Task<NuGetStatistics> GetNuGetStatisticsAsync()
		{
			string statsUrl = string.Format("{0}/stats/totals", this.nugetServerUrl);
			var jsonContent = await this.networkProvider.GetStringAsync(statsUrl).ConfigureAwait(false);
			var stats = JsonValue.Parse(jsonContent);
			return new NuGetStatistics 
			{
				TotalPackageDownloads = stats["Downloads"],
				UniquePackages = stats["UniquePackages"],
				TotalPackages = stats["TotalPackages"]
			};
		}

		/// <summary>
		/// Executes a search using the specified parameters
		/// </summary>
		/// <returns>Search Results</returns>
		/// <param name="searchTerm">Search term</param>
		/// <param name="orderBy">Order by</param>
		/// <param name="includePrerelease">If set to <c>true</c> include prerelease</param>
		/// <param name="skipCount">Number of results to skip</param>
		/// <param name="rowCount">Number of results to retrieve</param>
		/// <param name="includeCount">If set to <c>true</c> include count</param>
		public async Task<SearchResult> SearchAsync(string searchTerm, string orderBy, bool includePrerelease, int skipCount, int rowCount, bool includeCount)
		{
			string searchUrl = this.GetSearchUrl(searchTerm, orderBy, includePrerelease, skipCount, rowCount, includeCount);
			XDocument xml = await this.networkProvider.GetXmlAsync(searchUrl).ConfigureAwait(false);
			return new SearchResult
			{
				Count = includeCount ? int.Parse(xml.Descendants(Xmlns.M + "count").Single().Value) : 0,
				Items = xml.Descendants(Xmlns.Atom + "entry")
					.Select(entry => new SearchResultItem 
					{
						Id = entry.Descendants(Xmlns.Atom + "id").First().Value,
						IconUrl = entry.Descendants(Xmlns.D + "IconUrl").First().Value,
						Title = entry.Descendants(Xmlns.Atom + "title").First().Value,
						DisplayTitle = Utility.ChooseNotNullOrEmpty(entry.Descendants(Xmlns.D + "Title").First().Value, entry.Descendants(Xmlns.Atom + "title").First().Value),
						Description = Utility.Ellispize(entry.Descendants(Xmlns.D + "Description").First().Value, 250)
					})
			};
		}

		/// <summary>
		/// Gets the ID of the latest version of a package with the specified title
		/// </summary>
		/// <returns>Package ID</returns>
		/// <param name="title">Package title</param>
		public async Task<string> GetPackageLatestIdAsync(string title)
		{
			string url = string.Format("{0}/api/v2/Packages()?$filter=Id eq '{1}'&$top=1&$orderby=Created desc", this.nugetServerUrl, title.Replace("'", "''"));
			XDocument xml = await this.networkProvider.GetXmlAsync(url).ConfigureAwait(false);
			return xml.Descendants(Xmlns.Atom + "entry").Descendants(Xmlns.Atom + "id").First().Value;
		}

		/// <summary>
		/// Gets the history of all version of a package with the specified title
		/// </summary>
		/// <returns>Package history items</returns>
		/// <param name="title">Package title</param>
		public async Task<IEnumerable<HistoryItem>> GetPackageHistoryAsync(string title)
		{
			string url = string.Format("{0}/api/v2/Packages()?$filter=Id eq '{1}'&$orderby=Created desc", this.nugetServerUrl, title.Replace("'", "''"));
			XDocument xml = await this.networkProvider.GetXmlAsync(url).ConfigureAwait(false);
			return xml.Descendants(Xmlns.Atom + "entry")
                .Where(x => DateTime.Parse(x.Descendants(Xmlns.D + "Published").First().Value) != UnpublishedDateTime)
				.Select(x => new HistoryItem 
				{
					Id = x.Descendants(Xmlns.Atom + "id").First().Value,
					Title = x.Descendants(Xmlns.Atom + "title").First().Value,
					DisplayTitle = Utility.ChooseNotNullOrEmpty(x.Descendants(Xmlns.D + "Title").First().Value, x.Descendants(Xmlns.Atom + "title").First().Value),
					Version = x.Descendants(Xmlns.D + "Version").First().Value,
					VersionDownloadCount = int.Parse(x.Descendants(Xmlns.D + "VersionDownloadCount").First().Value),
					Created = DateTime.Parse(x.Descendants(Xmlns.D + "Created").First().Value),
					Published = DateTime.Parse(x.Descendants(Xmlns.D + "Published").First().Value),
				});
		}
		
		/// <summary>
		/// Retrieves package details
		/// </summary>
		/// <returns>Package details</returns>
		/// <param name="packageUrl">URL</param>
		public async Task<PackageDetail> GetPackageDetailAsync(string packageUrl)
		{
			XDocument xml = await this.networkProvider.GetXmlAsync(packageUrl).ConfigureAwait(false);
			return new PackageDetail
			{
				Id = xml.Descendants(Xmlns.Atom + "id").First().Value,
				IconUrl = xml.Descendants(Xmlns.D + "IconUrl").First().Value,
				Title = xml.Descendants(Xmlns.Atom + "title").First().Value,
				DisplayTitle = Utility.ChooseNotNullOrEmpty(xml.Descendants(Xmlns.D + "Title").First().Value, xml.Descendants(Xmlns.Atom + "title").First().Value),
				Description = xml.Descendants(Xmlns.D + "Description").First().Value,
				Version = xml.Descendants(Xmlns.D + "Version").First().Value,
				DownloadCount = int.Parse(xml.Descendants(Xmlns.D + "DownloadCount").First().Value),
				VersionDownloadCount = int.Parse(xml.Descendants(Xmlns.D + "VersionDownloadCount").First().Value),
				Created = DateTime.Parse(xml.Descendants(Xmlns.D + "Created").First().Value),
				Published = DateTime.Parse(xml.Descendants(Xmlns.D + "Published").First().Value),
				ProjectUrl = xml.Descendants(Xmlns.D + "ProjectUrl").First().Value,
				LicenseUrl = xml.Descendants(Xmlns.D + "LicenseUrl").First().Value,

				Tags = xml.Descendants(Xmlns.D + "Tags").First().Value.Split(' ')
								.Where(x => !string.IsNullOrWhiteSpace(x))
								.ToArray(),
				
				Authors = xml.Descendants(Xmlns.Atom + "author").Descendants(Xmlns.Atom + "name")
								.Select(x => x.Value)
								.Where(x => !string.IsNullOrWhiteSpace(x))
								.ToArray(),
		
				Dependencies = xml.Descendants(Xmlns.D + "Dependencies").First().Value.Split('|')
								.Where(x => !string.IsNullOrWhiteSpace(x))
								.Select(x => new PackageDependency(x))
								.Where(x => !string.IsNullOrWhiteSpace(x.Title))
								.ToArray(),
			};
		}

		/// <summary>
		/// Builds a search URL using the specified parameters
		/// </summary>
		/// <returns>The search URL</returns>
		/// <param name="searchTerm">Search term</param>
		/// <param name="orderBy">Order by</param>
		/// <param name="includePrerelease">If set to <c>true</c> include prerelease</param>
		/// <param name="skipCount">Number of results to skip</param>
		/// <param name="rowCount">Number of results to retrieve</param>
		/// <param name="includeCount">If set to <c>true</c> include count</param>
		private string GetSearchUrl(string searchTerm, string orderBy, bool includePrerelease, int skipCount, int rowCount, bool includeCount)
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendFormat("{0}/api/v2/Packages()?$filter=", this.nugetServerUrl);
			sb.AppendFormat("(substringof('{0}',Id) or substringof('{0}',Description) or substringof('{0}',Title) or substringof('{0}',Tags))", searchTerm.Replace("'", "''"));
			
			if (includePrerelease) 
			{
				sb.Append(" and IsAbsoluteLatestVersion");
			}
			else
			{
				sb.Append(" and IsLatestVersion");
				sb.Append(" and IsPrerelease eq false");
			}
			
			if (skipCount > 0)
			{
				sb.AppendFormat("&$skip={0}", skipCount);
			}
			
			sb.AppendFormat("&$top={0}", rowCount);
			
			if (includeCount)
			{
				sb.Append("&$inlinecount=allpages");
			}
			
			sb.AppendFormat("&$orderby={0}", orderBy);
			
			return sb.ToString();
		}
		
		/// <summary>
		/// Private class used to contain XML Namespace "Constants"
		/// </summary>
		private static class Xmlns
		{
			public static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";
			public static readonly XNamespace M = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
			public static readonly XNamespace D = "http://schemas.microsoft.com/ado/2007/08/dataservices";
		}
	}
}
