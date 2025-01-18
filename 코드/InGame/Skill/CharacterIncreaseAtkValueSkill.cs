using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region IncreaseAtkValue
    public class CharacterIncreaseAtkValueSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewAtkIncreaseSkill atkIncreaseSkillview;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;
        public CharacterIncreaseAtkValueSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.IncreaseAttack];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.IncreaseAttack];
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            _unit.animindex = 0;

            CreateSkillEffect();
            _unit._state.ChangeState(eActorState.Idle);
        }

        public void CreateSkillEffect()
        {
            if (atkIncreaseSkillview == null)
            {
                atkIncreaseSkillview = Object.Instantiate(SkillResourcesBundle.Loaded.atkIncreaseSkillview);
            }
            atkIncreaseSkillview.transform.position = _unit._view.BuffTransform.position;
            atkIncreaseSkillview.particle.Play();
            atkIncreaseSkillview.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.IncreaseAttack);
            AudioManager.Instance.Play(AudioSourceKey.Skill_IncreaseAttack);
            Player.Unit.buffUpdate?.Invoke();
        }


        public override eActorState GetState()
        {
            return eActorState.IncreaseAttack;
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
                if(atkIncreaseSkillview!=null)
                    atkIncreaseSkillview.gameObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.IncreaseAttack, false);
                Player.Unit.buffUpdate?.Invoke();
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.IncreaseAttack))
                {
                    atkIncreaseSkillview.transform.position = _unit._view.BuffTransform.position;
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if(currentTime> 0.5f)
                        {
                            currentTime = 0;
                        }
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+skillCache.SkillValue(1))
                        {
                            atkIncreaseSkillview.gameObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.IncreaseAttack, false);
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
