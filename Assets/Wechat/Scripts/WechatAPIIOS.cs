using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Wechat
{
    public class WechatAPIIOS : WechatAPIBase
    {
        delegate void CallBack(IntPtr param);
        static string wechatSecuret;

        [DllImport("__Internal")]
        private static extern bool Enroll(string appId, string universalLink);

        [DllImport("__Internal")]
        private static extern void SendAuthRequest(CallBack callBack);

        [DllImport("__Internal")]
        private static extern void ObtainAccessToken(string appId, string secret, string code, CallBack callBack);

        [MonoPInvokeCallback(typeof(CallBack))]
        static void CallBackAuthorizationCode(IntPtr param)
        {
            string path = Marshal.PtrToStringAuto(param);
            NameValueCollection nvc;
            string baseUrl;
            ParseUrl(path, out nvc, out baseUrl);
            if (baseUrl.IndexOf(wechatAppId) != -1)
            {
                if (!string.IsNullOrEmpty(nvc["code"]))
                {
                    authorizationCode = nvc["code"];
                    ObtainAccessToken(wechatAppId, wechatSecuret, authorizationCode, CallBackAccessToken);
                }
                else
                {
                    var errcode = nvc["errcode"];
                    Debug.LogError($"errcode:{errcode}");
                    wechatAccessTokenResponseData.errcode = WechatErrCode.UserCancel;
                    wechatAccessTokenResponseData.errmsg = "User cancel.";
                    onObtainAccessTokenComplete?.Invoke(wechatAccessTokenResponseData);
                }
            }
        }

        [MonoPInvokeCallback(typeof(CallBack))]
        static void CallBackAccessToken(IntPtr param)
        {
            string jsonStr = Marshal.PtrToStringAuto(param);
            if (!string.IsNullOrEmpty(jsonStr))
            {
                wechatAccessTokenResponseData = JsonUtility.FromJson<WechatAccessTokenResponseData>(jsonStr);
                onObtainAccessTokenComplete?.Invoke(wechatAccessTokenResponseData);
                Debug.Log(wechatAccessTokenResponseData);
            }
        }

        public override void Register(string appId, string secret, string universalLink)
        {
            if (Enroll(appId, universalLink))
            {
                wechatAppId = appId;
                wechatSecuret = secret;
            }
            else
            {
                Debug.LogError($"AppId:{appId},Secuet:{secret},UniversalLink:{universalLink},register failure.");
            }
        }

        protected override void SendAuthRequest()
        {
            SendAuthRequest(CallBackAuthorizationCode);
        }

        public override void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete)
        {
            wechatAccessTokenResponseData = new WechatAccessTokenResponseData();
            if (string.IsNullOrEmpty(wechatAppId))
            {
                Debug.LogError("Please register you app first!");
                wechatAccessTokenResponseData.errcode = WechatErrCode.NotSupport;
                onComplete?.Invoke(wechatAccessTokenResponseData);
            }
            else
            {
                onObtainAccessTokenComplete = onComplete;
                SendAuthRequest();
            }
        }
    }
}
