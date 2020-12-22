using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Wechat
{
    public enum WechatErrCode {Ok,NotSupport,UserCancel };
    public abstract class WechatAPIBase
    {
        protected static Action<WechatAccessTokenResponseData> onObtainAccessTokenComplete;
        protected static string wechatAppId;
        protected static string authorizationCode;
        protected static WechatAccessTokenResponseData wechatAccessTokenResponseData;
        public abstract void Register(string appId, string secret, string universalLink = "");
        protected abstract void SendAuthRequest();
        public abstract void GetAccessToken(Action<WechatAccessTokenResponseData> onComplete);

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
    }

    /// <summary>
    /// ウェチャットアクセストークン取得用
    /// https://developers.weixin.qq.com/doc/oplatform/Mobile_App/WeChat_Login/Development_Guide.html
    /// </summary>
    [Serializable]
    public class WechatAccessTokenResponseData
    {
        public WechatErrCode errcode = WechatErrCode.Ok;
        public string errmsg;
        public string access_token;

        public override string ToString()
        {
            return $"errcode:{errcode},errmsg:{errmsg},access_token:{access_token}";
        }
    }

    /// <summary>
    /// ウェチャット認証コード取得用
    /// https://developers.weixin.qq.com/doc/oplatform/Mobile_App/WeChat_Login/Development_Guide.html
    /// </summary>
    [Serializable]
    public class WechatAuthorizationCodeResponseData
    {
        public string errcode;
        public string errmsg;
        public string access_token;

        public override string ToString()
        {
            return $"errcode:{errcode},errmsg:{errmsg},access_token:{access_token}";
        }
    }
}
