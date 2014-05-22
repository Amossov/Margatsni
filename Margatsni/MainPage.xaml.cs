using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
//using System.Collections.Specialized;
using System.Windows.Data;
using Margatsni.Resources;
using Margatsni.Instagram.Core;
using Margatsni.Instagram.Requests;

using Margatsni.Utils.Visual.Extension;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using Windows.ApplicationModel.Core;

namespace Margatsni
{
    public partial class MainPage : PhoneApplicationPage
    {
        Core.CollageModel.CollageModel cmodel_ = new Core.CollageModel.CollageModel();
        Controls.Collections.TopItemsCollection<Core.Data.Item> top_items_ = new Controls.Collections.TopItemsCollection<Core.Data.Item>(16);

        public MainPage()
        {
            InitializeComponent();
            top_items_.Source = Core.MargatsniCore.Instance.Model.Items;
            cmodel_.SourceItems = top_items_.TopItems;//revent_list_.SelectedItems;// Core.MargatsniCore.Instance.Model;
            list_.ItemsSource = cmodel_.Items;
            revent_list_.ItemsSource = Core.MargatsniCore.Instance.Model.Items;
            revent_list_.ItemClicked += revent_list__ItemClicked;
            browser_.Navigating += browser__Navigating;
            var instagram = CoreApplication.Properties["InstagramDataSource"] as DataSources.InstagramDataSource;

            Binding b = new Binding();
            b.Source = instagram.NotReady;
            b.Mode = BindingMode.OneWay;
            b.Path = new PropertyPath("Data");
            b.Converter = new Controls.Converters.BooleanToVisibilityConverter();
            BindingOperations.SetBinding(progress_, ProgressBar.VisibilityProperty, b);

           

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        void revent_list__ItemClicked(object sender, Controls.ListCtrl.ItemClickEventArgs e)
        {
            cmodel_.SourceItems = revent_list_.SelectedItems;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SwitchState(false);
            browser_.Navigate(new Uri("https://api.instagram.com/oauth/authorize/?client_id=9065c5ebcb4c46f2ab138c2dffdb2480&redirect_uri=http://localhost&response_type=token"));

            var instagram = CoreApplication.Properties["InstagramDataSource"] as DataSources.InstagramDataSource;
            if (string.IsNullOrEmpty(instagram.UserId))
            {
                instagram.UserId = HardcoreSettings.InitialUserID;
            }
            else
            {
                instagram.UserId = instagram.UserId;
            }
        }

        void browser__Navigating(object sender, NavigatingEventArgs e)
        {
            var tok = InsatgramAccesTokenListener.GetToken(e.Uri);
            if (!string.IsNullOrEmpty(tok)){
                SwitchState(true);
                var instagram = CoreApplication.Properties["InstagramDataSource"] as DataSources.InstagramDataSource;
                instagram.Request.AccessToken = tok;
            }
        }

        private async void ReReqClicked(object sender, RoutedEventArgs e)
        {
            var instagram = CoreApplication.Properties["InstagramDataSource"] as DataSources.InstagramDataSource;

            var id = await instagram.Request.RequestUserId(user_name_.Text);
            if (!string.IsNullOrEmpty(id))
            {
                cmodel_.SourceItems = top_items_.TopItems;
                instagram.UserId = id;
            }
            else
            {
                MessageBox.Show("user not found or found to many or net problems"); 
            }
        }

        private void ShareCliecked(object sender, RoutedEventArgs e)
        {
            var c = list_.GetVisualChild<Canvas>();
            if (c != null)
            {
                var bmp = new WriteableBitmap(c, null);
                var width = (int)bmp.PixelWidth;
                var height = (int)bmp.PixelHeight;
                using (var ms = new MemoryStream(width * height * 4))
                {
                    bmp.SaveJpeg(ms, width, height, 0, 100);
                    ms.Seek(0, SeekOrigin.Begin);
                    var lib = new Microsoft.Xna.Framework.Media.MediaLibrary();
                    var picture = lib.SavePicture(string.Format("test.jpg"), ms);

                    var task = new Microsoft.Phone.Tasks.ShareMediaTask();

                    task.FilePath = picture.GetPath();

                    task.Show();
                }
            }

        }

        private void SwitchState(bool logined)
        {
            fun_block_.Visibility = logined ? Visibility.Visible : Visibility.Collapsed;
            browser_.Visibility = logined ? Visibility.Collapsed : Visibility.Visible;
        }
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}