using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree;
using BlackTree.Core;
using BlackTree.Definition;

public class ViewRangeStun : MonoBehaviour
{
    public ParticleSystem particle;
    public Collider2D collider;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    float skillValue_0;
    float skillValue_1;
    float skillValue_2;

    float currentTime=0;
    public void Init(Player.Skill.SkillCacheData skillCache)
    {
        collider.enabled = true;
        this.gameObject.SetActive(true);
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        if(Player.Cloud.optiondata.appearEffect)
        {
            particle.Play();
        }

        skillValue_0 = skillCache.SkillValue(0);
        skillValue_1 = skillCache.SkillValue(1);
        skillValue_2 = skillCache.SkillValue(2);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                double dmg = Player.Unit.SwordAtk*(skillValue_0*0.01f) * (Player.Unit.GetSkillIncreaseValue());

                if (enemycon.enemyType != EnemyType.Boss)
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                }
                else
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                }

                enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.RangeStun] += dmg;

                int randomNum = Random.Range(0, 100);

                if(randomNum<=skillValue_2&& skillValue_2>0)
                {
                    if(enemycon.enemyType!=EnemyType.Boss)
                    {
                        enemycon.InstantDie();
                    }
                }
                else
                {
                    enemycon.SetStunState(1+skillValue_1);
                }

                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
            }
        }
    }

    private void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            currentTime += Time.deltaTime;
            if(currentTime>=0.1f)
            {
                collider.enabled = false;
            }
            if (currentTime >= 1.5f)
            {
                currentTime = 0;
                this.gameObject.SetActive(false);
            }
        }
    }

}
