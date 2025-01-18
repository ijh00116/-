using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterSkyLightSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        List<ViewSkyLight> missilePool = new List<ViewSkyLight>();

        float skillValue_1;
        public CharacterSkyLightSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.SkyLight];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.SkyLight];

            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }


        public void CreateSkillEffect()
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;

            ViewSkyLight rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.viewSkyLight);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos;
            startpos = Player.Unit.userUnit._view.transform.position;

            Vector2 endpos = randomenemy._view.transform.position;

            rainObj.Fire(startpos, endpos, skillCache);

            AudioManager.Instance.Play(AudioSourceKey.Skill_SkyLight);

            //InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        public override eActorState GetState()
        {
            return eActorState.SkyLight;
        }
        protected override void OnEnter()
        {
            _unit._state.ChangeState(eActorState.Idle);
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SkyLight, true);

            skillValue_1 = skillCache.SkillValue(1);
        }

        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.SkyLight, false);
                for (int i = 0; i < missilePool.Count; i++)
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
                if (Player.Unit.IsSkillActive(Definition.SkillKey.SkyLight))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 0.1f)
                        {
                            if (Battle.Field.IsFightScene)
                            {
                                currentTime = 0;
                                CreateSkillEffect();
                            }
                        }
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+ skillValue_1)
                        {
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SkyLight, false);
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
