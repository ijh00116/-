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
    #region ±¤¿ªµ¥¹ÌÁö
    public class CharacterFreezeSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        const int DefaultObjectCount = 5;
        int currentFrame = 0;
        int skillPrefabIndex = 0;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;
        int maxSkillCount;

        const float freezeTime = 2;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
        public CharacterFreezeSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.FarEnemyFreeze];
            tabledata = skillCache.tabledataSkill;

            Main().Forget();
        }

        public void CreateSkillEffect()
        {
            var _targetcontroller = Battle.Enemy.GetFarRandomEnemiesController(_unit._view.transform, maxSkillCount, 3);

            for (int i = 0; i < _targetcontroller.Count; i++)
            {
                double dmg = (double)Player.Unit.BowAtk * (skillCache.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                if (_targetcontroller[i].enemyType != EnemyType.Boss)
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                }
                else
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                }

                _targetcontroller[i].DecreaseHp(dmg, UserDmgType.SkillNormal);

                Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.FarEnemyFreeze] += dmg;
                _targetcontroller[i].SetFreezeState(freezeTime + skillValue_1);

                WorldUIManager.Instance.InstatiateFont(_targetcontroller[i]._view.transform.position, dmg, false, false, skillColor);
            }

            AudioManager.Instance.Play(AudioSourceKey.Skill_FarFreeze);
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_1);
        }

    

        public override eActorState GetState()
        {
            return eActorState.FarEnemyFreeze;
        }

        protected override void OnEnter()
        {
            _unit.animindex = 0;
            skillPrefabIndex = 0;
            currentFrame = 0;

            skillValue_0 = Player.Skill.Get(Definition.SkillKey.FarEnemyFreeze).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.FarEnemyFreeze).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.FarEnemyFreeze).SkillValue(2);

            maxSkillCount = DefaultObjectCount + (int)skillValue_2;

            _unit._state.ChangeState(eActorState.Idle);
            CreateSkillEffect();

        }

        protected override void OnExit()
        {
        }


        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.FireRain))
                {
                    if (_unit._state.stop == false)
                    {
                     
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
