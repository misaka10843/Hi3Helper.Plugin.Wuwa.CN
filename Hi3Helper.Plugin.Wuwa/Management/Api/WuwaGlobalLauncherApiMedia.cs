using Hi3Helper.Plugin.Core;
using Hi3Helper.Plugin.Core.Management.Api;
using Hi3Helper.Plugin.Core.Utility;
using Hi3Helper.Plugin.Wuwa.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
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
    protected HttpClient ApiDownloadHttpClient
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
        => result = LauncherBackgroundFlag.TypeIsVideo | LauncherBackgroundFlag.TypeIsImage;

    public override void GetLogoFlag(out LauncherBackgroundFlag result)
        => result = LauncherBackgroundFlag.None;

    public override void GetLogoOverlayEntries(out nint handle, out int count, out bool isDisposable, out bool isAllocated)
    {
        isDisposable = false;
        handle = nint.Zero;
        count = 0;
        isAllocated = false;
    }

    protected override async Task<int> InitAsync(CancellationToken token)
    {
        using HttpResponseMessage response = await ApiDownloadHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, ApiDownloadHttpClient.BaseAddress), token);
        response.EnsureSuccessStatusCode();
        
#if USELIGHTWEIGHTJSONPARSER
        await using Stream networkStream = await response.Content.ReadAsStreamAsync(token);
        ApiResponse = await WuwaApiResponse<WuwaApiResponseMedia>.ParseFromAsync(networkStream, token: token);
#else
        string jsonResponse = await message.Content.ReadAsStringAsync(token);
        SharedStatic.InstanceLogger.LogTrace("API Media response: {JsonResponse}", jsonResponse);
        ApiResponse = JsonSerializer.Deserialize<HBRApiResponse<HBRApiResponseMedia>>(jsonResponse, HBRApiResponseContext.Default.HBRApiResponseHBRApiResponseMedia);
#endif
        // We don't have a way to check if the API response is valid, so we assume it is valid if we reach this point.
        return 0;
    }

    protected override async Task DownloadAssetAsyncInner(HttpClient? client, string fileUrl, Stream outputStream,
        PluginDisposableMemory<byte> fileChecksum, PluginFiles.FileReadProgressDelegate? downloadProgress, CancellationToken token)
    {
        await base.DownloadAssetAsyncInner(ApiResponseHttpClient, fileUrl, outputStream, fileChecksum, downloadProgress, token);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        
        using (ThisInstanceLock.EnterScope())
        {
            ApiResponseHttpClient?.Dispose();
            ApiDownloadHttpClient?.Dispose();
            
            ApiResponse = null;
            base.Dispose();
        }
    }
}

