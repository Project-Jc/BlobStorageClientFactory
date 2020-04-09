using System.Collections.Generic;

namespace Azure.Storage.BlobStorageClientFactory.Options
{
	public class BlobStorageClientFactoryOptions
	{
		public bool UseClientCaching { get; set; }

		public string ResourceId { get; set; }

		public string AuthInstance { get; set; }

		public IDictionary<string, BlobStorageClientOptions> ClientOptions { get; set; }

		public BlobStorageClientFactoryOptions()
		{
			ClientOptions = new Dictionary<string, BlobStorageClientOptions>();
		}
	}
}