using System;
using System.Collections.Generic;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Package Detail
	/// </summary>
	public class PackageDetail
	{
		private string iconUrl;
	
		public string Id { get; set; }
		
		public string Title { get; set; }
		
		public string DisplayTitle { get; set; }
		
		public string Version { get; set; }
		
		public bool IsPrerelease 
        {
            get
            {
                return this.Version.Contains("-");
            }
        }
		
		public string Description { get; set; }
		
		public int DownloadCount { get; set; }
		
		public int VersionDownloadCount { get; set; }
		
		public DateTime Created { get; set; }
		
		public DateTime Published { get; set; }
		
		public string ProjectUrl { get; set; }
		
		public string LicenseUrl { get; set; }
		
		public IList<string> Authors { get; set; }
		
		public IList<string> Tags { get; set; }
		
		public IList<PackageDependency> Dependencies { get; set; }
		
		public bool UseDefaultIcon { get; private set; }
		
		public string IconUrl 
		{ 
			get 
			{ 
				return this.iconUrl; 
			}
			
			set 
			{
				this.iconUrl = value;
				this.UseDefaultIcon = string.IsNullOrEmpty(this.iconUrl)
			        || this.iconUrl.Equals("https://nuget.org/Content/Images/packageDefaultIcon-50x50.png")
			        || this.iconUrl.Equals("http://icon_url_here_or_delete_this_line/");
			}
		}
	}
}
