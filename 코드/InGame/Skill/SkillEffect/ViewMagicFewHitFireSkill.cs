using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewMagicFewHitFireSkill : MonoBehaviour
    {
        public ParticleSystem particle;
        double currentDamage;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        [SerializeField] float movespeed = 2f;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                var enemy = collision.GetComponent<ViewEnemy>();
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                    if (enemycon != null)
                    {
                        double dmg = (double)Player.Unit.BowAtk * (skillValue * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                        if (enemycon.enemyType != EnemyType.Boss)
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                        }
                        else
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                        }

                        if (witchSkilltype == Definition.WitchSkillType.Witch)
                        {
                            enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                            Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.MagicFewHitFire] += dmg;
                        }
                        else
                        {
                            dmg= dmg * subunitskillcache.SkillValue(2) * 0.01f;
                            enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                            Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                        }
                        

                        WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                    }
                }
            }
        }

        Vector3 dir = Vector3.up;
        float currentTime = 0;
        float skillValue = 0;

        Definition.WitchSkillType witchSkilltype;

        Player.Skill.SkillCacheData subunitskillcache;
        public void MoveToArr(Vector3 _dir, float _skillValue, Definition.WitchSkillType _witchSkilltyep)
        {
            particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            currentTime = 0;
            dir = _dir;
            skillValue = _skillValue;
            witchSkilltype = _witchSkilltyep;

            if(subunitskillcache==null)
                subunitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
        }

        private void Update()
        {
            transform.Translate(dir * Time.deltaTime * (float)movespeed * 5);
            currentTime += Time.deltaTime;
            if (currentTime >= 2)
            {
                currentTime = 0;
                particle.Stop();
                this.gameObject.SetActive(false);
            }
        }
    }

}
