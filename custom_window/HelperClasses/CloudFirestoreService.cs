﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ControlzEx.Standard;
using custom_window.HelperClasses.DataModels;
using Google.Cloud.Firestore;
using libzkfpcsharp;
using Newtonsoft.Json;

namespace custom_window.HelperClasses
{
    public class CloudFirestoreService
    {
        string projectId = null;
        FirestoreDb fireStoreDb = null;
        private static CloudFirestoreService instance = null;
        private FingerprintHelper fpDeviceHelper = FingerprintHelper.GetInstance();

        public bool _isLoggedIn = false;
        private Hospital _loggedInHospitlal = null;

        public async Task<Tuple<Hospital, int>> Login(string phoneNumber, string password)
        {
            // check & get hashed password & compare..

            CollectionReference collection = fireStoreDb.Collection("hospitals");
            // A CollectionReference is a Query, so we can just fetch everything

            // But we can apply filters, perform ordering etc too.
            Query filterHospital = collection
                .WhereEqualTo("hospital_phone_number", phoneNumber);
            QuerySnapshot hospital = await filterHospital.GetSnapshotAsync();

            foreach (DocumentSnapshot document in hospital.Documents)
            {
                // Do anything you'd normally do with a DocumentSnapshot
                Hospital currentHospital = document.ConvertTo<Hospital>();
                // we expect getting at most one hospital..
                if (password == currentHospital.hospital_pass)
                {
                    _loggedInHospitlal = currentHospital;
                    _isLoggedIn = true;
                    return new Tuple<Hospital, int>(currentHospital, Constants.NO_ERROR);
                }

                _isLoggedIn = false;
                return new Tuple<Hospital, int>(null, Constants.BAD_PASS);
            }

            _isLoggedIn = false;
            return new Tuple<Hospital, int>(null, Constants.BAD_USER);
        }


        private void checkPass()
        {
        }


        public Hospital getLoggedInHospital()
        {
            if (_isLoggedIn)
                return _loggedInHospitlal;
            return null;
        }

        private CloudFirestoreService()
        {
            var filepath = "E:\\Projects\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json"; // zsumon -> desktop

            // var filepath = "G:\\emed\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json";  // zsumon -> laptop

            //var filepath = "G:\\emed\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json";  // for joaa-> laptop

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
                // ToastClass.NotifyMin("created! Hospital", retRef.Id);
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

        public async Task<string> AddFile(ReportFile reportFile)
        {
            try
            {
                CollectionReference colRef = fireStoreDb.Collection("files");
                var retRef = await colRef.AddAsync(reportFile);
                return retRef.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Patient> FindPatientByFingerprint(string template)
        {
            Patient ret = null;
            var patients = await getAllPatients();
            var score = 0;
            foreach (var patient in patients)
            {
                if (string.IsNullOrEmpty(patient.patient_fingerprint_template_right_thumb)) continue;

                byte[] blob1 = Convert.FromBase64String(template.Trim());
                byte[] blob2 = Convert.FromBase64String(patient.patient_fingerprint_template_right_thumb.Trim());
                var cScore = fpDeviceHelper.CompareFingerPrint(blob1, blob2);
                // Debug.WriteLine("Match template 1 vs template 2 score=" + cScore + "!\n");
                if (cScore > score && cScore >= 60)
                {
                    score = cScore;
                    ret = patient;
                }
            }

            return ret;
        }

        public async Task<string> AddPatient(Patient patient)
        {
            var coll = fireStoreDb.Collection("patients");
            var x = await coll.AddAsync(patient);
            return x.Id;
        }
    }
}