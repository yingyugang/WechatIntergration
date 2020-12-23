using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wechat
{
    public class WechatSample : MonoBehaviour
    {
        public static readonly string WechatAppId = "wxd930ea5d5a258f4f";
        public static readonly string WechatUniversalLink = "https://help.wechat.com/sdksample/";
        public static readonly string WechatSecret = "";
        public static readonly string AccosiateDemain = "applinks:help.wechat.com";
        WechatAPIBase wechatAPIBase;
        public Button btnRegister;
        public Button btnAuthorization;
        public Button btnGetAccessToken;
        public Button btnGetUserInfo;
        public TextMeshProUGUI txtAppId;
        public TextMeshProUGUI txtSecret;
        public TextMeshProUGUI txtUniversalLink;
        public TextMeshProUGUI txtAccosiateDemain;

        private void Awake()
        {
            txtAppId.text = $"AppId: {WechatAppId}";
            txtSecret.text = $"Secret: {WechatSecret}";
            txtUniversalLink.text = $"UniversalLink: {WechatUniversalLink}";
            txtAccosiateDemain.text = $"AccosiateDemain: {AccosiateDemain}";
#if UNITY_IOS
            wechatAPIBase = new WechatAPIIOS();
#elif UNITY_ANDROID
            wechatAPIBase = new WechatAPIAndroid();
#endif
            wechatAPIBase.Register(this, WechatAppId, WechatSecret, WechatUniversalLink);

            btnRegister.onClick.AddListener(() =>
            {
                wechatAPIBase.Register(this, WechatAppId, WechatSecret, WechatUniversalLink);
            });

            btnAuthorization.onClick.AddListener(() =>
            {
                wechatAPIBase.SendAuthRequest();
            });

            btnGetAccessToken.onClick.AddListener(() =>
            {
                wechatAPIBase.GetAccessToken((data) =>
                {
                    if (data.errcode == 0)
                    {
                        Debug.Log(data.access_token);
                    }
                    else
                    {
                        Debug.Log($"ErrCode:{data.errcode},ErrMsg:{data.errmsg}");
                    }
                });
            });

            btnGetUserInfo.onClick.AddListener(() =>
            {
                wechatAPIBase.GetUserInfo((data) =>
                {
                    if (data != null)
                    {
                        Debug.Log(data);
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
