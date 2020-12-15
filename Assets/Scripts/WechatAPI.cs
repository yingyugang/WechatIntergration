using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AOT;
using UnityEngine;

namespace Wechat
{
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
            ParseUrl(path, out nvc); ;
            if (!string.IsNullOrEmpty(nvc["code"]))
            {
                onComplete?.Invoke(nvc["code"]);
            }
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
        static void ParseUrl(string url, out NameValueCollection nvc)
        {
            Debug.Log(url);
            nvc = new NameValueCollection();
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                return;
            }
            if (questionMarkIndex == url.Length - 1)
            {
                return;
            }
            string ps = url.Substring(questionMarkIndex + 1);
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
    }
}

