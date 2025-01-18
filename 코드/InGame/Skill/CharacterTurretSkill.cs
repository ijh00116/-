using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region ≈Õ∑ø
    public class CharacterTurretSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        List<ViewTurretSkill> turretList = new List<ViewTurretSkill>();
        const int defaultMissileNum = 2;

        Player.Skill.SkillCacheData skillCache;

        int skillPrefabIndex = 0;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        int maxSkillCount;

        public CharacterTurretSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.SetTurret];

            Player.Skill.SkillActivate += SkillActivate;

            Main().Forget();
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;
            for (int i = 0; i < maxSkillCount; i++)
            {
                ViewTurretSkill turretObj;
                if (i >= turretList.Count)
                {
                    ViewTurretSkill prefab = SkillResourcesBundle.Loaded.turretskillView;
                    turretObj = UnityEngine.Object.Instantiate(prefab);
                    turretList.Add(turretObj);
                }
                else
                {
                    turretObj = turretList[i];
                }
                turretObj.gameObject.SetActive(true);
                turretObj.Init(skillValue_0, skillValue_2);
                float randomx = Random.Range(0.0f, 1.0f);
                float randomy = (1.0f - randomx);

                var playerpos = Player.Unit.userUnit._view.transform.position;
                turretObj.transform.position = new Vector3(playerpos.x + randomx * ((i+1)*2), playerpos.y + randomy * ((i + 1) * 2), 0);

                //turretObj.transform.SetParent(Player.Unit.userUnit._view.transform);
            }

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        public override eActorState GetState()
        {
            return eActorState.SetTurret;
        }

        protected override void OnEnter()
        {
            skillPrefabIndex = 0;
            skillValue_0 = Player.Skill.Get(Definition.SkillKey.SetTurret).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.SetTurret).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.SetTurret).SkillValue(2);

            maxSkillCount = defaultMissileNum + (int)skillValue_1;

            for (int i = 0; i < turretList.Count; i++)
            {
                turretList[i].gameObject.SetActive(false);
            }

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_SetTurret);
            _unit._state.ChangeState(eActorState.Idle);

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SetTurret, true);
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {

        }

        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                for (int i = 0; i < turretList.Count; i++)
                {
                    turretList[i].gameObject.SetActive(false);
                }
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.SetTurret, false);
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.SetTurret))
                {
                    if (_unit._state.stop == false)
                    {
                       
                        if (skillCache.elapsedCooltime >= skillCache.tabledataSkill.effectTime)
                        {
                            for (int i = 0; i < turretList.Count; i++)
                            {
                                turretList[i].gameObject.SetActive(false);
                            }
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.SetTurret, false);
                        }
                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
    #endregion
}