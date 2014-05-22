using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.BackgroundTransfer;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Runtime.Serialization;
using System.Windows.Threading;
namespace Margatsni.Utils
{
    public class DownloadInfo
    {
        public DownloadInfo(bool error = false)
        {
            if (error)
            {
                result_ = false;
                tcs_.SetResult(false);
            }
        }
        TaskCompletionSource<bool> tcs_ = new TaskCompletionSource<bool>();
        public async Task<bool> Wait()
        {
            if (!result_)
            {
                return false;
            }
            bool re = await tcs_.Task;
            return re;// await tcs_.Task;
        }
        public bool Result
        {
            get
            {
                return result_;
            }
        }
        public void Complete(bool code){
            result_ = code;
            tcs_.SetResult(code);
            List<TaskCompletionSource<bool>> timed_waiters_cpy = new List<TaskCompletionSource<bool>>(timed_waiters_);
            foreach (var t in timed_waiters_cpy)
            {
                t.SetResult(code);
            }
            if (Complited != null)
            {
                Complited(this, code);
            }
        }
     //   delegate void TPC(object sender, BackgroundTransferEventArgs e);

        public EventHandler<BackgroundTransferEventArgs> GetTPCDelegate()
        {
            return OnTPC;
        }
        public async Task<bool> Wait1()
        {
            if (!result_)
            {
                return false;
            }
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            timed_waiters_.Add(tcs);
            timer.Tick += (a, b) =>
            {
                tcs.SetResult(false);
            };
            timer.Start();
            bool re = await tcs.Task;
            timer.Stop();
            timed_waiters_.Remove(tcs);
            return re;
        }

