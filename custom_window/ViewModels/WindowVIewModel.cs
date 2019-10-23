using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using custom_window.Core;

namespace custom_window
{
    /// <summary>
    /// The view model for the custom flat window  
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        private Window mWindow;
        private int mOuterMarginSize = 10;
        private int mWindowRadius = 8;

        #region getter setter

        /// <summary>
        /// true if have a dimmed overlay on the window
        /// </summary>
        public bool DimableOverlayVisible { get; set; }

        public Thickness InnerContentPadding
        {
            get { return new Thickness(0); }
        }

        public double WindowMinimumWidth { get; set; } = 960;
        public double WindowMinimumHeight { get; set; } = 630;

        public int ResizeBorder
        {
            get { return mWindow.WindowState == WindowState.Maximized ? 0 : 6; }
        }

        public Thickness ResizeBorderThickness
        {
            get { return new Thickness(ResizeBorder + OuterMarginSize); }
        }

        public int OuterMarginSize
        {
            get { return mWindow.WindowState == WindowState.Maximized ? 0 : mOuterMarginSize; }
            set { mOuterMarginSize = value; }
        }

        public Thickness OuterMarginSizeThickness
        {
            get { return new Thickness(OuterMarginSize); }
        }

        public int WindowRadius
        {
            get { return mWindow.WindowState == WindowState.Maximized ? 0 : mWindowRadius; }
            set { mOuterMarginSize = value; }
        }

        public CornerRadius WindowCornerRadius
        {
            get { return new CornerRadius(WindowRadius); }
        }

        public int TitleHeight { get; set; } = 38;

        public GridLength TitleHeightGridLength
        {
            get { return new GridLength(TitleHeight + ResizeBorder); }
        }

        /// <summary>
        /// true if we want to 
        /// </summary>
        public bool PatientInfoCheckVisible
        {
            get => IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible;
            set => IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = value;
        }

        #endregion

        #region constructor

        public WindowViewModel(Window window)
        {
            mWindow = window;
            //fix window resize issue 
            var resizer = new WindowResizer(mWindow);

            mWindow.StateChanged += (sender, e) =>
            {
                //fire off events for all properties that are affected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginSizeThickness));
                OnPropertyChanged(nameof(WindowRadius));
                OnPropertyChanged(nameof(WindowCornerRadius));
            };

            //create command
            MinimizeCommand = new RelayCommand(() => { mWindow.WindowState = WindowState.Minimized; });
            MaximizeCommand = new RelayCommand(() => { mWindow.WindowState ^= WindowState.Maximized; });
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, getCurrentMousePosition()));
        }

        #endregion

        #region window commands

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }

        #endregion

        #region helper

        private Point getCurrentMousePosition()
        {
            var position = Mouse.GetPosition(mWindow);
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }

        #endregion
    }
}