using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BlackTree.Bundles
{
    [CreateAssetMenu(fileName = "UpgradeResourcesBundle", menuName = "Bundle/UpgradeResourcesBundle", order = 504)]
    public class UpgradeResourcesBundle : ScriptableObject
    {
        public static UpgradeResourcesBundle Loaded { get; private set; }


        [Header("Volatile Icon Image")]
        public Sprite[] GoldUpgradeSprites;
        public Sprite[] Tier2GoldUpgradeSprites;
        [Header("Stat Icon Image")]
        public Sprite[] StatUpSprites;

        public Sprite[] AwakeUpSprites;

        public bool IsTemporaryLogin;
        public string TempGoogleuserID;

        private const string BundleResourcekey = "UpgradeResourcesBundle";
        public static async UniTask StartLoadAsset(CancellationTokenSource cts)
        {
            Loaded = await Addressables.LoadAssetAsync<UpgradeResourcesBundle>(BundleResourcekey).WithCancellation(cts.Token);
        }
    }

}