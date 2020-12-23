using System;
using UnityEngine;

namespace Wechat
{
    /// <summary>
    /// アンドロイド繋げ用
    /// //TODO
    /// </summary>
    public class WechatAPIAndroid : WechatAPIBase
    {
        AndroidJavaObject wechatIntergration;

        public override void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete)
        {

        }

        public override void GetUserInfo(string openId, string accessToken, Action<WechatUserInfoResponseData> onComplete)
        {

        }

        public override void GetUserInfo(Action<WechatUserInfoResponseData> onComplete)
        {
            
        }

        public override void Register(MonoBehaviour monoBehaviour, string appId, string secret, string universalLink = "")
        {
#if UNITY_EDITOR
            return;
#endif
            WechatAPIBase.monoBehaviour = monoBehaviour;
            wechatIntergration = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            wechatIntergration.Call("Enroll", actObj, appId);
        }

        public override void SendAuthRequest()
        {
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
