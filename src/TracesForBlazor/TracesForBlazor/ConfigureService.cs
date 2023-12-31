﻿using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("TracerForBlazorTests")]
namespace TracesForBlazor;

    public enum OpenTelemetrySendTypes
    {
        //Grpc,
        HttpProto
    }

public static class ConfigureService
{
    public static void AddTracesForBlazor(this IServiceCollection services, TracerForBlazorOptions options,
        params TracesForBlazorActivitySource[] sources)
    {
        if(sources.Length == 0)
            throw new ArgumentException("At least one ActivitySource is required");
        VerifyOptions(options);
        TracerService tracerService = new(sources, options);
        services.AddSingleton(tracerService);
    }

    internal static void VerifyOptions(TracerForBlazorOptions options)
    {
        if(string.IsNullOrEmpty(options.Url))
            throw new ArgumentException("URL is required", nameof(options.Url));

        if (!IsValidUrl(options.Url))
        {
            throw new ArgumentException("Invalid URL");
        }

        var uri = new Uri(options.Url);
        if (!uri.IsAbsoluteUri)
        {
            throw new ArgumentException("URL must be absolute");
        }

        if (!options.Url[^6..].Contains(':'))
        {
            throw new ArgumentException("URL must contain a port number");
        }
        
        if(uri.Port<1 || uri.Port>65535)
            throw new ArgumentException("Port must be between 1 and 65535");
        if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        {
            
        }
        else
        {
            throw new ArgumentException("URL must be http or https");
        }
        
        if (string.IsNullOrEmpty(uri.Query))
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Path = "v1/traces";
            options.Url = uriBuilder.ToString();
        }
    }

    private static bool IsValidUrl(string url)
    {
        Uri? uriResult;
        return Uri.TryCreate(url, UriKind.Absolute, out uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    
}