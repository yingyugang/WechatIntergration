/*
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using Wechat;

public class TestCallWechat : MonoBehaviour
{
    WechatAPI wechatAPI;
    //官方测试APP的AppId
    public static readonly string WechatAppId = "wxd930ea5d5a258f4f";//wxd930ea5d5a258f4f
    //官方测试App的UniversalLink
    const string WechatUniversalLink = "https://help.wechat.com/sdksample/";
    public Button button;
    public Button button1;
    public Button button2;
    public Text text;

    string authorizationCode = "";

    void Start()
    {
        new DeepLink((url) =>
        {
            NameValueCollection nvc;
            string baseUrl;
            WechatAPI.ParseUrl(url, out nvc,out baseUrl);
            if (baseUrl.IndexOf(WechatAppId)!=-1)
            {
                if (!string.IsNullOrEmpty(nvc["code"]))
                {
                    Debug.Log($"deepLink:{nvc["code"]}");
                }
            }
        });
        wechatAPI = new WechatAPI();
        wechatAPI.Register(WechatAppId, WechatUniversalLink);
        button.onClick.AddListener(() =>
        {
            wechatAPI.Register(WechatAppId, WechatUniversalLink);
        });

        button1.onClick.AddListener(() =>
        {
            wechatAPI.Authenticate((string authenticateCode) =>
            {
                //text.text = authenticateCode;
                Debug.Log($"openUrl:{authenticateCode}");
                authorizationCode = authenticateCode;
            });
        });

        button2.onClick.AddListener(() =>
        {
            wechatAPI.GetAccessToken(authorizationCode);
        });
    }
}
*/