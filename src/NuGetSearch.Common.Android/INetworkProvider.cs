using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Network Provider Interface
	/// </summary>
	public interface INetworkProvider
	{
		Task<Stream> GetStreamAsync(string url);
		
		Task<XDocument> GetXmlAsync(string url);
		
		Task<string> GetStringAsync(string url);
	}
}
