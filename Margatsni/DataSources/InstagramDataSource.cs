using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


using Margatsni.Instagram.Core;
using Margatsni.Instagram.Requests;

namespace Margatsni.DataSources
{
    public class InstagramDataSource
    {
        public class PushSource:Core.I.IMargatsniPushSource
        {
            public event EventHandler Reseted;
            public event EventHandler<Core.I.ItemUpdateEventArgs> UpdateItem;
            public void FireItemUpdate(Core.I.IMargatsniDataItem item)
            {
                if (UpdateItem != null)
                {
                    UpdateItem(this, new Core.I.ItemUpdateEventArgs(item));
                }
            }
            public void FireReseted()
            {
                if (Reseted != null)
                {
                    Reseted(this, new EventArgs());
                }

            }
        }

        public Utils.BindableEx<bool> NotReady
        {
            get
            {
                return notready_;
            }
        }
        Utils.BindableEx<bool> notready_ = new Utils.BindableEx<bool>(false);
        public InstagramDataSource()
        {
            instagram_request_.ClientId = HardcoreSettings.InstagramClientId;
        }

        public string UserId
        {
            get
            {
                return user_id_;
            }
            set
            {
                var prev = user_id_;
                user_id_ = value;
                if (prev != user_id_)
                {
                    push_source_.FireReseted();
                }
                if (!string.IsNullOrEmpty(user_id_))
                {
                    DoRequest();
                }
            }
        }

        public object Source
        {
            get
            {
                return push_source_;
            }
        }
        private async void DoRequest()
        {
            await DoRequestInternal();
        }

        private async Task DoRequestInternal()
        {
            notready_.Data = true;
            var res = await instagram_request_.RequestRecentUserImages(UserId);
            if (res == null || res.data == null){
                notready_.Data = false;
                return;
            }
            var rd = res.data;
            foreach (var r in rd)
            {
                if (sup_type_.Equals(r.type))
                {
                    var new_item = new Core.Common.CommonDataItem() { Id = r.id, ImageUrl = r.images.standard_resolution.url, Sort = r.likes.count };
                    push_source_.FireItemUpdate(new_item);
                }
            }
            notready_.Data = false;
        }
        public InstagramRequest Request
        {
            get
            {
                return instagram_request_;
            }
        }
        private string user_id_ = string.Empty;
        InstagramRequest instagram_request_ = new InstagramRequest();
        const string sup_type_ = "image";
        PushSource push_source_ = new PushSource();
    }
}
