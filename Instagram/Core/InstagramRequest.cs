using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Instagram.Core
{
    public class InstagramRequest
    {
        public async Task<T> Request<T>(string point, List<KeyValuePair<string, string>> add_params, bool use_client_id)
        {
            string url_str = url_base_ + point;
            List<KeyValuePair<string, string>> req_params = new List<KeyValuePair<string, string>>();
            if (add_params != null)
            {
                req_params.AddRange(add_params);
            }
            if (use_client_id)
            {
                req_params.Add(new KeyValuePair<string,string>("client_id", ClientId));
            }else{
                req_params.Add(new KeyValuePair<string,string>("access_token", AccessToken));
            }
            return await Utils.NetRequest.RequestStatic<T>(url_str, req_params);
        }

        public string AccessToken
        {
            get
            {
                return access_token_;
            }
            set
            {
                access_token_ = value;
            }
        }
        public string ClientId
        {
            get
            {
                return client_id_;
            }
            set
            {
                client_id_ = value;
            }
        }


        private const string url_base_ = "https://api.instagram.com/v1";
        private string access_token_ = string.Empty;
        private string client_id_ = string.Empty;
    }
}
