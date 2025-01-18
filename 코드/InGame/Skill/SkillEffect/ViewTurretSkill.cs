using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTurretSkill : MonoBehaviour
{
    private BlackTree.Core.ControllerEnemyInGame target;
    ViewTurretSkill _bullet;

    Player.Skill.SkillCacheData skillCache;
    float skillValue_0;
    float skillValue_2;

    float currentTime = 0;
    const float delayFire= 0.9f;

    [SerializeField]ViewBulletForTurretSkill viewBullet;

    [SerializeField] float rotateSpeed= 200f;
    [SerializeField] float moveSpeed=5f;
    [SerializeField] ParticleSystem particle;

    Vector3 rotatedir;
    public void Init(float _skillValue_0, float _skillValue_2)
    {
        skillCache = Player.Skill.Get(BlackTree.Definition.SkillKey.SetTurret);

        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);

        skillValue_0 = _skillValue_0;
        skillValue_2 = _skillValue_2;
        currentTime = 0;

        int _random = Random.Range(0, 2);
        rotatedir = Vector3.forward;
        if (_random==0)
        {
            rotatedir = Vector3.back;
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(Player.Unit.userUnit._view.transform.position, this.transform.position);
        if (distance > 5)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.transform.position - Player.Unit.userUnit._view.transform.position), this.rotateSpeed * Time.deltaTime);
            this.transform.position += -this.transform.forward * this.moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.RotateAround(Player.Unit.userUnit._view.transform.position, rotatedir, Time.deltaTime*2);
        }
    
        //
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy == false)
            return;
        currentTime += Time.deltaTime;
        if(currentTime>= delayFire*(1-skillValue_2*0.01f))
        {
            currentTime = 0;
            var enemycon = Battle.Enemy.GetRandomEnemyController();
            if(enemycon!=null)
            {
                ViewBulletForTurretSkill bullet = null;
                bullet = PoolManager.Pop(viewBullet, InGameObjectManager.Instance.transform, this.transform.position, 10);
                double dmg = (double)Player.Unit.BowAtk* (skillValue_0 * 0.01f) * (Player.Unit.GetSkillIncreaseValue());
                bullet.Shoot(this.transform.position, enemycon, dmg);
            }
            
        }
    }
}
