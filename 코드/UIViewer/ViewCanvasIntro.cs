using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using BackEnd;
using UnityEngine.Android;
using BlackTree.Model;
using BlackTree.Core;
using BlackTree.Definition;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
namespace BlackTree
{
    public class ViewCanvasIntro : MonoBehaviour
    {
        [SerializeField] Sprite krimage;
        [SerializeField] Sprite enimage;
        [SerializeField] Sprite jpimage;
        [SerializeField] Sprite chimage;
        [SerializeField] Image titleimage;
        [SerializeField] GameObject titleObj;

        [SerializeField] GameObject textdescObj;
        [SerializeField] TMPro.TMP_Text touchToStartText;

        [SerializeField] Image curtainToMain;
        [SerializeField] BTButton toMainBtn;

        [SerializeField] Slider loadingBar;
        [SerializeField] TMPro.TMP_Text loadingText;
        UIStep uiStep;

        bool loadingComplete;
        float currentTime;

        Vector3 originalScale = new Vector3(0.97f, 0.97f, 0.97f);
        Vector3 zoomScale = new Vector3(1.03f, 1.03f, 1.03f);

        Color originalColor = Color.white;
        Color hideColor = new Color(1, 1, 1, 0);

        Coroutine loopUpdate;

        [SerializeField] GameObject SocialLoginWindow;
        [SerializeField] BTButton GoogleLoginBtn;
        [SerializeField] BTButton AppleLoginBtn;
        [SerializeField] BTButton GuestLoginBtn;
        [SerializeField] GameObject GuestLoginWarningWindow;
        [SerializeField] BTButton GuestLoginProcessBtn;
        [SerializeField] BTButton[] OffGuestLoginWindow;

        [SerializeField] GameObject AuthWindow;
        [SerializeField] Toggle AuthCheck;
        [SerializeField] Toggle privateCheck;
        [SerializeField] BTButton GoAuth;
        [SerializeField] BTButton GoPrivate;
        [SerializeField] BTButton Authconfirm;
        [SerializeField] GameObject ignoreConfirm;

        [SerializeField] GameObject nicknameWindow;
        [SerializeField] TMPro.TMP_InputField inputfield;
        [SerializeField] BTButton nickConfirm;

        [SerializeField] GameObject toastmessageObj;
        [SerializeField] TMPro.TMP_Text toastMessageTxt;

        [SerializeField] GameObject alarmWindow;
        [SerializeField] Toggle pushAlarm;
        [SerializeField] GameObject pushAlarmCheck;
        [SerializeField] Toggle pushAlarminNight;
        [SerializeField] GameObject pushAlarminNightCheck;
        [SerializeField] BTButton alarmConfirm;

        [Header("localized")]
        [SerializeField] TMPro.TMP_Text privateTitle;
        [SerializeField] TMPro.TMP_Text termTitle;
        [SerializeField] TMPro.TMP_Text privateBtnText;
        [SerializeField] TMPro.TMP_Text termBtnText;
        [SerializeField] TMPro.TMP_Text confirmText;

        [SerializeField] TMPro.TMP_Text nicknameTitle;
        [SerializeField] TMPro.TMP_Text nicknameDesc;
        [SerializeField] TMPro.TMP_Text nicknameConfirmBtnText;

        [SerializeField] TMPro.TMP_Text pushTitle;
        [SerializeField] TMPro.TMP_Text pushAgreeText;
        [SerializeField] TMPro.TMP_Text pushAgreenightText;
        [SerializeField] TMPro.TMP_Text pushDescText;
        [SerializeField] TMPro.TMP_Text pushConfirmBtnText;

        Coroutine toastCo;
        enum UIStep
        {
            First,Second,
        }
        public void Init()
        {
            Sprite titleImage=enimage;
            if(Application.systemLanguage==SystemLanguage.Korean)
            {
                titleImage = krimage;
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                titleImage = enimage;
            }
            else if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                titleImage = jpimage;
            }
            else if (Application.systemLanguage == SystemLanguage.Chinese)
            {
                titleImage = chimage;
            }
            else if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
            {
                titleImage = chimage;
            }
            else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
            {
                titleImage = chimage;
            }
            else
            {
                titleImage = enimage;
            }
            titleimage.sprite = titleImage;
            SocialLoginWindow.SetActive(false);
            GoogleLoginBtn.onClick.AddListener(()=> {

                SocialLoginWindow.SetActive(false);
                PlayerPrefs.SetInt("SocialLogined", 1);
                PlayerPrefs.SetInt("GuestLogin", 0);
                PlayerPrefs.SetInt("IOSLogin", 0);
                BlackTreeLogin.Instance.SocialInitialize(true,o => BlackTreeLogin.Instance.isSocialLogined=o).Forget();
             
            });

