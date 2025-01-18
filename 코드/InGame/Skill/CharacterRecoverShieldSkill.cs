using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterRecoverShieldSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewRecoverShield viewRecoverShield;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;
        ParticleSystem ShieldParticleTck;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;
        public CharacterRecoverShieldSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.RecoverShield];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.RecoverShield];
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_2 = skillCache.SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_RecoverShield);

            _unit._state.ChangeState(eActorState.Idle);
        }

        public void CreateSkillEffect()
        {
            if (viewRecoverShield == null)
            {
                viewRecoverShield = Object.Instantiate(SkillResourcesBundle.Loaded.viewRecoverShield);
            }
            if (ShieldParticleTck == null)
            {
                ShieldParticleTck = Object.Instantiate(SkillResourcesBundle.Loaded.viewHpRecoverTick);
            }
            viewRecoverShield.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);

            viewRecoverShield.transform.position = _unit._view.BuffTransform.position;
            viewRecoverShield.particle.Play();
            Player.Unit.SkillActiveUpdate(Definition.SkillKey.RecoverShield);
            AudioManager.Instance.Play(AudioSourceKey.Skill_IncreaseAttack);
            Player.Unit.buffUpdate?.Invoke();
        }

        public override eActorState GetState()
        {
            return eActorState.RecoverShield;
        }

        protected override void OnExit()
        {
        }
        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                if (viewRecoverShield != null)
                    viewRecoverShield.gameObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.RecoverShield, false);
                Player.Unit.buffUpdate?.Invoke();
            }
        }
        protected override void OnUpdate()
        {
          
        }

        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.RecoverShield))
                {
                    viewRecoverShield.transform.position = _unit._view.BuffTransform.position;
                    ShieldParticleTck.transform.position = _unit._view.BuffTransform.position;
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > 1.0f)
                        {
                            currentTime = 0;
                            ShieldParticleTck.gameObject.SetActive(true);
                            ShieldParticleTck.Play();
                            Player.Unit.IncreaseShieldPercent(skillValue_0);
                        }
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+skillValue_2)
                        {
                            viewRecoverShield.gameObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.RecoverShield, false);
                            Player.Unit.buffUpdate?.Invoke();
                        }
                    }
                }

                await UniTask.Yield(_unit._cts.Token);
            }
        }


    }

}
