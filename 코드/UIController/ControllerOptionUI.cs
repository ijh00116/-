using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;
using BackEnd;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

namespace BlackTree.Core
{
    public class ControllerOptionUI
    {
        private ViewCanvasOption _viewCanvasoption;
        private CancellationTokenSource _cts;
        public ControllerOptionUI(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewCanvasoption = ViewCanvas.Create<ViewCanvasOption>(parent);

            for(int i=0; i< _viewCanvasoption.closeButton.Length; i++)
            {
                int index = i;
                _viewCanvasoption.closeButton[index].onClick.AddListener(CloseWindow);
            }
            _viewCanvasoption.bgmValue.onValueChanged.AddListener(BgmSliderValueChanged);
            _viewCanvasoption.effectValue.onValueChanged.AddListener(EffectSliderValueChanged);

            _viewCanvasoption.damageAppearToggle.onValueChanged.AddListener(toggleon => {
                DmgToggleActivate(toggleon);
            });
            _viewCanvasoption.effectAppearToggle.onValueChanged.AddListener(toggleon => {
                EffectToggleActivate(toggleon);
            });
            _viewCanvasoption.autoSaveBatteryToggle.onValueChanged.AddListener(toggleon => {
                AutoSavemodeToggleActivate(toggleon);
            });
            _viewCanvasoption.bossAutoChallengeToggle.onValueChanged.AddListener(toggleon => {
                BossAutoActivate(toggleon);
            });
            _viewCanvasoption.camShakeToggle.onValueChanged.AddListener(toggleon => {
                CamShakeToggleActivate(toggleon);
            });
            _viewCanvasoption.pushAlarm.onValueChanged.AddListener(toggleon => {
                PushAlarmActivate(toggleon);
            });
            _viewCanvasoption.nightpushAlarm.onValueChanged.AddListener(toggleon => {
                PushAlarmnightActivate(toggleon);
            });


            _viewCanvasoption.idcopyBtn.onClick.AddListener(IDCopy);

            _viewCanvasoption.privatePolicyBtn.onClick.AddListener(OpenPrivatePolicy);
            _viewCanvasoption.termsofServiceBtn.onClick.AddListener(OpenTerms);
            _viewCanvasoption.sendASBtn.onClick.AddListener(TouchSendEmail);
            _viewCanvasoption.infoSaveBtn.onClick.AddListener(IngameSave);
#if UNITY_ANDROID
            _viewCanvasoption.GoogleLoginBtn.onClick.AddListener(()=> { GoogleLoginStart().Forget(); });
#endif
            _viewCanvasoption.AppleLoginBtn.onClick.AddListener(() => { AppleLoginStart().Forget(); });

            _viewCanvasoption.SocialConfirmRestartBtn.onClick.AddListener(()=> { StartSocialLoginAndQuit().Forget(); });
            _viewCanvasoption.SocialExistConfirmBtn.onClick.AddListener(StartAlreadyAccountAndQuit);

            for(int i=0; i< _viewCanvasoption.SocialExistOffBtn.Length; i++)
            {
                _viewCanvasoption.SocialExistOffBtn[i].onClick.AddListener(QuitSocialAlreadyAccount);
            }

            _viewCanvasoption.alreadyGoogleLoginedCheck.SetActive(false);
            _viewCanvasoption.alreadyAppleLoginedCheck.SetActive(false);

            if (Player.Cloud.optiondata.isGuest==false)
            {
                if(Player.Cloud.optiondata.isGoogleLogin)
                {
                    _viewCanvasoption.alreadyGoogleLoginedCheck.SetActive(true);
                }
                else
                {
                    _viewCanvasoption.alreadyAppleLoginedCheck.SetActive(true);
                }
                
            }
            else
            {
                _viewCanvasoption.alreadyGoogleLoginedCheck.SetActive(false);
                _viewCanvasoption.alreadyAppleLoginedCheck.SetActive(false);
            }
            Init();
            Player.Option.autoBossActive += BossAutoActivate;

            SaveProcess().Forget();
  


            currentTime = possibleSaveTime;
            Timer().Forget();

            _viewCanvasoption.settingTitle.text= StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Option].StringToLocal;
            _viewCanvasoption.gameSetting.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Option].StringToLocal;
            _viewCanvasoption.effectSound.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EffectSound].StringToLocal;
            _viewCanvasoption.bgmSound.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Bgm].StringToLocal;
            _viewCanvasoption.dmgAppear.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_DmgAppear].StringToLocal;
            _viewCanvasoption.effectAppear.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EffectAppear].StringToLocal;
            _viewCanvasoption.AutoSaveMode.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AutoSaving].StringToLocal;
            _viewCanvasoption.AutoBoss.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AutoBoss].StringToLocal;
            _viewCanvasoption.cameraEffect.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_CameraEffect].StringToLocal;
            _viewCanvasoption.pushAlarmTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Push].StringToLocal;
            _viewCanvasoption.pushAlarmNightTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PushNight].StringToLocal;
            _viewCanvasoption.EtcOptionTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EtcOption].StringToLocal;
            _viewCanvasoption.TermsTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Terms].StringToLocal;
            _viewCanvasoption.PrivacyTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Privacy].StringToLocal;
            _viewCanvasoption.QATxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_QA].StringToLocal;
            _viewCanvasoption.GameSaveTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SaveGame].StringToLocal;
            _viewCanvasoption.GoogleLoginTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoogleLogin].StringToLocal;
            _viewCanvasoption.AppleLoginTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AppleLogin].StringToLocal;
            _viewCanvasoption.IdCopy.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_IdCopy].StringToLocal;


