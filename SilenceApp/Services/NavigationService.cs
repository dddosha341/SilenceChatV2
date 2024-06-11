﻿using Silence.Infrastructure.Services;

namespace SilenceApp.Services;

public class NavigationService : INavigationService
{
    public Task GoToAsync(Route route, IDictionary<string, object> parameters, bool keepHistory = true)
    {
        var prefix = keepHistory ? "/" : "//";
        var routeS = $"{prefix}{route.MapRouteToPath()}";

        if (parameters is not null)
        {
            foreach (var parameter in parameters)
            {
                routeS += $"?{parameter.Key}={parameter.Value}";
            }
        }

        return Shell.Current.GoToAsync(routeS);
    }

    public Task GoBackAsync() => 
        Shell.Current.GoToAsync("..");
}

internal static class RouteExtensions
{
    public static string MapRouteToPath(this Route route) => route switch
    {
        Route.Back => "..",
        Route.Login => "login",
        Route.Register => "register",
        Route.Welcome => "welcome",
        Route.ChatRoom => "chatRoom",
        Route.EditProfile => "editProfile",
        _ => throw new NotSupportedException($"Route {route} is not supported")
    };
}

