using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree.Bundles
{
    public class Bullet : Poolable
    {
        public float Power { get; set; }
        public bool IsCritical { get; set; }

        protected Coroutine animco;
        [SerializeField] private float _speed = 3;
        [SerializeField] private float _acceleration = 0.2f;

        private Transform tr;
        private Vector2 distance;

        private float _originSpeed;
        [SerializeField] protected LayerMask layer;

        [SerializeField] bool haveBossHitEffect = false;
        [SerializeField] protected HitEffect hiteffectPrefab;
        [SerializeField] protected HitEffect bosshiteffectPrefab;
        bool alreadyTriggered;

        protected int hitCount;
        private void Awake()
        {
            tr = GetComponent<Transform>();
            _originSpeed = _speed;
        }

        private void OnEnable()
        {

            StartCoroutine(CoSetOff());
        }

        public void Shoot(Vector2 startPos, Vector2 targetPos)
        {
            alreadyTriggered = false;
            _speed = _originSpeed;
            tr.position = startPos;

            //Vector2 direction = targetPos - startPos;
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            var dir = targetPos - startPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            StartCoroutine(CoShoot(targetPos));

            if (animco != null)
            {
                StopCoroutine(animco);
                animco = null;
            }
        }

        private IEnumerator CoShoot(Vector2 targetPos)
        {
            distance = (targetPos - (Vector2)tr.position).normalized;
            while (true)
            {
                _speed += _acceleration;
                tr.Translate(distance * _speed * Time.deltaTime, Space.World);
                yield return null;
            }
        }

        private void SetOff()
        {
            hitCount = 0;
            PoolManager.Push(this);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;
            if (alreadyTriggered)
                return;
            if (enemy != null)
            {
                alreadyTriggered = true;

                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                double dmg = Player.Unit.SwordAtk;
                enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, Color.white);

                HitEffect effect = null;
                effect = PoolManager.Pop(hiteffectPrefab, InGameObjectManager.Instance.transform, this.transform.position);
                effect.transform.position = this.transform.position;
                effect.On();
                if (animco != null)
                {
                    StopCoroutine(animco);
                    animco = null;
                }
                PoolManager.Push(this);
            }
        }

        private IEnumerator CoSetOff()
        {
            yield return new WaitForSeconds(3f);
            SetOff();
        }
    }

}
