using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetShieldRecoverSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;

        ViewRecoverShield viewRecoverShield;


        public PetShieldRecoverSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;

            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            CreateSkillEffect();


            _unit._state.ChangeState(eActorState.Idle);
        }

        public void CreateSkillEffect()
        {
            Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.ShieldRecover);
            AudioManager.Instance.Play(AudioSourceKey.Skill_IncreaseAttack);
            Player.Unit.buffUpdate?.Invoke();
        }

        public override eActorState GetState()
        {
            return eActorState.PetShield;
        }

        protected override void OnExit()
        {
        }
        private void SkillActivate(Definition.PetSkillKey _skillkey, bool active)
        {
            if (_unit.petCache == null)
                return;
            if (_skillkey != _unit.petCache.tabledata.petskillKey)
                return;
            if (active == false)
            {
                Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.ShieldRecover, false);
                Player.Unit.buffUpdate?.Invoke();
            }
        }
        protected override void OnUpdate()
        {

        }

        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.ShieldRecover == _unit.petCache.tabledata.petskillKey)
                    {
                        if (Player.Pet.IsSkillActive(Definition.PetSkillKey.ShieldRecover))
                        {
                            if (_unit._state.stop == false)
                            {
                                currentTime += Time.deltaTime;
                                if (currentTime > 1.0f)
                                {
                                    currentTime = 0;
                                    double skillvalue = _unit.petCache.GetSkillPureValue();
                                    Player.Unit.IncreaseShieldPercent(skillvalue);
                                }
                                if (_unit.petCache.elapsedCooltime >= _unit.petCache.tabledata.skillEffectTime)
                                {
                                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.ShieldRecover, false);
                                    Player.Unit.buffUpdate?.Invoke();
                                }
                            }
                        }
                    }
                  
                }
         

                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }

}
