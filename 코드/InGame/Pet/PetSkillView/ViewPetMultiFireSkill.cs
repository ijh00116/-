using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewPetMultiFireSkill : MonoBehaviour
    {
        public ParticleSystem particle;
        double currentDamage;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        [SerializeField] float movespeed = 2f;

        Player.Pet.PetCacheData petCache;
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

                        WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
                    }
                }
            }
        }

        Vector3 dir = Vector3.up;
        float currentTime = 0;
        float skillValue = 0;
        Definition.WitchSkillType witchSkilltype;

        public void MoveToArr(Vector3 _dir, Player.Pet.PetCacheData _petcache)
        {
            particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            currentTime = 0;
            dir = _dir;

            petCache = _petcache;
        }

        private void Update()
        {
            transform.Translate(dir * Time.deltaTime * (float)movespeed * 5);
            currentTime += Time.deltaTime;
            if (currentTime >= 2)
            {
                currentTime = 0;
                particle.Stop();
                this.gameObject.SetActive(false);
            }
        }
    }

}
