package net.sourceforge.simcpux.wxapi;

import android.app.Activity;
import android.os.Bundle;

import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        for (int i=0;i<20;i++){
            System.out.println("WXEntryActivity Create!!!!!!");
        }
    }

    @Override
    public void onReq(BaseReq baseReq) {
        for (int i=0;i<20;i++){
            System.out.println("微信响应" + baseReq.openId);
        }
    }

    @Override
    public void onResp(BaseResp baseResp) {
        for (int i=0;i<20;i++){
            System.out.println("微信响应" + baseResp.errCode);
        }
    }
}