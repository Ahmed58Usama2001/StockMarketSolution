using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using ServiceContracts;

namespace Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");


        builder.ConfigureAppConfiguration((context, config) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                config.AddUserSecrets<Program>();
            }
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFinnhubService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockFinnhubService = new Mock<IFinnhubService>();

            mockFinnhubService.Setup(x => x.GetCompanyProfile(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, object>
                {
                    { "ticker", "MSFT" },
                    { "name", "Microsoft" }
                });

            mockFinnhubService.Setup(x => x.GetStockPriceQuote(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, object>
                {
                    { "c", 250.50 }
                });

            services.AddSingleton(mockFinnhubService.Object);
        });

        base.ConfigureWebHost(builder);
    }
}

