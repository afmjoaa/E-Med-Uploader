using System.Windows;
using custom_window.ViewModels;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {

        #region private members

        /// <summary>
        /// the view model for this window 
        /// </summary>
        private DialogWindowViewModel mViewModel;

        #endregion

        #region public properties

        public DialogWindowViewModel ViewModel
        {
            get => mViewModel;
            set
            {
                mViewModel = value;
                DataContext = mViewModel;
            }
        }

        #endregion

        #region constructor

        public DialogWindow()
        {
            InitializeComponent();

        }

        #endregion

    }
}
