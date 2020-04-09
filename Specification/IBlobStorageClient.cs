using Microsoft.WindowsAzure.Storage.Blob;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.Storage.BlobStorageClientFactory.Specification
{
	public interface IBlobStorageClient
	{
		Task<IEnumerable<CloudBlob>> SearchBlobsAsync(string query, int? segmentSize = null, int? resultLimit = null);

		Task<bool> SetContainerIfExistsAsync(string containerName);

		Task<bool> BlobExistsAsync(string blobName);

		Task UploadBlobAsync(byte[] content, string blobName);

		Task<byte[]> DownloadBlobAsync(string blobName);

		string GetBlobAbsoluteUrl(string blobName);

		string GetDefaultContainer();
	}
}