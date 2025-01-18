using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region IncreaseMoveValue
    public class CharacterIncreaseMoveValueSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewMoveIncreaseSkill moveIncreaseSkillview;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;
        public CharacterIncreaseMoveValueSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.IncreaseMoving];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.IncreaseMoving];
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            CreateSkillEffect();
            AudioManager.Instance.Play(AudioSourceKey.Skill_IncreaseMove);
            _unit._state.ChangeState(eActorState.Idle);
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            if (moveIncreaseSkillview == null)
            {
                moveIncreaseSkillview = Object.Instantiate(SkillResourcesBundle.Loaded.moveIncreaseSkillView);
            }
            moveIncreaseSkillview.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            moveIncreaseSkillview.transform.position = _unit._view.BuffTransform.position;
            moveIncreaseSkillview.particle.Play();
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.IncreaseMoving);
            AudioManager.Instance.Play(AudioSourceKey.Skill_IncreaseAttack);
            Player.Unit.buffUpdate?.Invoke();
        }

        public override eActorState GetState()
        {
            return eActorState.IncreaseMoving;
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
                if(moveIncreaseSkillview!=null)
                    moveIncreaseSkillview.gameObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.IncreaseMoving, false);
                Player.Unit.buffUpdate?.Invoke();
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.IncreaseMoving))
                {
                    moveIncreaseSkillview.transform.position = _unit._view.BuffTransform.position;
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 0.5f)
                        {
                            currentTime = 0;
                        }

                        if (skillCache.elapsedCooltime >= tabledata.effectTime+skillCache.SkillValue(1))
                        {
                            moveIncreaseSkillview.gameObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.IncreaseMoving, false);
                            Player.Unit.buffUpdate?.Invoke();
                        }
                    }
                }

                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
    #endregion
}