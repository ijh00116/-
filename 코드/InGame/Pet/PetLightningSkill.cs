using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetLightningSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;



        List<ViewPetLightningSkill> missilePool = new List<ViewPetLightningSkill>();

        public PetLightningSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;



            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();

        }

        public void CreateSkillEffect()
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;

            ViewPetLightningSkill rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.petLightningSpearSkill);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos = new Vector2(randomenemy._view.transform.position.x - 10, randomenemy._view.transform.position.y + 10);
            Vector2 endpos = randomenemy._view.transform.position;

            AudioManager.Instance.Play(AudioSourceKey.Skill_LightningForSeconds);

            rainObj.Fire(startpos, endpos, _unit.petCache);
        }

        public override eActorState GetState()
        {
            return eActorState.PetLightningSpear;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(1);
            _unit.animindex = 0;

            _unit._state.ChangeState(eActorState.Idle);

            Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.LightningSpear, true);
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
                    missilePool[i].isActive = false;
                    missilePool[i].gameObject.SetActive(false);
                }
                Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.LightningSpear, false);
            }
            Player.Cloud.petdata.UpdateHash().SetDirty(true);
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.LightningSpear == _unit.petCache.tabledata.petskillKey)
                    {
                        if (Player.Pet.IsSkillActive(Definition.PetSkillKey.LightningSpear))
                        {
                            if (_unit._state.stop == false)
                            {
                                currentTime += Time.deltaTime;
                                if (currentTime > 0.2f)
                                {
                                    if (Battle.Field.IsFightScene)
                                    {
                                        currentTime = 0;

                                        CreateSkillEffect();
                                    }

                                }
                                if (_unit.petCache.elapsedCooltime >= _unit.petCache.tabledata.skillEffectTime)
                                {
                                    for (int i = 0; i < missilePool.Count; i++)
                                    {
                                        missilePool[i].isActive = false;
                                        missilePool[i].gameObject.SetActive(false);
                                    }
                                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.LightningSpear, false);
                                    Player.Cloud.petdata.UpdateHash().SetDirty(true);
                                }
                            }
                        }
                    }
                   
                }
              

                await UniTask.Yield(_unit._cts.Token);
            }
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {

        }
    }

}
