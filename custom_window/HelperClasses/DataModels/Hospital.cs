using System;
using System.Windows.Forms;
using Google.Cloud.Firestore;

namespace custom_window.HelperClasses.DataModels
{
    [FirestoreData]
    public class Hospital
    {
        [FirestoreProperty] public string hospital_id { get; set; }
        [FirestoreProperty] public string hospital_pass { get; set; }
        [FirestoreProperty] public string hospital_hashed_pass { get; set; }
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
        [FirestoreProperty] public string file_id { get; set; }
        [FirestoreProperty] public string file_name { get; set; }
        [FirestoreProperty] public string file_url { get; set; }
        [FirestoreProperty] public string associated_patientId { get; set; }
        [FirestoreProperty] public string associated_hospitalId { get; set; }
        [FirestoreProperty] public Timestamp file_creation_date { get; set; }
    }

    public static class Prompt // temporary prompt for 
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() {Left = 50, Top = 20, Text = text};
            TextBox textBox = new TextBox() {Left = 50, Top = 50, Width = 400};

            /*Label textLabel = new Label() {Left = 50, Top = 40, Text = text};
            TextBox textBox = new TextBox() {Left = 50, Top = 50, Width = 400};*/


            Button confirmation = new Button()
                {Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK};
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}