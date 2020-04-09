using Azure.Storage.BlobStorageClientFactory.Options;
using Azure.Storage.BlobStorageClientFactory.Specification;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure.Storage.BlobStorageClientFactory.Concretions
{
	public class BlobStorageClient : CloudBlobClient, IBlobStorageClient
	{
		public BlobStorageClientOptions ClientOptions { get; }

		public CloudBlobContainer DefaultContainer { get; set; }

		public BlobStorageClient(StorageUri storageUri, StorageCredentials storageCredentials, BlobStorageClientOptions options)
			: base(storageUri, storageCredentials)
		{
			ClientOptions = options;

			DefaultContainer = GetContainerReference(ClientOptions.DefaultContainer);
		}

		public async Task<IEnumerable<CloudBlob>> SearchBlobsAsync(string query, int? segmentSize = null, int? resultLimit = null)
		{
			BlobContinuationToken continuationToken = null;
			CloudBlob blob;

			var searchResults = new List<CloudBlob>();

			try
			{
				do
				{
					var resultSegment = await DefaultContainer.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, segmentSize, continuationToken, null, null);

					var resultLimitedReached = false;

					foreach (var result in resultSegment.Results)
					{
						blob = result as CloudBlob;

						if (blob.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
						{
							searchResults.Add(blob);

							if (searchResults.Count >= resultLimit)
							{
								resultLimitedReached = true;
								break;
							}
						}
					}

					if (resultLimitedReached)
					{
						break;
					}

					//var segmentSearchResults = resultSegment.Results.Select(s => (s as CloudBlob))
					//	.Where(s => s.Name.Contains(query))
					//	.ToList();

					//searchResults.AddRange(segmentSearchResults);

					// Get the continuation token and loop until it is null.
					continuationToken = resultSegment.ContinuationToken;

				} while (continuationToken != null);

				return searchResults;
			}
			catch (Exception ex)
			{
				// Log
			}

			return null;
		}

		public async Task<bool> SetContainerIfExistsAsync(string containerName)
		{
			var container = GetContainerReference(containerName);

			if (await container.ExistsAsync())
			{
				DefaultContainer = container;

				return true;
			}

			return false;
		}

		public async Task<bool> BlobExistsAsync(string blobName)
		{
			if (string.IsNullOrEmpty(blobName))
				throw new ArgumentNullException(nameof(blobName));

			var blob = GetCloudBlockBlobReference(blobName);

			return await blob.ExistsAsync();
		}

		public async Task UploadBlobAsync(byte[] content, string blobName)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));

			if (blobName == null)
				throw new ArgumentNullException(nameof(blobName));

			var blockBlob = GetCloudBlockBlobReference(blobName);

			await blockBlob.UploadFromByteArrayAsync(content, 0, content.Length);
		}

		public async Task<byte[]> DownloadBlobAsync(string blobName)
		{
			if (blobName == null)
				throw new ArgumentNullException(nameof(blobName));

			var blockBlob = GetCloudBlockBlobReference(blobName);

			await blockBlob.FetchAttributesAsync();

			var fileLength = blockBlob.Properties.Length;

			var byteArray = new byte[fileLength];

			await blockBlob.DownloadToByteArrayAsync(byteArray, 0);

			return byteArray;
		}

		public string GetBlobAbsoluteUrl(string blobName)
		{
			var blockBlob = GetCloudBlockBlobReference(blobName);

			return blockBlob.Uri.ToString();
		}

		public static string ParseBlobNameFromUrl(string url)
		{
			var uri = new Uri(url);

			var cloudBlockBlob = new CloudBlockBlob(uri);

			return cloudBlockBlob.Name;
		}

		public string GetDefaultContainer()
		{
			return DefaultContainer.Name;
		}

		private CloudBlockBlob GetCloudBlockBlobReference(string blobName)
		{
			var blockBlob = DefaultContainer.GetBlockBlobReference(blobName);

			return blockBlob;
		}
	}
}