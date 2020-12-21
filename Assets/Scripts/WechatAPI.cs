using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AOT;
using UnityEngine;

namespace Wechat
{
    [Serializable]
    public class WechatAccessTokenResponseData
    {
        public string errcode;
        public string errmsg;
        public string access_token;

        public override string ToString()
        {
            return $"errcode:{errcode},errmsg:{errmsg},access_token:{access_token}";
        }
    }

    public class WechatAPI
    {
        delegate void CallBack(IntPtr param);

        static Action<string> onComplete;
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void Enroll(string appId, string universalLink);

        [DllImport("__Internal")]
        private static extern void SendAuthRequest(CallBack callBack);
#endif

        [DllImport("__Internal")]
        private static extern void ObtainAccessToken(string appId, string secret, string code, CallBack callBack);

#if UNITY_ANDROID// && !UNITY_EDITOR

        void Enroll(string appId,string universalLink)
        {
            AndroidJavaObject jo = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("Enroll", actObj, appId);
        }

        void SendAuthRequest()
        {
            AndroidJavaObject jo = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("Auth");
        }
#endif

        [MonoPInvokeCallback(typeof(CallBack))]
        static void CallBackFunc(IntPtr param)
        {
            string path = Marshal.PtrToStringAuto(param);
            NameValueCollection nvc;
            string baseUrl;
            ParseUrl(path, out nvc, out baseUrl);
            if (baseUrl.IndexOf(TestCallWechat.WechatAppId) != -1)
            {
                if (!string.IsNullOrEmpty(nvc["code"]))
                {
                    onComplete?.Invoke(nvc["code"]);
                }
            }
        }

        [MonoPInvokeCallback(typeof(CallBack))]
        static void CallBackAccessToken(IntPtr param)
        {
            string jsonStr = Marshal.PtrToStringAuto(param);
            if (!string.IsNullOrEmpty(jsonStr))
            {
                WechatAccessTokenResponseData data = JsonUtility.FromJson<WechatAccessTokenResponseData>(jsonStr);
                Debug.Log(data);
            }
        }

        public void GetAccessToken(string code)
        {
            ObtainAccessToken(TestCallWechat.WechatAppId, "", code, CallBackAccessToken);
        }

        public void Authenticate(Action<string> onComplete)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat SDK] This RuntimePlatform is not supported. Only iOS and Android devices are supported.");
#elif UNITY_IOS
            WechatAPI.onComplete = onComplete;
            SendAuthRequest(CallBackFunc);
#elif UNITY_ANDROID
			SendAuthRequest();
#endif
        }

        public void Register(string appId, string universalLink)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat SDK] This RuntimePlatform is not supported. Only iOS and Android devices are supported.");
#elif UNITY_IOS
            Enroll(appId, universalLink);
#elif UNITY_ANDROID
			//TODO
            Enroll(appId, universalLink);
#endif
        }

        /// <summary>
        /// urlからパラメーターを解析。
        /// </summary>
        public static void ParseUrl(string url, out NameValueCollection nvc, out string baseUrl)
        {
            nvc = new NameValueCollection();
            if (string.IsNullOrEmpty(url))
            {
                baseUrl = url;
                return;
            }
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            if (questionMarkIndex == url.Length - 1)
            {
                baseUrl = url;
                return;
            }
            string ps = url.Substring(questionMarkIndex + 1);
            baseUrl = url.Substring(0, questionMarkIndex + 1);
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
    }
}

