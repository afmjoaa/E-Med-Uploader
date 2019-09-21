using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace custom_window
{
    public sealed class HistoryListDm : HistoryListVm
    {
        private static HistoryListDm PrivateInstance { get; set; }
        private static readonly object padlock = new object();

        public static HistoryListDm Instance
        {
            get
            {
                lock (padlock)
                {
                    return PrivateInstance ?? (PrivateInstance = new HistoryListDm());
                }
            }
        }


        public HistoryListDm()
        {
            Items = new List<HistoryItemVm>
            {
                new HistoryItemVm
                {
                    Name = "ScreenShot 5678",
                    ReportType = "X-Ray Report",
                    RecieverID = "12345678",
                    Date = "12 July, 2019",
                    Status = "Uploaded",
                    Size = "4563 KB",
                    FileTypeImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/ai.png"))
                },
                new HistoryItemVm
                {
                    Name = "ScreenShot 5678",
                    ReportType = "X-Ray Report",
                    RecieverID = "12345678",
                    Date = "12 July, 2019",
                    Status = "Uploaded",
                    Size = "4563 KB",
                    FileTypeImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/pdf.png"))
                },
                new HistoryItemVm
                {
                    Name = "ScreenShot 5678",
                    ReportType = "X-Ray Report",
                    RecieverID = "12345678",
                    Date = "12 July, 2019",
                    Status = "Uploaded",
                    Size = "4563 KB",
                    FileTypeImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/dcm.png"))
                },
                new HistoryItemVm
                {
                    Name = "ScreenShot 5678",
                    ReportType = "X-Ray Report",
                    RecieverID = "12345678",
                    Date = "12 July, 2019",
                    Status = "Uploaded",
                    Size = "4563 KB",
                    FileTypeImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/ai.png"))
                },
                new HistoryItemVm
                {
                    Name = "ScreenShot 5678",
                    ReportType = "X-Ray Report",
                    RecieverID = "12345678",
                    Date = "12 July, 2019",
                    Status = "Uploaded",
                    Size = "4563 KB",
                    FileTypeImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/ai.png"))
                },
            };
        }
    }
}