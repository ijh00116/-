using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Bundles;
using BlackTree.Definition;

public class ViewMeteorFire : Poolable
{
    Player.Skill.SkillCacheData skillcache;
    float currentLiveTime = 0.0f;
    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
    [SerializeField] Collider2D _collider;
    [SerializeField] ParticleSystem explodeParticle;
    [SerializeField] GameObject shadow;

    [SerializeField] protected LayerMask layer;

    WitchSkillType witchSkilltype;
    Player.Skill.SkillCacheData subunitskillcache;
    private void Awake()
    {
        Player.Skill.SkillActivate += SkillActivate;
    }

    private void SkillActivate(BlackTree.Definition.SkillKey _skillkey, bool active)
    {
        if (skillcache == null)
            return;
        if (_skillkey != skillcache.tabledataSkill.skillKey)
            return;
        if (active == false)
        {
            this.gameObject.SetActive(false);
            PoolManager.Push(this);
        }
    }
    public void Fire(Player.Skill.SkillCacheData _skillcache, WitchSkillType witchtype)
    {
        skillcache = _skillcache;
        currentLiveTime = 0;
        witchSkilltype = witchtype;
        if (subunitskillcache == null)
            subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);

        explodeParticle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        shadow.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        this.gameObject.SetActive(true);
        explodeParticle.Play();
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            currentLiveTime += Time.deltaTime;
            
            if (currentLiveTime >= 4.0f)
            {
                this.gameObject.SetActive(false);
                PoolManager.Push(this);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;

        var enemy = collision.GetComponent<ViewEnemy>();

        if (enemy != null)
        {
            var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);

            enemycon.MeteorFireSetting(true, witchSkilltype);
         
        }
    }

  
}
