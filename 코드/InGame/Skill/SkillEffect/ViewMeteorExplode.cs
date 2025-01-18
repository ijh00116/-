using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Bundles;
using BlackTree.Definition;

public class ViewMeteorExplode : Poolable
{
    Player.Skill.SkillCacheData skillcache;
    float currentTime = 0.0f;
    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
    [SerializeField] Collider2D _collider;
    [SerializeField] ParticleSystem explodeParticle;

    [SerializeField] protected LayerMask layer;

    WitchSkillType witchSkilltype;
    Player.Skill.SkillCacheData subunitskillcache;
    public void Fire( Player.Skill.SkillCacheData _skillcache, WitchSkillType witchtype)
    {
        skillcache = _skillcache;
        currentTime = 0;
        _collider.enabled = true;
        witchSkilltype = witchtype;
        if (subunitskillcache == null)
            subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);
        
        this.gameObject.SetActive(true);
        explodeParticle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        explodeParticle.Play();
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= 0.2f)
            {
                _collider.enabled = false;
            }
            if(currentTime >=1.0f)
            {
                this.gameObject.SetActive(false);
                PoolManager.Push(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;

        if (collision != null)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                double dmg = (double)Player.Unit.BowAtk * (skillcache.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

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
                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SpawnMeteor] += dmg;
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
            }
        }
    }
}
