using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace custom_window.HelperClasses
{
    class ToastClass
    {
        private readonly NotifyIcon _notifyIcon;

        public ToastClass()
        {
            _notifyIcon = new NotifyIcon();
            // Extracts your app's icon and uses it as notify icon
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            // Hides the icon when the notification is closed
            _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
        }

        public void ShowNotification(string title, string msg, int timeOut)
        {
            _notifyIcon.Visible = true;
            // Shows a notification with specified message and title
            _notifyIcon.ShowBalloonTip(timeOut, title, msg, ToolTipIcon.Info);
        }

        public static void NotifyMin(string title, string message)
        {
            var _notifyIcon = new NotifyIcon();

            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
            _notifyIcon.Visible = true;

            _notifyIcon.ShowBalloonTip(2000, title, message, ToolTipIcon.Info);
        }
    }
}