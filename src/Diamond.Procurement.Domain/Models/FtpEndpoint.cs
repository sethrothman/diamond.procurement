public sealed class FtpEndpoint
{
    public int FtpEndpointId { get; init; }
    public string FtpAddress { get; init; } = "";
    public int FtpPort { get; init; } = 21;
    public string FtpPath { get; init; } = "";
    public string FileName { get; init; } = "";
    public string User { get; init; } = "";
    public string Password { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime? LastTested { get; init; }
}