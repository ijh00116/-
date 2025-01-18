using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Bundles
{
    [CreateAssetMenu(fileName = "StageResourcesBundle", menuName = "Bundle/StageResourcesBundle", order = 501)]
    public class StageResourcesBundle : ScriptableObject
    {
        public static StageResourcesBundle Loaded { get; private set; }
        private const string BundleResourcekey = "StageResourcesBundle";

        [Header("스테이지 아이콘")]
        public Sprite[] stageSlotBGSprite;
        public Sprite[] raidDungeonthumbnailList;
        public Sprite[] raidDungeonBossImage;
        public void Init()
        {
            Loaded = this;
        }
        public static async UniTask StartLoadAsset(CancellationTokenSource cts)
        {
            //Debug.Log("[StartLoadAsset]");
            Loaded = await Addressables.LoadAssetAsync<StageResourcesBundle>(BundleResourcekey).WithCancellation(cts.Token);
        }
    }
}
