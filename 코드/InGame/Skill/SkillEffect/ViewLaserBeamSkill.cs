using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree
{
    public class ViewLaserBeamSkill : MonoBehaviour
    {
        [SerializeField]GameObject startLaserBeam;
        [SerializeField] GameObject endLaserBeam;
        [SerializeField] LineRenderer laserBeam;
        Player.Skill.SkillCacheData skillCache;

        float currentTime = 0;
        float sendDmgTime = 0.2f;

        Vector3 dir;

        [Header("Width Pulse Options")]
        public float widthMultiplier = 1.5f;
        private float customWidth;
        private float originalWidth;
        private float lerpValue = 0.0f;
        public float pulseSpeed = 1.0f;
        private bool pulseExpanding = true;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        WitchSkillType witchSkilltype;
        Player.Skill.SkillCacheData subunitskillcache;

        public void Activate(Player.Skill.SkillCacheData _skillCache, Definition.WitchSkillType _witchSkilltyep)
        {
            this.gameObject.SetActive(true);
            startLaserBeam.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            endLaserBeam.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            laserBeam.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);

            skillCache = _skillCache;

            witchSkilltype = _witchSkilltyep;
            if (subunitskillcache == null)
                subunitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);

            if (witchSkilltype == WitchSkillType.Witch)
            {
                this.transform.position = Player.Unit.usersubUnit._view.transform.position;
            }
            else
            {
                this.transform.position = Player.Skill.skillSubUnitObject._view.transform.position;
            }
            
            startLaserBeam.transform.localPosition = Vector3.zero;

            var enemycon = Battle.Enemy.GetRandomEnemyController();
            if(enemycon!=null)
            {
                Vector2 targetDir = (enemycon._view.transform.position - this.transform.position).normalized;
                dir = targetDir * 50;

                endLaserBeam.transform.position = startLaserBeam.transform.position + dir;

                laserBeam.SetPosition(0, startLaserBeam.transform.position);
                laserBeam.SetPosition(1, endLaserBeam.transform.position);

                laserBeam.useWorldSpace = true;
                laserBeam.positionCount = 2;

                originalWidth = 0.6f;
                customWidth = originalWidth * widthMultiplier;
            }
        }

        public void InActivate()
        {
            this.gameObject.SetActive(false);
            startLaserBeam.gameObject.SetActive(false);
            endLaserBeam.gameObject.SetActive(false);
            laserBeam.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (this.gameObject.activeInHierarchy == false)
                return;
            if (skillCache == null)
                return;

            laserBeam.SetPosition(0, startLaserBeam.transform.position);
            laserBeam.SetPosition(1, endLaserBeam.transform.position);

            if (pulseExpanding)
            {
                lerpValue += Time.deltaTime * pulseSpeed;
            }
            else
            {
                lerpValue -= Time.deltaTime * pulseSpeed;
            }

            if (lerpValue >= 1.0f)
            {
                pulseExpanding = false;
                lerpValue = 1.0f;
            }
            else if (lerpValue <= 0.0f)
            {
                pulseExpanding = true;
                lerpValue = 0.0f;
            }

            float currentWidth = Mathf.Lerp(originalWidth, customWidth, Mathf.Sin(lerpValue * Mathf.PI));

            laserBeam.startWidth = currentWidth;
            laserBeam.endWidth = currentWidth;

            if (witchSkilltype == WitchSkillType.Witch)
            {
                this.transform.position = Player.Unit.usersubUnit._view.transform.position;
            }
            else
            {
                this.transform.position = Player.Skill.skillSubUnitObject._view.transform.position;
            }
            //endLaser 이동
            endLaserBeam.transform.RotateAround(startLaserBeam.transform.position, Vector3.back, Time.deltaTime * 40 * (1 + skillCache.SkillValue(2)*0.01f));
            
            //endLaser 이동

            //raycastHit
            currentTime += Time.deltaTime;
            if(currentTime>=sendDmgTime)
            {
                currentTime = 0;
                dir = (endLaserBeam.transform.position - this.transform.position).normalized;
                RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, dir, 40);

                for (int i = 0; i < hits.Length; i++)
                {
                    var enemy = hits[i].collider.gameObject.GetComponent<ViewEnemy>();
                    if (enemy != null)
                    {
                        var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                        double dmg = Player.Unit.BowAtk * (skillCache.SkillValue(0)*0.01f) * (Player.Unit.GetSkillIncreaseValue());

                        if (enemycon.enemyType != EnemyType.Boss)
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                        }
                        else
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                        }

                        if (witchSkilltype == Definition.WitchSkillType.Witch)
                        {
                            enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                            Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.LaserBeam] += dmg;
                        }
                        else
                        {
                            dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                            enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                            Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                        }

                        WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                    }
                }
            }
        }
    }
}

