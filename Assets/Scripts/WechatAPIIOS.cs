using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using AOT;

namespace Wechat
{
    public class WechatAPIIOS : WechatAPIBase
    {
        delegate void CallBack(IntPtr param);

        [DllImport("__Internal")]
        private static extern void Enroll(string appId, string universalLink);

        [DllImport("__Internal")]
        private static extern void SendAuthRequest(CallBack callBack);

        [MonoPInvokeCallback(typeof(CallBack))]
        static void CallBackFunc(IntPtr param)
        {
            string path = Marshal.PtrToStringAuto(param);
            NameValueCollection nvc;
            ParseUrl(path, out nvc); ;
            if (!string.IsNullOrEmpty(nvc["code"]))
            {
                onComplete?.Invoke(nvc["code"]);
            }
        }

        static Action<string> onComplete;

        public override void Register(string appId, string universalLink)
        {
            Enroll(appId, universalLink);
        }

        public override void SendAuthRequest(Action<string> onAuthReturn)
        {
            onComplete = onAuthReturn;
            SendAuthRequest(CallBackFunc);
        }
    }
}
