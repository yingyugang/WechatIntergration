using System;
using UnityEngine;

/// <summary>
/// UrlSchemeからパラメーターを受け取る。
/// </summary>
public class DeepLink
{
    Action<string> onDeepLinkCalled;

    public DeepLink(Action<string> onDeepLinkCalled)
    {
        this.onDeepLinkCalled = onDeepLinkCalled;
        Application.deepLinkActivated += OnDeepLinkActive;
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OnDeepLinkActive(Application.absoluteURL);
        }
    }

    void OnDeepLinkActive(string url)
    {
        onDeepLinkCalled?.Invoke(url);
    }
}