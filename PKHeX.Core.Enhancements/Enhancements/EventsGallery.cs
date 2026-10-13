using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace PKHeX.Core.Enhancements;

/// <summary>
/// Interactions with ProjectPokémon's Event Gallery (hosted on GitHub)
/// </summary>
public static class EventsGallery
{
    private const string RepoURL = "https://github.com/projectpokemon/EventsGallery/archive/refs/heads/master.zip";

    /// <summary>
    /// Downloads the entire repository from GitHub and extracts the contents to the <see cref="dest"/>.
    /// </summary>
    /// <param name="dest">Location to extract the repository to</param>
    /// <param name="entire">True to download the full repository, false to download only its Released database.</param>
    public static void DownloadMGDBFromGitHub(string dest, bool entire)
    {
        if (entire)
        {
            DownloadEntireRepo(dest);
        }
        else
        {
            DownloadReleasedMGDB(dest);
        }
    }

    private static void DownloadEntireRepo(string dest)
    {
        DownloadAndExtractZip(RepoURL, dest);

        // clean up; delete unneeded files
        var path = Path.Combine(dest, "EventsGallery-master");
        string getPath(string s) => Path.Combine(path, s);
        Directory.Delete(getPath("Unreleased"), true);
        Directory.Delete(getPath("Extras"), true);
        File.Delete(getPath(".gitignore"));
        File.Delete(getPath("README.md"));
    }

    private static void DownloadReleasedMGDB(string dest)
    {
        Directory.CreateDirectory(dest);
        var temp = Path.Combine(dest, $".eventsgallery-{Guid.NewGuid():N}");
        Directory.CreateDirectory(temp);
        try
        {
            DownloadAndExtractZip(RepoURL, temp);

            var repo = Path.Combine(temp, "EventsGallery-master");
            var released = Path.Combine(repo, "Released");
            if (!Directory.Exists(released))
                throw new InvalidDataException("The downloaded EventsGallery archive did not contain a Released database.");

            Directory.Move(released, Path.Combine(dest, "Released"));
        }
        finally
        {
            if (Directory.Exists(temp))
                Directory.Delete(temp, recursive: true);
        }
    }

    private static void DownloadAndExtractZip(string url, string dest)
    {
        var temp = Path.Combine(Path.GetTempPath(), $"mgdb-{Guid.NewGuid():N}.zip");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
        try
        {
            using (WebClient client = new())
            {
                client.DownloadFile(new Uri(url), temp);
            }

            ZipFile.ExtractToDirectory(temp, dest);
        }
        finally
        {
            if (File.Exists(temp))
                File.Delete(temp);
        }
#pragma warning restore SYSLIB0014 // Type or member is obsolete
    }
}
