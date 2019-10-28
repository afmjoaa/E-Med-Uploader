namespace custom_window.Core
{
    /// <summary>
    /// content type of the patientInfoCheck control
    /// existing account with/without nid is fingerprint detected 
    /// </summary>
    public enum ContentType
    {
        //if no account is available search by fingerprint
        NewPatientRegistration = 0,

        //search patient by mail id
        ExistingPatientInfo = 1,

    }
}
