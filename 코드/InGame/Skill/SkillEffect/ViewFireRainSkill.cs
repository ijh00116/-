using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFireRainSkill : MonoBehaviour
{
    public ParticleSystem[] particle;

    public ParticleSystem hitParticle;

    Vector3 dir = Vector3.down;
    float currentTime = 0;

    Vector2 endPos;
    [SerializeField] float movespeed = 2f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;
    float skillValue_0 = 0;
    float skillValue_2 = 0;
    BlackTree.Definition.WitchSkillType witchSkilltype;

    Player.Skill.SkillCacheData subunitskillcache;
    public void Fire(Vector2 _startPos,Vector2 _endPos,float _skillvalue_0, float _skillvalue_2,BlackTree.Definition.WitchSkillType _witchSkillType)
    {
        isActive = true;
        endPos = _endPos;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        hitParticle.gameObject.SetActive(false);
        for (int i = 0; i < particle.Length; i++)
        {
            particle[i].Play();
            particle[i].gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            //particle[i].gameObject.SetActive(true);
        }

        skillValue_0 = _skillvalue_0;
        skillValue_2 = _skillvalue_2;

        witchSkilltype = _witchSkillType;

        if (subunitskillcache == null)
            subunitskillcache = Player.Skill.Get(BlackTree.Definition.SkillKey.SummonSubunit);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                double dmg = (double)Player.Unit.BowAtk*(skillValue_0 * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                if (enemycon.enemyType != EnemyType.Boss)
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                }
                else
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                }

                if (witchSkilltype ==BlackTree.Definition.WitchSkillType.Witch)
                {
                    enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);
                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.FireRain] += dmg;
                }
                else
                {
                    dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                    enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                }
                hitParticle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
                hitParticle.transform.position = enemy.transform.position;
                hitParticle.Play();

                enemycon.SlowSetting(true, skillValue_2);

                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);

                for (int i = 0; i < particle.Length; i++)
                {
                    particle[i].Stop();
                    particle[i].gameObject.SetActive(false);
                }
                isActive = false;
            }
        }
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        transform.Translate(dir * Time.deltaTime * (float)movespeed * 5);

        var distance = Vector2.Distance(this.transform.position, endPos);
        if (distance <=0.3f)
        {
            currentTime = 0;
            for(int i=0; i< particle.Length; i++)
            {
                particle[i].Stop();
                particle[i].gameObject.SetActive(false);
            }

            hitParticle.gameObject.SetActive(true);
            hitParticle.transform.position =this.transform.position;
            hitParticle.Play();

            isActive = false;
        }
    }
}
