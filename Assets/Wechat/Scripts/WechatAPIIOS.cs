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

        [DllImport("__Internal")]
        private static extern bool Enroll(string appId, string universalLink);

        [DllImport("__Internal")]
        private static extern void SendAuthRequest(CallBack callBack);

        [DllImport("__Internal")]
        private static extern void ObtainAccessToken(string appId, string secret, string code, CallBack callBack);

        [DllImport("__Internal")]
        private static extern void ObtainUserInfo(string openId, string accessToken, CallBack callBack);

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
                    if (isWebRequest)
                    {
                        monoBehaviour.StartCoroutine(GetAccessTokenWebRequest(wechatAppId, wechatSecuret, authorizationCode));
                    }
                    else
                    {
                        ObtainAccessToken(wechatAppId, wechatSecuret, authorizationCode, CallBackAccessToken);
                    }
                }
                else
                {
                    var errcode = nvc["errcode"];
                    Debug.LogError($"errcode:{errcode}");
                    wechatAccessTokenResponseData.errcode = -1;
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

        [MonoPInvokeCallback(typeof(CallBack))]
        static void CallBackUserInfo(IntPtr param)
        {
            string jsonStr = Marshal.PtrToStringAuto(param);
            if (!string.IsNullOrEmpty(jsonStr))
            {
                wechatUserInfoResponseData = JsonUtility.FromJson<WechatUserInfoResponseData>(jsonStr);
                onObtainUserInfoComplete?.Invoke(wechatUserInfoResponseData);
                Debug.Log(wechatAccessTokenResponseData);
            }
        }

        public override void Register(MonoBehaviour monoBehaviour, string appId, string secret, string universalLink)
        {
#if UNITY_EDITOR
            return;
#endif
            if (Enroll(appId, universalLink))
            {
                wechatAppId = appId;
                wechatSecuret = secret;
                WechatAPIBase.monoBehaviour = monoBehaviour;
            }
            else
            {
                Debug.LogError($"AppId:{appId},Secuet:{secret},UniversalLink:{universalLink},register failure.");
            }
        }

        public override void SendAuthRequest()
        {
            SendAuthRequest(CallBackAuthorizationCode);
        }

        public override void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete)
        {
            wechatAccessTokenResponseData = new WechatAccessTokenResponseData();
            if (string.IsNullOrEmpty(wechatAppId))
            {
                wechatAccessTokenResponseData.errcode = -1;
                wechatAccessTokenResponseData.errmsg = "Please register you app first!";
                onComplete?.Invoke(wechatAccessTokenResponseData);
            }
            else
            {
                onObtainAccessTokenComplete = onComplete;
                SendAuthRequest();
            }
        }

        public override void GetUserInfo(string openId, string accessToken, Action<WechatUserInfoResponseData> onComplete)
        {
            wechatUserInfoResponseData = new WechatUserInfoResponseData();
            onObtainUserInfoComplete = onComplete;
            if (wechatAccessTokenResponseData != null
                && !string.IsNullOrEmpty(wechatAccessTokenResponseData.access_token)
                && !string.IsNullOrEmpty(wechatAccessTokenResponseData.openid))
            {
                if (isWebRequest)
                {
                    monoBehaviour.StartCoroutine(GetUserInfoWebRequest(openId, accessToken));
                }
                else
                {
                    ObtainUserInfo(openId, accessToken, CallBackUserInfo);
                }
            }
            else
            {
                Debug.LogError("Please obtain access token first!");
                wechatUserInfoResponseData.errcode = -1;
                onComplete?.Invoke(wechatUserInfoResponseData);
            }
        }

        public override void GetUserInfo(Action<WechatUserInfoResponseData> onComplete)
        {
            GetAccessToken((data) =>
            {
                if (data.errcode == 0)
                {
                    GetUserInfo(data.openid, data.access_token, onComplete);
                }
                else
                {
                    Debug.Log($"ErrCode:{data.errcode},ErrMsg:{data.errmsg}");
                    wechatUserInfoResponseData = new WechatUserInfoResponseData();
                    wechatUserInfoResponseData.errcode = data.errcode;
                    wechatUserInfoResponseData.errmsg = data.errmsg;
                    onComplete?.Invoke(wechatUserInfoResponseData);
                }
            });
        }
    }
}