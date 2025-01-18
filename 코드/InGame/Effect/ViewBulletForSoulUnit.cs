using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

namespace BlackTree
{
    public enum CriticalType
    {
        None,
        Cri,
        Super,Mega,
    }
    public class ViewBulletForSoulUnit : Core.Poolable
    {
        [SerializeField] protected LayerMask layer;

        private Rigidbody2D rb;

        private Core.ControllerEnemyInGame target;

        [SerializeField] private float speed = 13f;
        [SerializeField] private float rotateSpeed = 200f;
        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
        }
        double dmg;
        CriticalType criType;
         Definition.WitchSkillType witchType;
        public void Shoot(Vector2 start, Core.ControllerEnemyInGame enemy,double _dmg, Definition.WitchSkillType _witchType, CriticalType _criType)
        {
            target = enemy;
            transform.position = start;
            currentTime = 0;
            dmg = _dmg;

            dir = (Vector2)target._view.transform.position - (Vector2)this.transform.position;
            dir.Normalize();
            criType = _criType;

            witchType = _witchType;
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

        Vector2 dir;
        private void FixedUpdate()
        {
          

        }

        float currentTime = 0;
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;
            
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                if(enemycon!=null)
                {
                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    enemycon.DecreaseHp(dmg, UserDmgType.Normal);

                    enemycon._view.SetHitEffectOn();

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.bowHitEffect, null, this.transform.position);
                    _hitEffect.On();

                    AudioManager.Instance.Play(AudioSourceKey.witchFireHit);


                    switch (criType)
                    {
                        case CriticalType.None:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.white);
                            break;
                        case CriticalType.Cri:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.yellow);
                            break;
                        case CriticalType.Super:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.red);
                            break;
                        case CriticalType.Mega:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.blue);
                            break;
                        default:
                            break;
                    }
                  
                    if(witchType==Definition.WitchSkillType.Witch)
                    {
                        //Player.Skill.skillDamageList[(int)Definition.SkillKey.End] += dmg;
                    }
                    else
                    {
                        Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                    }
                    
                    
                }
            }
            currentTime = 0;
            PoolManager.Push(this);
        }
    }
}
