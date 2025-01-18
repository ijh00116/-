using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BlackTree.Core;
using GooglePlayGames.BasicApi;
using BackEnd;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using Google;
#endif
using Cysharp.Threading.Tasks;


using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System.Text;
using AppleAuth.Extensions;

namespace BlackTree
{
    public class BlackTreeLogin : Monosingleton<BlackTreeLogin>
    {
#if UNITY_ANDROID
        private GoogleSignInConfiguration configuration;
#endif

        const string webclientID = "519340931367-opneg8qi58urnrai0ovj6ulnohinq5sb.apps.googleusercontent.com";
        public string IdToken = null;
        public string userID = null;
        public string authorizationCodeForIOS;
        public string userIDForSave;

        public bool isSocialLogined = false;

        public bool isAuthAgreed = false;
        public bool nickNameDecided = false;
        public bool userPushAlarmComplete = false;
        public void InitGoogleConfig()
        {
#if UNITY_ANDROID
            configuration = new GoogleSignInConfiguration { WebClientId = webclientID, RequestEmail = true, RequestIdToken = true };
#endif
        }


        private IAppleAuthManager appleAuthManager;
        public string AppleUserIdKey { get; private set; }

        private void Start()
        {
            if(AppleAuthManager.IsCurrentPlatformSupported)
            {
                var deserializer = new PayloadDeserializer();
                appleAuthManager = new AppleAuthManager(deserializer);
            }
        }
        private void Update()
        {
            if(this.appleAuthManager!=null)
            {
                this.appleAuthManager.Update();
            }
        }

        public void GooglePlayGamesLoginConfig()
        {
#if UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .RequestServerAuthCode(false /* Don't force refresh */)
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();

            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                }
            });
#endif
        }

        public void IOSGamesLoginConfig()
        {
            Debug.LogError("progress_4");
            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                    Debug.LogError("progress_5");
                }
                else
                {
                    Debug.LogError("progress_6");
                }
            });
        }

        public void SignInWithIOS()
        {
           
        }

        public async UniTask SocialInitialize(bool google, System.Action<bool> callbackafterPost)
        {
            BTETC.isFirstUser = false;
#if UNITY_EDITOR
           string uuid = "BTadmin";
           string adminPw = "123123";
           BTETC.userUUID = uuid;

           BackendReturnObject signbro = Backend.BMember.CustomSignUp(uuid, adminPw, "105086810955435828747");
           if (signbro.IsSuccess())
           {
               BTETC.isFirstUser = true;
               callbackafterPost?.Invoke(true);
           }
           else if(signbro.GetStatusCode()=="409")
           {
               BackendReturnObject bro = Backend.BMember.CustomLogin(uuid, adminPw, "105086810955435828747");
               if (bro.IsSuccess())
               {
                   callbackafterPost?.Invoke(true);
               }
           }
#else

            if (google)
            {
#if UNITY_ANDROID
                await GooglesigninProcess();

                if (string.IsNullOrEmpty(IdToken) == false)
                {
                    BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(IdToken, FederationType.Google, userID);
                    //login
                    if (BRO.GetStatusCode() == "200")
                    {
                        BTETC.isFirstUser = false;
                        BTETC.isGuestUser = false;
                        callbackafterPost?.Invoke(true);
                    }
                    if (BRO.GetStatusCode() == "201")//new singup and login
                    {
                        BTETC.isFirstUser = true;
                        BTETC.isGuestUser = false;
                        callbackafterPost?.Invoke(true);
                    }
                    BTETC.userUUID = userID;
                    BTETC.googleTokenID = IdToken;
                }
                else
                {
                    callbackafterPost?.Invoke(false);
                }
#endif
            }
            else
            {
                await ApplesigninProcess();

                if (string.IsNullOrEmpty(IdToken) == false)
                {
                    BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(IdToken, FederationType.Apple, userIDForSave);
                    //login
                    if (BRO.GetStatusCode() == "200")
                    {
                        BTETC.isFirstUser = false;
                        BTETC.isGuestUser = false;
                        callbackafterPost?.Invoke(true);
                    }
                    if (BRO.GetStatusCode() == "201")//new singup and login
                    {
                        BTETC.isFirstUser = true;
                        BTETC.isGuestUser = false;
                        callbackafterPost?.Invoke(true);
                    }
                    BTETC.userUUID = userID;
                    BTETC.googleTokenID = IdToken;
                    Debug.LogError("statusCode" + BRO.GetStatusCode());
                    
                }
                else
                {
                    callbackafterPost?.Invoke(false);
                    
                }
            }
#endif


        }

        public async UniTask GuestInitialize(System.Action<bool> callbackafterPost)
        {
            BackendReturnObject bro = Backend.BMember.GuestLogin("Guest Login");
            if (bro.IsSuccess())
            {
                BTETC.isGuestUser = true;
                if (bro.GetStatusCode()=="200")
                {
                    BTETC.isFirstUser = false;
                   
                }
                if (bro.GetStatusCode() == "201")
                {
                    BTETC.isFirstUser = true;
                }
                BTETC.userUUID = "Guest";
                BTETC.googleTokenID = "Guest";

                callbackafterPost?.Invoke(true);
                Debug.LogError("GuestLogin Success");
            }
            else
            {
                callbackafterPost?.Invoke(false);
                Debug.LogError("GuserLoginError");
                Debug.LogError("CODE:"+bro.GetStatusCode());
            }
        }

        //playgames services
        void PlayGameInit()
        {
#if UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
                .RequestServerAuthCode(false)
                .RequestEmail()
                .RequestIdToken()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.Activate();
#endif
        }

        public string GetAuthCode()
        {
#if UNITY_ANDROID
            string authcode =PlayGamesPlatform.Instance.GetServerAuthCode();
            return authcode;
#endif
            return null;
        }
        public string GetTokens()
        {
#if UNITY_ANDROID
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                // ???? ???? ???? ?? ???? ????
                //string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
                // ?? ???? ????
                 string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
                return _IDtoken;
            }
            else
            {
                return null;
            }
