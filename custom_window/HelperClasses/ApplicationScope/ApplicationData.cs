using System.Threading;
using System.Threading.Tasks;
using custom_window;
using custom_window.HelperClasses;

namespace custom_window
{
    public sealed class ApplicationData
    {
        private static ApplicationData instance = null;

        private static Hospital userHospital = null;


        #region constructor
        public static ApplicationData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApplicationData();
                }
                return instance;
            }
        }
        #endregion

        #region public get method

        public async Task<Hospital> GetUserHospital(CancellationToken cancellationToken)
        {
            if (Properties.Settings.Default.isLogedIn)
            {
                if (userHospital == null)
                {
                    userHospital = await CloudFirestoreService.GetInstance().GetLoggedInHospital(Properties.Settings.Default.localId, cancellationToken);
                    return userHospital;
                }
                else
                {
                    return userHospital;
                }
            }
            else
            {
                return null;
            }
            
        }

        public void SetLogout()
        {
            userHospital = null;
        }


        #endregion

    }
}
