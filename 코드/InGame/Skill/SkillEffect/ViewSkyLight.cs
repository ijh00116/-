using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewSkyLight : MonoBehaviour
{
    public ParticleSystem particle;
    Vector3 dir = Vector3.one;
    float currentTime = 0;
    float endTime = 1.0f;

    Vector2 endPos;
    [SerializeField] float movespeed = 35f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Skill.SkillCacheData skillCache;

    bool isBackComplete = false;
    public void Fire(Vector2 _startPos, Vector2 _endPos, Player.Skill.SkillCacheData _skillCache)
    {
        isActive = true;
        endPos = _endPos;
        skillCache = _skillCache;

        isBackComplete = false;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        particle.Play();
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
    }

    public void BackToCharacter(Vector2 _startPos, Vector2 _endPos)
    {
        isBackComplete = true;

        isActive = true;
        endPos = _endPos;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        particle.Play();
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                if (enemycon != null)
                {
                    double dmg = (double)Player.Unit.SwordAtk * (skillCache.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);
                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SkyLight] += dmg;

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemycon._view.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                }

            }
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        transform.Translate(dir * Time.deltaTime * movespeed);

        currentTime += Time.deltaTime;

        if (currentTime >= endTime)
        {
            currentTime = 0;

            if(isBackComplete==false)
            {
                float randomValue = Random.Range(0, 100);
                if (randomValue < skillCache.SkillValue(2))
                {
                    Vector2 startpos = Player.Unit.userUnit._view.transform.position + dir * 25;
                    Vector2 endpos = Player.Unit.userUnit._view.transform.position;

                    BackToCharacter(startpos, endpos);
                }
                else
                {
                    particle.Stop();
                    particle.gameObject.SetActive(false);

                    isActive = false;
                }
            }
            else
            {
                particle.Stop();
                particle.gameObject.SetActive(false);

                isActive = false;
            }
         
        }
    }
}
