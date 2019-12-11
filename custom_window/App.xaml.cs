using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using custom_window.Core;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using IoC = custom_window.Core.IoC;

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

            //setup the main application
            ApplicationSetup();

            // show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

        }

        private void ApplicationSetup()
        {
            IoC.Setup();

            //bind a ui manager
            IoC.kernel.Bind<IUIManager>().ToConstant(new UIManager());

            //initializing the login state of the app
            if (custom_window.Properties.Settings.Default.isLogedIn &&
                !string.IsNullOrEmpty(custom_window.Properties.Settings.Default.displayName))
            {
                SideMenuVm.Instance.UpdateSideMenu();
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
            }
            else if (custom_window.Properties.Settings.Default.isLogedIn &&
                     string.IsNullOrEmpty(custom_window.Properties.Settings.Default.displayName))
            {
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Register);
            }
            else
            {
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
            }

            //initialize the watcher state
            var itemIndex = 0;
            var mPauseBtnEnable = false;
            if (custom_window.Properties.Settings.Default.watchFolder != null && custom_window.Properties.Settings.Default.watchFolderState != null)
            {
                //initialize the folder watcher with state 
                foreach (var currentItem in custom_window.Properties.Settings.Default.watchFolder)
                {
                    if (custom_window.Properties.Settings.Default.watchFolderState[itemIndex] == "true")
                    {
                        mPauseBtnEnable = true;
                    }
                    else
                    {
                        mPauseBtnEnable = false;
                    }
                    var folderItemVm = new FolderItemVm()
                    {
                        path = currentItem,
                        playBtnEnable = !mPauseBtnEnable,
                        pauseBtnEnable = mPauseBtnEnable
                    };
                    folderItemVm.PlayFolderWatching();
                    FolderListVm.Instance.myItem.Add(folderItemVm);

                    //update item count
                    itemIndex++;
                }
            }
        }


       

    }
}