using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterTimebombSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_0_0;
        float skillValue_0_1;
        float skillValue_1;
        float skillValue_2;

        int defaultEnemyCount = 1;
        public CharacterTimebombSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.TimeBomb];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.TimeBomb];

            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }


        public override eActorState GetState()
        {
            return eActorState.TimeBomb;
        }
        protected override void OnEnter()
        {
            _unit._state.ChangeState(eActorState.Idle);

            skillValue_0_0= skillCache.SkillValue(0,0);
            skillValue_0_1 = skillCache.SkillValue(0, 1);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2= skillCache.SkillValue(2);

            var randomenemies = Battle.Enemy.GetRandomEnemiesController(defaultEnemyCount + (int)skillValue_1);

            for(int i=0; i< randomenemies.Count; i++)
            {
                randomenemies[i].TimeBombSetting(true);
            }
        }

        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.TimeBomb))
                {
                    if (_unit._state.stop == false)
                    {
                    
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
