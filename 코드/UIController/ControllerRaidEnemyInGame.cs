using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;
using Pathfinding;
using DG.Tweening;

namespace BlackTree.Core
{
    public class ControllerRaidEnemyInGame : ControllerEnemyInGame
    {
        public ControllerRaidEnemyInGame(ViewEnemy view, CancellationTokenSource cts) : base(view, cts)
        {

        }

        public override void Init()
        {
            base.Init();

            _state.ChangeState(eActorState.Idle);
        }


        public override void DecreaseHp(double increasehpValue, UserDmgType dmgType)
        {
            Battle.Raid.currentRaidDungeonDMG += increasehpValue;
        }

        public override async UniTaskVoid Main()
        {
            while (true)
            {
                await UniTask.Yield(_cts.Token);

                if (_state.IsCurrentState(eActorState.Die) == false && _state.IsCurrentState(eActorState.InActive) == false)
                {
                    bool canattack = false;
                    var colliderObject = _view.triggeredUnit;
                    if (_view.isMelee)
                    {
                        if (colliderObject != null)
                        {
                            canattack = true;
                        }
                    }
                    else
                    {
                        canattack = IsInDistanceWithPlayer();
                    }

                    if (_state.IsCurrentState(eActorState.Stun) == false && _state.IsCurrentState(eActorState.Freeze) == false)
                    {
                        if (canattack)
                        {
                            _state.ChangeState(eActorState.Attack);
                        }
                        else
                        {
                            if (Vector2.Distance(Player.Unit.userUnit._view.transform.position, _view.transform.position) < AgroDistance)
                            {
                                _state.ChangeState(eActorState.Move);
                            }
                            else
                            {
                                _state.ChangeState(eActorState.Idle);
                            }
                        }
                    }
                    else
                    {

                    }
                }

                if (isPoisoned)
                {
                    poisonDmgTickTimer += Time.deltaTime;
                    if (poisonDmgTickTimer >= 0.7f)
                    {
                        var skilldata = Player.Skill.Get(SkillKey.ExplodePoisonCloud);
                        var dmg = Player.Unit.SwordAtk * (skilldata.SkillValue(0) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                        if (enemyType != EnemyType.Boss)
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                        }
                        else
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                        }

                        DecreaseHp(dmg, UserDmgType.SkillRange);
                        Player.Skill.skillDamageList[(int)Definition.SkillKey.ExplodePoisonCloud] += dmg;

                        WorldUIManager.Instance.InstatiateFont(this._view.transform.position, dmg, false, false, skillColor);
                        poisonDmgTickTimer = 0;
                    }

                    poisonCurrentTime += Time.deltaTime;
                    if (poisonCurrentTime >= maxPoisonTime)
                    {
                        isPoisoned = false;
                        _view.characterRenderer.color = Color.white;
                        poisonCurrentTime = 0;
                    }

                }

                if (ismeteorFire)
                {
                    meteorFireDmgTickTimer += Time.deltaTime;
                    if (meteorFireDmgTickTimer >= 0.5f)
                    {
                        var skilldata = Player.Skill.Get(SkillKey.SpawnMeteor);
                        var dmg = Player.Unit.BowAtk * (skilldata.SkillValue(0, 1) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());

                        if (enemyType != EnemyType.Boss)
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                        }
                        else
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                        }

                        if (witchskillType == Definition.WitchSkillType.Witch)
                        {
                            DecreaseHp(dmg, UserDmgType.SkillRange);
                            Player.Skill.skillDamageList[(int)Definition.SkillKey.SpawnMeteor] += dmg;
                        }
                        else
                        {
                            var subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);
                            dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                            DecreaseHp(dmg, UserDmgType.SkillRange);

                            Player.Skill.skillDamageList[(int)Definition.SkillKey.SummonSubunit] += dmg;
                        }

                        WorldUIManager.Instance.InstatiateFont(this._view.transform.position, dmg, false, false, skillColor);
                        meteorFireDmgTickTimer = 0;
                    }

                    meteorFireCurrentTime += Time.deltaTime;
                    if (meteorFireCurrentTime >= maxmeteorFire)
                    {
                        ismeteorFire = false;
                        _view.characterRenderer.color = Color.white;
                        meteorFireCurrentTime = 0;
                    }

                }
                if (isSlow)
                {
                    slowCurrentTime += Time.deltaTime;
                    if (slowCurrentTime >= maxSlowTime)
                    {
                        isSlow = false;
                        _view.characterRenderer.color = Color.white;
                        slowCurrentTime = 0;
                    }
                }

                if (isHaveBomb)
                {
                    BombCurrentTime += Time.deltaTime;
                    if (BombCurrentTime >= BombMaxTime)
                    {
                        isHaveBomb = false;
                        BombCurrentTime = 0;
                        bombUIObj.gameObject.SetActive(false);

                        ViewTimeBomb bombeffect = PoolManager.Pop(SkillResourcesBundle.Loaded.timeBombObj, null, this._view.hitPos.position);
                        bombeffect.FireStart(true);
                    }
                }

                _state.StateStop(Battle.Field.enemyActivePause);
                if (_state.stop)
                {
                    await UniTask.WaitUntil(() => Battle.Field.enemyActivePause == false);
                    _state.StateStop(Battle.Field.enemyActivePause);
                }

            }
        }

    }

}

