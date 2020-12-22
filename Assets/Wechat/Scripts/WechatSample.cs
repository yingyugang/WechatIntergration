using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

namespace Wechat
{
    public class WechatSample : MonoBehaviour
    {
        //官方测试APP的AppId
        public static readonly string WechatAppId = "wxd930ea5d5a258f4f";//wxd930ea5d5a258f4f
                                                                         //官方测试App的UniversalLink
        static readonly string WechatUniversalLink = "https://help.wechat.com/sdksample/";
        static readonly string WechatSecret = "";
        public readonly static string SampleDomain = "applinks:help.wechat.com";
        WechatAPIBase wechatAPIBase;
        public Button btnGetAccessToken;
        public Text text;

        private void Awake()
        {
#if UNITY_IOS
            wechatAPIBase = new WechatAPIIOS();
#elif UNITY_ANDROID
            wechatAPIBase = new WechatAPIAndroid();
#endif
            wechatAPIBase.Register(WechatAppId, WechatSecret, WechatUniversalLink);

            btnGetAccessToken.onClick.AddListener(() =>
            {
                wechatAPIBase.GetAccessToken((data) =>
                {
                    if (data.errcode == WechatErrCode.Ok)
                    {
                        Debug.Log(data.access_token);
                    }
                    else
                    {
                        Debug.Log($"ErrCode:{data.errcode},ErrMsg:{data.errmsg}");
                    }
                });
            });

            new DeepLink((url) =>
            {
                NameValueCollection nvc;
                string baseUrl;
                WechatAPIBase.ParseUrl(url, out nvc, out baseUrl);
                if (baseUrl.IndexOf(WechatAppId) != -1)
                {
                    if (!string.IsNullOrEmpty(nvc["code"]))
                    {
                        Debug.Log($"deepLink:{nvc["code"]}");
                    }
                }
            });
        }
    }
}
