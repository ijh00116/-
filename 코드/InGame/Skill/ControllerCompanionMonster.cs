using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using Pathfinding;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerCompanionMonster : Character
    {
        public ViewCompanionMonster _view;
        public Transform target;
        public ControllerEnemyInGame targetController;
        public int animindex = 0;
        public IAstarAI ai;

        public Player.Skill.SkillCacheData skillCache;
        float liveTime = 0;
        float currentTime = 0;
        const float defaultLiveTime= 5;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);
        public ControllerCompanionMonster(Transform parent, CancellationTokenSource cts) : base(cts)
        {
            if (_view == null)
            {
                _view = Object.Instantiate(InGameResourcesBundle.Loaded.viewCompanion, parent);
            }

            ai = _view.GetComponent<IAstarAI>();


            _view.Init(this);
            _view.gameObject.SetActive(false);

            _state = new StateMachine<eActorState>(true, cts);

            var idlestate = new CharacterIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var movestate = new CharacterMove(this);
            _state.AddState(movestate.GetState(), movestate);
            var attackstate = new CharacterAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);
            var diestate = new CharacterDie(this);
            _state.AddState(diestate.GetState(), diestate);
            var inActivestate = new CharacterInActive(this);
            _state.AddState(inActivestate.GetState(), inActivestate);

            _state.ChangeState(eActorState.Idle);

            _state.StateStop(false);

            Main().Forget();
        }

        public void Activate(Player.Skill.SkillCacheData _skillCache,Transform spawnPos)
        {
            if(spawnPos==null)
            {
                _view.transform.position = Player.Unit.userUnit._view.transform.position;
            }
            else
            {
                _view.transform.position = spawnPos.position;
            }
            
            _view.gameObject.SetActive(true);
            _view.spawnParticle.Play();

            skillCache = _skillCache;
            liveTime = _skillCache.SkillValue(1)+ defaultLiveTime;
            currentTime = 0;

            _state.ChangeState(eActorState.Idle);

        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Battle.Field.IsFightScene)
                {
                    var colliderObject = _view.triggeredEnemy;
                    if (colliderObject != null)
                    {
                        if (_state.IsCurrentState(eActorState.Move) || _state.IsCurrentState(eActorState.Idle))
                        {
                            target = colliderObject._view.gameObject.transform;
                            targetController = Battle.Enemy.GetHashEnemyController(target.GetComponent<ViewEnemy>().hash);
                            _state.ChangeState(eActorState.Attack);
                        }
                    }
                    else
                    {
                        //이동의 상태변환이 스킬상태변환을 막음
                        if (_state.IsCurrentState(eActorState.Move) || _state.IsCurrentState(eActorState.Idle))
                        {
                            var enemy = Battle.Enemy.GetClosedEnemyController(_view.transform);

                            if (enemy != null)
                            {
                                target = enemy._view.transform;
                                targetController = Battle.Enemy.GetHashEnemyController(target.GetComponent<ViewEnemy>().hash);
                                _state.ChangeState(eActorState.Move);
                            }
                            else
                            {
                                target = null;
                                _view.triggeredEnemy = null;
                                _state.ChangeState(eActorState.Idle);
                            }
                        }
                    }

                    currentTime += Time.deltaTime;
                    if(currentTime>=liveTime)
                    {
                        _state.ChangeState(eActorState.Die);
                    }
                }
                else
                {
                    _state.ChangeState(eActorState.Idle);
                }
                await UniTask.Yield(_cts.Token);

                if (_view.triggeredEnemy != null)
                {
                    if (_view.triggeredEnemy._state.IsCurrentState(eActorState.Die) || _view.triggeredEnemy._state.IsCurrentState(eActorState.InActive)
                        || _view.triggeredEnemy._view.gameObject.activeInHierarchy == false)
                    {
                        //Debug.Log("지금 적 업다");
                        _view.triggeredEnemy = null;
                    }
                    else
                    {
                        float distance = Vector3.Distance(_view.transform.position, _view.triggeredEnemy._view.transform.position);
                        if (distance >= 1.5f)
                        {
                            _view.triggeredEnemy = null;
                        }
                    }
                }
                

                _state.StateStop(Battle.Field.unitActivePause);
                if (_state.stop)
                {
                    await UniTask.WaitUntil(() => Battle.Field.unitActivePause == false);
                    _state.StateStop(Battle.Field.unitActivePause);
                }
            }
        }

        #region idle
        public class CharacterIdle : CharacterState
        {
            ControllerCompanionMonster _unit;
            public CharacterIdle(ControllerCompanionMonster unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;
            }

            protected override void OnExit()
            {
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Idle, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Idle, _unit.animindex);
            }
            public override eActorState GetState()
            {
                return eActorState.Idle;
            }
        }
        #endregion
        #region move

        public class CharacterMove : CharacterState
        {
            ControllerCompanionMonster _unit;
            Vector2 targetposition;
            public CharacterMove(ControllerCompanionMonster unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit.ai.canMove = true;
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;
                if (_unit._state.PreviousState != eActorState.Move)
                {

                }
                else
                {
                    //Debug.Log("이동애님 재생 안해!");
                }
            }

            protected override void OnExit()
            {
                _unit.ai.canMove = false;
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Move, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Move, _unit.animindex);

                _unit.ai.destination = _unit.target.position;

                float localscalex = Mathf.Abs(_unit._view.spriteTransform.localScale.x);
                float x = localscalex;

                var unitdir = _unit.ai.destination - _unit._view.spriteTransform.position;// _unit.ai.velocity;
                if (unitdir.x >= 0 || Mathf.Abs(unitdir.x) <= 0.1f)
                {
                    x = -localscalex;
                }
                else
                {
                    x = localscalex;
                }
                _unit._view.spriteTransform.localScale = new Vector2(x, _unit._view.spriteTransform.localScale.y);

            }
            public override eActorState GetState()
            {
                return eActorState.Move;
            }
        }
        #endregion
        #region Die

        public class CharacterDie : CharacterState
        {
            ControllerCompanionMonster _unit;
            public CharacterDie(ControllerCompanionMonster unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;
            
            }

            protected override void OnExit()
            {
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Die, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                    _unit._state.ChangeState(eActorState.InActive);
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Die, _unit.animindex);
            }
            public override eActorState GetState()
            {
                return eActorState.Die;
            }
        }
        #endregion

        public class CharacterInActive : CharacterState
        {
            ControllerCompanionMonster _unit;
            public CharacterInActive(ControllerCompanionMonster unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit._view.gameObject.SetActive(false);
            }

            protected override void OnExit()
            {
            }

            protected override void OnUpdate()
            {
                if (_unit._view.gameObject.activeInHierarchy)
                {
                    _unit._view.gameObject.SetActive(false);
                }

            }
            public override eActorState GetState()
            {
                return eActorState.InActive;
            }
        }

        #region attack
        public class CharacterAttack : CharacterState
        {
            ControllerCompanionMonster _unit;
            int attackSpeedFrame = 60;
            int attackCountForFewhitFireSkill = 0;
            public CharacterAttack(ControllerCompanionMonster unit)
            {
                _unit = unit;
            }

            bool targetDie;
            public virtual void Attack()
            {
                targetDie = false;
                var enemy = _unit.target.GetComponent<ViewEnemy>();
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                    if (enemycon == null)
                        return;
                    var skillcache = _unit.skillCache.SkillValue(0);
                    double dmg = Player.Unit.SwordAtk*(skillcache*0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }

                    enemycon.DecreaseHp(dmg, UserDmgType.SkillNormal);

                    Player.Skill.skillDamageList[(int)BlackTree.Definition.SkillKey.CompanionSpawn] += dmg;
                    if (enemycon.hp <= 0)
                    {
                        targetDie = true;
                    }
                    enemycon._view.SetHitEffectOn();
                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemy.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, _unit.skillColor);
                }
            }

            public override eActorState GetState()
            {
                return eActorState.Attack;
            }

            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(80);
                _unit.animindex = 0;
            }

            protected override void OnExit()
            {
            }
            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Attack_0, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                    if (targetDie)
                    {
                        _unit._state.ChangeState(eActorState.Idle);
                    }
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Attack_0, _unit.animindex);

                var animarray = _unit._view.animspriteinfo[UnitAnimSprtieType.Attack_0].eventFrame;
                for (int i = 0; i < animarray.Length; i++)
                {
                    if (animarray[i] == _unit.animindex)
                    {
                        Attack();
                    }
                }
            }
        }
        #endregion
    }

}
