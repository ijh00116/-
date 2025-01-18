using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewBigFireBall : MonoBehaviour
{
    public ParticleSystem particle;
    Vector3 dir = Vector3.one;
    float currentTime = 0;
    float endTime = 8.0f;

    float dmgCurrentTime = 0;
    float dmgTickTime = 0.8f;

    Vector2 endPos;
    [SerializeField] float movespeed = 2f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Skill.SkillCacheData skillCache;
    WitchSkillType witchSkilltype;

    Player.Skill.SkillCacheData subunitskillcache;

    List<int> enemyHashListInCollider = new List<int>();
    public void Fire(Vector2 _startPos, Vector2 _endPos, Player.Skill.SkillCacheData _skillCache, WitchSkillType witchtype)
    {
        isActive = true;
        endPos = _endPos;
        skillCache = _skillCache;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        witchSkilltype = witchtype;

        if (subunitskillcache == null)
            subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);

        particle.Play();
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);

        float scale =1+ skillCache.SkillValue(2) * 0.01f;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                if(enemycon!=null)
                {
                    double dmg = (double)Player.Unit.BowAtk * (skillCache.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }
                    if (witchSkilltype == WitchSkillType.Witch)
                    {
                       

                        enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                        Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.BigFireballForSeconds] += dmg;
                    }
                    else
                    {
                        dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                        enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                        Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                    }

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemycon._view.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                    enemyHashListInCollider.Add(enemy.hash);
                }
              
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<ViewEnemy>();
        if(enemy!=null)
        {
            if(enemyHashListInCollider.Contains(enemy.hash))
            {
                enemyHashListInCollider.Remove(enemy.hash);
            }
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float realMoveSpeed = ((float)movespeed * 5.0f) * (1 - skillCache.SkillValue(1) * 0.01f);
        transform.Translate(dir * Time.deltaTime * realMoveSpeed);

        currentTime += Time.deltaTime;
     
        if (currentTime>=endTime)
        {
            currentTime = 0;
            particle.Stop();
            particle.gameObject.SetActive(false);

            isActive = false;
        }

        dmgCurrentTime += Time.deltaTime;
        if(dmgCurrentTime>=dmgTickTime)
        {
            dmgCurrentTime = 0;
            for(int i=0; i<enemyHashListInCollider.Count; i++)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemyHashListInCollider[i]);
                if (enemycon != null)
                {
                    double dmg = (double)Player.Unit.BowAtk * (skillCache.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    if (witchSkilltype == WitchSkillType.Witch)
                    {
                        enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                        Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.BigFireballForSeconds] += dmg;
                    }
                    else
                    {
                        dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                        enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                        Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                    }

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemycon._view.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemycon._view.transform.position, dmg, false, false, skillColor);
                }
            }
        }
    }
}
