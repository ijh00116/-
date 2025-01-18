using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterMagicFewHitFireSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        static Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_0 = 0;
        static float skillValue_1 = 0;
        float skillValue_2 = 0;

        const int needAttackCount = 5;
        const int magicCount = 3;
        List<ViewMagicFewHitFireSkill> magicewHitObjList = new List<ViewMagicFewHitFireSkill>();
        public static int FireAttackCount()
        {
            int needCount = (int)System.Math.Round((needAttackCount - skillCache.SkillValue(1)));
            return needCount;
        }

        public CharacterMagicFewHitFireSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.MagicFewHitFire];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.MagicFewHitFire];
            Player.Skill.SkillActivate += SkillActivate;

            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            Main().Forget();
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_SwordFewHitFire);
            _unit._state.ChangeState(eActorState.Idle);
        }


        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            var subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
            {
                if(subUnitskillcache.userSkilldata.AwakeLv>=2)
                {
                    CreateSkillview(Definition.WitchSkillType.SkillWitch);
                }
            }
            CreateSkillview(Definition.WitchSkillType.Witch);

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }
        
        void CreateSkillview(Definition.WitchSkillType witchtype)
        {
            ControllerEnemyInGame targetEnemy;
            if (witchtype==Definition.WitchSkillType.Witch)
                targetEnemy = Battle.Enemy.GetClosedEnemyController(Player.Unit.usersubUnit._view.transform);
            else
                targetEnemy = Battle.Enemy.GetClosedEnemyController(Player.Skill.skillSubUnitObject._view.transform);

            if (targetEnemy == null)
                return;

            int missileNum = magicCount + (int)skillValue_2;
            for (int j = 0; j < missileNum; j++)
            {
                ViewMagicFewHitFireSkill tempObj = null;
                for (int i = 0; i < magicewHitObjList.Count; i++)
                {
                    if (magicewHitObjList[i].gameObject.activeInHierarchy == false)
                    {
                        tempObj = magicewHitObjList[i];
                        break;
                    }
                }
                if (tempObj == null)
                {
                    tempObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.magicFewHitSkillView);
                    magicewHitObjList.Add(tempObj);
                }
                tempObj.particle.Stop();
                tempObj.gameObject.SetActive(true);
                if (witchtype == Definition.WitchSkillType.Witch)
                {
                    tempObj.transform.position = Player.Unit.usersubUnit._view.transform.position;
                }
                else
                {
                    tempObj.transform.position = Player.Skill.skillSubUnitObject._view.transform.position;
                }
                    
                tempObj.particle.Play();
                Vector2 rotation;
                if (witchtype == Definition.WitchSkillType.Witch)
                    rotation = targetEnemy._view.transform.position - Player.Unit.usersubUnit._view.transform.position;
                else
                    rotation = targetEnemy._view.transform.position - Player.Skill.skillSubUnitObject._view.transform.position;

                float bulletAngleBetween = ((j - (int)(missileNum / 2)) * missileNum) * 5.0f;

                float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg + bulletAngleBetween;
                Vector3 dirVector = ConvertAngleToVector(rotz);
                tempObj.MoveToArr(dirVector, skillValue_0, witchtype);
            }
        }

        Vector3 ConvertAngleToVector(float _deg)
        {
            var rad = _deg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        public override eActorState GetState()
        {
            return eActorState.MagicFewHitFire;
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {

        }
        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                for (int i = 0; i < magicewHitObjList.Count; i++)
                {
                    magicewHitObjList[i].gameObject.SetActive(false);
                }
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.MagicFewHitFire))
                {
                    if (_unit._state.stop == false)
                    {

                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
}