using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterLightningSpear : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float attackTime = 1;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        List<ViewLightningSpear> missilePool = new List<ViewLightningSpear>();

        Player.Skill.SkillCacheData subUnitskillcache;
        public CharacterLightningSpear(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.LightningForSeconds];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.LightningForSeconds];
            Player.Skill.SkillActivate += SkillActivate;

            subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);

            Main().Forget();
        }


        public void CreateSkillEffect(Definition.WitchSkillType witchtype)
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;

            ViewLightningSpear rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.lightningSpear);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);
           
            Vector2 startpos = new Vector2(randomenemy._view.transform.position.x-10, randomenemy._view.transform.position.y+ 10);
            Vector2 endpos = randomenemy._view.transform.position;

            AudioManager.Instance.Play(AudioSourceKey.Skill_LightningForSeconds);

            rainObj.Fire(startpos, endpos,skillCache, witchtype);
        }

        public override eActorState GetState()
        {
            return eActorState.LightningForSeconds;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(1);
            _unit.animindex = 0;

            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            _unit._state.ChangeState(eActorState.Idle);

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.LightningForSeconds, true);
        }
        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                for(int i=0;i< missilePool.Count;i++)
                {
                    missilePool[i].isActive = false;
                    missilePool[i].gameObject.SetActive(false);
                }
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.LightningForSeconds, false);
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.LightningForSeconds))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 0.2f)
                        {
                            if(Battle.Field.IsFightScene)
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
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+skillValue_2)
                        {
                            for (int i = 0; i < missilePool.Count; i++)
                            {
                                missilePool[i].isActive = false;
                                missilePool[i].gameObject.SetActive(false);
                            }
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.LightningForSeconds, false);
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
