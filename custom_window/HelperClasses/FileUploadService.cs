using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace custom_window.HelperClasses
{
    class Service
    {
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
                // save file info in database..
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error uploading:: " + e.ToString());
            }

            // now save it to database along with all info
            //TODO
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
    }
}