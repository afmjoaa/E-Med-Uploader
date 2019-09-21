using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using custom_window.HelperClasses;

public class Watcher
{
    private string path = null;
    private FileSystemWatcher _watcher = null;
    public Watcher(string path)
    {
        _watcher = new FileSystemWatcher();
        this.path = path;
    }
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public void watch()
    {
        // Create a new FileSystemWatcher and set its properties.


        _watcher.Path = this.path;

        // Watch for changes in LastAccess and LastWrite times, and
        // the renaming of files or directories.
        _watcher.NotifyFilter = NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.FileName
                             | NotifyFilters.DirectoryName;

        // Only watch text files.
        // watcher.Filter = "*.txt";
        _watcher.Filter = "*.*";

        // Add event handlers.
        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += OnRenamed;

        // Begin watching.
        _watcher.EnableRaisingEvents = true;

        Thread.CurrentThread.Join();

        // while (true);


    }

    // Define the event handlers.
    public void OnChanged(object source, FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        Debug.WriteLine($"File: {e.Name} changed to {e.ChangeType}");
    }


    public void OnRenamed(object source, RenamedEventArgs e)
    {
        // Specify what is done when a file is renamed.
        Debug.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
    }

    public void Dispose()
    {
        _watcher.Changed -= OnChanged;
        _watcher.Created -= OnChanged;
        _watcher.Deleted -= OnChanged;
        _watcher.Renamed -= OnRenamed;
        _watcher.Dispose();
    }


}