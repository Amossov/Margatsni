using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Instagram.Requests
{
    public static class RecentImagesExtension
    {
        public static async Task<Data.Response.ImagesOnlyCollectionResponse> RequestRecentUserImages(this Core.InstagramRequest instagram_request, string user_id)
        {
            return await instagram_request.Request<Data.Response.ImagesOnlyCollectionResponse>(string.Format("/users/{0}/media/recent", user_id), null, true);
        }
    }
}