        void OnTPC(object sender, BackgroundTransferEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                double perc = (double)e.Request.BytesReceived/(double)e.Request.TotalBytesToReceive * 100.0;
                DownloadProgressChanged(this,perc);
            }
        }
        public EventHandler<double> DownloadProgressChanged;
        public EventHandler<bool> Complited;
        private bool result_ = true;
        List<TaskCompletionSource<bool>> timed_waiters_ = new List<TaskCompletionSource<bool>>();
    }


    public class Downloader
    {
        IEnumerable<BackgroundTransferRequest> transferRequests;
        Dictionary<string, DownloadInfo> current_downloads_ = new Dictionary<string,DownloadInfo>();
        Queue<Tuple<string, string, string, DownloadInfo>> download_queue_ = new Queue<Tuple<string, string, string, DownloadInfo>>();
        public async Task<StorageFolder> GetDownloadRoot()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(root_folder_, CreationCollisionOption.OpenIfExists);
        }


        public static Downloader GetDownloader()
        {
            return downloader_;
        }
        public static Downloader Instance
        {
            get
            {
                return GetDownloader();
            }
        }
        static Downloader downloader_ = new Downloader();

        public Downloader()
        {
            InitialTransferStatusCheck();
        }

        private void UpdateRequestsList()
        {
            // The Requests property returns new references, so make sure that
            // you dispose of the old references to avoid memory leaks.
            if (transferRequests != null)
            {
                foreach (var request in transferRequests)
                {
                    request.Dispose();
                }
            }
            transferRequests = BackgroundTransferService.Requests;
        }
        private void InitialTransferStatusCheck()
        {
            UpdateRequestsList();

            foreach (var transfer in transferRequests)
            {
                var di = new DownloadInfo();
                current_downloads_[transfer.DownloadLocation.OriginalString] = di;
                transfer.TransferStatusChanged += transfer_TransferStatusChanged;
                transfer.TransferProgressChanged += di.GetTPCDelegate();
                ProcessTransfer(transfer);
            }

        }

        public class FileDownloadedEventArgs : EventArgs
        {
            public FileDownloadedEventArgs(string folder, string file_name)
            {
                folder_ = folder;
                file_name_ = file_name;
            }
            public string FolderName
            {
                get
                {
                    return folder_;
                }
            }
            public string FileName
            {
                get
                {
                    return file_name_;
                }
            }
            
            private string folder_;
            private string file_name_;
        }

        public event EventHandler<FileDownloadedEventArgs> FileDownloaded; 

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            switch (transfer.TransferStatus)
            {
                case TransferStatus.Completed:
                    {
                        bool re = false;
                        // If the status code of a completed transfer is 200 or 206, the
                        // transfer was successful
                        if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                        {
                            re = true;
                            // Remove the transfer request in order to make room in the 
                            // queue for more transfers. Transfers are not automatically
                            // removed by the system.
                            RemoveTransferRequest(transfer.RequestId);
                            if (FileDownloaded != null)
                            {
                                FileDownloaded( this, 
                                                new FileDownloadedEventArgs(
                                                        System.IO.Path.GetDirectoryName(transfer.DownloadLocation.OriginalString).Replace(string.Format("\\{0}",root_folder_),""),
                                                        System.IO.Path.GetFileName(transfer.DownloadLocation.OriginalString)));
                            }
                        }
                        else
                        {
                            // This is where you can handle whatever error is indicated by the
                            // StatusCode and then remove the transfer from the queue. 
                            RemoveTransferRequest(transfer.RequestId);

                            if (transfer.TransferError != null)
                            {
                                // Handle TransferError if one exists.
                            }
                        }
                        DownloadInfo di = null;

                        if (current_downloads_.TryGetValue(transfer.DownloadLocation.OriginalString, out di))
                        {
                            di.Complete(re);
                            current_downloads_.Remove(transfer.DownloadLocation.OriginalString);
                        }
                        if (download_queue_.Count > 0)
                        {
                            var nexd = download_queue_.Dequeue();
                            StartDownload(nexd.Item1, nexd.Item2, nexd.Item3, nexd.Item4);
                        }

                        break;

                    }
                case TransferStatus.WaitingForExternalPower:
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    break;

                case TransferStatus.WaitingForWiFi:
                    break;
            }
        }

        void transfer_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            ProcessTransfer(e.Request);
        }
        private void RemoveTransferRequest(string transferID)
        {
            BackgroundTransferRequest transferToRemove = BackgroundTransferService.Find(transferID);
            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception e)
            {
                // Handle the exception.
            }
        }

        private async Task<bool> PrepareFolder(string folder_name)
        {
            try
            {
                var loca_folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(root_folder_+folder_name, CreationCollisionOption.OpenIfExists);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool IsDownloading(string folder, string file_name)
        {
            return transferRequests.Any(a =>
            {
                string l_file = a.DownloadLocation.OriginalString;
                string l_file_path = System.IO.Path.GetDirectoryName(l_file);
                string l_file_name = System.IO.Path.GetFileName(l_file);
                string o_folder = System.IO.Path.GetDirectoryName(folder);
                if (l_file_path.EndsWith(o_folder) && file_name.Equals(l_file_name))
                {
                    return true;
                }
                return false;
            });
        }

        public bool IsDownloading(Func<string, bool> predic)
        {
            return transferRequests.Any(a => { return predic(a.DownloadLocation.OriginalString); });
        }

        public DownloadInfo GetDownloadInfo(string folder, string file_name)
        {
            return GetDownloadInfo(a => { return a.Contains(System.IO.Path.Combine(folder, file_name)); });
        }
        public DownloadInfo GetDownloadInfo(Func<string, bool> predic)
        {
            var tr = transferRequests.FirstOrDefault(a => { return predic(a.DownloadLocation.OriginalString); });
            if (tr == null)
            {
                return null;
            }
            DownloadInfo di = null;
            if (current_downloads_.TryGetValue(tr.DownloadLocation.OriginalString, out di))
            {
                return di;
            }
            return null;
        }

        public async Task<bool> DownloadFileAsync(string url, string folder, string file_name)
        {
            return await (await StartDownload(url, folder, file_name)).Wait();
        }

        public async Task<DownloadInfo> StartDownload(string url, string folder, string file_name, DownloadInfo edi = null)
        {
            if (!await PrepareFolder(folder))
            {
                return new DownloadInfo(true);
            }
            string downloadFile = root_folder_ + folder + "\\" + file_name;
            var dic = GetDownloadInfo(folder, file_name);
            if (dic != null)
            {
                return dic;
            }
            var qc = download_queue_.FirstOrDefault(a => { return a.Item2 == folder && a.Item3 == file_name; });
            if (qc != null)
            {
                return qc.Item4; 
            }
            if (BackgroundTransferService.Requests.Count() >= 25)
            {
                // Note: Instead of showing a message to the user, you could store the
                // requested file URI in isolated storage and add it to the queue later.
                //                download_queue_.Enqueue(Tuple.Create<string, string, string>(url, folder, file_name));
                var re = new DownloadInfo();
                download_queue_.Enqueue(Tuple.Create<string,string,string,DownloadInfo>(url, folder, file_name, re));
                return re;
            }
            //     await PreRequest(url);

            // Get the URI of the file to be transferred from the Tag property
            // of the button that was clicked.
            Uri transferUri = new Uri(Uri.EscapeUriString(url), UriKind.RelativeOrAbsolute);

            // Create the new transfer request, passing in the URI of the file to 
            // be transferred.
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(transferUri);
            transferRequest.TransferPreferences = TransferPreferences.AllowBattery;
            // Set the transfer method. GET and POST are supported.
            transferRequest.Method = "GET";
            
            // Get the file name from the end of the transfer URI and create a local URI 
            // in the "transfers" directory in isolated storage.
            Uri downloadUri =  new Uri(downloadFile, UriKind.RelativeOrAbsolute);
            try
            {
                transferRequest.DownloadLocation = downloadUri;
            }
            catch(Exception ex)
            {

            }

            try
            {
                transferRequest.TransferStatusChanged += transfer_TransferStatusChanged;
                var re = edi != null ? edi : new DownloadInfo();
                transferRequest.TransferProgressChanged += re.GetTPCDelegate();
                BackgroundTransferService.Add(transferRequest);
                current_downloads_[transferRequest.DownloadLocation.OriginalString] = re;
                return re;
            }
            catch (InvalidOperationException ex)
            {
              //  MessageBox.Show("Unable to add background transfer request. " + ex.Message);
            }
            catch (Exception)
            {
               // MessageBox.Show("Unable to add background transfer request.");
            }
            return new DownloadInfo(true);
        }
        const string root_folder_ = "shared\\transfers\\";
    }
}
