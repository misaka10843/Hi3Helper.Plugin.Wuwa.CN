#if !USELIGHTWEIGHTJSONPARSER
using Hi3Helper.Plugin.Core.Utility.Json.Converters;
using System.Text.Json.Serialization;
#else
using System.Text.Json;
using Hi3Helper.Plugin.Core.Utility.Json;
#endif


namespace Hi3Helper.Plugin.Wuwa.Management.Api;

public class WuwaApiResponseMedia
#if USELIGHTWEIGHTJSONPARSER
    : IJsonElementParsable<WuwaApiResponseMedia>
#endif
{
#if !USELIGHTWEIGHTJSONPARSER
    [JsonPropertyName("backgroundFile")]
#endif
    public string? BackgroundImageUrl { get; set; }

#if USELIGHTWEIGHTJSONPARSER
    public static WuwaApiResponseMedia ParseFrom(JsonElement element)
    {
        WuwaApiResponseMedia returnValue = new WuwaApiResponseMedia
        {
            BackgroundImageUrl = element.GetString("backgroundFile")
        };

        return returnValue;
    }
#endif
}

