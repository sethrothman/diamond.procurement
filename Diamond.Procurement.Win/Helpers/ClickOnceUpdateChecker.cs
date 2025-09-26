using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Diamond.Procurement.Win.Helpers;

public static class ClickOnceUpdateChecker
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<Version?> GetDeploymentVersionAsync(string manifestUrl, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync(manifestUrl, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken).ConfigureAwait(false);

        var assemblyIdentity = document
            .Descendants()
            .FirstOrDefault(e => string.Equals(e.Name.LocalName, "assemblyIdentity", StringComparison.OrdinalIgnoreCase));

        var versionValue = assemblyIdentity?.Attribute("version")?.Value;
        if (Version.TryParse(versionValue, out var version))
        {
            return version;
        }

        return null;
    }

    public static Version GetCurrentVersion()
    {
        var versionString = Application.ProductVersion;
        if (Version.TryParse(versionString, out var version))
        {
            return version;
        }

        return new Version(0, 0, 0, 0);
    }
}
