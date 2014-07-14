using System;
using System.Text;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Package Dependency
	/// </summary>
	public class PackageDependency
	{
		public PackageDependency(string dependency)
		{
			if (string.IsNullOrEmpty(dependency)) 
			{ 
				return; 
			}
			
			// The dependency string contains the title and version range, deliminated by a colon
			var dependencyParts = dependency.Split(':');
			
			// The first part is the title
			this.Title = dependencyParts[0].Trim();
			
			// The second part is the version range
			this.VersionRange = PackageDependency.FormatVersionRange(dependencyParts[1]);
		}
		
		public string Title { get; private set; }
		
		public string VersionRange { get; private set; }

		/// <summary>
		/// Given a version range string, returns a formatted string suitable for display
		/// </summary>
		/// <returns>The formatted version range</returns>
		/// <param name="versionRange">The unformatted version range</param>
		private static string FormatVersionRange(string versionRange)
		{
			StringBuilder sb = new StringBuilder();

			// Split the unformatted version range into parts deliminated by comma
			var versionRangeParts = versionRange.Trim().Split(',');
			
			// Add the lower bound to the formatted version range
			sb.Append(PackageDependency.FormatVersionLowerBound(versionRangeParts[0]));
			
			// If there is an upper bound, then add the upper bound to the formatted range
			if (versionRangeParts.Length > 1)
			{
				// Add && to separate the lower and upper bounds
				if (sb.Length > 0) 
				{ 
					sb.Append(" && "); 
				}
				
				// Add the upper bound to the formatted version range
				sb.Append(PackageDependency.FormatVersionUpperBound(versionRangeParts[1]));				
			}
			
			return sb.ToString();
		}
		
		/// <summary>
		/// Formats the lower bound for display
		/// </summary>
		/// <returns>Formatted lower bound</returns>
		/// <param name="lowerBound">Unformatted lower bound</param>
		private static string FormatVersionLowerBound(string lowerBound)
		{
			if (lowerBound.StartsWith("[", StringComparison.Ordinal)) 
			{
				// Greater than or equal to
				return string.Format(">={0}", lowerBound.Replace("[", string.Empty).Trim());
			}
			else if (lowerBound.StartsWith("(", StringComparison.Ordinal)) 
			{
				// Greater than
				return string.Format(">{0}", lowerBound.Replace("[", string.Empty).Trim());
			}
			else if (lowerBound.Length > 0) 
			{
				// Greater than or equal to 
				return string.Format(">={0}", lowerBound.Trim());
			}
			else
			{
				return string.Empty;
			}
		}
		
		/// <summary>
		/// Formats the upper bound for display
		/// </summary>
		/// <returns>Formatted upper bound</returns>
		/// <param name="upperBound">Unformatted upper bound</param>
		private static string FormatVersionUpperBound(string upperBound)
		{
			if (upperBound.EndsWith("]", StringComparison.Ordinal))
			{
			 	// Less than or equal to
				return string.Format("<={0}", upperBound.Replace("]", string.Empty).Trim());
			}
			else if (upperBound.EndsWith(")", StringComparison.Ordinal)) 
			{
				// Less than
				return string.Format("<{0}", upperBound.Replace(")", string.Empty).Trim());
			}
			else if (upperBound.Length > 0) 
	        {
				// Less than or equal to
	            return string.Format("<={0}", upperBound.Trim());
	        }
	        else 
	        {
		        return string.Empty;
	        }
		}
    }
}
