namespace custom_window.Core
{
    /// <summary>
    /// Details for a dialog
    /// </summary>
    public class ConfirmDialogViewModel : BaseDialogViewModel
    {
        public string Message { get; set; }

        public string yesText { get; set; }

        public string noText { get; set; }

        public string dialogType { get; set; }
    }
}