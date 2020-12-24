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
        public Button btnRegister;
        public Button btnAuthorization;
        public Button btnGetAccessToken;
        public Button btnGetUserInfo;
        public TextMeshProUGUI txtAppId;
        public TextMeshProUGUI txtSecret;
        public TextMeshProUGUI txtUniversalLink;
        public TextMeshProUGUI txtAccosiateDemain;
        WechatSignIn wechatSignIn;

        private void Awake()
        {
            txtAppId.text = $"AppId: {WechatAppId}";
            txtSecret.text = $"Secret: {WechatSecret}";
            txtUniversalLink.text = $"UniversalLink: {WechatUniversalLink}";
            txtAccosiateDemain.text = $"AccosiateDemain: {AccosiateDemain}";
            wechatSignIn = new WechatSignIn();
            btnRegister.onClick.AddListener(() =>
            {
                wechatSignIn.Register(WechatAppId, WechatSecret, WechatUniversalLink);
            });
            btnAuthorization.onClick.AddListener(() =>
            {
                wechatSignIn.SendAuthRequest((str) =>
                {
                    Debug.Log($"[Wechat] AuthorizationCode:{str}");
                });
            });
            btnGetAccessToken.onClick.AddListener(() =>
            {
                wechatSignIn.GetAccessToken((data) =>
                {
                    if (data.errcode == 0)
                    {
                        Debug.Log($"[Wechat] AccessToken:{data.access_token}");
                    }
                    else
                    {
                        Debug.Log($"[Wechat] ErrCode:{data.errcode},ErrMsg:{data.errmsg}");
                    }
                });
            });
            btnGetUserInfo.onClick.AddListener(() =>
            {
                wechatSignIn.GetUserInfo((data) =>
                {
                    if (data != null)
                    {
                        Debug.Log($"[Wechat] UserInfo:{data}");
                    }
                });
            });
        }
    }
}
