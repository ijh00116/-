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
    public class PetMultiLightningSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;


        ViewPetMultiLightningSkill viewLightningSkill;
        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
        public PetMultiLightningSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;

            if (viewLightningSkill == null)
            {
                viewLightningSkill = Object.Instantiate(SkillResourcesBundle.Loaded.petMultiLightningSkill);
            }
            viewLightningSkill.gameObject.SetActive(false);

            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();
            
        }

        public void CreateSkillEffect()
        {
            if (viewLightningSkill == null)
            {
                viewLightningSkill = Object.Instantiate(SkillResourcesBundle.Loaded.petMultiLightningSkill);
            }
            viewLightningSkill.gameObject.SetActive(true);

            Vector2 startpos = Player.Unit.userUnit._view.transform.position;

            AudioManager.Instance.Play(AudioSourceKey.Skill_MultipleElectric);
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);
            viewLightningSkill.transform.position = startpos;
            viewLightningSkill.Fire();

            MoveToOtherEnemy();
        }

        public void MoveToOtherEnemy()
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;
            Vector2 startpos = randomenemy._view.transform.position;

            viewLightningSkill.transform.position = startpos;

            double dmg = _unit.petCache.GetSkillValue();

            randomenemy.DecreaseHp(dmg, UserDmgType.PetSkill);

            WorldUIManager.Instance.InstatiateFont(randomenemy._view.transform.position, dmg, false, false, skillColor);

        }

        public override eActorState GetState()
        {
            return eActorState.PetMultiElectric;
        }
        protected override void OnEnter()
        {
            if (Definition.PetSkillKey.MultiLightning != _unit.petCache.tabledata.petskillKey)
                return;
            _unit._state.ChangeState(eActorState.Idle);

            CreateSkillEffect();

            Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.MultiLightning, true);
        }


        private void SkillActivate(Definition.PetSkillKey _skillkey, bool active)
        {
            if (_unit.petCache == null)
                return;
            if (_skillkey != _unit.petCache.tabledata.petskillKey)
                return;
            if (active == false)
            {
                viewLightningSkill.gameObject.SetActive(false);
                Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.MultiLightning, false);
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.MultiLightning == _unit.petCache.tabledata.petskillKey)
                    {
                        if (Player.Pet.IsSkillActive(Definition.PetSkillKey.MultiLightning))
                        {
                            if (_unit._state.stop == false)
                            {
                                currentTime += Time.deltaTime;
                                if (currentTime > 0.1f)
                                {
                                    if (Battle.Field.IsFightScene)
                                    {
                                        currentTime = 0;
                                        MoveToOtherEnemy();
                                    }
                                }
                                if (_unit.petCache.elapsedCooltime >= _unit.petCache.tabledata.skillEffectTime)
                                {
                                    viewLightningSkill.gameObject.SetActive(false);
                                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.MultiLightning, false);
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
