using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewSwordFewHitFireSkill : MonoBehaviour
    {
        public ParticleSystem particle;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        double dmg = 0;

        [SerializeField] GameObject target;

        [SerializeField] Collider2D mycol;
        [ContextMenu("Target")]
        public void TargetRotate()
        {
            if (target!=null)
            {
                Vector3 dir = target.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }
        public void Activate(ViewEnemy _enemy)
        {
            particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            currentTime = 0;

            if(_enemy!=null)
            {
                target = _enemy.gameObject;
                   Vector3 dir = target.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

                var skillvalue = Player.Skill.Get(Definition.SkillKey.SwordFewHitFire).SkillValue(0);
                dmg = (double)Player.Unit.SwordAtk * (skillvalue * 0.01f)*(Player.Unit.GetSkillIncreaseValue());

                particle.Play();
                mycol.enabled = true;
            }
       
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                var enemy = collision.GetComponent<ViewEnemy>();
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);
                    Player.Skill.skillDamageList[(int)Definition.SkillKey.SwordFewHitFire] += dmg;

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                }
            }
        }

        float currentTime = 0.0f;
        private void Update()
        {
            if (this.gameObject.activeInHierarchy)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= 0.2f)
                {
                    
                    mycol.enabled = false;
                }
                if(currentTime>=1.0f)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}