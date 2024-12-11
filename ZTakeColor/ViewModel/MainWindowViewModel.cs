using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Threading;
using Windows.Win32;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows;
using System.Security.Cryptography;
using System.Windows.Media;
using System;
using ZTakeColor.Common;

namespace ZTakeColor.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            //系统DPI识别。此应用程序无法扩展DPI更改。它将查询DPI一次，并在应用程序的生命周期中使用该值。如果DPI发生变化，则应用程序将不会调整为新的DPI值。当DPI从系统值更改时，系统将自动按比例放大或缩小。
            PInvoke.SetProcessDpiAwareness(Windows.Win32.UI.HiDpi.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE);
            _isTop = Properties.Settings.Default.TopUp;
            _scale = Properties.Settings.Default.Scale;
            _imgColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xFF, 0xFF));
            _coordinate = "[0,0]";
            _currentColor = new ColorType();
            _currentColor.Code = "#FFFFFF";
            _currentColor.R = 255;
            _currentColor.G = 255;
            _currentColor.B = 255;
            _currentColor.CodeType = 0;

            StartTimer();

        }
        System.Drawing.Point Mouse;
        [ObservableProperty]
        private bool _isTop;
        [ObservableProperty]
        private int _scale;
        [ObservableProperty]
        private WriteableBitmap _wriImage;
        [ObservableProperty]
        private SolidColorBrush _imgColor;
        [ObservableProperty]
        private string _coordinate;

        private ColorType _currentColor;
        public ColorType CurrentColor
        {

            get => _currentColor;
            set {
                _currentColor = value;
                OnPropertyChanged();

            } 
        }



        private void StartTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(GetScreenshot);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }
        private void GetScreenshot(object sender, EventArgs e)
        {
            var sc = Properties.Settings.Default.Scale;
            if (sc == 0)
            {
                sc = 1;
            }
            if (WriImage == null)
            {
                WriImage = new WriteableBitmap(10 * Math.Abs(sc) - 1, 10 * Math.Abs(sc) - 1, 0, 0, System.Windows.Media.PixelFormats.Bgra32, null);
            }
            try
            {
                using (var bitmap = new Bitmap(10 * Math.Abs(sc) - 1, 10 * Math.Abs(sc) - 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (Graphics memoryGrahics = Graphics.FromImage(bitmap))
                {
                    //获取的鼠标位置
                    PInvoke.GetCursorPos(out Mouse);
                    Coordinate = "[" + Mouse.X + "," + Mouse.Y + "]";
                    //鼠标位置偏移高宽的一半就是中心点
                    memoryGrahics.CopyFromScreen(Mouse.X - (10 * Math.Abs(sc) / 2 - 1), Mouse.Y - (10 * Math.Abs(sc) / 2 - 1), 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                    var color = bitmap.GetPixel((10 * Math.Abs(sc) / 2 - 1), (10 * Math.Abs(sc) / 2 - 1));
                    ImgColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
                    CurrentColor.R = color.R;
                    CurrentColor.G = color.G;
                    CurrentColor.B = color.B;
                    if (CurrentColor.CodeType == 0)
                    {
                        CurrentColor.Code = "#" + CurrentColor.R.ToString("X2") + CurrentColor.G.ToString("X2") + CurrentColor.B.ToString("X2");
                    }
                    else
                    {

                    }
                    if (sc > 0)
                    {
                        var data = bitmap.LockBits(new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        WriImage.WritePixels(new Int32Rect(0, 0, bitmap.Width, bitmap.Height), data.Scan0, data.Height * data.Stride, data.Stride, 0, 0);
                        bitmap.UnlockBits(data);
                    }
                }

            }
            catch (Exception)
            {
                Debug.WriteLine("!");
            }
        }
        [RelayCommand]
        private void TopUpClick()
        {
            if (IsTop)
            {
                IsTop = false;
                Properties.Settings.Default.TopUp = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                IsTop = true;
                Properties.Settings.Default.TopUp = true;
                Properties.Settings.Default.Save();
            }
        }
        [RelayCommand]
        private void TripleClick()
        {
            var sc = 3;
            Properties.Settings.Default.Scale = sc;
            Properties.Settings.Default.Save();
            WriImage = new WriteableBitmap(10 * Math.Abs(sc) - 1, 10 * Math.Abs(sc) - 1, 0, 0, System.Windows.Media.PixelFormats.Bgra32, null);
        }
        [RelayCommand]
        private void NonupleClick()
        {
            var sc = 1;
            Properties.Settings.Default.Scale = sc;
            Properties.Settings.Default.Save();
            WriImage = new WriteableBitmap(10 * Math.Abs(sc) - 1, 10 * Math.Abs(sc) - 1, 0, 0, System.Windows.Media.PixelFormats.Bgra32, null);
        }
        [RelayCommand]
        private void ProhibitClick()
        {
            var sc = Properties.Settings.Default.Scale;
            if (sc == 0)
            {
                sc = 1;
            }
            Properties.Settings.Default.Scale = 0 - sc;
            Properties.Settings.Default.Save();
            WriImage = new WriteableBitmap(10 * Math.Abs(sc) - 1, 10 * Math.Abs(sc) - 1, 0, 0, System.Windows.Media.PixelFormats.Bgra32, null);
        }
    }

}
