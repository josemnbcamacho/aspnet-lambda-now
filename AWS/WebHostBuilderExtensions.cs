﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting.Server;
using AspNetNow.AWS.Internal;
using Microsoft.AspNetCore.Hosting;

namespace AspNetNow.AWS
{
    /// <summary>
    /// This class is a container for extensions methods to the IWebHostBuilder
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Extension method for configuring API Gateway as the server for an ASP.NET Core application.
        /// This is called instead of UseKestrel. If UseKestrel was called before this it will remove
        /// the service description that was added to the IServiceCollection.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseApiGateway(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                var serviceDescription = services.FirstOrDefault(x => x.ServiceType == typeof(IServer));
                if(serviceDescription != null)
                {
                    // If Api Gateway server has already been added the skip out.
                    if (serviceDescription.ImplementationType == typeof(APIGatewayServer))
                        return;
                    // If there is already an IServer registeried then remove. This is mostly likely caused
                    // by leaving the UseKestrel call.
                    else
                        services.Remove(serviceDescription);
                }

                services.AddSingleton<IServer, APIGatewayServer>();
            });
        }
    }
}
