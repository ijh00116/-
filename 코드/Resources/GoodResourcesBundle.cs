using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace BlackTree.Bundles
{
    [System.Serializable]
    public class PackageAnimationData
    {
        public float posX;
        public float posY;
        public float height;
        public float width;
        public Sprite[] images;
    }
    [CreateAssetMenu(fileName = "GoodResourcesBundle", menuName = "Bundle/GoodResourcesBundle", order = 503)]
    public class GoodResourcesBundle : ScriptableObject
    {
        public static GoodResourcesBundle Loaded { get; private set; }

        [Header("Good Image")]
        public Sprite[] goodSprites;
        public Sprite NormalAtkImage;

        [Header("reward Image")]
        public Sprite[] rewardSprites;
        public Sprite[] rewardSlotBGFrame;

        [Header("inappProduct")]
        public Sprite[] productBackgroundSprites;
        public Sprite[] productGoodsSprites;

        public Sprite[] packageBackgroundSprites;
        public Sprite[] packageBoxSprites;
        public Sprite[] packageCharacterSprites;

        public PackageAnimationData[] packageCharacterImages;

        public ViewPackageRewardSlot packageRewardslotPrefab;


        private const string BundleResourcekey = "GoodResourcesBundle";
        public static async UniTask StartLoadAsset(CancellationTokenSource cts)
        {
            Loaded = await Addressables.LoadAssetAsync<GoodResourcesBundle>(BundleResourcekey).WithCancellation(cts.Token);
        }
    }
}