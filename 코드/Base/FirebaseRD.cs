using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;
using System;
using Firebase.Auth;
using System.Text;
using System.Security.Cryptography;

namespace BlackTree
{
    public static class FirebaseRD
    {
        private static DatabaseReference referenceRoot;
        private static FirebaseApp app;
		private static FirebaseAuth auth;
        private static FirebaseUser user = null;

        public static async UniTask Init()
        {
			var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if(dependencyStatus == Firebase.DependencyStatus.Available)
            {
				app = FirebaseApp.DefaultInstance;
				auth = FirebaseAuth.DefaultInstance;
				auth.StateChanged += StateChanged;

				referenceRoot = FirebaseDatabase.GetInstance(app).RootReference;

			}
		}


		private static void StateChanged(object sender,EventArgs e)
        {
			FirebaseAuth firebaseAuth = sender as FirebaseAuth;
			if(firebaseAuth!=null)
            {
				user=firebaseAuth.CurrentUser;
				if(user!=null)
                {
                }
			}
        }

		public static async UniTask Signin(System.Action<bool> callback)
        {
			bool isLoginFail = false;
			bool isProcessSuccess = false;
#if UNITY_EDITOR
			if (BTETC.isGuestUser)
			{
				await auth.SignInAnonymouslyAsync().ContinueWith(task => {
					if (task.IsCanceled)
					{
						isProcessSuccess = false;
						return;
					}
					if (task.IsFaulted)
					{
						isProcessSuccess = false;
						return;
					}
					
					isProcessSuccess = true;
				});
				callback?.Invoke(isProcessSuccess);
			}
			else
            {
				await auth.SignInWithEmailAndPasswordAsync("ijh0116@naver.com", "ehfapddl12!").ContinueWith(task => {
					if (task.IsCanceled)
					{
						Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
						isLoginFail = true;
					}
					if (task.IsFaulted)
					{
						Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
						isLoginFail = true;
					}

					if (task.IsCompletedSuccessfully)
					{
						//var result = task.Result;
						//Firebase.Auth.FirebaseUser result = task.Result;
						
						Debug.LogFormat("User signed in successfully: {0} ({1})",
							task.Result.DisplayName, task.Result.UserId);
						isProcessSuccess = true;
					}

				});

				if (isLoginFail)
				{
					await auth.CreateUserWithEmailAndPasswordAsync("ijh0116@naver.com", "ehfapddl12!").ContinueWith(task => {
						if (task.IsCanceled)
						{
							Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
							isProcessSuccess = false;
							return;
						}
						if (task.IsFaulted)
						{
							Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
							isProcessSuccess = false;
							return;
						}

						isProcessSuccess = true;
						// Firebase user has been created.
						//Firebase.Auth.AuthResult result = task.Result;
						Debug.LogFormat("User signed in successfully: {0} ({1})",
							task.Result.DisplayName, task.Result.UserId);
					});
				}
				callback?.Invoke(isProcessSuccess);
			}


#elif UNITY_ANDROID
			if (BTETC.isGuestUser)
			{
				await auth.SignInAnonymouslyAsync().ContinueWith(task => {
					if (task.IsCanceled)
					{
						isProcessSuccess = false;
						return;
					}
					if (task.IsFaulted)
					{
						isProcessSuccess = false;
						return;
					}

					isProcessSuccess = true;
				});
				callback?.Invoke(isProcessSuccess);
			}
			else
            {
				Firebase.Auth.Credential credential = GoogleAuthProvider.GetCredential(BTETC.googleTokenID, null);
				await auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
					if (task.IsCanceled)
					{
						isProcessSuccess = false;
						return;
					}
					if (task.IsFaulted)
					{
						isProcessSuccess = false;
						return;
					}

					isProcessSuccess = true;
				});
				callback?.Invoke(isProcessSuccess);
			}
#elif UNITY_IOS
	if (BTETC.isGuestUser)
			{
				await auth.SignInAnonymouslyAsync().ContinueWith(task => {
					if (task.IsCanceled)
					{
						isProcessSuccess = false;
						return;
					}
					if (task.IsFaulted)
					{
						isProcessSuccess = false;
						return;
					}

					isProcessSuccess = true;
				});
				callback?.Invoke(isProcessSuccess);
			}
			else
			{
				var rawNonce = GenerateRandomString(32);
				Firebase.Auth.Credential credential = Firebase.Auth.OAuthProvider.GetCredential("apple.com", BlackTreeLogin.Instance.IdToken, rawNonce,
				BlackTreeLogin.Instance.authorizationCodeForIOS);

				await auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
					if (task.IsCanceled)
					{
						isProcessSuccess = false;
						return;
					}
					if (task.IsFaulted)
					{
						isProcessSuccess = false;
						return;
					}

					isProcessSuccess = true;
				});
				callback?.Invoke(isProcessSuccess);
			}
