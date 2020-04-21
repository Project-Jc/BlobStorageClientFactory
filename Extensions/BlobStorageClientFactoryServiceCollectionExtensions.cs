
using Azure.Storage.BlobStorageClientFactory.Options;
using Azure.Storage.BlobStorageClientFactory.Specification;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Azure.Storage.BlobStorageClientFactory.Extensions
{
	public static class BlobStorageClientFactoryServiceCollectionExtensions
	{
		public static IServiceCollection AddBlobStorageClient(this IServiceCollection services, IConfiguration configuration)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var factoryOptions = configuration.GetSection("BlobStorageClientFactoryOptions").Get<BlobStorageClientFactoryOptions>();

			services.AddSingleton(factoryOptions);

			services.AddScoped<IBlobStorageClientFactory, Concretions.BlobStorageClientFactory>();

			return services;
		}

		public static IServiceCollection AddBlobStorageClient(this IServiceCollection services, Action<BlobStorageClientFactoryOptions> configureFactory, Action<BlobStorageClientOptions> configureClient)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			if (configureFactory == null)
				throw new ArgumentNullException(nameof(configureFactory));

			if (configureClient == null)
				throw new ArgumentNullException(nameof(configureClient));

			var factoryOptions = new BlobStorageClientFactoryOptions();

			configureFactory(factoryOptions);

			var clientOption = new BlobStorageClientOptions();

			configureClient(clientOption);

			factoryOptions.ClientOptions.Add(clientOption.Name, clientOption);

			services.AddSingleton(factoryOptions);

			services.AddScoped<IBlobStorageClientFactory, Concretions.BlobStorageClientFactory>();

			return services;
		}
	}
}