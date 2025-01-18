using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region PoolingPortal
    public class CharacterSwordFewHitFireSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        static Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        float skillValue_0 = 0;
        static float skillValue_1 = 0;
        float skillValue_2 = 0;

        const int defaultNeedAttackCount = 5;

        List<ViewSwordFewHitFireSkill> swordFewHitObjList = new List<ViewSwordFewHitFireSkill>();
        public static int FireAttackCount()
        {
            int needCount = (int)System.Math.Round((defaultNeedAttackCount - skillCache.SkillValue(1)));
            return needCount;
        }

        public CharacterSwordFewHitFireSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.SwordFewHitFire];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.SwordFewHitFire];
            Player.Skill.SkillActivate += SkillActivate;

            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            Main().Forget();
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_SwordFewHitFire);
            _unit._state.ChangeState(eActorState.Idle);
        }

     
        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            ViewSwordFewHitFireSkill tempObj = null;
            for (int i = 0; i < swordFewHitObjList.Count; i++)
            {
                if (swordFewHitObjList[i].gameObject.activeInHierarchy == false)
                {
                    tempObj = swordFewHitObjList[i];
                    break;
                }
            }
            if (tempObj == null)
            {
                tempObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.swordFewHitSkillView);
                swordFewHitObjList.Add(tempObj);
            }
            tempObj.particle.Stop();
            tempObj.gameObject.SetActive(true);
            tempObj.transform.position = _unit._view.skillPos.transform.position;
            float skillscale = 1+skillValue_2*0.01f;
            tempObj.transform.localScale = new Vector3(skillscale, skillscale, skillscale);
            tempObj.Activate(Player.Unit.userUnit.targetController._view);

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        public override eActorState GetState()
        {
            return eActorState.SwordFewHitFire;
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
                for(int i=0; i< swordFewHitObjList.Count; i++)
                {
                    swordFewHitObjList[i].gameObject.SetActive(false);
                }
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.SwordFewHitFire))
                {
                    if (_unit._state.stop == false)
                    {
                       
                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
    #endregion
}