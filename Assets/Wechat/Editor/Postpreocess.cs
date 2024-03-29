﻿#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class Postpreocess
{
    [PostProcessBuild(100)]
    static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject)
    {
        string projPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        var targetGuid = proj.GetUnityMainTargetGuid();// ("Unity-iPhone");
        var frameworkTarget = proj.GetUnityFrameworkTargetGuid();

        //Wechat関連1、Security,WebKit,CoreLocation 三つのフレームワークが必要。
        proj.AddFrameworkToProject(targetGuid, "Security.framework", false);
        proj.AddFrameworkToProject(targetGuid, "WebKit.framework", false);
        proj.AddFrameworkToProject(targetGuid, "CoreLocation.framework", false);
        proj.AddFrameworkToProject(frameworkTarget, "Security.framework", false);
        proj.AddFrameworkToProject(frameworkTarget, "WebKit.framework", false);
        proj.AddFrameworkToProject(frameworkTarget, "CoreLocation.framework", false);
        //Wechat関連2、他のプロプーティ
        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "No");
        proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-Objc");
        proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-all_load");
        proj.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "No");
        //must.
        proj.AddBuildProperty(frameworkTarget, "OTHER_LDFLAGS", "-Objc");
        proj.AddBuildProperty(frameworkTarget, "OTHER_LDFLAGS", "-all_load");
        //TODO use the teamId of yourself.
        proj.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM", "LY5UJF7S34");
        File.WriteAllText(projPath, proj.WriteToString());
        /*
        string plistPath = pathToBuildProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementArray lsSchems = plist.root.CreateArray("LSApplicationQueriesSchemes");
        lsSchems.AddString("weixin");
        lsSchems.AddString("weixinULAPI");
        PlistElementDict rootDict = plist.root;
        PlistElementArray urlTypes = rootDict.CreateArray("CFBundleURLTypes");
        PlistElementDict wxUrl = urlTypes.AddDict();
        wxUrl.SetString("CFBundleTypeRole", "Editor");
        wxUrl.SetString("CFBundleURLName", "weixin");
        PlistElementArray wxUrlSheme = wxUrl.CreateArray("CFBundleURLSchemes");
        wxUrlSheme.AddString("wx7a3c7293fe47ea6a");
        */
        // info.plist
        var plistPath = Path.Combine(pathToBuildProject, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        //Wechat関連5、info.plist変更、WhiteListにウェチャットを追加。（Lineと同じい）
        PlistElementArray lsSchemes = plist.root.CreateArray("LSApplicationQueriesSchemes");
        lsSchemes.AddString("weixin");
        lsSchemes.AddString("weixinULAPI");
        //Wechat関連6、info.plist変更、ウェチャットからジャンプバック用。（Lineと同じい）
        PlistElementDict rootDict = plist.root;
        PlistElementArray urlTypes = rootDict.CreateArray("CFBundleURLTypes");
        PlistElementDict wxUrl = urlTypes.AddDict();
        wxUrl.SetString("CFBundleTypeRole", "Editor");
        wxUrl.SetString("CFBundleURLName", "weixin");
        PlistElementArray wxUrlSheme = wxUrl.CreateArray("CFBundleURLSchemes");
        //TODO wechat申請次第。
        //wxUrlSheme.AddString("wx7a3c7293fe47ea6a");
        //wxd930ea5d5a258f4f
        wxUrlSheme.AddString(Wechat.WechatSample.WechatAppId);
        plist.WriteToFile(plistPath);
        //注意，必须放到最后，避免被覆盖。
        //Wechat関連3、Associated Domains設定（Universal Link用、Wechatから呼び出す用かなぁ）
        ProjectCapabilityManager capManager = new ProjectCapabilityManager(projPath, "crosslink.entitlements", null, targetGuid);
        //yingyugang.s3-ap-northeast-1.amazonaws.com,TODO PR時に公式のリンkを書き換え必要。
        //var array = new string[] { "applinks:yingyugang.s3-ap-northeast-1.amazonaws.com" };
        var array = new string[] { Wechat.WechatSample.AccosiateDemain };
        capManager.AddAssociatedDomains(array);
        capManager.WriteToFile();
    }
}
#endif
