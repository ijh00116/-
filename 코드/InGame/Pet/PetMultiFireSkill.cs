using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetMultiFireSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;



        List<ViewPetMultiFireSkill> missilePool = new List<ViewPetMultiFireSkill>();

        public PetMultiFireSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;



            Player.Pet.SkillActivate += SkillActivate;
       

        }
        public void CreateSkillEffect()
        {
            CreateSkillView();

            AudioManager.Instance.Play(AudioSourceKey.Skill_MultiFireball);
        }

        void CreateSkillView()
        {
            if (_unit.targetController == null)
                return;
            var enemy = Battle.Enemy.GetRandomEnemyController();
            if (enemy != null)
            {
               

                int missileNum = 5;
                for (int i = 0; i < missileNum; i++)
                {
                    ViewPetMultiFireSkill missileobj = null;
                    if (i >= missilePool.Count)
                    {
                        missileobj = Object.Instantiate(SkillResourcesBundle.Loaded.petMultiFireSkill);
                        missilePool.Add(missileobj);
                    }
                    else
                    {
                        missileobj = missilePool[i];
                    }
                    missileobj.particle.Stop();
                    missileobj.gameObject.SetActive(true);
                    missileobj.transform.position = _unit._view.transform.position;

                    InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_1);

                    missileobj.particle.Play();

                    Vector2 rotation;
                   
                    rotation = _unit.targetController._view.transform.position - _unit._view.transform.position;

                    float bulletscale = 1;
                    float bulletAngleBetween = ((i - (int)(missileNum / 2)) * missileNum) * (bulletscale) * 2;

                    float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg + bulletAngleBetween;

                    Vector3 dirVector = ConvertAngleToVector(rotz);
                    missileobj.MoveToArr(dirVector, _unit.petCache);
                }
            }
        }

        Vector3 ConvertAngleToVector(float _deg)
        {
            var rad = _deg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
        }


        public override eActorState GetState()
        {
            return eActorState.PetMultiFire;
        }
        protected override void OnEnter()
        {
            _unit.animindex = 0;

            CreateSkillEffect();

            _unit._state.ChangeState(eActorState.Idle);
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
                Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.FireMultiShot, false);
            }
            Player.Cloud.petdata.UpdateHash().SetDirty(true);
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {

        }
    }

}
