using System.IO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace HorsesForCourses.WebApi;

internal class CustomWebAppFactory : WebApplicationFactory<Program>
{
    // Usually not needed, here yes, because of weird setup. 
    // Everything is in one .proj, namespaces and folders messed up ...
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var factory = new CustomWebAppFactory();
        var client = factory.CreateClient();
        var projectDir = Directory.GetCurrentDirectory();   //dit moet nog aangepast worden aan het huidige project!
        builder.UseContentRoot(projectDir);
        return base.CreateHost(builder);
    }
}