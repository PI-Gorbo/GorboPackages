using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace GP.MartenIdentity;
public static class Extensions
{
    public static IHostApplicationBuilder Test(this IHostApplicationBuilder builder)
    {
        Console.WriteLine("here");
        return builder;
    }
}