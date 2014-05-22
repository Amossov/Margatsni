using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Windows.Storage;
using Margatsni.Utils;

namespace Margatsni.Controls.Managers
{
    public class ImageManager
    {
        public class CachedImages
        {
            public class BindImageSource : Utils.BindableBase
            {
                public ImageSource Data
                {
                    get
                    {
                        ImageSource re = null;
                        if (data2_.TryGetTarget(out re))
                        {
                            return re;
                        }
                        if (file_ == null)
                        {
                            return null;
                        }
                        ReRead();
                        return null;
                    }
                }
                private async void ReRead(){
                    try
                    {
                        using (var str = await file_.OpenReadAsync())
                        {
                            BitmapImage bi = new BitmapImage();
                            using (var str1 = str.AsStreamForRead())
                            {
                                bi.SetSource(str1);
                            }
                            data2_.SetTarget(bi);
                            OnPropertyChanged("Data");
                        }
                    }
                    catch(Exception ex)
                    { 
                    }
                }
                public bool IsFake
                {
                    get
                    {
                        return is_fake_;
                    }
                    set
                    {
                        SetProperty(ref is_fake_, value);
                    }
                }
                public StorageFile ImageFile
                {
                    set
                    {
                        if (value != file_)
                        {
                            file_ = value;
                            data2_.SetTarget(null);
                            OnPropertyChanged("Data");
                        }
                    }
                }
                private ImageSource data_ = null;
                private bool is_fake_ = true;
                WeakReference<ImageSource> data2_ = new WeakReference<ImageSource>(null);
                StorageFile file_ = null;
            }

            public BindImageSource this[string url_str]
            {
                get
                {
                    try
                    {
                        BindImageSource re = null;
                        if (sources_.TryGetValue(url_str, out re))
                        {
                            return re;//-->
                        }
                        re = new BindImageSource();
                        sources_.Add(url_str, re);
                        Utils.AsyncHelpers.RunAsync(() =>
                        {
                            CreateResultImage(re, url_str);
                        });
                        return re;
                    }
                    catch (Exception ex)
                    {
                    }
                    return null;
                }
            }

            public CachedImages()
            {
            }

            private async void CreateResultImage(BindImageSource bis, string str_url)
            {
                string file_name = GetFileNameFromUrl(str_url);
                if (string.IsNullOrEmpty(file_name))
                {
                    return;//-->
                }
                if (await CheckFileCached(file_name))
                {
                    var file = await GetImageCashedFile(file_name);
                    if (file != null)
                    {
                        bis.ImageFile = file;
                    }
                    return;//-->
                }
                bis.ImageFile = await DownloadsHelpers.DownloadFile(str_url, images_folder_, file_name);
                
            }

            private async Task<StorageFile> GetImageCashedFile(string file_name)
            {
                return await DownloadsHelpers.GetDownloadedFile(images_folder_, file_name);
            }

            private async Task<bool> CheckFileCached(string file_name)
            {
                var df = await DownloadsHelpers.GetDownloadedFile(images_folder_, file_name);
                if (df == null)
                {
                    return false;
                }
                try
                {
                    var bp = await df.GetBasicPropertiesAsync();
                    return bp.Size > 0;
                }
                catch
                {
                }
                return false;
            }

            private string GetFileNameFromUrl(string str_url)
            {
                int index = str_url.LastIndexOf('/');
                if (index >= 0)
                {
                    return str_url.Substring(index + 1);
                }
                return string.Empty;
            }

            Dictionary<string, BindImageSource> sources_ = new Dictionary<string, BindImageSource>();
            private string images_folder_ = "images_";

        }
        private CachedImages cached_images_ = new CachedImages();
        public CachedImages Images
        {
            get
            {
                return cached_images_;
            }
        }
    }
}
