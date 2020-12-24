using System;
using UnityEngine;

namespace Wechat
{
    /// <summary>
    /// アンドロイド対応
    /// //TODO
    /// </summary>
    public class WechatAPIAndroid : WechatAPIBase
    {
        AndroidJavaObject wechatIntergration;

        public override void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete)
        {
            Debug.LogWarning("[Wechat] this platform is not support now.");
        }

        public override void GetUserInfo(string openId, string accessToken, Action<WechatUserInfoResponseData> onComplete)
        {
            Debug.LogWarning("[Wechat] this platform is not support now.");
        }

        public override void GetUserInfo(Action<WechatUserInfoResponseData> onComplete)
        {
            Debug.LogWarning("[Wechat] this platform is not support now.");
        }

        public override bool Register(string appId, string secret, string universalLink = "")
        {
            Debug.LogWarning("[Wechat] this platform is not support now.");
            wechatIntergration = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            wechatIntergration.Call("Enroll", actObj, appId);
            //TODO
            return true;
        }

        public override void SendAuthRequest(Action<string> onComplete)
        {
            Debug.LogWarning("[Wechat] this platform is not support now.");
            //AndroidJavaObject jo = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            //AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            var callback = new WechatCallback();
            //TODO
            //callback.onComplete = onComplete;
            wechatIntergration.Call("Auth", callback);
        }
    }

    /// <summary>
    /// アンドロイドのコールバック用クラス
    /// </summary>
    class WechatCallback : AndroidJavaProxy
    {

        public WechatCallback() : base("com.yyg.testwechat.wxapi.WechatCallBackInterface") { }

        public Action<string> onComplete;

        public void onCallback(string path)
        {
            onComplete?.Invoke(path);
        }
    }
}
