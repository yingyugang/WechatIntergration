using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Wechat
{
    public enum WechatErrCode { Ok, NotSupport, UserCancel };
    public abstract class WechatAPIBase
    {
        protected static Action<WechatAccessTokenResponseData> onObtainAccessTokenComplete;
        protected static Action<WechatUserInfoResponseData> onObtainUserInfoComplete;
        protected static string wechatAppId { get; set; }
        protected static string authorizationCode { get; set; }
        protected static string wechatSecuret { get; set; }
        protected static WechatAccessTokenResponseData wechatAccessTokenResponseData { get; set; }
        protected static WechatUserInfoResponseData wechatUserInfoResponseData { get; set; }
        public abstract void Register(MonoBehaviour monoBehaviour, string appId, string secret, string universalLink = "");
        protected abstract void SendAuthRequest();
        public abstract void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete);
        public abstract void GetUserInfo(string openId, string accessToken, Action<WechatUserInfoResponseData> onComplete);
        public abstract void GetUserInfo(Action<WechatUserInfoResponseData> onComplete);
        protected static MonoBehaviour monoBehaviour;
        public static bool isWebRequest = true;

        /// <summary>
        /// urlからパラメーターを解析。
        /// </summary>
        public static void ParseUrl(string url, out NameValueCollection nvc, out string baseUrl)
        {
            nvc = new NameValueCollection();
            if (string.IsNullOrEmpty(url))
            {
                baseUrl = url;
                return;
            }
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            if (questionMarkIndex == url.Length - 1)
            {
                baseUrl = url;
                return;
            }
            string ps = url.Substring(questionMarkIndex + 1);
            baseUrl = url.Substring(0, questionMarkIndex + 1);
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }

        protected static IEnumerator GetAccessTokenWebRequest(string appId, string secret, string code)
        {
            string requestStr = $"?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token" + requestStr;
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                wechatAccessTokenResponseData = JsonUtility.FromJson<WechatAccessTokenResponseData>(webRequest.downloadHandler.text);
                onObtainAccessTokenComplete?.Invoke(wechatAccessTokenResponseData);
            }
        }

        protected static IEnumerator GetUserInfoWebRequest(string openId, string accessToken)
        {
            string requestStr = $"?openid={openId}&access_token={accessToken}&lang=zh_CN";
            string url = "https://api.weixin.qq.com/sns/userinfo" + requestStr;
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                wechatUserInfoResponseData = JsonUtility.FromJson<WechatUserInfoResponseData>(webRequest.downloadHandler.text);
                onObtainUserInfoComplete?.Invoke(wechatUserInfoResponseData);
            }
        }
    }

    /// <summary>
    /// ウェチャットアクセストークン取得用
    /// https://developers.weixin.qq.com/doc/oplatform/Mobile_App/WeChat_Login/Development_Guide.html
    /// </summary>
    [Serializable]
    public class WechatAccessTokenResponseData
    {
        public WechatErrCode errcodeEnum = WechatErrCode.Ok;
        public int errcode;
        public string errmsg;
        public string access_token;
        public string openid;

        public override string ToString()
        {
            return $"errcode:{errcode},errmsg:{errmsg},access_token:{access_token},openid:{openid}";
        }
    }

    [Serializable]
    public class WechatUserInfoResponseData
    {
        public string openid;
        public string nickname;
        public int sex;
        public string city;
        public string country;
        public string headimgurl;
        public string province;
        public string[] privilege;
        public string unionid;
        public int errcode;
        public string errmsg;

        public override string ToString()
        {
            return $"errcode:{errcode},errmsg:{errmsg},openid:{openid},nickname:{nickname},sex:{sex},city:{city}," +
                $"country:{country},headimgurl:{headimgurl},province:{province},privilege:{privilege},unionid:{unionid}";
        }
    }
}