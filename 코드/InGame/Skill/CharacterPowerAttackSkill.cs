using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region powerAttack
    public class CharacterPowerAttackSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        List<ViewPowerfullAttack> powerfullattackobjList = new List<ViewPowerfullAttack>();
        
        Player.Skill.SkillCacheData skillCache;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        int skillPrefabIndex;
        int currentFrame = 0;

        
        public CharacterPowerAttackSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.SwordExplode];

            var powerfullattackobj = Object.Instantiate(SkillResourcesBundle.Loaded.powerAttackSkillview[0]);
            powerfullattackobj.particle.Stop();
            powerfullattackobj.gameObject.SetActive(false);
            powerfullattackobjList.Add(powerfullattackobj);
          
        }
        public void CreateSkillEffect()
        {
            if (_unit.target == null)
            {
                skillPrefabIndex++;
                return;
            }

            var enemy = Battle.Enemy.GetRandomEnemyController();
            if (enemy != null)
            {
                ViewPowerfullAttack powerfullattackobj = null;
                if (skillPrefabIndex>=powerfullattackobjList.Count)
                {
                    powerfullattackobj = Object.Instantiate(SkillResourcesBundle.Loaded.powerAttackSkillview[0]);
                    powerfullattackobj.particle.Stop();
                    powerfullattackobj.gameObject.SetActive(false);
                    powerfullattackobjList.Add(powerfullattackobj);
                }
                else
                {
                    powerfullattackobj = powerfullattackobjList[skillPrefabIndex];
                }

              
                powerfullattackobj.particle.Stop();
                powerfullattackobj.Activate();
                powerfullattackobj.gameObject.SetActive(true);
                powerfullattackobj.transform.position = enemy._view.transform.position;


                InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);

                powerfullattackobj.particle.Play();

                skillPrefabIndex++;

                AudioManager.Instance.Play(AudioSourceKey.Skill_SwordExplode);
            }
        }
     
        public override eActorState GetState()
        {
            return eActorState.SwordExplode;
        }

        protected override void OnEnter()
        {
            _unit.animindex = 0;
            skillPrefabIndex = 0;
            currentFrame = 0;
            float scaleValue = 1 + (skillCache.SkillValue(2) * 0.01f);

            for (int i = 0; i < powerfullattackobjList.Count; i++)
            {
                powerfullattackobjList[i].particle.Stop();
                powerfullattackobjList[i].gameObject.SetActive(false);
                
                powerfullattackobjList[i].transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            }

            Main().Forget();

            _unit._state.ChangeState(eActorState.Idle);
        }

        protected override void OnExit()
        {
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                currentFrame++;
                if (currentFrame >= 10)
                {
                    currentFrame = 0;

                    CreateSkillEffect();
                }

                if (skillPrefabIndex>=skillCache.SkillValue(1)+2 || _unit.target==null)
                {
                    break;
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
        
        protected override void OnUpdate()
        {
         
        }
 
    }
    #endregion
}