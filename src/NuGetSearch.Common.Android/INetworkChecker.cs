namespace NuGetSearch.Common
{
	/// <summary>
	/// Network Checker Interface
	/// </summary>
	public interface INetworkChecker
	{
		bool HasNetworkConnectivity();
		
		bool ValidateNetworkConnectivity();
	}	
}
