using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Application;

public static class EndpointRouteBuilderExtensions
{
    public static void MapAllHubs(this IEndpointRouteBuilder endpoints, string prefix = "")
    {
        var hubTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(Hub).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var hubType in hubTypes)
        {
            var hubName = hubType.Name.Replace("Hub", "").ToLower();
            var route = $"{prefix}/{hubName}hub";

            var mapMethod = typeof(HubEndpointRouteBuilderExtensions)
                .GetMethods()
                .First(m => m.Name == "MapHub" && m.GetGenericArguments().Length == 1);

            var genericMap = mapMethod.MakeGenericMethod(hubType);
            genericMap.Invoke(null, new object[] { endpoints, route });
        }
    }
}
