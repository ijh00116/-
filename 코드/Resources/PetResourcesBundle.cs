using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Bundles
{
    [CreateAssetMenu(fileName = "PetResourcesBundle", menuName = "Bundle/PetResourcesBundle", order = 501)]
    public class PetResourcesBundle : ScriptableObject
    {
        public static PetResourcesBundle Loaded { get; private set; }
        private const string BundleResourcekey = "PetResourcesBundle";

        [Header("Pet ¿ÃπÃ¡ˆ")]
        public Model.PetSpriteInfo[] petImage;
        public ViewPet viewPetPrefab;

        public Sprite[] runeImage;
        public void Init()
        {
            Loaded = this;
        }


      
        public static async UniTask StartLoadAsset(CancellationTokenSource cts)
        {
            //Debug.Log("[StartLoadAsset]");
            Loaded = await Addressables.LoadAssetAsync<PetResourcesBundle>(BundleResourcekey).WithCancellation(cts.Token);
        }
    }

}

