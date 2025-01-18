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

        public ViewPowerfullAttack[] powerAttackSkillview;//3���� ���� ���� �߻�
        public ViewMultipleMissileSkill multipleFireSkillView; //��ä�÷� �̻��� �߻�
        public ViewAtkIncreaseSkill atkIncreaseSkillview; //���ݷ� ����
        public ViewFireRainSkill viewFireRainSkill;
        public GameObject viewFireRainRangeObject;
        public ViewGuideMissileSkill viewHommingMissile_0; // �̻��� 2�� �߻�
        public ViewGuideMissileSkill viewHommingMissile_1;// �̻��� 2�� �߻�
        public ViewMissileFireEffect viewShotEffect;//�̻���2���߻� ��ų���� ��ü ����Ʈ
        public ViewExplodePoisonDamage viewExplodePoisonDmg; //���� ������ (����)_�����Ұ�
        public ViewMoveIncreaseSkill moveIncreaseSkillView;//�̼� ����
        public ViewSwordFewHitFireSkill swordFewHitSkillView;// 

        public ViewAbsorbLifeSkill absorbLifeskill; //ü�� ���
        public ViewRecoverHpTickSkill viewRecovertickHp;//ü�� ȸ��
        public ParticleSystem viewHpRecoverTick;
        public ViewCallGodSkill viewCallGodSkill;// �����

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
