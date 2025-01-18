using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCompanionMonster : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer cahracterRenderer;
        [SerializeField] public AnimSpriteInfo[] animSpriteinfoList;

        public Dictionary<UnitAnimSprtieType, SpriteAnimInfo> animspriteinfo = new Dictionary<UnitAnimSprtieType, SpriteAnimInfo>();

        public ControllerCompanionMonster _unit;
        public ControllerEnemyInGame triggeredEnemy;

        public LayerMask targetLayer;
        public Transform spriteTransform;

        public ParticleSystem spawnParticle;
        public void Init(ControllerCompanionMonster unit)
        {
            _unit = unit;

            for (int i = 0; i < animSpriteinfoList.Length; i++)
            {
                animspriteinfo.Add(animSpriteinfoList[i].spriteType, animSpriteinfoList[i].animInfo);
            }
        }

        public void SetSpriteImage(UnitAnimSprtieType _type, int index)
        {
            if (animspriteinfo.ContainsKey(_type))
            {
                cahracterRenderer.sprite = animspriteinfo[_type].spriteList[index];
            }
        }

        public bool IsInSpriteRange(UnitAnimSprtieType _type, int index)
        {
            if (animspriteinfo.ContainsKey(_type))
            {
                if (index >= animspriteinfo[_type].spriteList.Length)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if ((targetLayer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;

            var enemy = collision.GetComponent<ViewEnemy>();

            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);

                if (enemycon != null)
                {
                    if (enemycon._state.IsCurrentState(eActorState.Die) == false && enemycon._state.IsCurrentState(eActorState.InActive) == false)
                    {
                        if (triggeredEnemy == null)
                        {
                            triggeredEnemy = enemycon;
                        }
                    }
                }
            }
        }

    }
}

