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

		public static IBlobStorageClientFactory BlobStorageClientFactory { get; set; }

		static Program()
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
		}

		static async Task Main(string[] args)
		{
			var serviceCollection = new ServiceCollection();

			// Register the factory from a JSON configuration file.
			serviceCollection.AddBlobStorageClient(Configuration);

			// Register the factory with a single named client.
			serviceCollection.AddBlobStorageClient(
				factory =>
				{
					factory.UseClientCaching = true;
					factory.ResourceId = "https://storage.azure.com";
					factory.AuthInstance = "https://login.microsoftonline.com";
				},
				clientOptions =>
				{
					clientOptions.Name = "Example Client 1";
					clientOptions.TenantId = "Your tenant id";
					clientOptions.AppId = "Your app id";
					clientOptions.ClientSecret = "Your client secret";
					clientOptions.StorageAccount = "Your storage account name";
					clientOptions.StorageEndPoint = "core.windows.net";
					clientOptions.DefaultContainer = "Your default container name";
				});

			// Register the factory with a single named client (Use connection string auth)
			serviceCollection.AddBlobStorageClient(
				factory =>
				{
					factory.UseClientCaching = true;
					factory.ResourceId = "https://storage.azure.com";
					factory.AuthInstance = "https://login.microsoftonline.com";
				},
				clientOptions =>
				{
					clientOptions.Name = "gdblobstorage";
					clientOptions.DefaultContainer = "something";
					clientOptions.ConnectionString = "my connection string";
				});

			BlobStorageClientFactory = serviceCollection.BuildServiceProvider().GetRequiredService<IBlobStorageClientFactory>();

			await UploadBlobAsync();

			await DownloadBlobAsync();
		}

		const string filePath = @"C:\Users\Gethino\Pictures\";

		const string fileName = "100619_194223.jpg";

		public static async Task UploadBlobAsync()
		{
			try
			{
				var storageClient = await BlobStorageClientFactory.CreateClient("gdblobstorage");

				var fileBytes = await File.ReadAllBytesAsync($"{filePath}{fileName}");

				await storageClient.UploadBlobAsync(fileBytes, fileName);
			}
			catch (Exception ex)
			{
			}
		}

		public static async Task DownloadBlobAsync()
		{
			try
			{
				var storageClient = await BlobStorageClientFactory.CreateClient("gdblobstorage");

				var fileBytes = await storageClient.DownloadBlobAsync(fileName);

				await File.WriteAllBytesAsync($"{filePath}Downloaded_{fileName}", fileBytes);
			}
			catch (Exception ex)
			{
			}
		}
	}
}
