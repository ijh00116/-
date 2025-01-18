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
    public class CharacterMultipleLightningSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float attackTime = 1;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        ViewMultipleLightningSkill viewLightningSkill;
        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
        public CharacterMultipleLightningSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.MultipleElectric];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.MultipleElectric];
            Player.Skill.SkillActivate += SkillActivate;

            if (viewLightningSkill == null)
            {
                viewLightningSkill = Object.Instantiate(SkillResourcesBundle.Loaded.viewMultipleLightning);
            }
            viewLightningSkill.gameObject.SetActive(false);

            Main().Forget();
        }

        public void CreateSkillEffect()
        {
            if (viewLightningSkill == null)
            {
                viewLightningSkill = Object.Instantiate(SkillResourcesBundle.Loaded.viewMultipleLightning);
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

            double dmg = (double)Player.Unit.SwordAtk * (skillCache.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

            if (randomenemy.enemyType != EnemyType.Boss)
            {
                dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
            }
            else
            {
                dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
            }

            randomenemy.DecreaseHp(dmg, UserDmgType.SkillMissile);
            Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.MultipleElectric] += dmg;
            WorldUIManager.Instance.InstatiateFont(randomenemy._view.transform.position, dmg, false, false, skillColor);

            if (randomenemy.hp > 0 && skillCache.userSkilldata.AwakeLv >= 2)
            {
                randomenemy.SetStunState(skillCache.SkillValue(2),true);
            }

  
        }

        public override eActorState GetState()
        {
            return eActorState.MultipleElectric;
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            _unit._state.ChangeState(eActorState.Idle);

            CreateSkillEffect();

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.MultipleElectric, true);
        }


        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                viewLightningSkill.gameObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.MultipleElectric, false);
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.MultipleElectric))
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
                        if (skillCache.elapsedCooltime >= tabledata.effectTime + skillValue_1)
                        {
                            viewLightningSkill.gameObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.MultipleElectric, false);
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
