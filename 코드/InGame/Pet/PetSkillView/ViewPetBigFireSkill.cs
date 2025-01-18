using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewPetBigFireSkill : MonoBehaviour
{
    public ParticleSystem particle;
    Vector3 dir = Vector3.one;
    float currentTime = 0;
    float endTime = 8.0f;

    float dmgCurrentTime = 0;
    float dmgTickTime = 0.8f;

    Vector2 endPos;
    [SerializeField] float movespeed = 2f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Pet.PetCacheData petCache;

    List<int> enemyHashListInCollider = new List<int>();

    public void Fire(Vector2 _startPos, Vector2 _endPos, Player.Pet.PetCacheData _petCache)
    {
        isActive = true;
        endPos = _endPos;
        petCache = _petCache;

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
                    double dmg = petCache.GetSkillValue();

                    enemycon.DecreaseHp(dmg, UserDmgType.PetSkill);

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemycon._view.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                    enemyHashListInCollider.Add(enemy.hash);
                }

            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<ViewEnemy>();
        if (enemy != null)
        {
            if (enemyHashListInCollider.Contains(enemy.hash))
            {
                enemyHashListInCollider.Remove(enemy.hash);
            }
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float realMoveSpeed = ((float)movespeed * 5.0f);
        transform.Translate(dir * Time.deltaTime * realMoveSpeed);

        currentTime += Time.deltaTime;

        if (currentTime >= endTime)
        {
            currentTime = 0;
            particle.Stop();
            particle.gameObject.SetActive(false);

            isActive = false;
        }

        dmgCurrentTime += Time.deltaTime;
        if (dmgCurrentTime >= dmgTickTime)
        {
            dmgCurrentTime = 0;
            for (int i = 0; i < enemyHashListInCollider.Count; i++)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemyHashListInCollider[i]);
                if (enemycon != null)
                {
                    double dmg = petCache.GetSkillValue();

                    enemycon.DecreaseHp(dmg, UserDmgType.PetSkill);

                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemycon._view.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemycon._view.transform.position, dmg, false, false, skillColor);
                }
            }
        }
    }
}
