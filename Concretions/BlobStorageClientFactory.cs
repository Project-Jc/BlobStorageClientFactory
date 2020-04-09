
using Azure.Storage.BlobStorageClientFactory.Options;
using Azure.Storage.BlobStorageClientFactory.Specification;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Azure.Storage.BlobStorageClientFactory.Concretions
{
	public class BlobStorageClientFactory : IBlobStorageClientFactory
	{
		private readonly IDictionary<string, BlobStorageClientOptions> ClientOptions;

		private static readonly IDictionary<string, IBlobStorageClient> ClientCache;

		private readonly BlobStorageClientFactoryOptions FactoryOptions;

		static BlobStorageClientFactory()
		{
			ClientCache = new Dictionary<string, IBlobStorageClient>(StringComparer.OrdinalIgnoreCase);
		}

		public BlobStorageClientFactory(BlobStorageClientFactoryOptions clientFactoryOptions)
		{
			FactoryOptions = clientFactoryOptions;

			ClientOptions = new Dictionary<string, BlobStorageClientOptions>(FactoryOptions.ClientOptions, StringComparer.OrdinalIgnoreCase);
		}

		public async Task<IBlobStorageClient> CreateClient(string clientName)
		{
			if (string.IsNullOrEmpty(clientName))
				throw new ArgumentNullException(nameof(clientName));

			var client = GetCachedClient(clientName);

			if (client != null)
				return client;

			return await TryCreateClient(clientName);
		}

		private async Task<IBlobStorageClient> TryCreateClient(string clientName)
		{
			if (ClientOptions.TryGetValue(clientName, out var clientOptions))
			{
				// Favour creating a client via a connection string
				if (!string.IsNullOrEmpty(clientOptions.ConnectionString))
				{
					return CreateClientFromConnectionString(clientName, clientOptions);
				}
				else
				{
					return await CreateClientFromOAuth(clientName, clientOptions);
				}
			}

			return null;
		}

		private async Task<IBlobStorageClient> CreateClientFromOAuth(string clientName, BlobStorageClientOptions clientOptions)
		{
			string accessToken = await GetUserOAuthToken(clientOptions.TenantId, clientOptions.AppId, clientOptions.ClientSecret);

			var tokenCredential = new TokenCredential(accessToken);

			var storageCredentials = new StorageCredentials(tokenCredential);

			var cloudStorageAccount = new CloudStorageAccount(storageCredentials, clientOptions.StorageAccount, clientOptions.StorageEndPoint, useHttps: true);

			var client = new BlobStorageClient(cloudStorageAccount.BlobStorageUri, storageCredentials, clientOptions);

			CacheClient(clientName, client);

			return client;
		}

		private IBlobStorageClient CreateClientFromConnectionString(string clientName, BlobStorageClientOptions clientOptions)
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(clientOptions.ConnectionString);

			var client = new BlobStorageClient(cloudStorageAccount.BlobStorageUri, cloudStorageAccount.Credentials, clientOptions);

			CacheClient(clientName, client);

			return client;
		}

		private async Task<string> GetUserOAuthToken(string tenantId, string applicationId, string clientSecret)
		{
			string authority = string.Format(CultureInfo.InvariantCulture, FactoryOptions.AuthInstance, tenantId);

			var authContext = new AuthenticationContext(authority);

			var clientCredentials = new ClientCredential(applicationId, clientSecret);

			var result = await authContext.AcquireTokenAsync(FactoryOptions.ResourceId, clientCredentials);

			return result.AccessToken;
		}

		private IBlobStorageClient GetCachedClient(string clientName)
		{
			if (FactoryOptions.UseClientCaching && ClientCache.TryGetValue(clientName, out var client))
				return client;
			return null;
		}

		private void CacheClient(string clientName, IBlobStorageClient client)
		{
			if (FactoryOptions.UseClientCaching)
			{
				ClientCache.Add(clientName, client);
			}
		}
	}
}