            AppleLoginBtn.onClick.AddListener(() => {

                SocialLoginWindow.SetActive(false);
                PlayerPrefs.SetInt("SocialLogined", 1);
                PlayerPrefs.SetInt("IOSLogin", 1);
                PlayerPrefs.SetInt("GuestLogin", 0);
                BlackTreeLogin.Instance.SocialInitialize(false, o => BlackTreeLogin.Instance.isSocialLogined = o).Forget();

            });

            GuestLoginProcessBtn.onClick.AddListener(() => {

                SocialLoginWindow.SetActive(false);
                PlayerPrefs.SetInt("SocialLogined", 1);
                PlayerPrefs.SetInt("GuestLogin", 1);
                PlayerPrefs.SetInt("IOSLogin", 0);
                BlackTreeLogin.Instance.GuestInitialize(o => BlackTreeLogin.Instance.isSocialLogined = o).Forget();
            });

            GuestLoginBtn.onClick.AddListener(()=> {
                GuestLoginWarningWindow.SetActive(true);
            });

            for(int i=0; i< OffGuestLoginWindow.Length; i++)
            {
                OffGuestLoginWindow[i].onClick.AddListener(() => {
                    GuestLoginWarningWindow.SetActive(false);
                });
            }
            


            AuthWindow.SetActive(false);
            Authconfirm.onClick.AddListener(()=> { 
                if(AuthCheck.isOn&&privateCheck.isOn)
                {
                    BlackTreeLogin.Instance.isAuthAgreed = true;
                    AuthWindow.SetActive(false);
                    PlayerPrefs.SetInt("BTAuth", 1);
                }
                else
                {
                    if (toastCo != null)
                    {
                        StopCoroutine(toastCo);
                        toastCo = null;
                        toastmessageObj.SetActive(false);
                    }

                    fadeTime = 0;
                    string localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DisagreeNotGame].StringToLocal;
                    toastMessageTxt.text = localized;
                    toastmessageObj.SetActive(true);
                    toastCo = StartCoroutine(FadeAndOff());
                }
            });
            AuthCheck.onValueChanged.AddListener((isOn) => {
                bool bothOn = AuthCheck.isOn==true && privateCheck.isOn == true;
                ignoreConfirm.SetActive(!bothOn);
            });
            privateCheck.onValueChanged.AddListener((isOn) => {
                bool bothOn = AuthCheck.isOn == true && privateCheck.isOn == true;
                ignoreConfirm.SetActive(!bothOn);
            });

            pushAlarm.onValueChanged.AddListener((isOn) => {
                if(isOn)
                {
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeOnlyDay;
                    pushAlarmCheck.SetActive(true);
                    pushAlarminNightCheck.SetActive(true);
                }
                else
                {
                    Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.Disagree;
                    pushAlarmCheck.SetActive(false);
                    pushAlarminNightCheck.SetActive(false);
                }
            });
            pushAlarminNight.onValueChanged.AddListener((isOn) => {
                if (pushAlarm.isOn)
                {
                    if (isOn)
                    {
                        Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeAll;
                        pushAlarminNightCheck.SetActive(true);
                    }
                    else
                    {
                        Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeOnlyDay;
                        pushAlarminNightCheck.SetActive(false);
                    }
                }
            });

            alarmConfirm.onClick.AddListener(()=> { PushAlarmPermissionProgress().Forget(); } );

            nicknameWindow.SetActive(false);
            nickConfirm.onClick.AddListener(CreateNickName);

            GoAuth.onClick.AddListener(OpenTerms);
            GoPrivate.onClick.AddListener(OpenPrivatePolicy);


            privateTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PrivateDesc].StringToLocal;
            termTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_TermDesc].StringToLocal;
            privateBtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_OpenLink].StringToLocal;
            termBtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_OpenLink].StringToLocal;
            confirmText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Confirm].StringToLocal;

            nicknameTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_NickCreate].StringToLocal; 
            nicknameDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NickDesc].StringToLocal;
            nicknameConfirmBtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Confirm].StringToLocal;

            pushTitle.text=StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Push].StringToLocal;
            pushAgreeText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PushAgree].StringToLocal;
            pushAgreenightText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PushAgreeNight].StringToLocal;
            pushDescText.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_PushDesc].StringToLocal;
            pushConfirmBtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Confirm].StringToLocal;
#if UNITY_ANDROID
            AppleLoginBtn.gameObject.SetActive(false);
#elif UNITY_IOS
            GoogleLoginBtn.gameObject.SetActive(false);
