using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Bundles
{
    [CreateAssetMenu(fileName = "SkillResourcesBundle", menuName = "Bundle/SkillResourcesBundle", order = 501)]
    public class SkillResourcesBundle : ScriptableObject
    {
        public static SkillResourcesBundle Loaded { get; private set; }
        private const string BundleResourcekey = "SkillResourcesBundle";

        public ViewPowerfullAttack[] powerAttackSkillview;//3연속 강한 공격 발사
        public ViewMultipleMissileSkill multipleFireSkillView; //부채꼴로 미사일 발사
        public ViewAtkIncreaseSkill atkIncreaseSkillview; //공격력 증가
        public ViewFireRainSkill viewFireRainSkill;
        public GameObject viewFireRainRangeObject;
        public ViewGuideMissileSkill viewHommingMissile_0; // 미사일 2개 발사
        public ViewGuideMissileSkill viewHommingMissile_1;// 미사일 2개 발사
        public ViewMissileFireEffect viewShotEffect;//미사일2개발사 스킬에서 본체 이펙트
        public ViewExplodePoisonDamage viewExplodePoisonDmg; //적들 데미지 (폭발)_수정할것
        public ViewMoveIncreaseSkill moveIncreaseSkillView;//이속 증가
        public ViewSwordFewHitFireSkill swordFewHitSkillView;// 

        public ViewAbsorbLifeSkill absorbLifeskill; //체력 흡수
        public ViewRecoverHpTickSkill viewRecovertickHp;//체력 회복
        public ParticleSystem viewHpRecoverTick;
        public ViewCallGodSkill viewCallGodSkill;// 갓모드

        public ViewLightningSpear lightningSpear;
        public ViewNovaSkill viewNovaSkill;
        public ViewBigFireBall viewBigFireBall;

        public ViewRangeStun viewRangeStun;
        public ViewRecoverShield viewRecoverShield;
        public ParticleSystem viewShieldRecoverTick;
        public ViewSkillSubUnitSkill _viewSkillSubUnit;
        public ViewPowerFullGodAttack godAttackSkill;

        public ViewMagicFewHitFireSkill magicFewHitSkillView;
        public ViewTurretSkill turretskillView;
        [Header("laserbeam")]
        public ViewLaserBeamSkill laserSkillView;

        public ViewMeteorSkill meteorskillView;
        public ViewMeteorExplode meteorExplode;
        public ViewMeteorFire meteorFire;

        public ViewSkyLight viewSkyLight;

        public ViewMultipleLightningSkill viewMultipleLightning;

        public ViewTimeBomb timeBombObj;
        public ViewTimeBombUI timeBombUIObj;

        [Header("petSkill")]
        public ViewPetLightningSkill petLightningSpearSkill;
        public ViewPetMultiFireSkill petMultiFireSkill;
        public ViewPetSwordFewHitSkill petSwordFewHitSkill;
        public ViewPetFireRainSkill petFireRainSkill;
        public ViewPetBigFireSkill petBigFireSkill;
        public ViewPetSunLightSkill petSunLightSkill;
        public ViewPetRangeStunSkill petStunSkill;
        public ViewPetMultiLightningSkill petMultiLightningSkill;
        public void Init()
        {
            Loaded = this;
        }
        public static async UniTask StartLoadAsset(CancellationTokenSource cts)
        {
            //Debug.Log("[StartLoadAsset]");
            Loaded = await Addressables.LoadAssetAsync<SkillResourcesBundle>(BundleResourcekey).WithCancellation(cts.Token);
        }
    }

}
