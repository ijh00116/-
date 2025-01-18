using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewPowerfullAttack : MonoBehaviour
    {
        public ParticleSystem particle;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        double dmg = 0;
        public void Activate()
        {
            currentTime = 0;

            var skillvalue = Player.Skill.Get(Definition.SkillKey.SwordExplode).SkillValue(0);
            dmg = (double)Player.Unit.SwordAtk * (skillvalue * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

            hitCount = 0;
            particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        }

        int hitCount = 0;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                if(hitCount>=4)
                {
                    return;
                }
                var enemy = collision.GetComponent<ViewEnemy>();
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }
                    hitCount++;
                    if (enemycon!=null)
                    {
                        enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);
                        Player.Skill.skillDamageList[(int)Definition.SkillKey.SwordExplode] += dmg;
                        WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                    }
                    
                }
            }
        }

        float currentTime = 0.0f;
        private void Update()
        {
            if(this.gameObject.activeInHierarchy)
            {
                currentTime += Time.deltaTime;
                if(currentTime>=0.4f)
                {
                    currentTime = 0;
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

}
