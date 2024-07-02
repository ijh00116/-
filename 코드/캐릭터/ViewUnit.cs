using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using UnityEngine.UI;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewUnit : Poolable
    {
        [HideInInspector]public int hash;
        [SerializeField] public SpriteRenderer cahracterRenderer;
        [SerializeField] public AnimSpriteInfo[] animSpriteinfoList;

        public Transform skillPos;

        public Transform BuffTransform;
        public Transform PetTransform;
        public Transform AttackTriggerPos;

        public Transform spriteTransform;
        ControllerUnitInGame _unit;

        public LayerMask targetLayer;

        public float rayDistance;

        public Dictionary<UnitAnimSprtieType, SpriteAnimInfo> animspriteinfo = new Dictionary<UnitAnimSprtieType, SpriteAnimInfo>();

        public Slider hpBar;
        public Slider shieldBar;

        public float speed;


        public ControllerEnemyInGame triggeredEnemy;

        public Transform[] hitTransform;

        public GameObject[] buffObject;
        public ParticleSystem[] buffObjectParticle;

        [Header("speechBubble")]
        public GameObject speechBubbleObj;
        public TMPro.TMP_Text speechBubleText;
        public Febucci.UI.TypewriterByCharacter textTypeWriter;

        //temp
        public Transform temptarget;
        public void Init(ControllerUnitInGame unit)
        {
            _unit = unit;
            hash = GetHashCode();

            for (int i = 0; i < animSpriteinfoList.Length; i++)
            {
                animspriteinfo.Add(animSpriteinfoList[i].spriteType, animSpriteinfoList[i].animInfo);
            }
            SyncHpUI();
        }

        public void SyncHpUI()
        {
            hpBar.value = (float)(Model.Player.Unit.Hp) / (float)(Model.Player.Unit.MaxHp);
            shieldBar.value = (float)(Model.Player.Unit.Shield) / (float)(Model.Player.Unit.MaxShield);
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

        private void Update()
        {
            temptarget = Player.Unit.userUnit.target;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
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
                    if (enemycon._state.IsCurrentState(eActorState.Die) == false && enemycon._state.IsCurrentState(eActorState.InActive) == false&& enemycon._state.stop==false)
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
