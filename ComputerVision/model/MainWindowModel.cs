using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ComputerVision.model
{
    class MainWindowModel : INotifyPropertyChanged
    {

        ImageProcessing ImageProcessing = new ImageProcessing();

        private string sourceUri;
        public string SourceUri
        {
            get => sourceUri;
            set
            {
                sourceUri = value;
                OnProperyChanged(nameof(SourceUri));
            }
        }

        private BitmapSource sourceBitmapImage; 
        public BitmapSource SourceBitmapImage
        {
            get => sourceBitmapImage ?? new BitmapImage(new Uri("/Assets/defaultIMG.png", UriKind.RelativeOrAbsolute));
            set
            { 
                sourceBitmapImage = value;
                OnProperyChanged(nameof(SourceBitmapImage));
            }
        }

        private BitmapSource resultBitmapImage;
        public BitmapSource ResultBitmapImage
        {
            get => resultBitmapImage ?? new BitmapImage(new Uri("/Assets/defaultIMG.png", UriKind.RelativeOrAbsolute));
            set
            {
                resultBitmapImage = value;
                OnProperyChanged(nameof(ResultBitmapImage));
            }
        }

        #region radioButton Properties
        private bool mask3x3;
        public bool Mask3x3
        {
            get => mask3x3;
            set
            {
                mask3x3 = value;
                OnProperyChanged(nameof(Mask3x3));
            }
        }

        private bool mask5x5;
        public bool Mask5x5
        {
            get => mask5x5;
            set
            {
                mask5x5 = value;
                OnProperyChanged(nameof(Mask5x5));
            }
        }

        private bool mask7x7;
        public bool Mask7x7
        {
            get => mask7x7;
            set
            {
                mask7x7 = value;
                OnProperyChanged(nameof(mask7x7));
            }
        }
        #endregion

        #region function
        public ICommand SoreceLoadButton { get; set; }
        void SoreceLoadFunc(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            if(openFileDialog.FileName.Length > 0)
            {
                SourceBitmapImage = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                SourceUri = openFileDialog.FileName;
            }
        }

        public ICommand MeanFilterFunc { get; set; }

        void MeanFilterCommand(object obj)
        {
            ImageProcessing.Filtering = new MeanFilter();
            if(mask3x3)
            {
                ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage,3);
            }else if (mask5x5)
            {
                ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 5);
            }
            else
            {
                ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 7);
            }
            
        }


        public ICommand MedianFilterFunc { get; set; }

        void MedianFilterCommand(object obj)
        {
            ImageProcessing.Filtering = new MedianFilter();
            if (mask3x3)
            {
                ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 3);
            }
            else if (mask5x5)
            {
                ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 5);
            }
            else
            {
                ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 7);
            }
        }
        public ICommand LaflasianCommand { get; set; }

        void LaflasianFilterCommand(object obj)
        {

            ImageProcessing.Filtering = new LafilasianFilter();
 
            ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 3);
           
        }


        public ICommand EnhanceLaflasianCommand { get; set; }
        void EnhanceLaflasianFilterCommand(object obj)
        {

            ImageProcessing.Filtering = new EnhanceLafilasianFilter();

            ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 3);

        }

        public ICommand BigLaflasianCommand { get; set; }
        void BigLaflasianFilterCommand(object obj)
        {

            ImageProcessing.Filtering = new BigLafilasianFilter();

            ResultBitmapImage = ImageProcessing.ImageProcess(SourceBitmapImage, 5);

        }


        public ICommand SabeButton { get; set; }
        void SabeButtonFunc(object obj)
        {
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();

            jpegBitmapEncoder.QualityLevel = 50;
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(ResultBitmapImage));

            using (FileStream fileStream = new FileStream(SourceUri.Substring(0, SourceUri.LastIndexOf(@"\")+1) + new Random().Next(0,101)+".jpeg", FileMode.Create, FileAccess.Write))
            {

                jpegBitmapEncoder.Save(fileStream);

            }

        }

        #endregion

        #region ctor and INotifyPropertyChangde
        public MainWindowModel()
        {
            Mask3x3 = true;
            SoreceLoadButton = new Command(SoreceLoadFunc, null);
            MeanFilterFunc = new Command(MeanFilterCommand, null);
            MedianFilterFunc = new Command(MedianFilterCommand, null);
            SabeButton = new Command(SabeButtonFunc, null);
            LaflasianCommand = new Command(LaflasianFilterCommand,null);
            EnhanceLaflasianCommand = new Command(EnhanceLaflasianFilterCommand, null);
            BigLaflasianCommand = new Command(BigLaflasianFilterCommand, null);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    class Command : ICommand
    {
        Action<Object> excuteMethod;
        Func<object, bool> canExcute;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public Command(Action<Object> excuteMethod,Func<object,bool> canExcute)
        {
            this.excuteMethod = excuteMethod;
            this.canExcute = canExcute;
            CanExecuteChanged = (sender, e) => System.Console.WriteLine("조건 변경");
        }

        public void Execute(object parameter)
        {
            excuteMethod(parameter);
        }
    }
}
