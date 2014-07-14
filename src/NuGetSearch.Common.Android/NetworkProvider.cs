using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Network provider encapsulates all network access
	/// </summary>
	public class NetworkProvider : INetworkProvider
	{	
		/// <summary>
		/// Asynchronously retrieves a stream from the specified URL
		/// </summary>
		/// <returns>stream</returns>
		/// <param name="url">URL</param>
		public async Task<Stream> GetStreamAsync(string url)
		{
			System.Diagnostics.Debug.WriteLine(url);
			Stream content;
			using (var http = new HttpClient())
			{
				content = await http.GetStreamAsync(url).ConfigureAwait(false);
			}
			
			System.Diagnostics.Debug.WriteLine(string.Concat(url, " (Stream)"));
			return content;
		}

		/// <summary>
		/// Asynchronously retrieves XML from the specified URL
		/// </summary>
		/// <returns>XML</returns>
		/// <param name="url">URL</param>
		public async Task<XDocument> GetXmlAsync(string url)
		{
			System.Diagnostics.Debug.WriteLine(url);
			string xmlContent;
			using (var http = new HttpClient())
			{
				xmlContent = await http.GetStringAsync(url).ConfigureAwait(false);
			}
			
			System.Diagnostics.Debug.WriteLine(xmlContent);
			return XDocument.Parse(xmlContent);
		}
		
		/// <summary>
		/// Asynchronously retrieves a string from the specified URL
		/// </summary>
		/// <returns>string</returns>
		/// <param name="url">URL</param>
		public async Task<string> GetStringAsync(string url)
		{
			System.Diagnostics.Debug.WriteLine(url);
			string content;
			using (var http = new HttpClient())
			{
				content = await http.GetStringAsync(url).ConfigureAwait(false);
			}
			
			System.Diagnostics.Debug.WriteLine(content);
			return content;
		}
	}
}
