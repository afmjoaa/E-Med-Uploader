using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using ControlzEx.Standard;
using custom_window.HelperClasses.DataModels;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using libzkfpcsharp;
using Newtonsoft.Json;
using FirebaseException = Firebase.Database.FirebaseException;

namespace custom_window.HelperClasses
{
    public class CloudFirestoreService
    {
        #region Init

        string projectId = null;
        FirestoreDb fireStoreDb = null;
        private static CloudFirestoreService instance = null;
        public FirebaseAuthProvider authProvider = null;

        public delegate void DbFileChangedEvent(List<ReportFile> updatedFiles);

        public DbFileChangedEvent OnDbFileChanged;

        private FingerprintHelper fpDeviceHelper = FingerprintHelper.GetInstance();

        public bool _isLoggedIn = false;
        private Hospital _loggedInHospitlal = null;
        private CollectionReference hospitalCollection = null;
        private CollectionReference patientCollection = null;

        #endregion

        #region constructor

        private CloudFirestoreService()
        {
            var filepath = "";
            string pcName = Environment.MachineName;
            if (pcName == "DESKTOP-91FG7PD")
            {
                filepath = "F:\\emed\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json"; // zsumon laptop
            }
            else if (pcName == "DESKTOP-5PTT5JA")
            {
                filepath = "E:\\Projects\\emed\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json"; // zsumon -> desktop
            }
            else if (pcName == "DESKTOP-331SMHA")
            {
                filepath = "D:\\emed(all)\\E-Med_Uploader\\emed-4490e-ddff9c9b9237.json"; // joaa --> Laptop
            }

            if (string.IsNullOrWhiteSpace(filepath))
            {
                throw new FileNotFoundException("Please set correct path of your credentials..");
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "emed-4490e";
            fireStoreDb = FirestoreDb.Create(projectId);
            authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBIdfmwhWFPB1PvTqNwqrukIOBB8sas16c"));
            hospitalCollection = fireStoreDb.Collection("hospitals");
            patientCollection = fireStoreDb.Collection("patients");
        }

        #endregion

        #region unUsedMethods

        /*public async Task<Tuple<Hospital, int>> Login(string phoneNumber, string password)
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
        }*/

        #endregion

        public async Task AddHospital(Hospital hospital)
        {
            try
            {
                hospitalCollection = fireStoreDb.Collection("hospitals");
                var retRef = await hospitalCollection.Document(hospital.uid).CreateAsync(hospital);
                // ToastClass.NotifyMin("created! Hospital", retRef.Id);
            }
            catch
            {
                throw;
            }
        }


        public async Task UpdateHospital(string uid, string key, string value)
        {
            try
            {
                hospitalCollection = fireStoreDb.Collection("hospitals");
                var retRef = await hospitalCollection.Document(uid).UpdateAsync(key, value);
                // ToastClass.NotifyMin("created! Hospital", retRef.Id);
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdatePatient(string uid, string key, string value)
        {
            try
            {
                var retRef = await patientCollection.Document(uid).UpdateAsync(key, value);
                // ToastClass.NotifyMin("created! Hospital", retRef.Id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Hospital> GetLoggedInHospital(string hospitalUid, CancellationToken cancellationToken)
        {
            try
            {
                DocumentSnapshot reportFileDoc =
                    await hospitalCollection.Document(hospitalUid).GetSnapshotAsync(cancellationToken);
                if (reportFileDoc != null && reportFileDoc.Exists)
                {
                    var hospital = reportFileDoc.ConvertTo<Hospital>();
                    return hospital;
                }
                else
                {
                    return null;
                }
            }
            catch (FirebaseException exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public async Task<Hospital> GetLoggedInHospitalByEmail(string mEmail, CancellationToken cancellationToken)
        {
            hospitalCollection = fireStoreDb.Collection("hospitals");
            try
            {
                var hospitalShanpShot = await hospitalCollection.WhereEqualTo("email", mEmail)
                    .GetSnapshotAsync(cancellationToken);
                if (hospitalShanpShot != null)
                {
                    var documentSnapshots = hospitalShanpShot.Documents;
                    if (documentSnapshots.Count == 1)
                    {
                        foreach (DocumentSnapshot document in documentSnapshots)
                        {
                            var hospital = document.ConvertTo<Hospital>();
                            return hospital;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (FirebaseException exception)
            {
                Console.WriteLine(exception);
                return null;
            }

            return null;
        }

        public static CloudFirestoreService GetInstance()
        {
            if (instance != null) return instance;
            instance = new CloudFirestoreService();
            return instance;
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
                DocumentReference documentReference = colRef.Document();
                reportFile.file_id = documentReference.Id;
                var retRef = await documentReference.CreateAsync(reportFile);
                return reportFile.file_name;
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
                foreach (var patientFingerprintTemplate in patient.fingerprint_templates)
                {
                    if (string.IsNullOrEmpty(patientFingerprintTemplate)) continue;
                    byte[] blob1 = Convert.FromBase64String(template.Trim());
                    byte[] blob2 = Convert.FromBase64String(patientFingerprintTemplate.Trim());
                    var cScore = fpDeviceHelper.CompareFingerPrint(blob1, blob2);
                    // Debug.WriteLine("Match template 1 vs template 2 score=" + cScore + "!\n");
                    if (cScore > score && cScore >= 60)
                    {
                        score = cScore;
                        ret = patient;
                        return ret;
                    }
                }
            }

            return ret;
        }

        public async Task<Patient> FindPatientBy(string fieldPath, string fieldValue,
            CancellationToken cancellationToken)
        {
            try
            {
                var patientSnapshot = await patientCollection.WhereEqualTo(fieldPath, fieldValue)
                    .GetSnapshotAsync(cancellationToken);
                if (patientSnapshot != null)
                {
                    var documentSnapshots = patientSnapshot.Documents;
                    if (documentSnapshots.Count >= 1)
                    {
                        foreach (DocumentSnapshot document in documentSnapshots)
                        {
                            var patient = document.ConvertTo<Patient>();
                            return patient;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (FirebaseException exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            return null;
        }

        public async Task<Patient> RetrivePatientByUid(string uid)
        {
            try
            {
                var patientSnapshot = await patientCollection.Document(uid).GetSnapshotAsync(default);
                var patient = patientSnapshot.ConvertTo<Patient>();
                return patient;
            }
            catch (FirebaseException exception)
            {
                Console.WriteLine(exception);
            }

            return null;
        }

        public async Task AddPatient(Patient patient)
        {
            try
            {
                var coll = fireStoreDb.Collection("patients");
                var retRef = await coll.Document(patient.id).CreateAsync(patient);
                // ToastClass.NotifyMin("created! Hospital", retRef.Id);
            }
            catch
            {
                throw;
            }
        }

        public bool LogOut()
        {
            Properties.Settings.Default.displayName = "";
            Properties.Settings.Default.isLogedIn = false;
            Properties.Settings.Default.watchFolder = new StringCollection();
            Properties.Settings.Default.watchFolderState = new StringCollection();
            Properties.Settings.Default.Save();

            ApplicationData.Instance.SetLogout();
            return true;
        }

        public async Task<List<ReportFile>> GetUploadedFiles()
        {
            var ret = new List<ReportFile>();
            if (_isLoggedIn)
            {
                var collection = fireStoreDb.Collection("files");
                Query queryRef = collection.WhereEqualTo("associated_hospitalId", _loggedInHospitlal.uid);


                FirestoreChangeListener listener = queryRef.Listen((snap) =>
                {
                    var invokeParam = new List<ReportFile>();

                    ToastClass.NotifyMin("Changes in /files", "lookup");
                    foreach (var _docSnap in snap.Documents)
                    {
                        invokeParam.Add(_docSnap.ConvertTo<ReportFile>());
                    }

                    OnDbFileChanged.Invoke(invokeParam);
                });

                var docSnap = await queryRef.GetSnapshotAsync();
                foreach (DocumentSnapshot reportFileDoc in docSnap)
                {
                    ReportFile currFile = reportFileDoc.ConvertTo<ReportFile>();
                    ret.Add(currFile);
                }

                return ret;
            }

            else
            {
                return null;
            }
        }
    }
}