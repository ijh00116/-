using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewExplodePoisonDamage : MonoBehaviour
{
    public ParticleSystem particle;
    [SerializeField] protected LayerMask layer;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    Player.Skill.SkillCacheData skillcache;
    float currentTime;
    float dirChangeTime=2.0f;

    ControllerEnemyInGame currentTarget;
    public void SetSkill(Player.Skill.SkillCacheData _skillcache)
    {
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        skillcache = _skillcache;
        currentTarget = Battle.Enemy.GetRandomEnemyController();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<ViewEnemy>();
        if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;

        if (enemy != null)
        {
            var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
            enemycon.PoisonSetting(true);
        }
    }
  
    private void Update()
    {
        if (this.gameObject.activeInHierarchy == false)
            return;
        if (currentTarget == null)
            return;

        Vector3 _dir = currentTarget._view.transform.position - this.transform.position;

        this.transform.Translate(_dir.normalized * Time.deltaTime*2*(1-skillcache.SkillValue(2)*0.01f));

        float distance = Vector3.Distance(this.transform.position, currentTarget._view.transform.position);

        if (distance <=0.3f ||currentTime>= dirChangeTime)
        {
            currentTime = 0;
            currentTarget = Battle.Enemy.GetRandomEnemyController();
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }


}