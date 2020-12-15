using UnityEngine;
using UnityEngine.UI;

namespace Wechat
{
    public class TestCallWechat : MonoBehaviour
    {
        const string WechatAppId = "wxd930ea5d5a258f4f";//wxd930ea5d5a258f4f
        const string WechatUniversalLink = "https://help.wechat.com/sdksample/";
        public Button btnRegister;
        public Button btnAuth;
        public Text text;
        WechatAPIBase wechatAPIBase;

        void Start()
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Wechat SDK] This RuntimePlatform is not supported. Only iOS and Android devices are supported.");
#endif

#if UNITY_IOS
            wechatAPIBase = new WechatAPIIOS();
#elif UNITY_ANDROID
            wechatAPIBase = new WechatAPIAndroid();
#endif
            btnRegister.onClick.AddListener(() =>
            {
                wechatAPIBase.Register(WechatAppId, WechatUniversalLink);
            });

            btnAuth.onClick.AddListener(() =>
            {
                wechatAPIBase.SendAuthRequest((string authenticateCode) =>
                {
                    text.text = authenticateCode;
                });
            });
        }
    }
}