#endif
        }

        void OpenTerms()
        {
            Application.OpenURL(Urls.terms_of_service);
        }
        void OpenPrivatePolicy()
        {
            Application.OpenURL(Urls.privacy_policy);
        }

        public void StartSocialLogin()
        {
            curtainToMain.color = new Color(0, 0, 0, 0);
            curtainToMain.gameObject.SetActive(false);
            SocialLoginWindow.SetActive(true);
        }
        public void AuthWindowPopup()
        {
            curtainToMain.color = new Color(0, 0, 0, 0);
            curtainToMain.gameObject.SetActive(false);
            AuthWindow.SetActive(true);
        }
        public void PushAlarmWindowPopup()
        {
            curtainToMain.color = new Color(0, 0, 0, 0);
            curtainToMain.gameObject.SetActive(false);
            alarmWindow.SetActive(true);
            Model.Player.Cloud.optiondata.pushAlarm = Definition.PushAlaramType.AgreeAll;
            pushAlarmCheck.SetActive(true);
            pushAlarminNightCheck.SetActive(true);
        }
        string authtoken = string.Empty;
        async UniTask PushAlarmPermissionProgress()
        {
            Model.Player.Cloud.optiondata.SetDirty(true).UpdateHash();

            await GetAuthToken();

            switch (Model.Player.Cloud.optiondata.pushAlarm)
            {
                case Definition.PushAlaramType.NotDecided:
                    break;
                case Definition.PushAlaramType.AgreeOnlyDay:
#if UNITY_ANDROID
                    BackEnd.Backend.Android.PutDeviceToken();
                    Backend.Android.AgreeNightPushNotification(false);

#elif UNITY_IOS
                    Backend.iOS.PutDeviceToken(authtoken,isDevelopment.iosDev);

                    Backend.iOS.AgreeNightPushNotification(false);
#endif
                    InitializeAndroidLocalPush();
                    break;
                case Definition.PushAlaramType.AgreeAll:
#if UNITY_ANDROID
                    BackEnd.Backend.Android.PutDeviceToken();
                    Backend.Android.AgreeNightPushNotification(true);
#elif UNITY_IOS

Debug.LogError("Error_0_intro");
                     var bro= Backend.iOS.PutDeviceToken(authtoken,isDevelopment.iosDev);
                     Debug.LogError("broError:" + bro.GetStatusCode());
                     Debug.LogError("Error_1_intro");

                    Backend.iOS.AgreeNightPushNotification(true);
#endif
                    InitializeAndroidLocalPush();
                    break;
                case Definition.PushAlaramType.Disagree:
#if UNITY_ANDROID
                    BackEnd.Backend.Android.DeleteDeviceToken();
#elif UNITY_IOS
                    Backend.iOS.DeleteDeviceToken();
#endif
                    BlackTreeLogin.Instance.userPushAlarmComplete = true;
                    alarmWindow.SetActive(false);
                    break;
                default:
                    break;
            }
        }


        async UniTask GetAuthToken()
        {

#if UNITY_IOS
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;

             using (var req = new AuthorizationRequest(authorizationOption, true))
             {
                 while (!req.IsFinished)
                 {
                    await UniTask.Yield();
                 };

                 authtoken = req.DeviceToken;

                 Debug.Log("IOS Push Token Created : " + authtoken);
             }
#endif
            await UniTask.Yield();
        }

        int apiLevel;

        public void InitializeAndroidLocalPush()
        {
#if UNITY_ANDROID
            string androidInfo = SystemInfo.operatingSystem;
            Debug.Log("androidInfo: " + androidInfo);
            apiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
            Debug.Log("apiLevel: " + apiLevel);

            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                PermissionCallbacks pCallbacks = new PermissionCallbacks();
                pCallbacks.PermissionGranted += str => {
                    Debug.Log($"{str} ????");
                };
                pCallbacks.PermissionGranted += _ => ActionIfPermissionGranted(true); // ???? ?? ???? ????

                pCallbacks.PermissionDenied += str => {
                    Debug.Log($"{str} ????");
                    BlackTreeLogin.Instance.userPushAlarmComplete = true;
                    alarmWindow.SetActive(false);
                };
               
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", pCallbacks);
            }
            else
            {
                BlackTreeLogin.Instance.userPushAlarmComplete = true;
                alarmWindow.SetActive(false);
                Debug.Log($"???? ?????? ???? ???????? ????");
            }

#elif UNITY_IOS
            BlackTreeLogin.Instance.userPushAlarmComplete = true;
            alarmWindow.SetActive(false);
