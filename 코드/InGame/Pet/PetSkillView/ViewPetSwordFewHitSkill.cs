using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewPetSwordFewHitSkill : MonoBehaviour
    {
        public ParticleSystem particle;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        double dmg = 0;

        [SerializeField] GameObject target;

        [SerializeField] Collider2D mycol;
        [ContextMenu("Target")]


        public void Activate(ViewEnemy _enemy,Player.Pet.PetCacheData petCache)
        {
            particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            currentTime = 0;

            if (_enemy != null)
            {
                target = _enemy.gameObject;
                Vector3 dir = target.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

                dmg = petCache.GetSkillValue();

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

                    enemycon.DecreaseHp(dmg, UserDmgType.PetSkill);

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
                if (currentTime >= 1.0f)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

}
