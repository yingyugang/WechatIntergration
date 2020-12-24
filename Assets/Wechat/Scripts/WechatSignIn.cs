using System;
using UnityEngine;

namespace Wechat
{
    /// <summary>
    /// ウェチャット認証
    /// </summary>
    public class WechatSignIn
    {
        WechatAPIBase wechatAPIBase;

        public WechatSignIn()
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
#else
#if UNITY_IOS
            wechatAPIBase = new WechatAPIIOS();
#elif UNITY_ANDROID
            wechatAPIBase = new WechatAPIAndroid();
#endif
#endif
        }

        public WechatSignIn(string wechatAppId, string wechatSecret, string wechatUniversalLink)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
#else
#if UNITY_IOS
            wechatAPIBase = new WechatAPIIOS();
#elif UNITY_ANDROID
            wechatAPIBase = new WechatAPIAndroid();
#endif
            wechatAPIBase.Register(wechatAppId, wechatSecret, wechatUniversalLink);
#endif
        }

        public void SignIn(Action<WechatAccessTokenResponseData> onComplete)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
            return;
#else
            wechatAPIBase.GetAccessToken(onComplete);
#endif
        }

        public bool Register(string wechatAppId, string wechatSecret, string wechatUniversalLink)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
            return false;
#else
            return wechatAPIBase.Register(wechatAppId, wechatSecret, wechatUniversalLink);
#endif
        }

        public void SendAuthRequest(Action<string> onComplete)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
            return;
#else
            wechatAPIBase.SendAuthRequest(onComplete);
#endif
        }

        public void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
            return;
#else
            wechatAPIBase.GetAccessToken(onComplete);
#endif
        }

        public void GetUserInfo(string openId, string accessToken, Action<WechatUserInfoResponseData> onComplete)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
            return;
#else
            wechatAPIBase.GetUserInfo(openId, accessToken, onComplete);
#endif
        }

        public void GetUserInfo(Action<WechatUserInfoResponseData> onComplete)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat] Editor is not supported.");
            return;
#else
            wechatAPIBase.GetUserInfo(onComplete);
#endif
        }
    }
}
