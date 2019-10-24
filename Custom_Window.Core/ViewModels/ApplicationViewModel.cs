namespace custom_window.Core
{
    public class ApplicationViewModel : BaseViewModel
    {
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;

        /// <summary>
        /// true if the side menu is to be shown
        /// </summary>
        public bool SideMenuVisible { get; set; } = false;

        

        public string pName { get; set; }
        public string pPhone { get; set; }

        /// <summary>
        /// navigate to specified page
        /// </summary>
        /// <param name="pageName"></param>
        public void GoToPage(ApplicationPage pageName)
        {
            CurrentPage = pageName;
            //show side menu or not
            if (pageName != ApplicationPage.Login && pageName != ApplicationPage.Register)
            {
                SideMenuVisible = true;
            }
        }
    }
}