#endif
            return null;
        }
        //playgames services

#if UNITY_ANDROID
        //google login
        public async UniTask GooglesigninProcess()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestAuthCode = true;

            IdToken = null;
            userID = null;

            await GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        }
#endif

        public async UniTask ApplesigninProcess()
        {
            bool appleLogined = false;

            IdToken = null;
            userID = null;

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            this.appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // Obtained credential, cast it to IAppleIDCredential
                    var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {
                        // Apple User ID
                        // You should save the user ID somewhere in the device
                       
                      

                        var userId = appleIdCredential.User;
                        // Email (Received ONLY in the first login)
                        var email = appleIdCredential.Email;
                        // Full name (Received ONLY in the first login)
                        var fullName = appleIdCredential.FullName;
                        // Identity token
                        var identityToken = Encoding.UTF8.GetString(
                                        appleIdCredential.IdentityToken,
                                        0,
                                        appleIdCredential.IdentityToken.Length);
                        // Authorization code
                        var authorizationCode = Encoding.UTF8.GetString(
                                    appleIdCredential.AuthorizationCode,
                                    0,
                                    appleIdCredential.AuthorizationCode.Length);

                        IdToken = identityToken;
                        userID = appleIdCredential.User;
                        authorizationCodeForIOS = authorizationCode;
                        string useridforsave = userID.Replace(".", "-");
                        userIDForSave = useridforsave;

                        // And now you have all the information to create/login a user in your system
                        appleLogined = true;

                        Debug.LogError("apple login success");
                    }
                },
                error =>
                {
                    // Something went wrong
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                });

            await UniTask.WaitUntil(()=>appleLogined==true);
        }
#if UNITY_ANDROID
        public void OnSignOut()
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
        public void OnDisconnect()
        {
            GoogleSignIn.DefaultInstance.Disconnect();
        }
#endif

#if UNITY_ANDROID
        void OnAuthenticationFinished(System.Threading.Tasks.Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    }
                    else
                    {
                    }
                }
            }
            else if (task.IsCanceled)
            {
            }
            else
            {
                IdToken = task.Result.IdToken;
                userID = task.Result.UserId;
            }
        }
#endif

        //google login
    }
    public enum SocialLoginResponseStatus
    {
        Error = 0, // ?????????????
        Success = 1, // ????? id???uuid ????????????????? ???? ??????
        AlreadyUsed = 2, // ????? ?????id??? ???? ????? ????????????????????????????
        AlreadyLogined = 3, // ????? ????? id??????? ????? ?????????????????????
    }

    public enum SocialLoginType
    {
        None = -1,

        PlayGame = 0,
        GameCenter = 1,
        GoogleSignIn = 2,
        AppleSignInWith = 3,
    }


    [Serializable]
    public class ResponseSocialSignIn
    {
        public SocialLoginResponseStatus status;
        public string loginedUuid;
        public string loginedNickname;
    }
}