#endif
        }

        void ActionIfPermissionGranted(bool agree)
        {
            BlackTreeLogin.Instance.userPushAlarmComplete = true;
            alarmWindow.SetActive(false);
            Debug.Log($"?? ????");
        }

        public void NickNameWindowPopup()
        {
            nicknameWindow.SetActive(true);
        }

        float fadeTime;
        void CreateNickName()
        {
            string tempnick = inputfield.text;
            Model.Player.Cloud.optiondata.nickname = string.Format("{0}", tempnick);

            var datetime = DateTime.Now;
            string nowtime = datetime.ToIsoString();
            Model.Player.Cloud.optiondata.nicknameChangeUnlocktime = nowtime;

            var nicknamebro = Backend.BMember.UpdateNickname(Model.Player.Cloud.optiondata.nickname);

            string msg=null;
            string localized = "";
            if (nicknamebro.GetStatusCode()=="204")
            {
                BlackTreeLogin.Instance.nickNameDecided = true;
                nicknameWindow.SetActive(false);
                localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NickCreated].StringToLocal;
                msg = localized;
            }
            else if (nicknamebro.GetStatusCode() == "409")//????
            {
                localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NickSame].StringToLocal;
                msg = localized;
            }
            else
            {
                localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NotNickname].StringToLocal;
                msg = localized;
            }
            if (toastCo != null)
            {
                StopCoroutine(toastCo);
                toastCo = null;
                toastmessageObj.SetActive(false);
            }

            fadeTime = 0;
            toastMessageTxt.text = msg;
            toastmessageObj.SetActive(true);
            toastCo =StartCoroutine(FadeAndOff());
        
        }

        IEnumerator FadeAndOff()
        {
            while (fadeTime < 1)
            {
                fadeTime += Time.unscaledDeltaTime / 3;
                yield return null;
            }
            toastmessageObj.SetActive(false);
        }

        public void StartLoading()
        {
            curtainToMain.color = new Color(0, 0, 0, 0);
            curtainToMain.gameObject.SetActive(false);
            loadingComplete = false;
            currentTime = 0;

            toMainBtn.onClick.AddListener(GoToMain);
            titleObj.transform.localScale = Vector3.zero;
            titleObj.SetActive(false);

            textdescObj.SetActive(false);
            touchToStartText.text = "";

            loadingBar.gameObject.SetActive(true);
            loadingText.gameObject.SetActive(true);
            
            loadingBar.value = 0;
            loadingText.text = "0%";

            Core.SceneLoadResources.loadingAction += LoadingProgress;
        }

        public void LoadingProgress(float value)
        {
            //Debug.Log($"????????:{value}");
            float slidervalue = Mathf.Clamp(value, 0, 1);
            loadingBar.value = slidervalue;
            loadingText.text = string.Format("{0:F0}%", slidervalue * 100);
        }

        IEnumerator coLoading()
        {
            while (true)
            {
                switch (uiStep)
                {
                    case UIStep.First:
                        currentTime += Time.deltaTime*0.7f;
                        titleObj.transform.localScale = Vector3.Lerp(originalScale, zoomScale, currentTime);
                        touchToStartText.color = Color.Lerp(originalColor, hideColor, currentTime);
                        if(currentTime>=1)
                        {
                            currentTime = 0;
                            uiStep = UIStep.Second;
                        }
                        break;
                    case UIStep.Second:
                        currentTime += Time.deltaTime * 0.7f;
                        titleObj.transform.localScale = Vector3.Lerp(zoomScale, originalScale, currentTime);
                        touchToStartText.color = Color.Lerp(hideColor, originalColor, currentTime);
                        if (currentTime >= 1)
                        {
                            currentTime = 0;
                            uiStep = UIStep.First;
                        }
                        break;
                    default:
                        break;
                }
                yield return null;
            }
        }

        public void LoadingComplete()
        {
            loadingBar.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(false);
            textdescObj.SetActive(true);
            touchToStartText.text = "TOUCH TO START";
            loadingComplete = true;

            titleObj.SetActive(true);
            titleObj.transform.DOScale(1f, 1).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
                loopUpdate = StartCoroutine(coLoading());
            });
        }

        void GoToMain()
        {
            if(loadingComplete)
            {
                StartCoroutine(GoMain());
            }
        }

        IEnumerator GoMain()
        {
            loadingComplete = false;
            curtainToMain.gameObject.SetActive(true);
            curtainToMain.color = new Color(0, 0, 0, 0);
            Tween fadeTween = curtainToMain.DOFade(1, 0.7f);
            yield return fadeTween.WaitForCompletion();

            if (loopUpdate != null)
            {
                StopCoroutine(loopUpdate);
                loopUpdate = null;
            }
            
            //curtainToMain.DOFade(1, 0.7f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
            //    StopCoroutine(loopUpdate);
            //    loopUpdate = null;
            //});
            SceneManager.LoadScene(BTETC.Scene.mainStagescene);
        }
    }

}