#if UNITY_IOS
            _viewCanvasoption.AppleLoginBtn.gameObject.SetActive(true);
            _viewCanvasoption.GoogleLoginBtn.gameObject.SetActive(false);
#else
            _viewCanvasoption.AppleLoginBtn.gameObject.SetActive(false);
            _viewCanvasoption.GoogleLoginBtn.gameObject.SetActive(true);
#endif

            _viewCanvasoption.deleteAccountText_0.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_DeleteAccount].StringToLocal;
            _viewCanvasoption.deleteAccountText_1.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_DeleteAccount].StringToLocal;

            _viewCanvasoption.deleteAccountDesc_1.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AskDelete].StringToLocal;
            _viewCanvasoption.deleteAccountConfirmTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Confirm].StringToLocal;
            _viewCanvasoption.deleteAccountCancelTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Cancel].StringToLocal;

            _viewCanvasoption.deleteAcountWindow.SetActive(false);

            _viewCanvasoption.popupDeleteAccount.onClick.AddListener(OpenDeleteWindow);

            for(int i=0; i< _viewCanvasoption.closeDeleteAccount.Length; i++)
            {
                _viewCanvasoption.closeDeleteAccount[i].onClick.AddListener(CloseDeleteWindow);
            }

            _viewCanvasoption.confirmDeleteAccount.onClick.AddListener(StartDeleteAccount);

#if UNITY_IOS
            _viewCanvasoption.restorePurchase.gameObject.SetActive(true);
            _viewCanvasoption.restorePurchase.onClick.AddListener(() => IAPManager.Instance.RestorePurchase());
            _viewCanvasoption.restoreTxt.text= StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RestorePurchase].StringToLocal;
#else
            _viewCanvasoption.restorePurchase.gameObject.SetActive(false);
            

#endif
        }

        float currentTime;
        float possibleSaveTime= 1200;
        float questPlayingTime = 0;
        async UniTaskVoid Timer()
        {
            while (true)
            {
                if(currentTime<900)
                {
                    currentTime += Time.deltaTime;
                }

                questPlayingTime += Time.deltaTime;

                if(questPlayingTime>=1)
                {
                    questPlayingTime = 0;
                    Player.Quest.TryCountUp(QuestType.PlayingTime_sec, 1);
                }

                await UniTask.Yield(_cts.Token);
            }
        }

        //save to firebase every 900000milsecond
        async UniTaskVoid SaveProcess()
        {
            await UniTask.Delay(60000);//1minutes
            while (true)
            {
                Player.Cloud.optiondata.lastSaveTime = System.DateTime.Now.Ticks.ToString();
                Player.Cloud.optiondata.SetDirty(true);
                Player.SaveUserDataToFirebaseAndLocal().Forget();
                await UniTask.Delay(900000);//15minutes
            }
        }
       
        void Init()
        {
            BgmSliderValueChanged(Player.Cloud.optiondata.bgmSound);
            _viewCanvasoption.bgmValue.value = Player.Cloud.optiondata.bgmSound;
            EffectSliderValueChanged(Player.Cloud.optiondata.effectSound);
            _viewCanvasoption.effectValue.value = Player.Cloud.optiondata.effectSound;

            DmgToggleActivate(Player.Cloud.optiondata.appearDmg);
            EffectToggleActivate(Player.Cloud.optiondata.appearEffect);
            AutoSavemodeToggleActivate(Player.Cloud.optiondata.autoSaveMode);
            BossAutoActivate(Player.Cloud.optiondata.autoChallengeBoss);
            CamShakeToggleActivate(Player.Cloud.optiondata.camShake);

            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                _viewCanvasoption.pushAlarmObj.SetActive(false);
                _viewCanvasoption.nightpushAlarmObj.SetActive(false);
            }
            else
            {
                _viewCanvasoption.pushAlarmObj.SetActive(Player.Cloud.optiondata.pushAlarm == PushAlaramType.AgreeAll || Player.Cloud.optiondata.pushAlarm == PushAlaramType.AgreeOnlyDay);
                _viewCanvasoption.nightpushAlarmObj.SetActive(Player.Cloud.optiondata.pushAlarm == PushAlaramType.AgreeAll);
            }
            

            _viewCanvasoption.uuidText.text = BTETC.backendUUID;
        }

        void IDCopy()
        {
            GUIUtility.systemCopyBuffer = BTETC.backendUUID;

            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_IdCopied].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);


        }
        void CloseWindow()
        {
            _viewCanvasoption.blackBG.PopupCloseColorFade();
            _viewCanvasoption.Wrapped.CommonPopupCloseAnimationUp(() => {
                _viewCanvasoption.SetVisible(false);
            });
        }

        void OpenTerms()
        {
            Application.OpenURL(Urls.terms_of_service);
        }
        void OpenPrivatePolicy()
        {
            Application.OpenURL(Urls.privacy_policy);
        }
        void IngameSave()
        {             
            Player.Cloud.optiondata.lastSaveTime = System.DateTime.Now.Ticks.ToString();
            Player.Cloud.optiondata.SetDirty(true);
            SaveData().Forget();
        }

