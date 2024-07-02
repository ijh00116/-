using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterNovaSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float attackTime = 1;

        ViewNovaSkill novaSkill;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        public CharacterNovaSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.NoveForSeconds];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.NoveForSeconds];
            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }


        public void CreateSkillEffect()
        {
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
            if (novaSkill == null)
            {
                novaSkill = Object.Instantiate(SkillResourcesBundle.Loaded.viewNovaSkill);
            }
            novaSkill.gameObject.SetActive(true);

            novaSkill.Fire(Player.Unit.userUnit._view.skillPos.position,skillValue_0);

            AudioManager.Instance.Play(AudioSourceKey.Skill_NoveForSeconds);
        }

        public override eActorState GetState()
        {
            return eActorState.NoveForSeconds;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(1);
            _unit.animindex = 0;

            skillValue_0 = Player.Skill.Get(Definition.SkillKey.NoveForSeconds).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.NoveForSeconds).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.NoveForSeconds).SkillValue(2);

            _unit._state.ChangeState(eActorState.Idle);

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.NoveForSeconds, true);
            
            CreateSkillEffect();
        }
        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                if(novaSkill!=null)
                    novaSkill.gameObject.SetActive(false);
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.NoveForSeconds, false);
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.NoveForSeconds))
                {
                    if (_unit._state.stop == false)
                    {
                        currentTime += Time.deltaTime;
                        
                        if (currentTime > 1.0f * (1 - skillValue_1 * 0.01f))
                        {
                            currentTime = 0;
                            CreateSkillEffect();
                        }
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+ skillValue_2)
                        {
                            currentTime = 0;
                            novaSkill.InActive();
                            novaSkill.gameObject.SetActive(false);
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.NoveForSeconds, false);
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