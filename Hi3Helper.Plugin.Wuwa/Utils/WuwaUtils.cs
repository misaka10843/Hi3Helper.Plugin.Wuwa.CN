using Hi3Helper.Plugin.Core;
using Hi3Helper.Plugin.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hi3Helper.Plugin.Wuwa.Utils;

internal static class WuwaUtils
{
    internal static HttpClient CreateApiHttpClient(string? apiBaseUrl = null, string? gameTag = null, string? authCdnToken = "", string? apiOptions = "", string? hash1 = "")
        => CreateApiHttpClientBuilder(gameTag, authCdnToken, apiOptions, null, hash1).Create();

    internal static PluginHttpClientBuilder CreateApiHttpClientBuilder(string? apiBaseUrl, string? gameTag = null, string? authCdnToken= "", string? accessOption = null, string? hash1 = "")
    {
        PluginHttpClientBuilder builder = new PluginHttpClientBuilder()
            .SetUserAgent($"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");

        // ReSharper disable once ConvertIfStatementToSwitchStatement      
        if (authCdnToken == null)
        {
            throw new ArgumentNullException(nameof(authCdnToken), "authCdnToken cannot be empty. Use string.Empty if you want to ignore it instead.");
        }

        if (string.IsNullOrEmpty(authCdnToken))
        {
            authCdnToken.Aggregate(string.Empty, (current, c) => current + (char)(c ^ 99));
            authCdnToken = Convert.FromBase64String(authCdnToken).Aggregate(string.Empty, (current, b) => current + (char)(b ^ 99));
#if DEBUG
            SharedStatic.InstanceLogger.LogTrace("Decoded authCdnToken: {}", authCdnToken);
#endif
        }

        switch (accessOption)
        {
            case "news":
                builder.SetBaseUrl(apiBaseUrl + "launcher/" + authCdnToken + "/" + gameTag + "/" + "information/en.json");
                break;
            case "bg":
                builder.SetBaseUrl(apiBaseUrl + "launcher/" + authCdnToken + "/" + gameTag + "/background/" + hash1 + "/en.json");
                break;
            case "media":
                builder.SetBaseUrl(apiBaseUrl + "launcher/" + gameTag + "/" + authCdnToken + "/social/en.json");
                break;
            default:
                break;
        }


#if DEBUG
        SharedStatic.InstanceLogger.LogTrace("Created HttpClient with Token: {}", authCdnToken);
#endif
        builder.AddHeader("Host", apiBaseUrl.Substring(7, apiBaseUrl.IndexOf('/', 8) - 7)); // exclude "https://"
        builder.AddHeader("Accept-Encoding", "gzip");

        // Enforce gzip decompression for responses
        builder.SetAllowedDecompression(DecompressionMethods.GZip);

        return builder;
    }
}

