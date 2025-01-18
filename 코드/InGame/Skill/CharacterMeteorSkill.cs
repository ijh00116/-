using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterMeteorSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float attackTime = 1;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        List<ViewMeteorSkill> missilePool = new List<ViewMeteorSkill>();
        float currentTime = 0;
        Player.Skill.SkillCacheData subUnitskillcache;
        public CharacterMeteorSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.SpawnMeteor];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.SpawnMeteor];
            Player.Skill.SkillActivate += SkillActivate;
            subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            Main().Forget();
        }

        public void CreateSkillEffect(Definition.WitchSkillType witchtype)
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;

            ViewMeteorSkill rainObj = null;
            rainObj = missilePool.Find(o => o.gameObject.activeInHierarchy == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.meteorskillView);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos = new Vector2(randomenemy._view.transform.position.x - 10, randomenemy._view.transform.position.y + 10);
            Vector2 endpos = randomenemy._view.transform.position;

            rainObj.Fire(startpos, endpos, skillCache,witchtype);


            AudioManager.Instance.Play(AudioSourceKey.Skill_SpawnMeteor);
        }

        public override eActorState GetState()
        {
            return eActorState.SpawnMeteor;
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            currentTime = 10;
            _unit._state.ChangeState(eActorState.Idle);

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SpawnMeteor, true);
        }
        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                for (int i = 0; i < missilePool.Count; i++)
                {
                    missilePool[i].gameObject.SetActive(false);
                }
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.SpawnMeteor, false);
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.SpawnMeteor))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 1.0f*(1-skillValue_2*0.01f))
                        {
                            if (Battle.Field.IsFightScene)
                            {
                                currentTime = 0;
                                if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
                                {
                                    if (subUnitskillcache.userSkilldata.AwakeLv >= 2)
                                    {
                                        CreateSkillEffect(Definition.WitchSkillType.SkillWitch);
                                    }
                                }
                                CreateSkillEffect(Definition.WitchSkillType.Witch);
                            }

                        }
                        if (skillCache.elapsedCooltime >= tabledata.effectTime + skillValue_1)
                        {
                            //for (int i = 0; i < missilePool.Count; i++)
                            //{
                            //    missilePool[i].gameObject.SetActive(false);
                            //}
                            currentTime = 0;
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SpawnMeteor, false);
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
