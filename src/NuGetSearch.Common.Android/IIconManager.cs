using System;
using System.Collections.Generic;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Icon Manager Interface
	/// </summary>
	/// <typeparam name="T">The platform-specific icon type</typeparam>
	public interface IIconManager<T> : IDisposable where T : class
	{
		void Load(IEnumerable<string> urlsToLoad, Action<string> callback);
		
		void Load(string urlToLoad, Action<string> callback);
		
		bool IsLoaded(string url);
		
		T GetIcon(string url);
	}
}
