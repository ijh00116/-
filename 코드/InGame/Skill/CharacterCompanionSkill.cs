using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterCompanionSkill : CharacterState
    {
        ControllerUnitInGame _unit;
        List<ControllerCompanionMonster> companionControllerList= new List<ControllerCompanionMonster>();

        static Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_0 = 0;
        float skillValue_1 = 0;
        static float skillValue_2;

        CancellationTokenSource _cts;
        public static int FireAttackCount()
        {
            int needCount = 11 - (int)skillCache.SkillValue(2);
            return needCount;
        }

        public CharacterCompanionSkill(ControllerUnitInGame unit)
        {
            _unit = unit;
            _cts = new CancellationTokenSource();
            skillCache = Player.Skill.skillCaches[Definition.SkillKey.CompanionSpawn];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.CompanionSpawn];
            Player.Skill.SkillActivate += SkillActivate;

            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            Main().Forget();
            CountProgress().Forget();

            Battle.Field.skillCompanionDelete += AllunitDisappear;
        }

        void AllunitDisappear()
        {
            for(int i=0; i< companionControllerList.Count; i++)
            {
                companionControllerList[i]._state.ChangeState(eActorState.InActive);
                companionControllerList[i]._view.gameObject.SetActive(false);
            }
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_CompanionSpawn);
            _unit._state.ChangeState(eActorState.Idle);
        }


        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            ControllerCompanionMonster tempObj = null;
            for (int i = 0; i < companionControllerList.Count; i++)
            {
                if (companionControllerList[i]._view.gameObject.activeInHierarchy == false)
                {
                    tempObj = companionControllerList[i];
                    break;
                }
            }
            if (tempObj == null)
            {
                tempObj = new ControllerCompanionMonster(InGameObjectManager.Instance.transform, _cts);
                companionControllerList.Add(tempObj);
            }
            tempObj.Activate(skillCache,Player.Unit.userUnit.target);

            //Player.Rune.companioncount++;
        }

        public override eActorState GetState()
        {
            return eActorState.CompanionSpawn;
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
                for (int i = 0; i < companionControllerList.Count; i++)
                {
                    companionControllerList[i]._state.ChangeState(eActorState.InActive);
                    companionControllerList[i]._view.gameObject.SetActive(false);
                }
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.CompanionSpawn))
                {
                    if (_unit._state.stop == false)
                    {

                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }

        int companionCount;
        async UniTaskVoid CountProgress()
        {
            while (true)
            {
                companionCount = 0;
                for (int i = 0; i < companionControllerList.Count; i++)
                {
                    if(companionControllerList[i]._view.gameObject.activeInHierarchy)
                    {
                        companionCount++;
                    }
                    else
                    {
                        
                    }
                }

                Player.Rune.companioncount = companionCount;
                Player.Rune.SyncAllData();
                await UniTask.Delay(2000);
            }
        }
    }
}

