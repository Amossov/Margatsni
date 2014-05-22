using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
namespace Margatsni.Utils
{
    public class DownloadsHelpers
    {
        public static async Task<StorageFile> GetDownloadedFile(string папка, string имя_файла)
        {
            try
            {
                StorageFolder папка_загрузок = await Downloader.Instance.GetDownloadRoot();
                StorageFolder папка_загрузки = await папка_загрузок.GetFolderAsync(папка);
                return await папка_загрузки.GetFileAsync(имя_файла);
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return null;
            }
            catch
            {
            }
            return null;
        }
        public static async Task<bool> CheckIsDownlodingOrDownloaded(string folder, string file_name)
        {
            var is_downloading_now = Downloader.Instance.IsDownloading(folder, file_name);
            if (is_downloading_now)
            {
                return true;
            }
            return await GetDownloadedFile(folder, file_name) != null;
        }
        public static async Task<StorageFile> DownloadFile(string url,string folder, string file_name)
        {
            var di = Downloader.Instance.GetDownloadInfo(folder, file_name);
            if (di != null)
            {
                if (await di.Wait())
                {
                    return await GetDownloadedFile(folder, file_name);
                }
            }
            else
            {
                var ok = await Downloader.Instance.DownloadFileAsync(url, folder, file_name);
                if (ok)
                {
                    return await GetDownloadedFile(folder, file_name);
                }
            }
            return null;
        }
    }
}
