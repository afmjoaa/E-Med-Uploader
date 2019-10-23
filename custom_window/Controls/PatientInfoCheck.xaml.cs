using System.Windows.Controls;
using custom_window.Core;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for PatientInfoCheck.xaml
    /// </summary>
    public partial class PatientInfoCheck : UserControl
    {
        public PatientInfoCheck()
        {
            InitializeComponent();
            DataContext = IoC.Get<PatientInfoCheckViewModel>();
        }
    }
}
