using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class PetMagicAtkIncreaseSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;



        public PetMagicAtkIncreaseSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;
            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();
           

        }
        protected override void OnEnter()
        {

            _unit._state.ChangeState(eActorState.Idle);
        }


        public override eActorState GetState()
        {
            return eActorState.PetMagicIncrease;
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {

        }

        private void SkillActivate(PetSkillKey arg1, bool arg2)
        {
            if (_unit.petCache == null)
                return;
            if (Definition.PetSkillKey.MagicAtkIncrease == _unit.petCache.tabledata.petskillKey)
            {
                if (arg2 == false)
                {
                    _unit.petCache.SetPassiveAbil(0);
                }
            }
        }

        async UniTaskVoid Main()
        {
            float currentTime = 0;
            int increaseCount = 0;
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.MagicAtkIncrease == _unit.petCache.tabledata.petskillKey)
                    {
                        if (_unit.petCache.IsEquiped && _unit.petCache.IsSkillUnlocked)
                        {
                            currentTime += Time.deltaTime;
                            int count = (int)(currentTime / 1200);
                            if (count > increaseCount)
                            {
                                increaseCount = count;
                                double skillvalue = _unit.petCache.GetSkillPureValue();
                                if (increaseCount >= 5)
                                    increaseCount = 5;
                                _unit.petCache.SetPassiveAbil(skillvalue * increaseCount);
                            }
                        }
                    }
                  
                }
              

                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }

}
