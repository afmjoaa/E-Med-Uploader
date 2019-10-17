using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ControlzEx.Standard;
using custom_window.HelperClasses.DataModels;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace custom_window.HelperClasses
{
    public class CloudFirestoreService
    {
        public static CloudFirestoreService instance = null;
        private bool _isLoggedIn = false;

        string projectId;
        FirestoreDb fireStoreDb;

        private CloudFirestoreService(string firebaseCredentialPath = "")
        {
            var filepath = "E:\\Projects\\emed\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json";
            if (!string.IsNullOrEmpty(firebaseCredentialPath))
            {
                filepath = firebaseCredentialPath;
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "emed-4490e";
            fireStoreDb = FirestoreDb.Create(projectId);
        }

        public static CloudFirestoreService GetInstance()
        {
            if (instance != null) return instance;
            instance = new CloudFirestoreService();
            return instance;
        }

        public async void AddHospital(Hospital hospital)
        {
            try
            {
                CollectionReference colRef = fireStoreDb.Collection("hospitals");
                var retRef = await colRef.AddAsync(hospital);
                ToastClass.NotifyMin("created! Hospital", retRef.Id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Patient>> getAllPatients()
        {
            List<Patient> patientsList = new List<Patient>();
            try
            {
                Query patientQuery = fireStoreDb.Collection("patients");
                QuerySnapshot patientQuerySnapshot = await patientQuery.GetSnapshotAsync();
                foreach (DocumentSnapshot documentSnapshot in patientQuerySnapshot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> patientObj = documentSnapshot.ToDictionary();
                        var json = JsonConvert.SerializeObject(patientObj);
                        Patient patient = JsonConvert.DeserializeObject<Patient>(json);
                        patientsList.Add(patient);
                    }
                }

                //List<Employee> sortedEmployeeList = lstEmployee.OrderBy(x => x.date).ToList();
            }
            catch
            {
                throw;
            }

            return patientsList;
        }
    }
}