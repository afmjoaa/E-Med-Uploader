using System.Threading;
using System.Threading.Tasks;
using custom_window;

namespace custom_window.Core
{
    public class ApplicationViewModel : BaseViewModel
    {
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;

        public bool CurrentWindowVisible { get; set; } = true;

        /// <summary>
        /// true if the side menu is to be shown
        /// </summary>
        public bool SideMenuVisible { get; set; } = false;


        /// <summary>
        /// navigate to specified page
        /// </summary>
        /// <param name="pageName"></param>
        public void GoToPage(ApplicationPage pageName)
        {
            this.CurrentPage = pageName;
            //show side menu or not
            if (pageName != ApplicationPage.Login && pageName != ApplicationPage.Register)
            {
                SideMenuVisible = true;
            }else
            {
                SideMenuVisible = false;
            }
        }
    }
}