using FluentFTP;
using System.IO;
using System.Net;

public static class FtpDownloader
{
    public static async Task<string> DownloadAsync(
        FtpEndpoint ep,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        var host = ep.FtpAddress;
        var port = ep.FtpPort > 0 ? ep.FtpPort : 21;
        var remotePath = $"{(ep.FtpPath ?? string.Empty).TrimEnd('/')}/{ep.FileName}";
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{ep.FileName}");

        var cfg = new FtpConfig
        {
            EncryptionMode = FtpEncryptionMode.Auto,            // auto-FTPS if available
            DataConnectionType = FtpDataConnectionType.PASV,    // passive first
            ValidateAnyCertificate = true,                      // tighten for prod
            ConnectTimeout = 15_000,
            ReadTimeout = 60_000
        };

        using var client = new AsyncFtpClient(host, new NetworkCredential(ep.User, ep.Password), port, cfg);

        // Your FluentFTP version expects (FtpTraceLevel, string)
        //client.LegacyLogger = (level, message) => {
        //    progress?.Report($"[FTP {level}] {message}");
        //};

        client.LegacyLogger = (level, msg) => {
            if (level == FtpTraceLevel.Info)
            {
                progress?.Report($"[FTP] {msg}");
            }
        };

        progress?.Report($"[FTP] Connecting to {host}:{port} …");
        await client.Connect(ct);

        // Early, clear failure if path or file wrong
        if (!await client.FileExists(remotePath, ct))
            throw new FileNotFoundException($"Remote file not found: {remotePath}");

        // Your FtpProgress may only have .Progress (percentage) and possibly .TransferredBytes
        //var ftpProgress = new Progress<FtpProgress>(p => {
        //    if (p.Progress >= 0)
        //        progress?.Report($"[FTP] {p.Progress:0}%");
        //    // If your build has TransferredBytes, you can optionally add:
        //    // progress?.Report($"[FTP] {p.Progress:0}% ({p.TransferredBytes} bytes)");
        //});

        // NOTE: Overloads vary by version. This one uses the progress parameter and
        // omits a CancellationToken arg if your build doesn't support it on this method.
        var status = await client.DownloadFile(
            localPath: tempFile,
            remotePath: remotePath,
            existsMode: FtpLocalExists.Overwrite,
            verifyOptions: FtpVerify.Retry
            //progress: ftpProgress
        // If your installed version supports it, you can add: , token: ct
        );

        if (status != FtpStatus.Success)
            throw new IOException($"FTP download failed with status {status}");

        progress?.Report($"[FTP] Downloaded to {tempFile}.  File ready to import.");
        return tempFile;
    }
}
