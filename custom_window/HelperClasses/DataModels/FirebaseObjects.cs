using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        [FirestoreProperty] public string id { get; set; }
        [FirestoreProperty] public string name { get; set; }
        [FirestoreProperty] public string phone { get; set; }
        [FirestoreProperty] public string email { get; set; }
        [FirestoreProperty] public string birth { get; set; }
        [FirestoreProperty] public string permanent_address { get; set; }
        [FirestoreProperty] public string present_address { get; set; }
        [FirestoreProperty] public string voting_area { get; set; }
        [FirestoreProperty] public string issue_date { get; set; }
        [FirestoreProperty] public string pk { get; set; }
        [FirestoreProperty] public string signature { get; set; }
        [FirestoreProperty] public string display_pic { get; set; }
        [FirestoreProperty] public List<String> fingerprint_templates { get; set; } = new List<string>();
        [FirestoreProperty] public string old_nid { get; set; }
        [FirestoreProperty] public string new_nid { get; set; }
        [FirestoreProperty] public string firebaseAuthToken { get; set; }
        [FirestoreProperty] public string firebaseRefreshToken { get; set; }
        [FirestoreProperty] public string FederatedId { get; set; }
    }

    [FirestoreData]
    public class ReportFile
    {
        [FirestoreProperty] public string file_id { get; set; }
        [FirestoreProperty] public string file_name { get; set; }
        [FirestoreProperty] public string file_size { get; set; }
        [FirestoreProperty] public string file_url { get; set; }
        [FirestoreProperty] public string file_type { get; set; }
        [FirestoreProperty] public string associated_patientId { get; set; }
        [FirestoreProperty] public string associated_hospitalId { get; set; }
        [FirestoreProperty] public Timestamp file_creation_date { get; set; }
    }
}