using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AutoModPlugins.Properties;
using PKHeX.Core;
using PKHeX.Core.Enhancements;

namespace AutoModPlugins;

public class MGDBDownloader : AutoModPlugin
{
    public override string Name => "Download Latest MGDB";
    public override int Priority => 1;
    public static string MGDatabasePath => Path.Combine(Directory.GetCurrentDirectory(), "mgdb");

    protected override void AddPluginControl(ToolStripDropDownItem modmenu)
    {
        var ctrl = new ToolStripMenuItem(Name) { Image = WinFormsUtil.GetIconForTheme(Resources.mgdbdownload, Application.IsDarkModeEnabled) };
        ctrl.Click += DownloadMGDB;
        ctrl.Name = "Menu_MGDBDownloader";
        modmenu.DropDownItems.Add(ctrl);
    }

    private static void DownloadMGDB(object? o, EventArgs e)
    {
        var hasExistingDatabase = Directory.Exists(MGDatabasePath);
        if (hasExistingDatabase)
        {
            var result = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "MGDB already exists!", "Replace it with the latest public MGDB release?");
            if (result != DialogResult.Yes)
                return;
        }

        var parent = Path.GetDirectoryName(MGDatabasePath)!;
        var downloadPath = Path.Combine(parent, $".mgdb-download-{Guid.NewGuid():N}");
        var backupPath = Path.Combine(parent, $".mgdb-backup-{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(downloadPath);
            EventsGallery.DownloadMGDBFromGitHub(downloadPath, entire: false);
            if (!MysteryUtil.GetGiftsFromFolder(downloadPath).Any())
                throw new InvalidDataException("The downloaded archive did not contain any recognized mystery gifts.");

            if (hasExistingDatabase)
                Directory.Move(MGDatabasePath, backupPath);

            try
            {
                Directory.Move(downloadPath, MGDatabasePath);
                EncounterEvent.RefreshMGDB(MGDatabasePath);
            }
            catch
            {
                if (Directory.Exists(MGDatabasePath))
                    DeleteDirectory(MGDatabasePath);
                if (Directory.Exists(backupPath))
                    Directory.Move(backupPath, MGDatabasePath);
                if (hasExistingDatabase)
                    EncounterEvent.RefreshMGDB(MGDatabasePath);
                else
                    EncounterEvent.RefreshMGDB();
                throw;
            }

            if (Directory.Exists(backupPath))
                DeleteDirectory(backupPath);

            WinFormsUtil.Alert("Latest MGDB downloaded and loaded.");
        }
        catch (Exception ex)
        {
            if (Directory.Exists(downloadPath))
                DeleteDirectory(downloadPath);
            WinFormsUtil.Error("Failed to download or load the latest MGDB.", ex.Message);
        }
    }

    public static void DeleteDirectory(string target_dir)
    {
        var files = Directory.GetFiles(target_dir);
        var dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }
}
