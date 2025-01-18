using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class PetSwordFewHitSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;


        List<ViewPetSwordFewHitSkill> missilePool = new List<ViewPetSwordFewHitSkill>();

        public PetSwordFewHitSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;

            Player.Pet.SkillActivate += SkillActivate;
            
        }
        protected override void OnEnter()
        {
            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_SwordFewHitFire);
            _unit._state.ChangeState(eActorState.Idle);
        }


        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            ViewPetSwordFewHitSkill tempObj = null;
            for (int i = 0; i < missilePool.Count; i++)
            {
                if (missilePool[i].gameObject.activeInHierarchy == false)
                {
                    tempObj = missilePool[i];
                    break;
                }
            }
            if (tempObj == null)
            {
                tempObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.petSwordFewHitSkill);
                missilePool.Add(tempObj);
            }
            tempObj.particle.Stop();
            tempObj.gameObject.SetActive(true);
            tempObj.transform.position = _unit._view.transform.position;
            float skillscale = 1;
            tempObj.transform.localScale = new Vector3(skillscale, skillscale, skillscale);
            tempObj.Activate(Player.Unit.userUnit.targetController._view, _unit.petCache);

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        public override eActorState GetState()
        {
            return eActorState.PetSwordFewHit;
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
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
                for (int i = 0; i < missilePool.Count; i++)
                {
                    missilePool[i].gameObject.SetActive(false);
                }
            }
        }
    }

}