#if UNITY_ANDROID
        async UniTask GoogleLoginStart()
        {
            int guestlogin= PlayerPrefs.GetInt("GuestLogin");
            if (guestlogin != 1)
            {
                string logined = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AlreadyLogined].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(logined);
                return;
            }
                

            await BlackTreeLogin.Instance.GooglesigninProcess();
            PlayerPrefs.SetInt("IOSLogin", 0);
            Model.Player.Cloud.optiondata.isGoogleLogin = true;
            if (BlackTreeLogin.Instance.IdToken == null)
            {

            }
            else
            {
                BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(BlackTreeLogin.Instance.IdToken, FederationType.Google);

                if (BRO.GetStatusCode() == "200")//이미 가입되있음
                {
                    _viewCanvasoption.SocialExistWindow.SetActive(true);
                }
                else
                {
                    _viewCanvasoption.SocialConfirmWindow.SetActive(true);
                }
            }
       
        }
#endif

        async UniTask AppleLoginStart()
        {
            int guestlogin = PlayerPrefs.GetInt("GuestLogin");
            if (guestlogin != 1)
            {
                string logined = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AlreadyLogined].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(logined);
                return;
            }

            await BlackTreeLogin.Instance.ApplesigninProcess();
            PlayerPrefs.SetInt("IOSLogin", 1);
            Model.Player.Cloud.optiondata.isGoogleLogin = false;
            if (BlackTreeLogin.Instance.IdToken == null)
            {

            }
            else
            {
                BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(BlackTreeLogin.Instance.IdToken, FederationType.Apple);

                if (BRO.GetStatusCode() == "200")//이미 가입되있음
                {
                    _viewCanvasoption.SocialExistWindow.SetActive(true);
                }
                else
                {
                    _viewCanvasoption.SocialConfirmWindow.SetActive(true);
                }
            }

        }


        async UniTask StartSocialLoginAndQuit()
        {
            BTETC.userUUID = BlackTreeLogin.Instance.userID;
            BTETC.googleTokenID = BlackTreeLogin.Instance.IdToken;

            PlayerPrefs.SetInt("GuestLogin", 0);
            Model.Player.Cloud.optiondata.isGuest = false;
            Model.Player.Cloud.optiondata.useruuid= BlackTreeLogin.Instance.userID;


            int iosLogin= PlayerPrefs.GetInt("IOSLogin");

            if(iosLogin!=1)
            {
                Model.Player.Cloud.optiondata.isGoogleLogin = true;
            }
            else
            {
                Model.Player.Cloud.optiondata.isGoogleLogin = false;

            }


            bool isFirebaseLogined = false;
            await FirebaseRD.Signin(o => { isFirebaseLogined = o; });
            if (isFirebaseLogined == false)
            {
                SceneManager.LoadScene(BTETC.Scene.ServerFixScene);
                return;
            }

            if (iosLogin != 1)
            {
                BackendReturnObject bro = Backend.BMember.ChangeCustomToFederation(BlackTreeLogin.Instance.IdToken, FederationType.Google);
            }
            else
            {
                BackendReturnObject bro = Backend.BMember.ChangeCustomToFederation(BlackTreeLogin.Instance.IdToken, FederationType.Apple);
            }



            await FirebaseRD.SaveFullData(Model.Player.Cloud);

            Application.Quit();
        }

        void StartAlreadyAccountAndQuit()
        {
            LocalSaveLoader.SaveForbid = true;
            LocalSaveLoader.DeleteLocalData();

            PlayerPrefs.SetInt("GuestLogin", 0);
            PlayerPrefs.SetInt("SocialLogined", 1);
            PlayerPrefs.SetInt("BTAuth", 1);
            Backend.BMember.WithdrawAccount();

            Application.Quit();
        }

        void QuitSocialAlreadyAccount()
        {
            _viewCanvasoption.SocialExistWindow.SetActive(false);
        }


    
        void TouchSendEmail()
        {
            string email = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Admin_Email].StringToLocal;
            string subject = MyEscapeURL(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Admin_EmailTitle].StringToLocal);
            string body = MyEscapeURL(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Admin_EmailDesc].StringToLocal + "\n uuid : " + Player.Cloud.optiondata.backenduuid);
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        async UniTaskVoid SaveData()
        {
            if(currentTime>=possibleSaveTime)
            {
                await Player.SaveUserDataToFirebaseAndLocal();
                currentTime = 0;
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ServerSaved].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_canFewminuteLater].StringToLocal;
                string msg = string.Format(localizedvalue, (int)((possibleSaveTime-currentTime)/60));
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(msg);
            }
            
        }
    

        void BgmSliderValueChanged(float _value)
        {
            Player.Cloud.optiondata.bgmSound = _value;
            AudioManager.Instance.SetVolume(true, _value);
        }

        void EffectSliderValueChanged(float _value)
        {
            Player.Cloud.optiondata.effectSound= _value;
            AudioManager.Instance.SetVolume(false, _value);
        }

        void DmgToggleActivate(bool active)
        {
            Player.Cloud.optiondata.appearDmg = active;
            _viewCanvasoption.damageAppearEnableObj.SetActive(active);
            Player.Cloud.optiondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }

        void CamShakeToggleActivate(bool active)
        {
            Player.Cloud.optiondata.camShake = active;
            _viewCanvasoption.camShakeEnableObj.SetActive(active);
            Player.Cloud.optiondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }

        void PushAlarmActivate(bool active)
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (active)
            {
                
                InitializeAndroidLocalPush();
            }
            else
            {
                Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.Disagree;
                _viewCanvasoption.pushAlarmObj.SetActive(false);
                _viewCanvasoption.nightpushAlarmObj.SetActive(false);

                BackEnd.Backend.Android.DeleteDeviceToken();
                Player.Cloud.optiondata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
#elif UNITY_IOS
  if (active)
            {
                Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeAll;
                _viewCanvasoption.pushAlarmObj.SetActive(true);
                _viewCanvasoption.nightpushAlarmObj.SetActive(true);
                BackEnd.Backend.iOS.PutDeviceToken(isDevelopment.iosProd);
                Backend.iOS.AgreeNightPushNotification(true);

                Player.Cloud.optiondata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                userPushAlarmComplete = true;
            }
            else
            {
                Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.Disagree;
                _viewCanvasoption.pushAlarmObj.SetActive(false);
                _viewCanvasoption.nightpushAlarmObj.SetActive(false);

                BackEnd.Backend.iOS.DeleteDeviceToken();
                Player.Cloud.optiondata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
#endif


        }
        void PushAlarmnightActivate(bool active)
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID
            if (_viewCanvasoption.pushAlarmObj.activeInHierarchy)
            {
                if (active)
                {
                    _viewCanvasoption.nightpushAlarmObj.SetActive(true);
                    Backend.Android.AgreeNightPushNotification(true);
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeAll;
                }
                else
                {
                    Backend.Android.AgreeNightPushNotification(false);
                    _viewCanvasoption.nightpushAlarmObj.SetActive(false);
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeOnlyDay;

                }
                Player.Cloud.optiondata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
#elif UNITY_IOS
    if (_viewCanvasoption.pushAlarmObj.activeInHierarchy)
            {
                if (active)
                {
                    _viewCanvasoption.nightpushAlarmObj.SetActive(true);
                    Backend.iOS.AgreeNightPushNotification(true);
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeAll;
                }
                else
                {
                    Backend.iOS.AgreeNightPushNotification(false);
                    _viewCanvasoption.nightpushAlarmObj.SetActive(false);
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeOnlyDay;

                }
                Player.Cloud.optiondata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
#endif


        }

        bool userPushAlarmComplete = false;
        int apiLevel;

        public void InitializeAndroidLocalPush()
        {
            string androidInfo = SystemInfo.operatingSystem;
            Debug.Log("androidInfo: " + androidInfo);
            apiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
            Debug.Log("apiLevel: " + apiLevel);

            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                PermissionCallbacks pCallbacks = new PermissionCallbacks();
                pCallbacks.PermissionGranted += str => {
                    Debug.Log($"{str} 승인");
                };
                pCallbacks.PermissionGranted += _ => ActionIfPermissionGranted(true); // 승인 시 기능 실행

                pCallbacks.PermissionDenied += str => {
                    Debug.Log($"{str} 거절");
                    userPushAlarmComplete = true;
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.Disagree;
                    _viewCanvasoption.pushAlarmObj.SetActive(false);
                    _viewCanvasoption.nightpushAlarmObj.SetActive(false);

                };

                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", pCallbacks);
            }
            else
            {
                _viewCanvasoption.pushAlarmObj.SetActive(true);
                _viewCanvasoption.nightpushAlarmObj.SetActive(true);
                BackEnd.Backend.Android.PutDeviceToken();
                Backend.Android.AgreeNightPushNotification(true);
                userPushAlarmComplete = true;
                Debug.Log($"요청 못하고 그냥 넘어가고 있음");
            }
        }

        void ActionIfPermissionGranted(bool agree)
        {
            Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeAll;
            _viewCanvasoption.pushAlarmObj.SetActive(true);
            _viewCanvasoption.nightpushAlarmObj.SetActive(true);
            BackEnd.Backend.Android.PutDeviceToken();
            Backend.Android.AgreeNightPushNotification(true);
   
            Player.Cloud.optiondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();

            userPushAlarmComplete = true;
            Debug.Log($"고 있음");
        }


        void EffectToggleActivate(bool active)
        {
            Player.Cloud.optiondata.appearEffect = active;
            _viewCanvasoption.effectAppearEnableObj.SetActive(active);
            Player.Cloud.optiondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }
        void AutoSavemodeToggleActivate(bool active)
        {
            Player.Cloud.optiondata.autoSaveMode = active;
            _viewCanvasoption.autoSaveBatteryEnableObj.SetActive(active);
            Player.Cloud.optiondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }
        void BossAutoActivate(bool active)
        {
            Player.Cloud.optiondata.autoChallengeBoss = active;
            _viewCanvasoption.bossautoEnableObj.SetActive(active);
            Player.Cloud.optiondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }

#region DeleteAccount

        void OpenDeleteWindow()
        {
            _viewCanvasoption.deleteAcountWindow.SetActive(true);
        }
        void CloseDeleteWindow()
        {
            _viewCanvasoption.deleteAcountWindow.SetActive(false);
        }

        void StartDeleteAccount()
        {
            DeleteAccount().Forget();
        }
        async UniTaskVoid DeleteAccount()
        {
            for (int i = 0; i < _viewCanvasoption.closeDeleteAccount.Length; i++)
            {
                _viewCanvasoption.closeDeleteAccount[i].enabled = false;
            }
            _viewCanvasoption.confirmDeleteAccount.enabled = false;

            _viewCanvasoption.deleteAccountDesc_1.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DeleteComplete].StringToLocal;

            await FirebaseRD.DeleteMyData();
            LocalSaveLoader.SaveForbid = true;
            PlayerPrefs.DeleteAll();
#if UNITY_ANDROID
            Backend.BMember.WithdrawAccount(12);

#elif UNITY_IOS
            string authCode = BlackTreeLogin.Instance.authorizationCodeForIOS;

            bool isDeleteComplete = false;
            Backend.BMember.RevokeAppleToken(authCode, callback =>
            {
                if (callback.IsSuccess())
                {
                    Backend.BMember.WithdrawAccount(callback2 =>
                    {
                        if (callback2.IsSuccess())
                        {
                            Debug.Log("회원 탈퇴에 성공했습니다.");
                            isDeleteComplete = true;
                        }
                    });
                }
            });

            await UniTask.WaitUntil(() => isDeleteComplete == true);
#endif
            await UniTask.Delay(1000);
            Application.Quit();
            Debug.Log("delete account game quit");
        }
#endregion
    }
}
