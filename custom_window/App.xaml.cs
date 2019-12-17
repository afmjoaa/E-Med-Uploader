using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using custom_window.Core;
using custom_window.IocHere;
using custom_window.ViewModels.folder;
using custom_window.ViewModels.side;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Win32;
using Squirrel;
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

            /*string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true);
            key.SetValue("E-Med Uploader", System.Reflection.Assembly.GetExecutingAssembly().Location);*/


            checkForUpdates();


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


        private async Task checkForUpdates()
        {
            using (var manager = new UpdateManager(@"C:\Emed\Releases"))
            {
                await manager.UpdateApp();
            }
        }

    }
}