#endif


		}


		private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
		{
			var sha = new SHA256Managed();
			var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
			var hash = sha.ComputeHash(utf8RawNonce);

			var result = string.Empty;
			for (var i = 0; i < hash.Length; i++)
			{
				result += hash[i].ToString("x2");
			}

			return result;
		}

		private static string GenerateRandomString(int length)
		{
			const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
			var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
			var result = string.Empty;
			var remainingLength = length;

			var randomNumberHolder = new byte[1];
			while (remainingLength > 0)
			{
				var randomNumbers = new List<int>(16);
				for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
				{
					cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
					randomNumbers.Add(randomNumberHolder[0]);
				}

				for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
				{
					if (remainingLength == 0)
					{
						break;
					}

					var randomNumber = randomNumbers[randomNumberIndex];
					if (randomNumber < charset.Length)
					{
						result += charset[randomNumber];
						remainingLength--;
					}
				}
			}

			return result;
		}

		public static async UniTask SaveFullData(UserCloudData data)
        {

#if UNITY_EDITOR
			await referenceRoot
			.Child("users")
			.Child("BTadmin")
			.Child("cloudData")
			.SetRawJsonValueAsync(JsonUtility.ToJson(data)).ContinueWithOnMainThread(task => {
				if (task.IsFaulted)
				{
					UnityEngine.Debug.LogError(task.Exception);
				}
			});
#elif UNITY_ANDROID
			await referenceRoot
			.Child("users")
			.Child(BTETC.userUUID)
			.Child("cloudData")
			.SetRawJsonValueAsync(JsonUtility.ToJson(data)).ContinueWithOnMainThread(task => {
				if (task.IsFaulted)
				{
					UnityEngine.Debug.LogError(task.Exception);
				}
			});
#elif UNITY_IOS
	await referenceRoot
					.Child("users")
					.Child(BlackTreeLogin.Instance.userIDForSave)
					.Child("cloudData")
					.SetRawJsonValueAsync(JsonUtility.ToJson(data)).ContinueWithOnMainThread(task => {
						if (task.IsFaulted)
						{
							UnityEngine.Debug.LogError(task.Exception);
						}
					});
#endif

		}

		public static async UniTask UpdateData(string key, string value)
		{

#if UNITY_EDITOR
			await referenceRoot
			.Child("users")
			.Child("BTadmin")
			.Child("cloudData")
			.Child(key)
			.SetRawJsonValueAsync(value);
#elif UNITY_ANDROID
	await referenceRoot
				.Child("users")
				.Child(BTETC.userUUID)
				.Child("cloudData")
				.Child(key)
				.SetRawJsonValueAsync(value);
#elif UNITY_IOS
await referenceRoot
				.Child("users")
				.Child(BlackTreeLogin.Instance.userIDForSave)
				.Child("cloudData")
				.Child(key)
				.SetRawJsonValueAsync(value);
#endif

			//#endif
		}

		public static async UniTask DeleteMyData()
		{

#if UNITY_EDITOR
			var _ref = referenceRoot.Child("users").Child("BTadmin");
			await _ref.RemoveValueAsync();
#elif UNITY_ANDROID
			var _ref = referenceRoot.Child("users").Child(BTETC.userUUID);
			await _ref.RemoveValueAsync();
#elif UNITY_IOS
			var _ref = referenceRoot.Child("users").Child(BlackTreeLogin.Instance.userIDForSave);
			await _ref.RemoveValueAsync();
#endif
			//#endif
		}
		public static async UniTask InitUserData()
		{
#if UNITY_ANDROID
			var reference = referenceRoot
			.Child("users")
			.Child(BTETC.userUUID)
			.Child("cloudData")
			.Child("CreateVersion");
#elif UNITY_IOS
			var reference = referenceRoot
		.Child("users")
		.Child(BlackTreeLogin.Instance.userIDForSave)
		.Child("cloudData")
		.Child("CreateVersion");
#endif

			await reference.SetValueAsync(Application.version);

		}

		public static async UniTask<UserCloudData> Load()
		{

#if BT_TEST
			var tempdata = new UserCloudData();
			return tempdata;
#endif
			Firebase.Database.DatabaseReference reference;

#if UNITY_EDITOR
			if(Bundles.UpgradeResourcesBundle.Loaded.IsTemporaryLogin)
            {
				reference = referenceRoot
				.Child("users")
				.Child(Bundles.UpgradeResourcesBundle.Loaded.TempGoogleuserID)
				//.Child(BTETC.userUUID)
				.Child("cloudData");

			}
            else
            {
				reference = referenceRoot
				.Child("users")
				//.Child(Bundles.UpgradeResourcesBundle.Loaded.TempGoogleuserID)
				.Child(BTETC.userUUID)
				.Child("cloudData");
			}
#elif UNITY_ANDROID
	reference = referenceRoot
				.Child("users")
				//.Child(Bundles.UpgradeResourcesBundle.Loaded.TempGoogleuserID)
				.Child(BTETC.userUUID)
				.Child("cloudData");
#elif UNITY_IOS

	reference = referenceRoot
				.Child("users")
				.Child(BlackTreeLogin.Instance.userIDForSave)
				.Child("cloudData");
#endif
			Debug.Log("???????????? ???????? ???????? ????!");
			UserCloudData data = null;


		
			Debug.Log("???????????? ???????? ??????");
			await reference
				.GetValueAsync().ContinueWithOnMainThread(task =>
				{
					if (task.IsFaulted)
					{
						Debug.Log("???????????? taskException ??????");
						UnityEngine.Debug.LogError(task.Exception);
						Debug.Log("?? ??????");
						throw new Exception("[Player.firebase.Load] get task faulted");
					}
					else if (task.IsCompleted)
					{
						data = JsonUtility.FromJson<UserCloudData>(task?.Result?.GetRawJsonValue());
							// Do something with snapshot...
					}
				});

			return data;

		}

		public static void DeleteUserDB()
        {
			//var _ref=referenceRoot.Child("users");
			//_ref.RemoveValueAsync();

		}


		public static async UniTask SaveTableDataToFirebase()
        {
			var tableData = Core.StaticData.Wrapper;

			var tempdata = Player.Cloud;
			string versionString = Application.version.Replace(".","_");
			await referenceRoot
			.Child("TableData").Child(versionString)
			.SetRawJsonValueAsync(JsonUtility.ToJson(tableData)).ContinueWithOnMainThread(task => {
				if (task.IsFaulted)
				{
					UnityEngine.Debug.LogError(task.Exception);
				}
			});

		}

		public static async UniTask<Definition.StaticDataWrapper> LoadTableDataFromFirebase()
		{
			BlackTree.Definition.StaticDataWrapper tempWrapper=null;

			string versionString = Application.version.Replace(".", "_");
			var reference = referenceRoot
				.Child("TableData").Child(versionString);
			Debug.Log("???????????? ???????? ??????");
			await reference
				.GetValueAsync().ContinueWithOnMainThread(task =>
				{
					if (task.IsFaulted)
					{
						Debug.Log("???????????? taskException ??????");
						UnityEngine.Debug.LogError(task.Exception);
						Debug.Log("?? ??????");
						throw new Exception("[Player.firebase.Load] get task faulted");
					}
					else if (task.IsCompleted)
					{
						
						tempWrapper = JsonUtility.FromJson<Definition.StaticDataWrapper>(task?.Result?.GetRawJsonValue());
						// Do something with snapshot...
					}
				});

			return tempWrapper;

		}
	}

}
