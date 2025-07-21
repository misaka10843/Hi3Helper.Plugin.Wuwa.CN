using Hi3Helper.Plugin.Core;
using Hi3Helper.Plugin.Core.Management.Api;
using Hi3Helper.Plugin.Wuwa.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.InteropServices.Marshalling;

namespace Hi3Helper.Plugin.Wuwa.Management.Api;

[GeneratedComClass]
internal partial class WuwaGlobalLauncherApiMedia(string apiResponseBaseUrl, string gameTag, string AuthenticationHash, string ApiOptions) : LauncherApiMediaBase
{
    [field: AllowNull, MaybeNull]
    protected override HttpClient ApiResponseHttpClient { 
        get => field ??= WuwaUtils.CreateApiHttpClient(apiResponseBaseUrl, gameTag, AuthenticationHash, ApiOptions);
        set;
    }

    [field: AllowNull, MaybeNull]
    protected HttpClient ApiDownloadClient
    {
        get => field ??= WuwaUtils.CreateApiHttpClient(apiResponseBaseUrl, gameTag, AuthenticationHash, ApiOptions);
        set;
    }

    //protected override string ApiResponseBaseUrl { get; } = apiResponseBaseUrl;
    private WuwaApiResponse<WuwaApiResponseMedia>? ApiResponse { get; set; }

    public override void GetBackgroundEntries(out nint handle, out int count, out bool isDisposable, out bool isAllocated)
    {
        using (ThisInstanceLock.EnterScope())
        {
            PluginDisposableMemory<LauncherPathEntry> backgroundEntries = PluginDisposableMemory<LauncherPathEntry>.Alloc();

            try
            {
                ref LauncherPathEntry entry = ref backgroundEntries[0];

                if (ApiResponse?.ResponseData == null)
                {
                    isDisposable = false;
                    handle = nint.Zero;
                    count = 0;
                    isAllocated = false;
                    return;
                }

                unsafe
                {
                    entry.Write(ApiResponse.ResponseData.BackgroundImageUrl, new Span<byte>((void*)null, sizeof(ulong)));
                }
                isAllocated = true;
            }
            finally
            {
                isDisposable = backgroundEntries.IsDisposable == 1;
                handle = backgroundEntries.AsSafePointer();
                count = backgroundEntries.Length;
            }
        }
    }

    public override void GetBackgroundFlag(out LauncherBackgroundFlag result)
    {
        throw new NotImplementedException();
    }

    public override void GetLogoFlag(out LauncherBackgroundFlag result)
    {
        throw new NotImplementedException();
    }

    public override void GetLogoOverlayEntries(out nint handle, out int count, out bool isDisposable, out bool isAllocated)
    {
        throw new NotImplementedException();
    }
}

