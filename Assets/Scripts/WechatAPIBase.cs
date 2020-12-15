using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Wechat
{
    public abstract class WechatAPIBase
    {
        public abstract void Register(string appId, string universalLink = "");

        public abstract void SendAuthRequest(Action<string> onComplete);

        /// <summary>
        /// urlからパラメーターを解析。
        /// </summary>
        protected static void ParseUrl(string url, out NameValueCollection nvc)
        {
            Debug.Log(url);
            nvc = new NameValueCollection();
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                return;
            }
            if (questionMarkIndex == url.Length - 1)
            {
                return;
            }
            string ps = url.Substring(questionMarkIndex + 1);
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
    }
}
