var username = Environment.GetCommandLineArgs()[1];

// Define paths
string sourceFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"StardewValley\Saves\" + username);
string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"StardewValley\Backup");
string currentBackup = Path.Combine(backupFolder, "current");

// Check if the source folder exists
if (!Directory.Exists(sourceFolder))
{
    Console.WriteLine($"Source folder not found: {sourceFolder}");
    return;
}

// Ensure backup directory exists
if (!Directory.Exists(backupFolder))
{
    Directory.CreateDirectory(backupFolder);
}

// Rename existing "current" backup if it exists
if (Directory.Exists(currentBackup))
{
    string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmm");
    string newBackupName = Path.Combine(backupFolder, timestamp);

    if (Directory.Exists(newBackupName)) { Directory.Delete(newBackupName, true); }

    Directory.Move(currentBackup, newBackupName);
}

// Create new "current" backup
CopyDirectory(sourceFolder, currentBackup);

// Clean up old backups (keep only the 5 most recent)
var oldBackups = Directory.GetDirectories(backupFolder)
                          .Where(d => Path.GetFileName(d).Length == 13 && Path.GetFileName(d).Contains("-"))
                          .OrderByDescending(d => new DirectoryInfo(d).Name)
                          .Skip(5);

foreach (var oldBackup in oldBackups)
{
    Directory.Delete(oldBackup, true);
}

Console.WriteLine("Backup complete.");

static void CopyDirectory(string sourceDir, string destinationDir)
{

    Directory.CreateDirectory(destinationDir);

    foreach (string file in Directory.GetFiles(sourceDir))
    {
        string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
        File.Copy(file, destFile, true);
    }

    foreach (string directory in Directory.GetDirectories(sourceDir))
    {
        string destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
        CopyDirectory(directory, destDir);
    }
}