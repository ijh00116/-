using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Core
{
    #region AbsorbLife
    public class CharacterAbsorbLifeSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewAbsorbLifeSkill viewabsorbLife;
        Player.Skill.SkillCacheData skillCache;
        Definition.SkillTableData tabledata;

        const int enemyCount = 7;
        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        List<AbsorbLifeBullet> absorbBulletList = new List<AbsorbLifeBullet>();

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
        public CharacterAbsorbLifeSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.AbsorbLife];
            tabledata = StaticData.Wrapper.skillDatas[(int)Definition.SkillKey.AbsorbLife];

            viewabsorbLife = Object.Instantiate(SkillResourcesBundle.Loaded.absorbLifeskill);
            viewabsorbLife.transform.SetParent(InGameObjectManager.Instance.transform, false);
        }
        protected override void OnEnter()
        {
            skillValue_0 = skillCache.SkillValue(0);
            skillValue_1 = skillCache.SkillValue(1);
            skillValue_2 = skillCache.SkillValue(2);

            CreateSkillEffect();

            AudioManager.Instance.Play(AudioSourceKey.Skill_AbsorbLife);

            _unit._state.ChangeState(eActorState.Idle);
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;
            var enemylist = Battle.Enemy.GetRandomEnemiesController(enemyCount+(int)skillValue_1);

            for (int i=0; i< enemylist.Count; i++)
            {
                var absorbEffect = PoolManager.Pop<AbsorbLifeEffect>(viewabsorbLife.particleEffect, InGameObjectManager.Instance.transform, enemylist[i]._view.transform.position);
                absorbEffect.transform.position = enemylist[i]._view.transform.position;
                absorbEffect.particle.Play();
                absorbEffect.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);

                AbsorbLifeBullet bullet;
                if (i>= absorbBulletList.Count)
                {
                    bullet = UnityEngine.Object.Instantiate(viewabsorbLife.particleBullet);
                    bullet.transform.SetParent(InGameObjectManager.Instance.transform, false);
                    absorbBulletList.Add(bullet);
                }
                else
                {
                    bullet = absorbBulletList[i];
                }
                bullet.transform.position = enemylist[i]._view.transform.position;
                bullet.gameObject.SetActive(true);
                bullet.Shoot(enemylist[i]._view.transform.position, _unit._view.transform.position);

                float dmgpercent = (skillValue_0) * 0.01f;
                double maxdmg = Player.Unit.SwordAtk * 7;
                if (enemylist[i].maxhp* dmgpercent > maxdmg)
                {
                    enemylist[i].DecreaseHp(maxdmg, UserDmgType.SkillNormal);
                }
                else
                {
                    enemylist[i].DecreaseHpPercentageForAbsorbSkill(skillValue_0);
                }
                    

                WorldUIManager.Instance.InstatiateFont(enemylist[i]._view.transform.position, enemylist[i].GetHpPercentageToValue(skillValue_0), false, false, skillColor);
            }
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
        }

        public override eActorState GetState()
        {
            return eActorState.AbsorbLife;
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {
            
        }


    }
    #endregion
}