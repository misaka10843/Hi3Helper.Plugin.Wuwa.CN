using System.Collections.Generic;

#if !USELIGHTWEIGHTJSONPARSER
using System.Text.Json.Serialization;
#else
using Hi3Helper.Plugin.Core.Utility.Json;
using System.Linq;
using System.Text.Json;
#endif

// ReSharper disable InconsistentNaming

namespace Hi3Helper.Plugin.Wuwa.Management.Api;

public class WuwaApiResponseSocial
#if USELIGHTWEIGHTJSONPARSER
    : IJsonElementParsable<WuwaApiResponseSocial>
#endif
{
#if !USELIGHTWEIGHTJSONPARSER
    [JsonPropertyName("data")]
#endif
    public List<WuwaApiResponseSocialResponse>? SocialMediaEntries { get; set; }
    
#if USELIGHTWEIGHTJSONPARSER
    public static WuwaApiResponseSocial? ParseFrom(JsonElement element)
    {
        if (!element.TryGetProperty("data", out JsonElement arrayElement))
            return null;

        List<WuwaApiResponseSocial> entries = [];
        entries.AddRange(arrayElement.EnumerateArray().Select(WuwaApiResponseSocial.ParseFrom));

        return new WuwaApiResponseSocial
        {
            SocialMediaEntries = entries
        };
    }
#endif
}

public class WuwaApiResponseSocialResponse
#if USELIGHTWEIGHTJSONPARSER
    : IJsonElementParsable<WuwaApiResponseSocialResponse>
#endif
{
#if !USELIGHTWEIGHTJSONPARSER
    [JsonPropertyName("iconJumpUrl")]
#endif
    public string? ClickUrl { get; set; }
    
#if !USELIGHTWEIGHTJSONPARSER
    [JsonPropertyName("icon")]
#endif
    public string? IconUrl { get; set; }
    
#if !USELIGHTWEIGHTJSONPARSER
    [JsonPropertyName("name")]
#endif
    public string? SocialMediaName { get; set; }
    
#if USELIGHTWEIGHTJSONPARSER
    public static WuwaApiResponseSocialMediaResponse ParseFrom(JsonElement element)
    {
        string? clickUrl = element.GetString("iconJumpUrl");
        string? iconUrl = element.GetString("icon");
        string? socialMediaName = element.GetString("name");

        return new WuwaApiResponseSocialMediaResponse
        {
            ClickUrl = clickUrl,
            IconUrl = iconUrl,
            SocialMediaName = socialMediaName
        };
    }
#endif
}
