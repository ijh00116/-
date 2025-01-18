using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterBigFireBall : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float attackTime = 1;

        List<ViewBigFireBall> missilePool = new List<ViewBigFireBall>();

        Player.Skill.SkillCacheData subUnitskillcache;
        public CharacterBigFireBall(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.BigFireballForSeconds];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.BigFireballForSeconds];

            Player.Skill.SkillActivate += SkillActivate;
            subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            Main().Forget();
        }


        public void CreateSkillEffect(Definition.WitchSkillType witchtype)
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;

            ViewBigFireBall rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.viewBigFireBall);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos;
            if (witchtype == Definition.WitchSkillType.Witch)
            {
                startpos = Player.Unit.usersubUnit._view.transform.position;
            }
            else
            {
                startpos = Player.Skill.skillSubUnitObject._view.transform.position;
            }
            Vector2 endpos = randomenemy._view.transform.position;

            rainObj.Fire(startpos, endpos,skillCache,witchtype);

            AudioManager.Instance.Play(AudioSourceKey.Skill_BigFireballForSeconds);

            if (witchtype == Definition.WitchSkillType.Witch)
                InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        public override eActorState GetState()
        {
            return eActorState.BigFireballForSeconds;
        }
        protected override void OnEnter()
        {
            _unit._state.ChangeState(eActorState.Idle);
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.BigFireballForSeconds, true);
        }

        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active==false)
            {
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.BigFireballForSeconds, false);
                for(int i=0; i< missilePool.Count; i++)
                {
                    missilePool[i].gameObject.SetActive(false);
                }
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.BigFireballForSeconds))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 0.8f)
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
                        if (skillCache.elapsedCooltime >= tabledata.effectTime)
                        {
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.BigFireballForSeconds, false);
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
