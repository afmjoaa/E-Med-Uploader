using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using custom_window.Core;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// custom startup 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {   
            // firebase storage

            //let the base application do what it needs 
            base.OnStartup(e);
            //setup IoC
            IoC.Setup();
            // show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

        }
    }
}
