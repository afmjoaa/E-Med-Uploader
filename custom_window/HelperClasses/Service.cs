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
        public async Task InitApp()
        {

            FirestoreDb db = FirestoreDb.Create("emed-4490e");


            // Create a document with a random ID in the "users" collection.
            CollectionReference collection = db.Collection("files");

            // DocumentReference document = await collection.AddAsync(new { Name = new { First = "Ada", Last = "Lovelace" }, Born = 1815 });
            var document = await collection.AddAsync(new { name = new { First = "Ada", Last = "Lovelace" }, Born = 1815 });



            // A DocumentReference doesn't contain the data - it's just a path.
            // Let's fetch the current document.
            DocumentSnapshot snapshot = await document.GetSnapshotAsync();

            // We can access individual fields by dot-separated path
            Console.WriteLine(snapshot.GetValue<string>("Name.First"));
            Console.WriteLine(snapshot.GetValue<string>("Name.Last"));
            Console.WriteLine(snapshot.GetValue<int>("Born"));

            // Or deserialize the whole document into a dictionary
            Dictionary<string, object> data = snapshot.ToDictionary();
            Dictionary<string, object> name = (Dictionary<string, object>)data["Name"];
            Console.WriteLine(name["First"]);
            Console.WriteLine(name["Last"]);

            // See the "data model" guide for more options for data handling.

            // Query the collection for all documents where doc.Born < 1900.
            Query query = collection.WhereLessThan("Born", 1900);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot queryResult in querySnapshot.Documents)
            {
                string firstName = queryResult.GetValue<string>("Name.First");
                string lastName = queryResult.GetValue<string>("Name.Last");
                int born = queryResult.GetValue<int>("Born");
                Console.WriteLine($"{firstName} {lastName}; born {born}");
            }
        }

        public async Task UploadFile(object fileInfo,string path)
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
                Debug.WriteLine(e.ToString());
            }
            

            // now save it to database along with all info
            // TODO
        }

        public async Task deleteFile(string fileName)
        {
            try
            {
                Debug.WriteLine("File deleted: " + fileName);
                var task = new FirebaseStorage("emed-4490e.appspot.com").Child(fileName).DeleteAsync();
                await task;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: "+e.ToString());

            }
            
        }
    }

    interface IFileModel
    {
        string name{ get; set; }
        string type{ get; set; }
        string date_added { get; set; }
        string uploader { get; set; }
        string receiver { get; set; }
        string link { get; set; }
        string uid { get; set; }
        int size { get; set; }
    }

}
