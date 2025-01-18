using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region petMissile
    public class CharacterFireRainSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewFireRainSkill fireRainViewEffect;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float attackTime = 1;

        GameObject viewFireRainRangeObject;

        List<ViewFireRainSkill> missilePool = new List<ViewFireRainSkill>();

        List<Vector2> randomPos = new List<Vector2>();
        List<Vector2> randomPos_forSkillsubunit = new List<Vector2>();
        const float randomDistance = 3.0f;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        float spawnRainTime=0.1f;

        Player.Skill.SkillCacheData subUnitskillcache;
        public CharacterFireRainSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.FireRain];
            tabledata = skillCache.tabledataSkill;
            Player.Skill.SkillActivate += SkillActivate;
            subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            Main().Forget();
        }


        public void CreateSkillEffect(Definition.WitchSkillType witchtype)
        {
            if (randomPos_forSkillsubunit.Count <= 0)
                return;
            ViewFireRainSkill rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.viewFireRainSkill);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos;
            Vector2 endpos;
            if (witchtype == Definition.WitchSkillType.Witch)
            {
                int rainindex = Random.Range(0, randomPos.Count);
                startpos = new Vector2(randomPos[rainindex].x, randomPos[rainindex].y + 7);
                endpos   = randomPos[rainindex];
            }
            else
            {
                int rainindex = Random.Range(0, randomPos_forSkillsubunit.Count);
                startpos = new Vector2(randomPos_forSkillsubunit[rainindex].x, randomPos_forSkillsubunit[rainindex].y + 7);
                endpos = randomPos_forSkillsubunit[rainindex];
            }
               


        
            rainObj.Fire(startpos, endpos,skillValue_0, skillValue_2, witchtype);

        }

        public override eActorState GetState()
        {
            return eActorState.FireRain;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(1);
            _unit.animindex = 0;

            randomPos.Clear();
            randomPos_forSkillsubunit.Clear();

            skillValue_0 = Player.Skill.Get(Definition.SkillKey.FireRain).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.FireRain).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.FireRain).SkillValue(2);

            AudioManager.Instance.Play(AudioSourceKey.Skill_FireRain);

            var enemy = Battle.Enemy.GetClosedEnemyController(Player.Unit.usersubUnit._view.transform);
            var otherenemy = Battle.Enemy.GetClosedEnemyController(Player.Skill.skillSubUnitObject._view.transform);

            if(enemy==null)
            {
                _unit._state.ChangeState(eActorState.Idle);
                return;
            }
            for (int i=0; i<10; i++)
            {
                Vector2 randomposition = Random.insideUnitCircle;
                var _random= (Vector2)enemy._view.transform.position + (new Vector2(randomposition.x,0) * randomDistance*(1+ skillValue_1*0.01f));
                randomPos.Add(_random);
            }

            if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
            {
                if (subUnitskillcache.userSkilldata.AwakeLv >= 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 randomposition = Random.insideUnitCircle;
                        var _random = (Vector2)otherenemy._view.transform.position + (new Vector2(randomposition.x, 0) * randomDistance * (1 + skillValue_1 * 0.01f));
                        randomPos_forSkillsubunit.Add(_random);
                    }
                }
            }

            _unit._state.ChangeState(eActorState.Idle);

    
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.FireRain,true);

            spawnRainTime =(0.1f/ (skillValue_1 == 0 ? 1 : (skillValue_1 * 0.06f)));

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);
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
                //viewFireRainRangeObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.FireRain, false);
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.FireRain))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > spawnRainTime)
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
                        if (skillCache.elapsedCooltime >= tabledata.effectTime)
                        {
                            for(int i=0; i< missilePool.Count; i++)
                            {
                                missilePool[i].gameObject.SetActive(false);
                            }
                           // viewFireRainRangeObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.FireRain, false);
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
    #endregion
}