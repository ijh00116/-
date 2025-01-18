using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    //요것은 적들의 bullet으로 사용되는중
    public class ViewBullet : Poolable
    {
        [SerializeField] protected LayerMask layer;

        private Rigidbody2D rb;

        private Transform target;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotateSpeed = 200f;

        ControllerEnemyInGame enemyUnit;

        Vector2 rotateDir;

        [SerializeField] GameObject bulletRender;
        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
        }
        public void Shoot(Vector2 start, Transform enemy,ControllerEnemyInGame unit)
        {
            isActive = true;
            target = enemy;
            transform.position = start;
            enemyUnit = unit;
            currentTime = 0;

            rotateDir= (Vector2)target.position-(Vector2)transform.position;
     

            float angle = Mathf.Atan2(rotateDir.y, rotateDir.x) * Mathf.Rad2Deg;
            bulletRender.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            dir = (Vector2)target.position - (Vector2)this.transform.position;
            dir.Normalize();
        }

        bool isActive = false;
        private void Update()
        {
            if (isActive==false)
                return;
            currentTime += Time.deltaTime;
            if (currentTime >= 2.0f)
            {
                currentTime = 0;
                PoolManager.Push(this);
            }
        }

        Vector2 dir;
        private void FixedUpdate()
        {
            //this.transform.LookAt(dir);
            this.transform.Translate(dir * Time.deltaTime * 5.0f);
        }

        float currentTime = 0;
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.GetComponent<ViewUnit>();
            if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;


            if (player != null)
            {
                double atkdmg = enemyUnit.atk;
                if (enemyUnit.enemyType != EnemyType.Boss)
                {
                    atkdmg = enemyUnit.atk * (1 - Player.Unit.DecreaseDmgFromMonster);
                }
                Player.Unit.DecreaseHp(atkdmg);

                PoolManager.Push(this);
                isActive = false;

                int randomIndex = UnityEngine.Random.Range(0, Player.Unit.userUnit._view.hitTransform.Length);
                var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, Player.Unit.userUnit._view.hitTransform[randomIndex].position);
                _hitEffect.On();

                //WorldUIManager.Instance.InstatiateFont(Player.Unit.userUnit._view.transform.position, 10, false, Color.white);

                if (Player.Skill.Get(SkillKey.RecoverShield).userSkilldata.AwakeLv >= 1)
                {
                    if (Player.Unit.IsSkillActive(SkillKey.RecoverShield))
                    {
                        float skillvalue_1 = Player.Skill.Get(SkillKey.RecoverShield).SkillValue(1);
                        double dmg = Player.Unit.SwordAtk * (skillvalue_1 * 0.01f);

                        if (enemyUnit.enemyType != EnemyType.Boss)
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                        }
                        else
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                        }

                        enemyUnit.DecreaseHp(dmg, UserDmgType.SkillNormal);

                        enemyUnit._view.SetHitEffectOn();
                        
                        var _mirrorhitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemyUnit._view.hitPos.position);
                        _mirrorhitEffect.On();

                        WorldUIManager.Instance.InstatiateFont(enemyUnit._view.transform.position, dmg, false, false, Color.green);
                    }
                }
            }
        }
    }

}
