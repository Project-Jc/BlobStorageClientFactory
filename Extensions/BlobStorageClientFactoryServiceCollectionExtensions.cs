
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

		public static IServiceCollection AddBlobStorageClient(this IServiceCollection services, Action<BlobStorageClientFactoryOptions> factoryImplementation, Action<string, BlobStorageClientOptions> clientImplementation)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			if (factoryImplementation == null)
				throw new ArgumentNullException(nameof(factoryImplementation));

			if (clientImplementation == null)
				throw new ArgumentNullException(nameof(clientImplementation));

			var factoryOptions = new BlobStorageClientFactoryOptions();

			factoryImplementation(factoryOptions);

			var clientOption = new BlobStorageClientOptions();
			var clientName = string.Empty;

			clientImplementation(clientName, clientOption);

			factoryOptions.ClientOptions.Add(clientName, clientOption);

			services.AddSingleton(factoryOptions);

			services.AddScoped<IBlobStorageClientFactory, Concretions.BlobStorageClientFactory>();

			return services;
		}
	}
}