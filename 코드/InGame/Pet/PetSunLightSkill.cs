using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetSunLightSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;


        ViewPetSunLightSkill laserbeamSkillView;

        public PetSunLightSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;

            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();
         
            
        }

        private void SkillActivate(Definition.PetSkillKey _skillkey, bool active)
        {
            if (_unit.petCache == null)
                return;
            if (_skillkey != _unit.petCache.tabledata.petskillKey)
                return;
            if (active == false)
            {
                if (laserbeamSkillView != null)
                {
                    laserbeamSkillView.InActivate();
                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.SunLight, false);

                }
            }
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;
            if (laserbeamSkillView == null)
            {
                laserbeamSkillView = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.petSunLightSkill);
            }

            laserbeamSkillView.Activate(_unit);
          

        }

        public override eActorState GetState()
        {
            return eActorState.PetSunLight;
        }

        protected override void OnEnter()
        {
            if (laserbeamSkillView != null)
                laserbeamSkillView.InActivate();

       
            CreateSkillEffect();

            _unit._state.ChangeState(eActorState.Idle);

            Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.SunLight, true);
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {

        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.SunLight == _unit.petCache.tabledata.petskillKey)
                    {
                        if (Player.Pet.IsSkillActive(Definition.PetSkillKey.SunLight))
                        {
                            if (_unit._state.stop == false)
                            {
                                if (_unit.petCache.elapsedCooltime >= _unit.petCache.tabledata.skillEffectTime)
                                {
                                    laserbeamSkillView.InActivate();
                                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.SunLight, false);
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

