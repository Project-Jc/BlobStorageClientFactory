namespace Azure.Storage.BlobStorageClientFactory.Options
{
	public class BlobStorageClientOptions
	{
		public string TenantId { get; set; }

		public string AppId { get; set; }

		public string ClientSecret { get; set; }

		public string StorageAccount { get; set; }

		public string StorageEndPoint { get; set; }

		public string DefaultContainer { get; set; }

		public string ConnectionString { get; set; }
	}
}