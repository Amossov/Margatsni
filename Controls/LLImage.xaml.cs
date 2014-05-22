using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Margatsni.Controls
{
    using Managers;
    public sealed partial class LLImage : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public LLImage()
        {
            this.InitializeComponent();
            Unloaded += LLImage_Unloaded;
        }

        void LLImage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (prev_source_ != null)
            {
                prev_source_.PropertyChanged -= prev_source__PropertyChanged;
                prev_source_ = null;
            }      
        }


        private static readonly DependencyProperty SourceUrlProperty =
            DependencyProperty.Register("SourceUrl", typeof(string), typeof(LLImage), new PropertyMetadata(null, new PropertyChangedCallback(SourceUrlChangedCB)));// ((double)100, new PropertyChangedCallback(OnLabelChanged)));

        static void SourceUrlChangedCB(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var a = e.OldValue;
            var b = e.NewValue;
            (obj as LLImage).BS2(b as string);
        }

        void BS2(string url)
        {

            if (prev_source_ != null)
            {
                prev_source_.PropertyChanged -= prev_source__PropertyChanged;
            }
            prev_source_ = Source2;
            Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged("Data");
            });

            prev_source_.PropertyChanged += prev_source__PropertyChanged;
            return;
        }

        void prev_source__PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Data")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    OnPropertyChanged("Data");
                });
            }
            return;
        }
        

        public string SourceUrl
        {
            get
            {
                return (string)GetValue(SourceUrlProperty);
            }
            set
            {
                SetValue(SourceUrlProperty, value);
            }
        }

        public ImageSource Data
        {
            get
            {
                return Source2.Data;
            }
        }
        public ImageManager.CachedImages.BindImageSource Source2
        {
            get
            {
                if (string.IsNullOrEmpty(SourceUrl))
                {
                    return new ImageManager.CachedImages.BindImageSource();//-->
                }
                return image_manager_.Images[SourceUrl];
            }
        }
        private ImageManager.CachedImages.BindImageSource prev_source_ = null;

        static ImageManager image_manager_ = new ImageManager();




    }
}
