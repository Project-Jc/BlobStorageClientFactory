using System.Threading.Tasks;

namespace Azure.Storage.BlobStorageClientFactory.Specification
{
	public interface IBlobStorageClientFactory
	{
		Task<IBlobStorageClient> CreateClient(string clientName);
	}
}