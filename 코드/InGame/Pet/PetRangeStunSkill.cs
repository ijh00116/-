using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetRangeStunSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;

        ViewPetRangeStunSkill viewRangeStun;
  

        public PetRangeStunSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;
        }

        public override eActorState GetState()
        {
            return eActorState.PetRangeStun;
        }

        public void CreateSkillEffect()
        {
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);

            if (viewRangeStun == null)
            {
                viewRangeStun = Object.Instantiate(SkillResourcesBundle.Loaded.petStunSkill);
            }
            viewRangeStun.Init(_unit.petCache);
            //viewRangeStun.gameObject.SetActive(true);

            viewRangeStun.transform.position = _unit._view.transform.position;
        }

        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(50);
            _unit.animindex = 0;

            CreateSkillEffect();


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
