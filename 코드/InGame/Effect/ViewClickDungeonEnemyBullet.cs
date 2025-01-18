using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    public class ViewClickDungeonEnemyBullet : Poolable
    {
        [SerializeField] protected LayerMask layer;

        private Rigidbody2D rb;

        private Vector2 target;

        [SerializeField] private float speed = 5f;

        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            Battle.Clicker.clickDungeonenemyDisapear += PushThisObj;
        }

        void PushThisObj()
        {
            PoolManager.Push(this);
        }
        public void Shoot(Vector2 start, Vector2 enemy)
        {
            target = enemy;
            transform.position = start;

            currentTime = 0;

            dir = target - (Vector2)this.transform.position;
            dir.Normalize();
        }

        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= 6.0f)
            {
                currentTime = 0;
                PoolManager.Push(this);
            }
        }

        Vector2 dir;
        private void FixedUpdate()
        {
            this.transform.Translate(dir * Time.deltaTime * 5.0f);
        }

        float currentTime = 0;
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;

            var player = collision.GetComponent<ViewClickerSubUnit>();

            if (player != null)
            {
                PoolManager.Push(this);

                Battle.Clicker.clickerSubUnit.DecreaseHp();

                var _mirrorhitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, Battle.Clicker.clickerSubUnit._view.hitpos.position);
                _mirrorhitEffect.On();

                WorldUIManager.Instance.InstatiateFont(Battle.Clicker.clickerSubUnit._view.transform.position, 1, false, false, Color.green);

                
            }
        }
    }

}
