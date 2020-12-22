package com.crosslink.wechat;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.widget.Toast;

import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;

public class WechatIntergration {

    // APP_ID 替换为你的应用从官方网站申请到的合法appID
    private static String APP_ID = "";
    // IWXAPI 是第三方app和微信通信的openApi接口
    private static IWXAPI wxApi;

    public void showToast(Context act, String msg){
        Toast.makeText(act, msg, Toast.LENGTH_LONG).show();
    }

    public void Enroll(Context ac,String appId){
        for(int i=0;i<20;i++){
            System.out.println("--------------------------Enroll--------------------------");
        }
        wxApi = WXAPIFactory.createWXAPI(ac, appId, false);
        if(null == wxApi)
        {
            for(int i=0;i<20;i++){
                System.out.println("--------------------------微信登录失败--------------------------");
            }
            return;
        }
        System.out.println(wxApi.registerApp(appId));
    }

    public void Auth(){
        for(int i=0;i<20;i++){
            System.out.println("--------------------------Auth--------------------------");
        }
        final SendAuth.Req req = new SendAuth.Req();
        req.scope = "snsapi_userinfo";
        req.state = "wechat_sdk_demo";
        wxApi.sendReq(req);
    }
}
