using System;
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
        private CloudFirestoreService _cfService = CloudFirestoreService.GetInstance();

        public static Patient _currentPatient = null;
        // updated on each fingerprints or null
        // needs to be reseted after each upload

        private Hospital _currentHospital = CloudFirestoreService.GetInstance().getLoggedInHospital();

        public async Task UploadFile(object fileInfo, string path)
        {
            var stream = File.Open(path, FileMode.Open);
            var task = new FirebaseStorage("emed-4490e.appspot.com")
                .Child(Path.GetFileName(path))
                .PutAsync(stream);
            task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");
            try
            {
                var downloadUrl = await task;
                Debug.WriteLine(downloadUrl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
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
            if (_currentPatient == null)
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
            try
            {
                var downloadUrl = await task;
                Debug.WriteLine(downloadUrl);

                // save file info in database..
                // cfService.

                var reportFile = new ReportFile
                {
                    file_name = Path.GetFileName(filePath),
                    file_url = downloadUrl,
                    associated_patientId = _currentPatient.patient_id,
                    associated_hospitalId = _currentHospital.hospital_id
                };
                var res = await _cfService.AddFile(reportFile);
                ToastClass.NotifyMin("Uploaded & saved info to server", res);
                return res;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
            }

            return "Error";
        }
    }
}