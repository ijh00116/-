using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterRangeStun : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewRangeStun viewRangeStun;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        public CharacterRangeStun(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.RangeStun];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.RangeStun];
        }

        public override eActorState GetState()
        {
            return eActorState.RangeStun;
        }

        public void CreateSkillEffect()
        {
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);

            if (viewRangeStun == null)
            {
                viewRangeStun = Object.Instantiate(SkillResourcesBundle.Loaded.viewRangeStun);
            }
            viewRangeStun.Init(skillCache);
            //viewRangeStun.gameObject.SetActive(true);
            
            viewRangeStun.transform.position = _unit._view.transform.position;
        }

        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(50);
            _unit.animindex = 0;

            skillValue_0 = Player.Skill.Get(Definition.SkillKey.RangeStun).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.RangeStun).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.RangeStun).SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_RangeStun);

            _unit._state.ChangeState(eActorState.Idle);
        }

        protected override void OnExit()
        {
           
        }

        protected override void OnUpdate()
        {
            
        }
    }

}
