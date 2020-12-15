using UnityEngine;
using UnityEngine.UI;
using Wechat;

public class TestCallWechat : MonoBehaviour
{
    WechatAPI wechatAPI;
    const string WechatAppId = "wxd930ea5d5a258f4f";//wxd930ea5d5a258f4f
    const string WechatUniversalLink = "https://help.wechat.com/sdksample/";
    public Button button;
    public Button button1;
    public Text text;

    void Start()
    {
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
                text.text = authenticateCode;
            });
        });
    }
}
