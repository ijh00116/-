using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Bundles
{
    [CreateAssetMenu(fileName = "InGameResources", menuName = "Bundle/InGameResources", order = 501)]
    public class InGameResourcesBundle : ScriptableObject
    {
        public static InGameResourcesBundle Loaded { get; private set; }
        private const string BundleResourcekey = "InGameResourcesBundle";

        [Header("유닛")]
        public ViewUnit unit;
        public ViewSubUnit subunit;
        public ViewSubUnitRenderer subunitRender;
        public Sprite[] unitUIAnimSprite;
        public ViewCompanionMonster viewCompanion;
        [Header("유닛(미니게임)")] 
        public ViewClickerUnit clickerUnit;
        public ViewClickerSubUnit clickerSubUnit;

        [Header("화살 오브젝트")]
        public ViewBulletForSoulUnit bullet;
        [Header("스테이지")]
        public ViewDefaultStage[] mapForchapter_0;
        public ViewDefaultStage[] mapForchapter_1;
        public ViewDefaultStage[] mapForchapter_2;
        public ViewDefaultStage[] mapForchapter_3;
        public ViewDefaultStage[] mapForchapter_4;
        public ViewDefaultStage[] stagelist;

        public ViewDefaultStage goldDungeon;
        public ViewDefaultStage EXPDungeon;
        public ViewDefaultStage AwakeDungeon;
        public ViewDefaultStage RiftDungeon;
        public ViewClickerStage ClickerDungeon;
        public ViewDefaultStage[] RaidDungeon;
        public ViewDefaultStage RuneDungeon;
        [Header("무기")]
        public Sprite[] weaponIcon;
        public Sprite[] bowIcon;
        public Sprite[] armorUIIcon;

        public Sprite[] weaponGradeInnerFrameSprite;

        [Header("스킬")]
        public Sprite[] skillIcon;

        [Header("이펙트")]
        public HitEffect swordHitEffect;
        public HitEffect bowHitEffect;
        public HitEffect lightningSpearEffect;

        [Header("Grey매테리얼")]
        public Material greyscale;

        public Material GodModeMat;
        public Material defaultMat;

        [Header("연구 아이콘 이미지")]
        public Sprite[] researchIconSprites;

        [Header("Particle_ETC")]
        public ParticleSystem soulStunParticle;
        public ParticleSystem soulRecoverParticle;

        [Header("debuff")]
        public GameObject stunDebuffObject;
        public GameObject freezeDebuffObject;

        public ParticleSystem landingDustExplosion;

        public ViewClickDungeonEnemyBullet viewClickenemyBullet;
        public ParticleSystem novaEffect;

        public ParticleSystem lvUpEffect_0;
        public ParticleSystem lvUpEffect_1;

        public ParticleSystem awakeParticle;

        [Header("shield obj")]
        public GameObject shieldObj;

        [Header("touchParticle")]
        public ParticleSystem touchParticle;
        public ParticleSystem touchParticleToSubUnit;
        public void Init()
        {
            Loaded = this;
        }
        public static async UniTask StartLoadAsset(CancellationTokenSource cts)
        {
            //Debug.Log("[StartLoadAsset]");

            Loaded = await Addressables.LoadAssetAsync<InGameResourcesBundle>(BundleResourcekey).WithCancellation(cts.Token);
        }
    }

}
