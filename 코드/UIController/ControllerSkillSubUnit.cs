using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerSkillSubUnit : Character
    {
        public ViewSkillSubUnitSkill _view;
        public ControllerEnemyInGame target;
        public int animindex = 0;

        bool isActive;
        public ControllerSkillSubUnit(CancellationTokenSource cts) : base(cts)
        {
            isActive = false;

            _state = new StateMachine<eActorState>(true, cts);

            var idlestate = new CharacterIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var attackstate = new CharacterAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);
            var inactiveState = new CharacterInActive(this);
            _state.AddState(inactiveState.GetState(), inactiveState);

            _state.ChangeState(eActorState.InActive);
            _state.StateStop(false);

            Main().Forget();

            if (_view == null)
            {
                _view = Object.Instantiate(SkillResourcesBundle.Loaded._viewSkillSubUnit);
                _view.Init();
            }
            _view.gameObject.SetActive(false);

            Player.Skill.skillSubUnitObject = this;


            Battle.Field.UnitStop += UnitStopCallback;
            Battle.Field.UnitRestart += UnitRestartCallback;
        }

        void UnitStopCallback()
        {
    
        }
        void UnitRestartCallback()
        {

        }

        public void SetActive(bool active)
        {
            isActive = active;
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if(isActive)
                {
                    if (Battle.Field.IsFightScene)
                    {
                        if (_state.IsCurrentState(eActorState.Idle) || _state.IsCurrentState(eActorState.Attack))
                        {
                            var closedenemy = Battle.Enemy.GetClosedEnemyController(_view.transform, 7);
                            if (closedenemy != null)
                            {
                                target = closedenemy;
                                _state.ChangeState(eActorState.Attack);
                            }
                            else
                            {
                                target = null;
                                _state.ChangeState(eActorState.Idle);
                            }
                        }
                    }
                    else
                    {
                        target = null;
                        _state.ChangeState(eActorState.Idle);
                    }
                }
                await UniTask.Yield(_cts.Token);
            }
        }

        public void ActivateViewer()
        {
            isActive = true;
            if (_view == null)
            {
                _view = Object.Instantiate(SkillResourcesBundle.Loaded._viewSkillSubUnit);
                _view.Init();
            }
            _view.gameObject.SetActive(true);
            _view.transform.position = new Vector2(Player.Unit.userUnit._view.transform.position.x - 2.5f, Player.Unit.userUnit._view.transform.position.y + 1.5f);
            _state.ChangeState(eActorState.Idle);
        }

        public void InActivateViewer()
        {
            isActive = false;
            if(_view!=null)
            {
                _view.gameObject.SetActive(false);
            }
            
            _state.ChangeState(eActorState.InActive);
        }

        #region attack
        public class CharacterAttack : CharacterState
        {
            ControllerSkillSubUnit _unit;
            public CharacterAttack(ControllerSkillSubUnit unit)
            {
                _unit = unit;
            }

            public virtual void Attack()
            {
                var enemy = _unit.target._view;
                if (enemy != null)
                {
                    float skillvalue_0 = Player.Skill.Get(Definition.SkillKey.SummonSubunit).SkillValue(0);
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                    double dmg = Player.Unit.BowAtk*(skillvalue_0*0.01f) * (Player.Unit.GetSkillIncreaseValue());

                    ViewBulletForSoulUnit bullet = null;
                    bullet = PoolManager.Pop(InGameResourcesBundle.Loaded.bullet, null, _unit._view.transform.position);

                    bullet.Shoot(_unit._view.transform.position, _unit.target, dmg,Definition.WitchSkillType.SkillWitch,CriticalType.None);

                }
            }

            public override eActorState GetState()
            {
                return eActorState.Attack;
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
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Attack_0, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Attack_0, _unit.animindex);

                Vector2 dir = ((Vector2)_unit.target._view.transform.position - (Vector2)_unit._view.transform.position).normalized;

                float localscalex = Mathf.Abs(_unit._view.transform.localScale.x);
                float x = localscalex;
                if (dir.x >= 0)
                {
                    x = -localscalex;
                }
                else
                {
                    x = localscalex;
                }
                _unit._view.transform.localScale = new Vector2(x, _unit._view.transform.localScale.y);

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
        #region idle

        public class CharacterIdle : CharacterState
        {
            ControllerSkillSubUnit _unit;
            public CharacterIdle(ControllerSkillSubUnit unit)
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

        #region idle

        public class CharacterInActive : CharacterState
        {
            ControllerSkillSubUnit _unit;
            public CharacterInActive(ControllerSkillSubUnit unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                
            }

            protected override void OnExit()
            {
            }

            protected override void OnUpdate()
            {
             
            }
            public override eActorState GetState()
            {
                return eActorState.InActive;
            }
        }
        #endregion

    }

}
