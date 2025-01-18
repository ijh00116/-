using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewEnemy : Poolable
    {
        public int hash;

        [SerializeField] public SpriteRenderer characterRenderer;
        [SerializeField] public AnimSpriteInfo[] animSpriteinfoList;

        public Dictionary<UnitAnimSprtieType, SpriteAnimInfo> animspriteinfo = new Dictionary<UnitAnimSprtieType, SpriteAnimInfo>();

        public Transform debuffParent;
        public Transform raycastTr;

        public LayerMask targetLayer;
        public float rayDistance;

        public ControllerEnemyInGame con;
        public eActorState currentstate;

        public Transform hitPos;

        public Material mat;

        public bool isMelee;
        public ViewBullet bullet;
        public Transform firePos;

        public Slider hpBar;

        public float speed;

        public Collider2D collider;

        public EnemyType enemyType;

        public ViewUnit triggeredUnit;

        public int animationDelayFram = 50;

        public Color freezedColor = new Color(107f/255f,135f/255f,1.0f,1.0f);
        public Color poisonedColor = new Color(125f / 255f, 1.0f,107f/255f, 1.0f);
        public Color normalColor = Color.white;
        public void Init(ControllerEnemyInGame _con)
        {
            hash = GetHashCode();
            con = _con;

            for (int i = 0; i < animSpriteinfoList.Length; i++)
            {
                animspriteinfo.Add(animSpriteinfoList[i].spriteType, animSpriteinfoList[i].animInfo);
            }

            //mat.EnableKeyword("HITEFFECT_ON");
            if(mat==null)
            {
                mat = characterRenderer.material;
            }
            mat.SetFloat("_HitEffectBlend", 0);
            
            hitOnTime = 1;
        }
        public void SyncHpUI()
        {
            hpBar.value = (float)(con.hp) / (float)(con.maxhp);
        }
  

        public void OnDamage(float power, float isCritical)
        {
            var con = Battle.Enemy.GetHashEnemyController(hash);
        }

        public void SetSpriteImage(UnitAnimSprtieType _type, int index)
        {
            if (animspriteinfo.ContainsKey(_type))
            {
                characterRenderer.sprite = animspriteinfo[_type].spriteList[index];
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
           // if(con!=null)
            currentstate = con._state.CurrentState;
            
            hitOnTime += Time.deltaTime;

            if (hitOnTime>=0.2f && isHiteffectOn)
            {
                isHiteffectOn = false;
                mat.SetFloat("_HitEffectBlend", 0);
            }
        }

        float hitOnTime;
        bool isHiteffectOn = false;
        public void SetHitEffectOn()
        {
            isHiteffectOn = true;
            mat.SetFloat("_HitEffectBlend", 1);
            hitOnTime = 0;
        }
        [SerializeField] protected LayerMask layer;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((targetLayer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;

            var unit = collision.GetComponent<ViewUnit>();

            if (unit != null)
            {
                triggeredUnit = Player.Unit.userUnit._view;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;

            var enemy = collision.GetComponent<ViewEnemy>();

            float mydistance = Vector3.Distance(Player.Unit.userUnit._view.transform.position, this.transform.position);
            float otherEnemyDistance = Vector3.Distance(Player.Unit.userUnit._view.transform.position, enemy.transform.position);

            bool aiStop = mydistance > otherEnemyDistance;
            con.isCollidingWithOthers = aiStop;
            enemy.con.isCollidingWithOthers = !aiStop;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if ((targetLayer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;

            var unit = collision.GetComponent<ViewUnit>();

            if (unit != null)
            {
                triggeredUnit = null;
            }
        }
    }

}
