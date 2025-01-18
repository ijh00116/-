using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region ±¤¿ªµ¥¹ÌÁö
    public class CharacterExplodePoisonSkill : CharacterState
    {
        ControllerUnitInGame _unit;
        List<ViewExplodePoisonDamage> explodeSkillObj=new List<ViewExplodePoisonDamage>();

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        const int explodeDefaultCount = 1;
        int currentFrame = 0;
        int skillPrefabIndex = 0;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;
        int maxSkillCount;
        public CharacterExplodePoisonSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.ExplodePoisonCloud];
            tabledata = skillCache.tabledataSkill;
            Player.Skill.SkillActivate += SkillActivate;

            Main().Forget();
        }

        public void CreateSkillEffect()
        {
            if (_unit.target != null)
            {
                ViewExplodePoisonDamage effectObj = null;
               
                if(skillPrefabIndex>= explodeSkillObj.Count)
                {
                    effectObj = Object.Instantiate(SkillResourcesBundle.Loaded.viewExplodePoisonDmg);
                    explodeSkillObj.Add(effectObj);
                }
                else
                {
                    effectObj = explodeSkillObj[skillPrefabIndex];
                }

                var _targetcontroller = Battle.Enemy.GetFarRandomEnemiesController(_unit._view.transform, 4,6);
                if(_targetcontroller!=null)
                {
                    effectObj.gameObject.SetActive(true);
                    effectObj.particle.Play();
                }
                effectObj.SetSkill(skillCache);
                skillPrefabIndex++;

                AudioManager.Instance.Play(AudioSourceKey.Skill_ExplodePoisonCloud);
            }
        }

        private void SkillActivate(Definition.SkillKey _skillkey ,bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                for (int i = 0; i < explodeSkillObj.Count; i++)
                {
                    explodeSkillObj[i].gameObject.SetActive(false);
                }
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.ExplodePoisonCloud, false);
            }
        }

        public override eActorState GetState()
        {
            return eActorState.ExplodePoisonCloud;
        }

        protected override void OnEnter()
        {
            _unit.animindex = 0;
            skillPrefabIndex = 0;
            currentFrame = 0;

            skillValue_0= Player.Skill.Get(Definition.SkillKey.ExplodePoisonCloud).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.ExplodePoisonCloud).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.ExplodePoisonCloud).SkillValue(2);

            maxSkillCount = explodeDefaultCount + (int)skillValue_1;

            for (int i = 0; i < explodeSkillObj.Count; i++)
            {
                explodeSkillObj[i].particle.Stop();
                explodeSkillObj[i].gameObject.SetActive(false);
            }

            _unit._state.ChangeState(eActorState.Idle);
            CreateSkillViewer().Forget();

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.ExplodePoisonCloud, true);
        }

        protected override void OnExit()
        {
        }

        async UniTaskVoid CreateSkillViewer()
        {
            while (true)
            {
                currentFrame++;
                if (currentFrame >= 10)
                {
                    currentFrame = 0;

                    CreateSkillEffect();
                }

                if (skillPrefabIndex >= maxSkillCount)
                {
                    break;
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.ExplodePoisonCloud))
                {
                    if (_unit._state.stop == false)
                    {
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+ skillValue_2)
                        {
                            for (int i = 0; i < explodeSkillObj.Count; i++)
                            {
                                explodeSkillObj[i].gameObject.SetActive(false);
                            }
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.ExplodePoisonCloud, false);
                        }
                    }
                }

                await UniTask.Yield(_unit._cts.Token);
            }
        }

        protected override void OnUpdate()
        {
        }
    }
    #endregion
}