using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using Wechat;

public class TestCallWechat : MonoBehaviour
{
    WechatAPI wechatAPI;
    public static readonly string WechatAppId = "wxd930ea5d5a258f4f";//wxd930ea5d5a258f4f
    const string WechatUniversalLink = "https://help.wechat.com/sdksample/";
    public Button button;
    public Button button1;
    public Text text;
    DeepLink deepLink;

    void Start()
    {
        deepLink = new DeepLink((url) =>
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
            /*
            wechatAPI.Authenticate((string authenticateCode) =>
            {
                text.text = authenticateCode;
            });*/
        });

        button1.onClick.AddListener(() =>
        {
            wechatAPI.Authenticate((string authenticateCode) =>
            {
                //text.text = authenticateCode;
                Debug.Log($"openUrl:{authenticateCode}");
            });
        });
    }
}
