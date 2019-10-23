using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;

namespace custom_window.Core
{
    public class BaseDialogViewModel : BaseViewModel
    {
        public string Title { get; set; }
    }
}
