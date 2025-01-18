using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewNovaSkill : MonoBehaviour
{
    public ParticleSystem[] particle;

    Vector2 spawnPos;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    float currentTime = 0;
    float liveTime = 0.8f;
    float skillValue_0 = 0;

    [SerializeField] BoxCollider2D _collider;
    public void Fire(Vector2 SpawnPos,float _skillValue_0)
    {

        skillValue_0 = _skillValue_0;

        isActive = true;
        spawnPos = SpawnPos;

        this.gameObject.SetActive(true);
        this.transform.position = spawnPos;

        currentTime = 0.0f;
        for (int i = 0; i < particle.Length; i++)
        {
            particle[i].Play();
            particle[i].gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        }
        _collider.enabled = true;
    }

    public void InActive()
    {
        for (int i = 0; i < particle.Length; i++)
        {
            particle[i].gameObject.SetActive(false);
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
                if(enemycon!=null)
                {
                    double dmg = (double)Player.Unit.SwordAtk * (skillValue_0 * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    enemycon.DecreaseHp(dmg, UserDmgType.SkillRange);

                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.NoveForSeconds] += dmg;

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

        currentTime += Time.deltaTime;
        if(currentTime>=0.2f)
        {
            _collider.enabled = false;
        }
        if(currentTime>=liveTime)
        {
            this.gameObject.SetActive(false);
            isActive = false;
        }
        this.gameObject.transform.position = Player.Unit.userUnit._view.transform.position;
    }
}
