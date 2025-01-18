using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterSubUnitSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ControllerSkillSubUnit controllerSubUnit;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_1;
        
        public CharacterSubUnitSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.SummonSubunit];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.SummonSubunit];

            controllerSubUnit = new ControllerSkillSubUnit(_unit._cts);
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            skillValue_1 = skillCache.SkillValue(1);

            CreateViewer();

            AudioManager.Instance.Play(AudioSourceKey.Skill_SummonSubunit);

            _unit._state.ChangeState(eActorState.Idle);
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        void CreateViewer()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            controllerSubUnit.ActivateViewer();
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SummonSubunit);
        }

        void OffViewer()
        {
            controllerSubUnit.InActivateViewer();
        }

   

        public override eActorState GetState()
        {
            return eActorState.SummonSubunit;
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
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.SummonSubunit, false);
                OffViewer();
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
                {
                    if (_unit._state.stop == false)
                    {
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+ skillValue_1)
                        {
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SummonSubunit, false);
                            OffViewer();
                        }
                    }
                }

                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
}
