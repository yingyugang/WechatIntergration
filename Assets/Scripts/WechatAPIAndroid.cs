using System;
using UnityEngine;

namespace Wechat
{
    /// <summary>
    /// アンドロイド繋げ用
    /// </summary>
    public class WechatAPIAndroid : WechatAPIBase
    {
        AndroidJavaObject wechatIntergration;

        public override void Register(string appId, string universalLink = "")
        {
            wechatIntergration = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            wechatIntergration.Call("Enroll", actObj, appId);
        }

        public override void SendAuthRequest(Action<string> onComplete)
        {
            //AndroidJavaObject jo = new AndroidJavaObject("com.crosslink.wechat.WechatIntergration");
            //AndroidJavaClass act = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //var actObj = act.GetStatic<AndroidJavaObject>("currentActivity");
            var callback = new WechatCallback();
            callback.onComplete = onComplete;
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
