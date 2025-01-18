using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Definition;

public class ViewBulletForTurretSkill : Poolable
{
    [SerializeField] protected LayerMask layer;
    private ControllerEnemyInGame target;
    float currentTime = 0;
    [SerializeField] private float speed = 13f;
    Vector2 dir;
    double dmg;
    [SerializeField] ParticleSystem particle;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
    public void Shoot(Vector2 start, ControllerEnemyInGame enemy, double _dmg)
    {
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);

        target = enemy;
        transform.position = start;
        currentTime = 0;

        dir = (Vector2)target._view.transform.position - (Vector2)this.transform.position;
        dir.Normalize();

        dmg = _dmg;
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy == false)
            return;

        currentTime += Time.deltaTime;
        this.transform.Translate(dir * Time.deltaTime * speed);
        if (currentTime >= 2.0f)
        {
            currentTime = 0;
            PoolManager.Push(this);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<ViewEnemy>();
        if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;

        if (enemy != null)
        {
            var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
            if (enemycon != null)
            {
                if (enemycon.enemyType != EnemyType.Boss)
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                }
                else
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                }

                enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SetTurret] += dmg;

                enemycon._view.SetHitEffectOn();

                var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.bowHitEffect, null, this.transform.position);
                _hitEffect.On();

                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
            }
        }
        currentTime = 0;
        PoolManager.Push(this);
    }
}
