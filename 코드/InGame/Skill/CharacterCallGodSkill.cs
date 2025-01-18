using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region CallGod
    public class CharacterCallGodSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewCallGodSkill viewCallGodSkill;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;
        float triggerTime;

        float skillValue_0_0;
        float skillValue_0_1;
        float skillValue_1;
        float skillValue_2;
        public CharacterCallGodSkill(ControllerUnitInGame unit)
        {
            _unit = unit;
     
            skillCache = Player.Skill.skillCaches[Definition.SkillKey.GodMode];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.GodMode];

            Player.Skill.SkillActivate += SkillActivate;
            Main().Forget();
        }
        protected override void OnEnter()
        {
            triggerTime = 0;
            _unit._state.SetUpdateFrameDelay(50);

            skillValue_0_0 = skillCache.SkillValue(0);
            skillValue_0_1 = skillCache.SkillValue(0,1);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_GodMode);
            _unit._state.ChangeState(eActorState.Idle);

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_1);
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;
            

            if (viewCallGodSkill == null)
            {
                viewCallGodSkill = Object.Instantiate(SkillResourcesBundle.Loaded.viewCallGodSkill);
            }
            viewCallGodSkill.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            viewCallGodSkill.gameObject.SetActive(true);
            viewCallGodSkill.particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            viewCallGodSkill.transform.position = _unit._view.skillPos.position;
            viewCallGodSkill.particle.Play();

            _unit._view.cahracterRenderer.material = InGameResourcesBundle.Loaded.GodModeMat;

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.GodMode);

            Player.Rune.SyncAllData();
        }

        public override eActorState GetState()
        {
            return eActorState.GodMode;
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
                Player.Unit.SkillActiveUpdate(Definition.SkillKey.GodMode, false);
                if(viewCallGodSkill!=null)
                    viewCallGodSkill.gameObject.SetActive(false);
                _unit._view.cahracterRenderer.material = InGameResourcesBundle.Loaded.defaultMat;

                Player.Rune.SyncAllData();
            }
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.GodMode))
                {
                    if (_unit._state.stop == false)
                    {
                        viewCallGodSkill.transform.position = _unit._view.skillPos.position;
                        if (skillCache.elapsedCooltime >= tabledata.effectTime+ skillValue_1)
                        {
                            viewCallGodSkill.gameObject.SetActive(false);
                            _unit._view.cahracterRenderer.material = InGameResourcesBundle.Loaded.defaultMat;
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.GodMode, false);

                            Player.Rune.SyncAllData();
                        }
                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
    #endregion
}
