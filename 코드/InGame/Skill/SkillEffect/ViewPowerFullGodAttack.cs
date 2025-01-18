using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewPowerFullGodAttack : MonoBehaviour
    {
        public ParticleSystem particle;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        double dmg = 0;
        public void Activate()
        {
            currentTime = 0;

            var skillvalue = Player.Skill.Get(Definition.SkillKey.GodMode).SkillValue(2);
            dmg = (double)Player.Unit.SwordAtk * (skillvalue * 0.01f) * (Player.Unit.GetSkillIncreaseValue());
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
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

                    enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                }
            }
        }

        float currentTime = 0.0f;
        private void Update()
        {
            if (this.gameObject.activeInHierarchy)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= 0.2f)
                {
                    currentTime = 0;
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

}
