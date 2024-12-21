#if NETCORE
using Akismet.Net;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Akismet.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string apiKey = Environment.GetEnvironmentVariable("AKISMET_API_KEY").Trim();
            string blogUrl = Environment.GetEnvironmentVariable("AKISMET_API_KEY_URL").Trim();
            services.AddAkismet(apiKey, "Akismet Test Application", blogUrl);
        }
    }
}
#endif