#include <iostream>
using namespace std;
#import "WechatSignInAppController.h"
#import <Foundation/Foundation.h>
#import <objc/runtime.h>
#import "WXApi.h"

@implementation UnityAppController (WechatSignInAppController)

extern "C" {
  
typedef void(*CallBack)(const char* p);

CallBack ct;
    
    CallBack accessTokenCB;
    
    CallBack userInfoCB;
    
    BOOL isAuthorization;
    
    //ウェチャットに登録
    //向微信注册
    //Register to wechat side
    BOOL Enroll(char* appId,char* universalLink)
    {
        NSString *weichatId = [NSString stringWithFormat:@"%s", appId];
        NSString *universalLinkPath = [NSString stringWithFormat:@"%s", universalLink];
        BOOL handled =  [WXApi registerApp:weichatId universalLink:universalLinkPath];
        NSLog(@" %s",handled ? "True" : "False");
        return handled;
    }
    //ウェチャットと認証を開始
    //开始向微信发起请求认证
    //Authenticate to wechat
    void SendAuthRequest(CallBack cb)
    {
        ct = cb;
        isAuthorization = true;
        SendAuthReq* req = [[SendAuthReq alloc] init];
        req.scope = @"snsapi_userinfo";
        req.state = @"wechat_sdk_demo";
        [WXApi sendReq:req completion:^(BOOL success) {
            NSLog(@" %s",success ? "True" : "False");
        }];
    }
    
    //Obtain and resolve access token.
    void ObtainAccessToken(char* appId,char* secret,char* code,CallBack cb){
        
        accessTokenCB = cb;
        
        NSString *appIdStr = [NSString stringWithUTF8String:appId];
        NSString *secretStr = [NSString stringWithUTF8String:secret];
        
        NSString *path = [NSString stringWithFormat:@"https://api.weixin.qq.com/sns/oauth2/access_token?appid=%@&secret=%@&code=%s&grant_type=authorization_code",appIdStr,secretStr,code];
       
        NSLog(@"%@",path);
        
        NSURLRequest *request = [[NSURLRequest alloc] initWithURL:[NSURL URLWithString:path] cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:10.0];
        
        NSOperationQueue *queue = [[NSOperationQueue alloc] init];
        
        [NSURLConnection sendAsynchronousRequest:request queue:queue completionHandler:
         ^(NSURLResponse *response,NSData *data,NSError *connectionError)
         {
             if (connectionError != NULL)
             {
                 NSLog(@"%ld",(long)connectionError.code);
                 accessTokenCB("");
             }
             else
             {
                 if (data != NULL)
                 {
                     NSString *string = [[NSString alloc] initWithData:data
                                                              encoding:NSUTF8StringEncoding];
                     NSLog(@"%@", string);
                     accessTokenCB([string UTF8String]);
                 }
             }
         }];
    }
    
    void ObtainUserInfo(char* accessToken,char* openid,CallBack cb)
    {
        userInfoCB = cb;
        NSString *path = [NSString stringWithFormat:@"https://api.weixin.qq.com/sns/userinfo?access_token=%@&openid=%@&lang=zh_CN",accessToken,openid];
        
        NSURLRequest *request = [[NSURLRequest alloc] initWithURL:[NSURL URLWithString:path] cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:10.0];
        NSOperationQueue *queue = [[NSOperationQueue alloc] init];
        [NSURLConnection sendAsynchronousRequest:request queue:queue completionHandler:
         ^(NSURLResponse *response,NSData *data,NSError *connectionError) {
             if (connectionError != NULL) {
                 NSLog(@"%ld",(long)connectionError.code);
                 userInfoCB("");
             } else {
                 if (data != NULL) {
                     NSString *string = [[NSString alloc] initWithData:data
                                                              encoding:NSUTF8StringEncoding];
                     NSLog(@"%@", string);
                     userInfoCB([string UTF8String]);
                 }
             }
         }];
    }
}

/*
 Called when the category is loaded.  This is where the methods are swizzled
 out.
 */
+ (void)load {
  Method original;
  Method swizzled;

  original = class_getInstanceMethod(
      self, @selector(application:didFinishLaunchingWithOptions:));
  swizzled = class_getInstanceMethod(
      self,
      @selector(WechatSignInAppController:didFinishLaunchingWithOptions:));
  method_exchangeImplementations(original, swizzled);

  original = class_getInstanceMethod(
      self, @selector(application:openURL:sourceApplication:annotation:));
  swizzled = class_getInstanceMethod(
      self, @selector
      (WechatSignInAppController:openURL:sourceApplication:annotation:));
  method_exchangeImplementations(original, swizzled);

  original =
      class_getInstanceMethod(self, @selector(application:openURL:options:));
  swizzled = class_getInstanceMethod(
      self, @selector(WechatSignInAppController:openURL:options:));
  method_exchangeImplementations(original, swizzled);
}

- (BOOL)WechatSignInAppController:(UIApplication *)application
    didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    NSLog(@"当程序载入后执行");
    //BOOL handled = [WXApi registerApp:@"wx7a3c7293fe47ea6a" universalLink:@"applinks:yingyugang.s3-ap-northeast-1.amazonaws.com"];
    //返回True，有用，证明可以
    //BOOL handled =  [WXApi registerApp:@"wxd930ea5d5a258f4f" universalLink:@"https://help.wechat.com/sdksample/"];
    //NSLog(@" %s",handled ? "True" : "False");
  return  [self WechatSignInAppController:application
            didFinishLaunchingWithOptions:launchOptions];
}

/**
 * Handle the auth URL
 */
- (BOOL)WechatSignInAppController:(UIApplication *)application
                          openURL:(NSURL *)url
                sourceApplication:(NSString *)sourceApplication
                       annotation:(id)annotation {
    BOOL handled = [self WechatSignInAppController:application
                                           openURL:url
                                 sourceApplication:sourceApplication
                                        annotation:annotation];
  return handled;
}

/**
 * Handle the auth URL.
 */
- (BOOL)WechatSignInAppController:(UIApplication *)app
                          openURL:(NSURL *)url
                          options:(NSDictionary *)options {
    
    
    BOOL handled =
        [self WechatSignInAppController:app openURL:url options:options];
    if(isAuthorization){
        NSLog(@"%@", url.absoluteString);
        const char *str2=[url.absoluteString UTF8String];
        ct(str2);
        isAuthorization = false;
    }
    return handled;
}
@end
