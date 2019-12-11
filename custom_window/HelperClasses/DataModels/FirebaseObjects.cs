using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using custom_window.HelperClasses;
using Google.Cloud.Firestore;

namespace custom_window
{
    [FirestoreData]
    public class Hospital
    {
        [FirestoreProperty] public string uid { get; set; }
        [FirestoreProperty] public string registrationNumber { get; set; }
        [FirestoreProperty] public string mobileNumber { get; set; }
        [FirestoreProperty] public string email { get; set; }
        [FirestoreProperty] public string photoUrl { set; get; }
        [FirestoreProperty] public string hospitalName { get; set; }
        [FirestoreProperty] public string firebaseAuthToken { get; set; }
        [FirestoreProperty] public bool isEmailVerified { get; set; }
        [FirestoreProperty] public GeoPoint location { get; set; }

    }

    [FirestoreData]
    public class Patient
    {
        [FirestoreProperty] public string patient_id { get; set; }
        [FirestoreProperty] public string patient_name { get; set; }
        [FirestoreProperty] public string patient_phone { get; set; }
        [FirestoreProperty] public string patient_email { get; set; }
        [FirestoreProperty] public string patient_birth { get; set; }
        [FirestoreProperty] public string patient_permanent_address { get; set; }
        [FirestoreProperty] public string patient_present_address { get; set; }
        [FirestoreProperty] public string patient_fingerprint_template_right_thumb { get; set; }
        
        [FirestoreProperty] public string patient_old_nid { get; set; }
        [FirestoreProperty] public string patient_new_nid { get; set; }
    }

    [FirestoreData]
    public class ReportFile
    {
        [FirestoreProperty] public string file_id { get; set; }
        [FirestoreProperty] public string file_name { get; set; }
        [FirestoreProperty] public string file_url { get; set; }
        [FirestoreProperty] public string associated_patientId { get; set; }
        [FirestoreProperty] public string associated_hospitalId { get; set; }
        [FirestoreProperty] public Timestamp file_creation_date { get; set; }
    }
}