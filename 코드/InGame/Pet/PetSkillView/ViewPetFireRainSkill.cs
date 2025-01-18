using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPetFireRainSkill : MonoBehaviour
{
    public ParticleSystem particle;

    public ParticleSystem hitParticle;

    Vector3 dir = Vector3.down;
    float currentTime = 0;

    Vector2 endPos;
    [SerializeField] float movespeed = 2f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Pet.PetCacheData petCache;
    public void Fire(Vector2 _startPos, Vector2 _endPos,Player.Pet.PetCacheData _petCache)
    {
        isActive = true;
        endPos = _endPos;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        hitParticle.gameObject.SetActive(false);

        petCache = _petCache;

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

                enemycon.DecreaseHp(dmg, UserDmgType.PetSkill);

                hitParticle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
                hitParticle.transform.position = enemy.transform.position;
                hitParticle.Play();


                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);

                particle.Stop();
                particle.gameObject.SetActive(false);

                isActive = false;
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

            hitParticle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            hitParticle.transform.position = this.transform.position;
            hitParticle.Play();

            isActive = false;
        }
    }
}
