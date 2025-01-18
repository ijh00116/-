using BlackTree.Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;


namespace BlackTree.Model
{
    public static partial class Player
    {
        public static bool isFirstLogin=false;
    
        public static float LastCloudSaveTime { get; private set; }

        private static UserCloudData _cloudLoadData;
        public static UserCloudData Cloud
        {
            get
            {
                if (_cloudLoadData == null)
                    _cloudLoadData = Player.New();

                return _cloudLoadData;
            }
            set
            {
                _cloudLoadData = value;
            }
        }
        public static Action<bool> sleepMode;

        public static UserCloudData New()
        {
            var userCloudData = new UserCloudData { };
            return userCloudData;
        }


        public static async UniTask<bool> LoadLocalSaveinfoatStart()
        {
            return false;
        }

        #region local 계정

        private const string PlayerPrefsKey = "USER_LOCAL_SAVED_DATA";

        public static string GetLocalUuid()
        {
            return PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
        }
        public static void SetLocalUuid(string uuid)
        {
            PlayerPrefs.SetString(PlayerPrefsKey, uuid);
            PlayerPrefs.Save();
        }

        private static Dictionary<string, UserDataBase> _userDataMap = new Dictionary<string, UserDataBase>();

        public static void CreateCache()
        {
            foreach (var info in typeof(UserCloudData).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var data = (UserDataBase)info.GetValue(_cloudLoadData);
                _userDataMap.Add(info.Name, data);
            }
        }

        public static async UniTask SaveUserDataToFirebaseAndLocal()
        {
            LocalSaveLoader.SaveUserCloudData();
            await UpdateUserData(false, () => { });
       
                
        }

        public static async UniTask UpdateUserData(bool isLoading = false, UnityAction callback = null)
        {

            Dictionary<string, string> updateData = new Dictionary<string, string>();
            foreach (var kv in _userDataMap)
            {
                if (kv.Value.IsDirty)
                {
#if UNITY_EDITOR
                    //Debug.Log($"<color=#f0f>[UpdateData] {kv.Key}</color>");
#endif
                    updateData.Add(kv.Key, kv.Value.GetDataString());
                    kv.Value.SetDirty(false);
                }
            }
            if (updateData.Count > 0)
            {
                if (isLoading)
                    Bundles.LoadingManager.Instance
                        .SetBackgroundAlpha(0.3f)
                        .Open();

                var tasks = new List<UniTask>();
                foreach (var kv in updateData)
                {
                    //서버 저장
                    tasks.Add(FirebaseRD.UpdateData(kv.Key, kv.Value));
                }
                await UniTask.WhenAll(tasks);
                if (isLoading)
                    Bundles.LoadingManager.Instance.Close();
            }

            callback?.Invoke();
        }
       

        public static async UniTask SaveFullUserData(bool isLoading = false, UnityAction callback = null)
        {
            if (isLoading)
               Bundles.LoadingManager.Instance
                    .SetBackgroundAlpha(0.3f)
                    .Open();

            //파베에 저장
            //await FirebaseLinker.SaveFullData(_cloudLoadData);
            //Debug.Log("[AccountCloudSave] saved");
            LastCloudSaveTime = UnityEngine.Time.time;
            if (isLoading) 
                Bundles.LoadingManager.Instance.Close();
            callback?.Invoke();
        }

        private static void CheckHash(UserDataBase data)
        {
            if (!data.IsValidHash())
            {
                //Debug.LogWarning($"[CheckHash] invalid hash\n{data.GetType().Name}\n{data.GetDataString()}");
                //ErrorScene.Open("Invalid Access");
                throw new Exception("[CheckHash] invalid hash");
            }
        }

        #endregion
    }
}
