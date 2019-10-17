﻿using System;
using Google.Cloud.Firestore;

namespace custom_window.HelperClasses.DataModels
{
    [FirestoreData]
    public class Hospital
    {
        [FirestoreProperty] public string hospital_id { get; set; }
        [FirestoreProperty] public string hospital_pass { get; set; }
        [FirestoreProperty] public string hospital_name { get; set; }
        [FirestoreProperty] public string hospital_email { set; get; }
        [FirestoreProperty] public string hospital_phone_number { get; set; }
        [FirestoreProperty] public string hospital_registration_num { get; set; }
        [FirestoreProperty] public string hospital_location { get; set; }
    }

    [FirestoreData]
    public class Patient
    {
        [FirestoreProperty] public string patient_id { get; set; }
        [FirestoreProperty] public string patient_name { get; set; }
        [FirestoreProperty] public string patient_age { get; set; }
        [FirestoreProperty] public string patient_phone { get; set; }
        [FirestoreProperty] public string patient_email { get; set; }
        [FirestoreProperty] public string patient_address { get; set; }
        [FirestoreProperty] public string patient_fingerprint_template_right_thumb { get; set; }
    }

    [FirestoreData]
    public class ReportFile
    {
        [FirestoreProperty] public string file_name { get; set; }
        [FirestoreProperty] public string file_url { get; set; }
        [FirestoreProperty] public string associated_patientId { get; set; }
        [FirestoreProperty] public string associated_hospitalId { get; set; }
        [FirestoreProperty] public Timestamp file_creation_date { get; set; }
    }
}