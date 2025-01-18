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
    public class ControllerPetUnitInGame:Character
    {
        public Player.Pet.PetCacheData petCache=null;
        public ViewPet _view;
        
        int petID=-1;
        int equipSlotIndex = -1;
        public int animindex = 0;

        public ControllerEnemyInGame targetController;
        Transform target;

        const int defaultMoveSpeed = 6;
        public ControllerPetUnitInGame(Transform parent,CancellationTokenSource cts,int _petID,int equipIndex):base(cts)
        {
            petID = _petID;
            equipSlotIndex = equipIndex;
            
            _view = UnityEngine.Object.Instantiate(PetResourcesBundle.Loaded.viewPetPrefab);
            _view.transform.SetParent(InGameObjectManager.Instance.transform, false);
            _view.gameObject.SetActive(false);
            _view.Init(this);

            if (petID != -1)
            {
                petCache = Player.Pet.Get(petID);
            }
            else
            {
                petCache = null;
            }

            _state = new StateMachine<eActorState>(true, cts);

            var idlestate = new CharacterIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var movestate = new CharacterMove(this);
            _state.AddState(movestate.GetState(), movestate);
            var inactivestate = new CharacterInActive(this);
            _state.AddState(inactivestate.GetState(), inactivestate);
            var attackstate = new CharacterAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);

            var _swordstate = new PetSwordFewHitSkill(this);
            _state.AddState(_swordstate.GetState(), _swordstate);
            var _fireRainstate = new PetFireRainSkill(this);
            _state.AddState(_fireRainstate.GetState(), _fireRainstate);
            var _multiFirestate = new PetMultiFireSkill(this);
            _state.AddState(_multiFirestate.GetState(), _multiFirestate);
            var _lightningstate = new PetLightningSkill(this);
            _state.AddState(_lightningstate.GetState(), _lightningstate);
            var _fireBallstate = new PetBigFireBallSkill(this);
            _state.AddState(_fireBallstate.GetState(), _fireBallstate);
            var _sunLightstate = new PetSunLightSkill(this);
            _state.AddState(_sunLightstate.GetState(), _sunLightstate);

            var _multiLightningstate = new PetMultiLightningSkill(this);
            _state.AddState(_multiLightningstate.GetState(), _multiLightningstate);

            var _rangeStunstate = new PetRangeStunSkill(this);
            _state.AddState(_rangeStunstate.GetState(), _rangeStunstate);
            var _shieldstate = new PetShieldRecoverSkill(this);
            _state.AddState(_shieldstate.GetState(), _shieldstate);
            var _atkIncreasestate = new PetAtkIncreaseSkill(this);
            _state.AddState(_atkIncreasestate.GetState(), _atkIncreasestate);
            var _magicatkIncreasestate = new PetMagicAtkIncreaseSkill(this);
            _state.AddState(_magicatkIncreasestate.GetState(), _magicatkIncreasestate);
            var _dmgDecreasestate = new PetDmgDecreaseSkill(this);
            _state.AddState(_dmgDecreasestate.GetState(), _dmgDecreasestate);


            _state.StateStop(false);

            if (petID != -1)
            {
                _view.gameObject.SetActive(true);
                _view.transform.position = Player.Unit.userUnit._view.transform.position;
                _state.ChangeState(eActorState.Idle);
            }
            else
            {
                _state.ChangeState(eActorState.InActive);
            }


            Main().Forget();

            Battle.Field.UnitStop += UnitStopCallback;
            Battle.Field.UnitRestart += UnitRestartCallback;
            Battle.Field.PetPositionDecide += PositionDecide;

            Player.Pet.onAfterEquip += EquipSync;
            Player.Pet.changePetPresetUpdate += UpdateChangePreset;
        }

        void EquipSync(int _petID)
        {
            var equipPet= Player.Pet.Get(_petID);
            if (equipPet.EquipedIndex != equipSlotIndex)
                return;

            petID = _petID;
            if (petID != -1)
            {
                petCache = Player.Pet.Get(petID);
                _view.gameObject.SetActive(true);
                _view.transform.position = Battle.Field.GetStage().enemyPos[(int)Random.Range(0, Battle.Field.GetStage().enemyPos.Length)].position;
                _state.ChangeState(eActorState.Idle);
            }
        }

        void UpdateChangePreset()
        {
            int index = 0;
            foreach (var _petID in Player.Pet.currentPetContainer())
            {
                if(index== equipSlotIndex)
                {
                    petID = _petID;
                    if (petID != -1)
                    {
                        petCache = Player.Pet.Get(petID);
                        _view.gameObject.SetActive(true);
                        _view.transform.position = Battle.Field.GetStage().enemyPos[(int)Random.Range(0, Battle.Field.GetStage().enemyPos.Length)].position;
                        _state.ChangeState(eActorState.Idle);
                    }
                    else
                    {
                        _view.gameObject.SetActive(false);
                    }
                }
                index++;
            }
        }
        void PositionDecide()
        {
            _view.transform.position = Battle.Field.GetStage().enemyPos[(int)Random.Range(0, Battle.Field.GetStage().enemyPos.Length)].position;
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Battle.Field.IsFightScene)
                {
                    var colliderObject = _view.triggeredEnemy;
                    if(colliderObject!=null)
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

                    
                }

                if (_view.triggeredEnemy!=null)
                {
                    if (_view.triggeredEnemy._state.IsCurrentState(eActorState.Die) || _view.triggeredEnemy._state.IsCurrentState(eActorState.InActive)
                        || _view.triggeredEnemy._view.gameObject.activeInHierarchy == false)
                    {
                        _view.triggeredEnemy = null;
                    }
                }
                if (target!= null)
                {
                    if (targetController._state.IsCurrentState(eActorState.Die) || targetController._state.IsCurrentState(eActorState.InActive)
                        || target.gameObject.activeInHierarchy == false)
                    {
                        target = null;
                    }
                }

                //skill
                if (Battle.Field.IsFightScene && petCache!=null)
                {
                    if (_state.IsCurrentState(eActorState.Move) || _state.IsCurrentState(eActorState.Idle) || _state.IsCurrentState(eActorState.Attack))
                    {
                        var indexes = Player.Pet.GetAllRegisterSkillInput();
                        Definition.PetSkillKey _skillhashkey = Definition.PetSkillKey.None;
                        foreach (var index in indexes)
                        {
                            var catkey = (Definition.PetSkillKey)index;
                            if (catkey != Definition.PetSkillKey.None)
                            {
                                _skillhashkey = catkey;
                                break;
                            }
                        }
                        if (_skillhashkey != Definition.PetSkillKey.None && _skillhashkey==petCache.tabledata.petskillKey)
                        {
                            switch (_skillhashkey)
                            {
                                case PetSkillKey.None:
                                    break;
                                case PetSkillKey.SwordFewHit:
                                    _state.ChangeState(eActorState.PetSwordFewHit);
                                    break;
                                case PetSkillKey.FireRain:
                                    _state.ChangeState(eActorState.PetFireRain);
                                    break;
                                case PetSkillKey.FireMultiShot:
                                    _state.ChangeState(eActorState.PetMultiFire);
                                    break;
                                case PetSkillKey.LightningSpear:
                                    _state.ChangeState(eActorState.PetLightningSpear);
                                    break;
                                case PetSkillKey.BigFireBall:
                                    _state.ChangeState(eActorState.PetBigFire);
                                    break;
                                case PetSkillKey.SunLight:
                                    _state.ChangeState(eActorState.PetSunLight);
                                    break;
                                case PetSkillKey.RangeStun:
                                    _state.ChangeState(eActorState.PetRangeStun);
                                    break;
                                case PetSkillKey.MultiLightning:
                                    _state.ChangeState(eActorState.PetMultiElectric);
                                    break;
                                case PetSkillKey.MagicAtkIncrease:
                                    _state.ChangeState(eActorState.PetMagicIncrease);
                                    break;
                                case PetSkillKey.ShieldRecover:
                                    _state.ChangeState(eActorState.PetShield);
                                    break;
                                case PetSkillKey.AtkIncrease:
                                    _state.ChangeState(eActorState.PetAtkIncrease);
                                    break;
                                case PetSkillKey.DmgDecrease:
                                    _state.ChangeState(eActorState.PetDmgDecrease);
                                    break;
                                default:
                                    break;
                            }

                            petCache.elapsedCooltime = 0;
                            Player.Pet.RemoveSkillInput((int)_skillhashkey);
                        }
                    }
                }
                await UniTask.Yield(_cts.Token);

                if (petCache != null)
                {
                    if (petCache.IsEquiped && petCache.IsSkillUnlocked && _state.stop == false)
                    {
                        if (petCache.tabledata.petskillKey != PetSkillKey.None)
                        {
                            petCache.elapsedCooltime += Time.deltaTime;
                        }

                        if (petCache.leftCooltime > 0)
                        {

                        }
                        else
                        {
                            if (Battle.Field.IsFightScene)
                            {
                                Player.Pet.RegisterSkillInput((int)petCache.tabledata.petskillKey);

                                Player.Cloud.petdata.UpdateHash().SetDirty(true);
                            }
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

        void UnitStopCallback()
        {
            _state.ChangeState(eActorState.Idle);
            _view.triggeredEnemy = null;
            target = null;
        }
        void UnitRestartCallback()
        {
            _view.triggeredEnemy = null;
            target = null;
        }


        #region idle

        public class CharacterIdle : CharacterState
        {
            ControllerPetUnitInGame _unit;
            public CharacterIdle(ControllerPetUnitInGame unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(100);
                _unit.animindex = 0;
                _unit._view.ai.canMove = false;
            }

            protected override void OnExit()
            {
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Idle,_unit.petID ,_unit.animindex) == false)
                {
                    _unit.animindex = 0;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Idle, _unit.petID, _unit.animindex);
            }
            public override eActorState GetState()
            {
                return eActorState.Idle;
            }
        }
        #endregion

        #region Run

        public class CharacterMove : CharacterState
        {
            ControllerPetUnitInGame _unit;
            
            public CharacterMove(ControllerPetUnitInGame unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(100);
                _unit.animindex = 0;
                _unit._view.ai.destination = _unit.target.position;
                _unit._view.ai.canMove = true;
            }

            protected override void OnExit()
            {
                _unit._view.ai.canMove = false;
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Move, _unit.petID, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Move, _unit.petID, _unit.animindex);

                if (_unit.target == null)
                    return;
                _unit._view.ai.destination = _unit.target.position;

                Vector2 dir = ((Vector2)_unit.target.position - (Vector2)_unit._view.transform.position).normalized;

                float localscalex = Mathf.Abs(_unit._view.spriteTransform.localScale.x);
                float x = localscalex;
                if (dir.x >= 0)
                {
                    x = localscalex;
                }
                else
                {
                    x = -localscalex;
                }
                _unit._view.spriteTransform.localScale = new Vector2(x, _unit._view.spriteTransform.localScale.y);

                if(_unit.petCache!=null)
                {
                    _unit._view.ai.maxSpeed = defaultMoveSpeed * _unit.petCache.tabledata.moveSpeed;
                }
                
            }
            public override eActorState GetState()
            {
                return eActorState.Move;
            }
        }
        #endregion

        #region Run

        public class CharacterInActive : CharacterState
        {
            ControllerPetUnitInGame _unit;
            public CharacterInActive(ControllerPetUnitInGame unit)
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
                
            }
            public override eActorState GetState()
            {
                return eActorState.InActive;
            }
        }
        #endregion

        #region attack
        public class CharacterAttack : CharacterState
        {
            ControllerPetUnitInGame _unit;
            int attackSpeedFrame = 60;
            int attackCountForFewhitFireSkill = 0;
            int attackCountForCompanionSkill = 0;
            public CharacterAttack(ControllerPetUnitInGame unit)
            {
                _unit = unit;
            }

            bool targetDie;
            List<ViewPowerFullGodAttack> godAttackObjList = new List<ViewPowerFullGodAttack>();

            public virtual void Attack()
            {
                targetDie = false;
                if (_unit.target == null)
                {
                    targetDie = true;
                    return;
                }
                
                var enemy = _unit.target.GetComponent<ViewEnemy>();
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                    if (enemycon == null)
                        return;

                    double dmg = 0;
                    
                    if(_unit.petCache.tabledata.isMelee)
                    {
                        dmg=(_unit.petCache.atkRate*0.01f)* Player.Unit.SwordAtk;
                    }
                    else
                    {
                        dmg = (_unit.petCache.atkRate * 0.01f) * Player.Unit.BowAtk;
                    }
                    dmg= dmg * (Player.Unit.IncreasePetAtk);



                    enemycon.DecreaseHp(dmg, UserDmgType.Pet);
                    if (enemycon.hp <= 0)
                    {
                        targetDie = true;
                    }

                    enemycon._view.SetHitEffectOn();
                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemy.hitPos.position);
                    _hitEffect.On();

                    WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, Color.white);
                }
            }

            public override eActorState GetState()
            {
                return eActorState.Attack;
            }

            UnitAnimSprtieType attackanimType;
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;
                attackanimType = UnitAnimSprtieType.Attack_0;
            }

            protected override void OnExit()
            {
            }
            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(attackanimType, _unit.petID, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                    if (targetDie)
                    {
                        _unit._state.ChangeState(eActorState.Idle);
                    }
                }

                if (_unit.target == null)
                {
                    targetDie = true;
                    _unit._state.ChangeState(eActorState.Idle);
                    return;
                }
                    

                _unit._view.SetSpriteImage(attackanimType, _unit.petID, _unit.animindex);

                Vector2 dir = ((Vector2)_unit.target.position - (Vector2)_unit._view.transform.position).normalized;

                float localscalex = Mathf.Abs(_unit._view.spriteTransform.localScale.x);
                float x = localscalex;
                if (dir.x >= 0)
                {
                    x = localscalex;
                }
                else
                {
                    x = -localscalex;
                }
                _unit._view.spriteTransform.localScale = new Vector2(x, _unit._view.spriteTransform.localScale.y);

                if(_unit.animindex == _unit._view.attackEventFrame)
                {
                    Attack();
                }

                if (Vector2.Distance(_unit._view.transform.position, _unit.target.position) > 2)
                {
                    _unit._view.triggeredEnemy = null;
                    _unit._state.ChangeState(eActorState.Idle);
                }
            }
        }
        #endregion

    }

}
