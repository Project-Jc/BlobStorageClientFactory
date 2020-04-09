using Azure.Storage.BlobStorageClientFactory.Extensions;
using Azure.Storage.BlobStorageClientFactory.Options;
using Azure.Storage.BlobStorageClientFactory.Specification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Azure.Storage.BlobStorageClientFactory.Examples
{
	class Program
	{
		public static IConfiguration Configuration { get; set; }

		public static IServiceProvider ServiceProvider { get; set; }

		static async Task Main(string[] args)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var serviceCollection = new ServiceCollection();

			serviceCollection.AddBlobStorageClient(Configuration);

			ServiceProvider = serviceCollection.BuildServiceProvider();

			await UploadBlobAsync();

			//serviceCollection.AddBlobStorageClient(
			//	factory =>
			//	{
			//		factory.UseClientCaching = true;
			//		factory.ResourceId = "https://storage.azure.com";
			//		factory.AuthInstance = "https://login.microsoftonline.com";
			//	},
			//	(clientName, clientOptions) =>
			//	{
			//		clientName = "Example Client 1";
			//		clientOptions.TenantId = "Your tenant id";
			//		clientOptions.AppId = "Your app id";
			//		clientOptions.ClientSecret = "Your client secret";
			//		clientOptions.StorageAccount = "Your storage account name";
			//		clientOptions.StorageEndPoint = "core.windows.net";
			//		clientOptions.DefaultContainer = "Your default container name";
			//	});
		}

		public static async Task UploadBlobAsync()
		{
			var blobStorageClientFactory = ServiceProvider.GetService<IBlobStorageClientFactory>();

			try
			{
				var fileBytes = File.ReadAllBytes(@"C:\Users\Gethino\Pictures\100619_194223.jpg");

				var storageClient = await blobStorageClientFactory.CreateClient("gdblobstorage");

				await storageClient.UploadBlobAsync(fileBytes, "blob name");
			}
			catch (Exception ex)
			{
			}
		}
	}
}
