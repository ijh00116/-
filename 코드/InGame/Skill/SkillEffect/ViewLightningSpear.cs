using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewLightningSpear : MonoBehaviour
{
    public ParticleSystem[] particle;
    Vector3 dir = new Vector3(1,-1,0);
    float currentTime = 0;

    Vector2 endPos;
    [SerializeField] float movespeed = 5f;

    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    public bool isActive = false;

    Player.Skill.SkillCacheData skillcache;

    WitchSkillType witchSkilltype;

    Player.Skill.SkillCacheData subunitskillcache;
    public void Fire(Vector2 _startPos, Vector2 _endPos, Player.Skill.SkillCacheData _skillcache,WitchSkillType witchtype)
    {
        isActive = true;
        endPos = _endPos;
        skillcache = _skillcache;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        for (int i = 0; i < particle.Length; i++)
        {
            particle[i].Play();
            particle[i].gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        }

        witchSkilltype = witchtype;

        if (subunitskillcache == null)
            subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            var enemy = collision.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                double dmg = (double)Player.Unit.BowAtk*(skillcache.SkillValue(0)* 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                if (enemycon.enemyType != EnemyType.Boss)
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                }
                else
                {
                    dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                }

                if (witchSkilltype == WitchSkillType.Witch)
                {
                    enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.LightningForSeconds] += dmg;
                }
                else
                {
                    dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                    enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
                }


                if(enemycon.hp>0&&skillcache.userSkilldata.AwakeLv>=1)
                {
                    enemycon.SetStunState(skillcache.SkillValue(1));
                }

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
            for (int i = 0; i < particle.Length; i++)
            {
                particle[i].Stop();
                particle[i].gameObject.SetActive(false);
            }

            isActive = false;
        }
    }
}
