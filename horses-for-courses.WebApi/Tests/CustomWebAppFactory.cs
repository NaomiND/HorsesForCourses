// using System.IO;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.Extensions.Hosting;

// namespace horses_for_courses.WebApi;

// public class CustomWebAppFactory : WebApplicationFactory<Program>
// {
//     // Usually not needed, here yes, because of weird setup. 
//     // Everything is in one .proj, namespaces and folders messed up ...
//     protected override IHost CreateHost(IHostBuilder builder)
//     {
//         var factory = new CustomWebAppFactory();
//         var client = factory.CreateClient();
//         var projectDir = Directory.GetCurrentDirectory();
//         builder.UseContentRoot(projectDir);
//         return base.CreateHost(builder);
//     }
// }