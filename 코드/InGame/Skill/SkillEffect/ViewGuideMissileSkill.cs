using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

public class ViewGuideMissileSkill : MonoBehaviour
{
    [SerializeField] protected LayerMask layer;
    float currentTime;
    Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

    private int dmgCount=5;
    private int currentCount =0;
    private Rigidbody2D rb;

    private BlackTree.Core.ControllerEnemyInGame target;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotateSpeed = 200f;

    const float normalDmg = 100f;
    public ParticleSystem particle;
    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    Player.Skill.SkillCacheData skillCache;
    float skillValue_1;
    float skillValue_2;
    public void Init( float _skillValue_1, float _skillValue_2)
    {
        skillCache = Player.Skill.Get(BlackTree.Definition.SkillKey.GuidedMissile);

        skillValue_1 = _skillValue_1;
        skillValue_2= _skillValue_2;

        dmgCount = 5 + (int)skillValue_1;

        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
    }
    WitchSkillType witchSkilltype;

    Player.Skill.SkillCacheData subunitskillcache;
    public void Shoot(Vector2 start, WitchSkillType _witchSkilltyep)
    {
        currentCount = 0;

        transform.position = start;

        var _targetcontroller = Battle.Enemy.GetRandomEnemyController();
        target = _targetcontroller;

        witchSkilltype = _witchSkilltyep;

        if (subunitskillcache == null)
            subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);
    }

    private Vector2 distance;

    private void FixedUpdate()
    {
        if(target!=null)
        {
            Vector2 dir = (Vector2)target._view.transform.position - rb.position;

            dir.Normalize();

            float rotateAmount = Vector3.Cross(dir, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;

            rb.velocity = transform.up * speed;

            if (target._state.IsCurrentState(BlackTree.Core.eActorState.Die) || target._state.IsCurrentState(BlackTree.Core.eActorState.InActive))
            {
                var _targetcontroller = Battle.Enemy.GetRandomEnemyController();
                if (_targetcontroller != null)
                    target = _targetcontroller;
            }

        }
        
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<ViewEnemy>();
        if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;
        
        if (enemy != null)
        {
            var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
            double dmg = Player.Unit.BowAtk*( (normalDmg + skillValue_2)*0.01f) * (Player.Unit.GetSkillIncreaseValue());

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
                Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.GuidedMissile] += dmg;
            }
            else
            {
                dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                enemycon.DecreaseHp(dmg, UserDmgType.SkillMissile);

                Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.SummonSubunit] += dmg;
            }
            //AudioManager.Instance.Play(AudioSourceKey.Missile_Hit);

            if (currentCount < dmgCount)
            {
                currentCount++;
                var _targetcontroller = Battle.Enemy.GetRandomEnemyController();
                target = _targetcontroller;

            }
            else
            {
                this.gameObject.SetActive(false);
            }



            WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
        }
    }
}
