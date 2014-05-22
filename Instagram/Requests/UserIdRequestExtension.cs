using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Instagram.Requests
{
    public static class UserIdRequestExtension
    {
        public static async Task<string> RequestUserId(this Core.InstagramRequest instagram_request, string user_name)
        {
            var t = await instagram_request.Request<Data.Response.UserSearchCollectionResponse>("/users/search", Utils.NetRequest.CreateParams("q",user_name), false);
            if (t == null || t.data == null || t.data.Count == 0 || t.data.Count > 1)
            {
                return string.Empty;
            }
            return t.data.First().id;
        }

    }
}
