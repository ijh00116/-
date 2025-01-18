using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewTimeBomb : Poolable
{
    public ParticleSystem particle;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Skill.SkillCacheData skillCache;

    float currentTime = 0.0f;

    bool isTimeElapsed;
    public void FireStart(bool _isTimeElapsed)
    {
        currentTime = 0;
        skillCache = Player.Skill.Get(SkillKey.TimeBomb);

        this.gameObject.SetActive(true);
        isTimeElapsed = _isTimeElapsed;
        particle.Play();
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        AudioManager.Instance.Play(AudioSourceKey.Skill_TimeBomb);
    }

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
                    double skillvalue = (isTimeElapsed) ? skillCache.SkillValue(0,0) : skillCache.SkillValue(0, 1);

                    double dmg = (double)Player.Unit.BowAtk * (skillvalue * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);
                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.TimeBomb] += dmg;

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemycon._view.hitPos.position);
                    _hitEffect.On();
                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);

                    float randomData = skillCache.SkillValue(2);
                    float random = UnityEngine.Random.Range(0.0f, 100.0f);
                   
                    if (random<randomData)
                    {
                        if(enemycon.isHaveBomb==false)
                        {
                            enemycon.TimeBombSetting(true);
                        }
                    }
                }

            }
        }
    }

    private void Update()
    {
        if(this.gameObject.activeInHierarchy)
        {
            currentTime += Time.deltaTime;
            if(currentTime>=0.5f)
            {
                this.gameObject.SetActive(false);
                PoolManager.Push(this);
            }
        }
        
    }
}
