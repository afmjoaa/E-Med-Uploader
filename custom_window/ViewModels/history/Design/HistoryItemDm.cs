﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace custom_window
{
    public class HistoryItemDm : HistoryItemVm
    {
        public static HistoryItemDm Instance => new HistoryItemDm();

        public HistoryItemDm()
        {
            Name = "ScreenShot 5678";
            ReportType = "X-Ray Report";
            RecieverID = "12345678";
            Date = "12 July, 2019";
            Status = "Uploaded";
            Size = "4563 KB";
            FileTypeImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/ai.png"));
        }
    }
}