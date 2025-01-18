using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region 파동발사
    public class CharacterGuideMissileSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        List<ViewGuideMissileSkill> missileList = new List<ViewGuideMissileSkill>();
        const int defaultMissileNum = 5;

        Player.Skill.SkillCacheData skillCache;

        int skillPrefabIndex = 0;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;
        int maxSkillCount;

        public CharacterGuideMissileSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.GuidedMissile];
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            var subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
            {
                if (subUnitskillcache.userSkilldata.AwakeLv >= 2)
                {
                    CreateSkillview(Definition.WitchSkillType.SkillWitch);
                }
            }
            CreateSkillview(Definition.WitchSkillType.Witch);
          
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_1);
        }

        void CreateSkillview(Definition.WitchSkillType witchtype)
        {
            for (int i = 0; i < maxSkillCount; i++)
            {
                ViewGuideMissileSkill missileObj= missileList.Find(o=>o.gameObject.activeInHierarchy==false);
                
                if (missileObj==null)
                {
                    int index = Random.Range(0, 2);
                    ViewGuideMissileSkill prefab = index == 0 ? SkillResourcesBundle.Loaded.viewHommingMissile_0 : SkillResourcesBundle.Loaded.viewHommingMissile_1;
                    missileObj = UnityEngine.Object.Instantiate(prefab);
                    missileList.Add(missileObj);
                }
                else
                {
                    //missileObj = missileList[i];
                }
                missileObj.gameObject.SetActive(true);
                missileObj.Init(skillValue_1, skillValue_2);

                if (witchtype == Definition.WitchSkillType.Witch)
                {
                    missileObj.Shoot(Player.Unit.usersubUnit._view.transform.position,witchtype);
                }
                else
                {
                    missileObj.Shoot(Player.Skill.skillSubUnitObject._view.transform.position, witchtype);
                }
            }
        }
        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                for(int i=0; i< missileList.Count; i++)
                {
                    missileList[i].gameObject.SetActive(false);
                }
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.GuidedMissile, false);
                Player.Rune.SyncAllData();
            }
        }

        public override eActorState GetState()
        {
            return eActorState.GuidedMissile;
        }

        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(50);
            _unit.animindex = 0;

            skillPrefabIndex = 0;
            skillValue_0 = Player.Skill.Get(Definition.SkillKey.GuidedMissile).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.GuidedMissile).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.GuidedMissile).SkillValue(2);

            maxSkillCount = (int)skillValue_0;

            for (int i = 0; i < missileList.Count; i++)
            {
                missileList[i].gameObject.SetActive(false);
            }

            CreateSkillEffect();

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.GuidedMissile, true);

            AudioManager.Instance.Play(AudioSourceKey.Skill_GuidedMissile);
            _unit._state.ChangeState(eActorState.Idle);

            Player.Rune.SyncAllData();
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {
           
        }

        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.GuidedMissile))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                       
                        if (skillCache.elapsedCooltime >= skillCache.tabledataSkill.effectTime)
                        {
                            for (int i = 0; i < missileList.Count; i++)
                            {
                                missileList[i].gameObject.SetActive(false);
                            }
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.GuidedMissile, false);
                            Player.Rune.SyncAllData();
                        }
                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
    #endregion
}