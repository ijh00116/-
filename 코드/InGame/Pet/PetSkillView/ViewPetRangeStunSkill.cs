using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree;
using BlackTree.Core;
using BlackTree.Definition;

public class ViewPetRangeStunSkill : MonoBehaviour
{
    public ParticleSystem particle;
    public Collider2D collider;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);


    float currentTime = 0;

    Player.Pet.PetCacheData petCache;
    public void Init(Player.Pet.PetCacheData _petCache)
    {
        petCache = _petCache;
        collider.enabled = true;
        this.gameObject.SetActive(true);
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        if (Player.Cloud.optiondata.appearEffect)
        {
            particle.Play();
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
                double dmg = petCache.GetSkillValue();

                enemycon.DecreaseHp(dmg, UserDmgType.PetSkill);

                enemycon.SetStunState(1);
                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
            }
        }
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= 0.1f)
            {
                collider.enabled = false;
            }
            if (currentTime >= 1.5f)
            {
                currentTime = 0;
                this.gameObject.SetActive(false);
            }
        }
    }
}
