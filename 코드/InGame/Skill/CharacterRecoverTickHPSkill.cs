using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region RecoverHPTick
    public class CharacterRecoverTickHPSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewRecoverHpTickSkill viewRecovertickHp;
        ParticleSystem hpParticleTck;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_0 = 0;
        float skillValue_1 = 0;
        float skillValue_2 = 0;
        public CharacterRecoverTickHPSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.RecoverHpTick];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.RecoverHpTick];
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(50);
            _unit.animindex = 0;

            skillValue_0 = Player.Skill.Get(Definition.SkillKey.RecoverHpTick).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.RecoverHpTick).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.RecoverHpTick).SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_RecoverHp);
            _unit._state.ChangeState(eActorState.Idle);
        }

        public void CreateSkillEffect()
        {
            if (viewRecovertickHp == null)
            {
                viewRecovertickHp = Object.Instantiate(SkillResourcesBundle.Loaded.viewRecovertickHp);
            }
            if(hpParticleTck==null)
            {
                hpParticleTck= Object.Instantiate(SkillResourcesBundle.Loaded.viewHpRecoverTick);
            }
            viewRecovertickHp.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
           
            viewRecovertickHp.transform.position = _unit._view.BuffTransform.position;
            viewRecovertickHp.particle.Play();
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.RecoverHpTick);
            AudioManager.Instance.Play(AudioSourceKey.Skill_IncreaseAttack);
            Player.Unit.buffUpdate?.Invoke();
        }

        public override eActorState GetState()
        {
            return eActorState.RecoverHpTick;
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
                if(viewRecovertickHp!=null)
                viewRecovertickHp.gameObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.RecoverHpTick, false);
                Player.Unit.buffUpdate?.Invoke();
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.RecoverHpTick))
                {
                    viewRecovertickHp.transform.position = _unit._view.BuffTransform.position;
                    hpParticleTck.transform.position= _unit._view.BuffTransform.position;
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 1.0f)
                        {
                            currentTime = 0;
                            hpParticleTck.gameObject.SetActive(true);
                            hpParticleTck.Play();
                            Player.Unit.IncreaseHpPercentAtHprecoverSkill(skillValue_0,skillValue_1, skillValue_2);
                        }

                        if (skillCache.elapsedCooltime >= tabledata.effectTime)
                        {
                            viewRecovertickHp.gameObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.RecoverHpTick, false);
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
