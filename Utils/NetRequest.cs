using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Margatsni.Utils
{
    public class NetRequest
    {
        public enum Result
        {
            kOK,
            kBadRequest,
            kInternalServerError,
            kUnknonwnError,
            kDidNotExecuted
        }

        public class NetResponce : IDisposable
        {
            public NetResponce(Result result, Stream stream, string ff = "")
            {
                result_ = result;
                responce_stream_ = stream;
                ff_ = ff;
            }
            ~NetResponce()
            {
                if (responce_stream_ != null)
                {
                    responce_stream_.Dispose();
                }
            }
            public Result ReqyestResult
            {
                get { return result_; }
            }
            public Stream GetResultStream()
            {
                return responce_stream_;
            }

            public void Dispose()
            {
                if (responce_stream_ != null)
                {
                    responce_stream_.Dispose();
                }
            }

            Result result_;
            Stream responce_stream_;
            public string ff_ = string.Empty;
        }

        static private async Task<NetResponce> DoRequest(string request_url,
                                                            List<KeyValuePair<string, string>> request_params,
                                                            System.Threading.CancellationToken ct)
        {
            if (request_params == null)
            {
                request_params = new List<KeyValuePair<string, string>>();
            }
            System.Net.Http.HttpResponseMessage rsp = null;
            System.Net.Http.HttpClient cl = new System.Net.Http.HttpClient();
            System.Net.Http.HttpContent content = new System.Net.Http.FormUrlEncodedContent(request_params);
            Stream res_stream = null;
            try
            {
                var t = new UriBuilder(request_url);
                bool flag = true;
                foreach (var rp in request_params)
                {
                    string ap = rp.Key + "=" + rp.Value;
                    if (!flag)
                    {
                        t.Query = t.Query.Substring(1) + "&" + ap;
                    }
                    else
                    {
                        t.Query = ap;
                    }
                    flag = false;
                }
                rsp = await cl.GetAsync(t.Uri, ct);
                res_stream = await rsp.Content.ReadAsStreamAsync();
            }
            catch
            {
            }
            if (rsp == null)
            {
                return new NetResponce(Result.kUnknonwnError, null);//-->
            }
            Result result;// = NetResponce.Result.kOK;
            switch (rsp.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    result = Result.kBadRequest;
                    break;
                case HttpStatusCode.InternalServerError:
                    result = Result.kInternalServerError;
                    break;
                case HttpStatusCode.OK:
                    result = Result.kOK;
                    break;
                default:
                    result = Result.kUnknonwnError;
                    break;
            }

            return new NetResponce(result, res_stream, "");
        }

        static public async Task<T> RequestStatic<T>(string url,
                                                              System.Threading.CancellationToken ct,
                                                              List<KeyValuePair<string, string>> request_params_list)
        {
            T re = default(T);
            using (NetResponce net_resonce = await DoRequest(url, request_params_list, ct))
            {
                try
                {
                    DataContractJsonSerializer serialized = new DataContractJsonSerializer(typeof(T));
                    T s = (T)serialized.ReadObject(net_resonce.GetResultStream());
                    re = s;
                }
                catch
                {
                }

            }
            return re;//-->            
        }
        static public async Task<T> RequestStatic<T>(string url,
                                                      List<KeyValuePair<string, string>> request_params_list)
        {
            using (System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource())
            {
                return await RequestStatic<T>(url, cts.Token, request_params_list);
            }
        }

        public static List<KeyValuePair<string, string>> CreateParams(params string[] par)
        {
            if (par.Length % 2 != 0)
            {
                throw new Exception();
            }
            var re = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < par.Length; i += 2)
            {
                re.Add(new KeyValuePair<string, string>(par[i], par[i + 1]));
            }
            return re;
        }

    }
}
