using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;
using System.Text;
using Cysharp.Threading.Tasks;
using BlackTree;

public class FirebaseManager : MonoBehaviour
{
    class LogModel
    {
        public string _eventName = null;
        public Parameter[] _params = null;
    }
    private FirebaseApp firebaseApp;
    private static FirebaseManager instance;
    public static FirebaseManager Instance
    {
        get { return instance; }
    }

    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    const string fcmTokenKey = "fcmToken";
    public static string FcmToken
    {
        get
        {
            return PlayerPrefs.GetString(fcmTokenKey, string.Empty);
        }
        set
        {
            PlayerPrefs.GetString(fcmTokenKey, string.Empty);
            PlayerPrefs.Save();
        }
    }

    bool IsSomeWhereInitOnGoing = false;
    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public async UniTaskVoid FirebaseAnalyticsInit(Action logeventCallback)
    {
        IsSomeWhereInitOnGoing = true;
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith((task) =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                Firebase.Analytics.FirebaseAnalytics.SetUserId(BTETC.userUUID);
                Firebase.Analytics.FirebaseAnalytics.SetUserProperty("userLevel",BlackTree.Model.Player.Cloud.userLevelData.currentLevel.ToString());
                Firebase.Analytics.FirebaseAnalytics.SetUserProperty("ChapterLevel", BlackTree.Model.Player.Cloud.field.bestChapter.ToString());

                logeventCallback?.Invoke();
            }
        });
    }

    public void InitializeFirebase(Dictionary<string, object> defaultDic)
    {
        TimeSpan time = TimeSpan.FromMilliseconds(600000);
        Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(time);

    }
   
    class FirebaseLogParam
    {
        public string eventName;
        public Parameter[] paramArray;
    }
    List<FirebaseLogParam> _eventPool = new List<FirebaseLogParam>();


    void CheckProcess()
    {
        if (isProgressing)
            return;
        if (_eventPool.Count > 0)
        {
            isProgressing = true;

            if (_eventPool[0].paramArray != null)
            {
                EventLoading = StartCoroutine(waitLogEvent(_eventPool[0].eventName, _eventPool[0].paramArray));
            }
            else
            {
                EventLoading = StartCoroutine(waitLogEvent(_eventPool[0].eventName));
            }
            _eventPool.RemoveAt(0);
        }
    }

    bool isProgressing = false;
    Coroutine EventLoading = null;
    //비동기 작업으로 설정
    public void LogEvent(string eventName)
    {
#if  !UNITY_EDITOR
        FirebaseLogParam _event = new FirebaseLogParam() {eventName=eventName,paramArray=null };
        _eventPool.Add(_event);
        CheckProcess();
#endif
    }


    private IEnumerator waitLogEvent(string eventName)
    {
#if !UNITY_EDITOR
        yield return FirebaseAnalyticsInit(() => FirebaseAnalytics.LogEvent(eventName));

        EventLoading = null;
        isProgressing = false;

        CheckProcess();
#endif
        yield break;
    }

    public void LogEvent(string eventName, params Parameter[] paramArray)
    {
#if !UNITY_EDITOR
        FirebaseLogParam _event = new FirebaseLogParam() { eventName = eventName, paramArray = paramArray };
        _eventPool.Add(_event);
        CheckProcess();
#endif
    }

    private IEnumerator waitLogEvent(string eventName, params Parameter[] paramArray)
    {
 
        yield return FirebaseAnalyticsInit(() => FirebaseAnalytics.LogEvent(eventName, paramArray));

        isProgressing = false;
        CheckProcess();

        yield break;
    }
}
