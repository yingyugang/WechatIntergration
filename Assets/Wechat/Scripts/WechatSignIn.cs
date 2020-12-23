using System;

namespace Wechat
{
    /// <summary>
    /// ウェチャット認証
    /// </summary>
    public class WechatSignIn
    {
        WechatAPIBase wechatAPIBase;

        public WechatSignIn(string wechatAppId, string wechatSecret, string wechatUniversalLink)
        {
#if UNITY_IOS
            wechatAPIBase = new WechatAPIIOS();
#elif UNITY_ANDROID
            wechatAPIBase = new WechatAPIAndroid();
#endif
            wechatAPIBase.Register(null, wechatAppId, wechatSecret, wechatUniversalLink);
        }

        public void SignIn(Action<WechatAccessTokenResponseData> onComplete)
        {
            wechatAPIBase.GetAccessToken(onComplete);
        }
    }
}
