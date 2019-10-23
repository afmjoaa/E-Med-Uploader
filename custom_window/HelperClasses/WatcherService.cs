using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;
using custom_window.Pages;

public class WatcherService
{
    public bool IsWatching = false;
    private string Path = null;
    private FileSystemWatcher _watcher = null;
    private FileUploadService _fileUploadService = null;

    public WatcherService(string path)
    {
        this._watcher = new FileSystemWatcher();
        this.Path = path;
        this._fileUploadService = FileUploadService.GetInstance();
        this.IsWatching = true;
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public void watch()
    {
        // Create a new FileSystemWatcher and set its properties.

        _watcher.Path = this.Path;
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

    public async void OnChanged(object source, FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        // Debug.WriteLine($"File: {e.FullPath} changed to {e.ChangeType}");

        if (e.ChangeType == WatcherChangeTypes.Created)
        {
            Debug.WriteLine($"File: {e.FullPath} created.");
            try
            {
                WaitForFile(e.FullPath);
//                await this._service.UploadFile(new {name = e.Name}, e.FullPath);
                await _fileUploadService.UploadFileForPatient(e.FullPath);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Waiting for file to be released. " + exception.ToString());
            }
        }
        else if (e.ChangeType == WatcherChangeTypes.Deleted)
        {
            Debug.WriteLine($"File: {e.FullPath} Deleted.");
            try
            {
                await this._fileUploadService.deleteFile(e.Name);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    private void OnRenamed(object source, RenamedEventArgs e)
    {
        // Specify what is done when a file is renamed.
        Debug.WriteLine($"File: {e.FullPath} renamed to {e.Name}");
    }

    public void Dispose()
    {
        this.IsWatching = false;
        _watcher.Changed -= OnChanged;
        _watcher.Created -= OnChanged;
        _watcher.Deleted -= OnChanged;
        _watcher.Renamed -= OnRenamed;
        _watcher.Dispose();
    }

    private void WaitForFile(string fullPath)
    {
        Debug.WriteLine(fullPath);
        while (true)
        {
            try
            {
                using (var stream = new StreamReader(fullPath))
                {
                    stream?.Close();
                    break;
                }
            }
            catch
            {
                Thread.Sleep(1000);
            }
        }
    }
}