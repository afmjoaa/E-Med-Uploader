﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using custom_window.HelperClasses.DataModels;
using custom_window.Pages;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using File = System.IO.File;

namespace custom_window.HelperClasses
{
    class FileUploadService
    {
        #region Init

        private static FileUploadService _instance = null;

        public delegate void ProgressUpdatedDelegate(int percentage);

        public ProgressUpdatedDelegate OnProgressUpdated = null;

        private readonly CloudFirestoreService _cfService = CloudFirestoreService.GetInstance();

        public static Patient CurrentPatient = null;

        // updated on each fingerprints or null
        // needs to be reseted after each upload

        #endregion

        private FileUploadService()
        {
        }

        public static FileUploadService GetInstance()
        {
            if (_instance != null) return _instance;
            _instance = new FileUploadService();
            return _instance;
        }

        public async Task<string> UploadFile(string path)
        {
            var stream = File.Open(path, FileMode.Open);
            var task = new FirebaseStorage("emed-4490e.appspot.com")
                .Child(Path.GetFileName(path))
                .PutAsync(stream);
            try
            {
                var downloadUrl = await task;
                Debug.WriteLine(downloadUrl);
                return downloadUrl;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
                return null;
            }
        }

        public async Task<string> UploadFile(string path, string uploadFileName)
        {
            var stream = File.Open(path, FileMode.Open);
            var task = new FirebaseStorage("emed-4490e.appspot.com")
                .Child(uploadFileName)
                .PutAsync(stream);
            try
            {
                var downloadUrl = await task;
                Debug.WriteLine(downloadUrl);
                return downloadUrl;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
                return null;
            }
        }

        public async Task<string> UploadHospitalDisplayPic(Stream stream, string uploadFileName)
        {
            var task = new FirebaseStorage("emed-4490e.appspot.com")
                .Child(uploadFileName)
                .PutAsync(stream);
            try
            {
                var downloadUrl = await task;
                Properties.Settings.Default.photoUrl = downloadUrl;
                Properties.Settings.Default.Save();

                await CloudFirestoreService.GetInstance()
                    .UpdateHospital(Properties.Settings.Default.localId, "photoUrl", downloadUrl);
                return downloadUrl;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
                return null;
            }
        }

        public async Task deleteFile(string fileName)
        {
            try
            {
                Debug.WriteLine("File deleted: " + fileName);
                var task = new FirebaseStorage("emed-4490e.appspot.com").Child(fileName).DeleteAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public async Task<string> UploadFileForPatient(string filePath)
        {
            if (CurrentPatient == null)
            {
                ToastClass.NotifyMin("No Patient is identifed But Files ared created!!",
                    "Please scan for fingerprint and then copy the report file to the listening directory");
                return "Error";
            }

            var stream = File.Open(filePath, FileMode.Open);
            var task = new FirebaseStorage("emed-4490e.appspot.com")
                .Child(Path.GetFileName(filePath))
                .PutAsync(stream);
            task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");
            task.Progress.ProgressChanged += (s, e) =>
            {
                OnProgressUpdated.Invoke(e.Percentage);
                Debug.WriteLine($"Progress: {e.Percentage} %");
            };

            try
            {
                var downloadUrl = await task;
                Debug.WriteLine(downloadUrl);

                // save file info in database..
                // cfService.

                var reportFile = new ReportFile();
                reportFile.file_name = Path.GetFileName(filePath);
                reportFile.file_url = downloadUrl;
                reportFile.associated_patientId = CurrentPatient.patient_id;
                reportFile.associated_hospitalId = Properties.Settings.Default.localId;
                var res = await _cfService.AddFile(reportFile);

                ToastClass.NotifyMin("Uploaded & saved info to server", res);
                CurrentPatient = null;
                return res;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
            }

            return "Error";
        }

        public void Dispose()
        {
            FolderItemVm.DisposeAllWatchers();
        }
    }
}