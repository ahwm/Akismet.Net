#if NETSTANDARD
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Akismet.Net
{
    public static class ServicesExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="username"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static IServiceCollection AddAkismet(this IServiceCollection services, string key, string applicationName)
        {
            // https://stackoverflow.com/a/79111722/1892993
            services.AddOptions<AkismetClientOptions>()
                .Configure(options => {
                    options.Key = key;
                });
            services.AddHttpClient<AkismetClient>(client =>
            {
                client.BaseAddress = new Uri("https://rest.akismet.com/");
                client.DefaultRequestHeaders.Add("User-Agent", $"{applicationName} | Akismet.NET/{Assembly.GetExecutingAssembly().GetName().Version} (https://github.com/ahwm/Akismet.Net)");
            });

            return services;
        }
    }
}
#endif