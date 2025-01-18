using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewPetLightningSkill : MonoBehaviour
{
    public ParticleSystem particle;
    Vector3 dir = new Vector3(1, -1, 0);
    float currentTime = 0;

    Vector2 endPos;
    [SerializeField] float movespeed = 5f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Pet.PetCacheData petCache;

    public void Fire(Vector2 _startPos, Vector2 _endPos, Player.Pet.PetCacheData _petcache)
    {
        isActive = true;
        endPos = _endPos;
        petCache = _petcache;

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
                double dmg = petCache.GetSkillValue();

                //if (enemycon.enemyType != EnemyType.Boss)
                //{
                //    dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                //}
                //else
                //{
                //    dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                //}

                enemycon.DecreaseHp(dmg, UserDmgType.PetSkill);

                var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.lightningSpearEffect, null, enemycon._view.hitPos.position);
                _hitEffect.On();

                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);

                isActive = false;
                this.gameObject.SetActive(false);

                InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_0);
            }
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        transform.Translate(dir * Time.deltaTime * (float)movespeed * 5);

        var distance = Vector2.Distance(this.transform.position, endPos);
        if (distance <= 0.3f)
        {
            currentTime = 0;

            particle.Stop();
            particle.gameObject.SetActive(false);
            
            isActive = false;
        }
    }
}
