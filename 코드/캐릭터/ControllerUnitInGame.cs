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
    [System.Serializable]
    public class AnimSpriteInfo
    {
        public UnitAnimSprtieType spriteType;
        public SpriteAnimInfo animInfo;
    }
    public enum UnitAnimSprtieType
    {
        Idle, Move,
        Attack_0, Attack_1, Attack_2,
        Skill_0, Skill_1, BuffSkill,
        Die,
        Stun,Recover,
    }

    public enum ShieldState
    {
        Idle,Recovering
    }
    public enum buffType
    {
        Attack, MoveSpeed, HpRecover, ShieldRecover,
    }

    [System.Serializable]
    public class SpriteAnimInfo
    {
        public int[] eventFrame;
        public Sprite[] spriteList;
    }
    public class ControllerUnitInGame:Character
    {
        public ViewUnit _view;
        public Transform target;
        public ControllerEnemyInGame targetController;
        public int animindex = 0;

        float skilluseGlobalCoolTime = 0.5f;
        float currentskillGlobalCooltime = 0.0f;

        public IAstarAI ai;

        ParticleSystem lvUpEffect_0;
        ParticleSystem lvUpEffect_1;

        public Vector3 normalScale;
        public Vector3 awakeScale;

        GameObject shieldObj; 
        //유저의 데이터(cloud.user:여기에는 레벨만 일단 있고 player.unit에서 생성된 정보 가져다가 세팅할것임)
        public ControllerUnitInGame(Transform parent, CancellationTokenSource cts):base(cts)
        {
            if (_view == null)
            {
                _view = Object.Instantiate(InGameResourcesBundle.Loaded.unit, parent);
            }

            ai = _view.GetComponent<IAstarAI>();

           
            _view.Init(this);
            _view.gameObject.SetActive(true);

            InGameObjectManager.Instance.proCam.AddCameraTarget(_view.transform);

            _state = new StateMachine<eActorState>( true, cts);

            var idlestate = new CharacterIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var movestate = new CharacterMove(this);
            _state.AddState(movestate.GetState(), movestate);
            var attackstate = new CharacterAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);
            var diestate= new CharacterDie(this);
            _state.AddState(diestate.GetState(), diestate);

            //스킬
            var powerattackstate = new CharacterPowerAttackSkill(this); 
            _state.AddState(powerattackstate.GetState(), powerattackstate);

            var explodeSkill= new CharacterMultipleMissileSkill(this); 
            _state.AddState(explodeSkill.GetState(), explodeSkill);

            var increaseatkSkill = new CharacterIncreaseAtkValueSkill(this); 
            _state.AddState(increaseatkSkill.GetState(), increaseatkSkill);

            var petmissiletkSkill = new CharacterFireRainSkill(this); 
            _state.AddState(petmissiletkSkill.GetState(), petmissiletkSkill);

            var shootwaveSkill = new CharacterGuideMissileSkill(this); 
            _state.AddState(shootwaveSkill.GetState(), shootwaveSkill);
          
            //7/24
            var alldmg = new CharacterExplodePoisonSkill(this);
            _state.AddState(alldmg.GetState(), alldmg);
            var moveBuff = new CharacterIncreaseMoveValueSkill(this);
            _state.AddState(moveBuff.GetState(), moveBuff);


            var poolingPortal = new CharacterSwordFewHitFireSkill(this);
            _state.AddState(poolingPortal.GetState(), poolingPortal);

            var absorbLife = new CharacterAbsorbLifeSkill(this); 
            _state.AddState(absorbLife.GetState(), absorbLife);
   
            var recoverTickHp = new CharacterRecoverTickHPSkill(this); 
            _state.AddState(recoverTickHp.GetState(), recoverTickHp);

            var callGodSkill = new CharacterCallGodSkill(this); 
            _state.AddState(callGodSkill.GetState(), callGodSkill);

            var stunskill = new CharacterRangeStun(this);
            _state.AddState(stunskill.GetState(), stunskill);

            var lightningskill = new CharacterLightningSpear(this);
            _state.AddState(lightningskill.GetState(), lightningskill);

            var novaSkill = new CharacterNovaSkill(this);
            _state.AddState(novaSkill.GetState(), novaSkill);

            var bigFireball = new CharacterBigFireBall(this);
            _state.AddState(bigFireball.GetState(), bigFireball);

            var shieldRecover = new CharacterRecoverShieldSkill(this);
            _state.AddState(shieldRecover.GetState(), shieldRecover);

            var summonsubUnit = new CharacterSubUnitSkill(this);
            _state.AddState(summonsubUnit.GetState(), summonsubUnit);

            var magicFewHit = new CharacterMagicFewHitFireSkill(this);
            _state.AddState(magicFewHit.GetState(), magicFewHit);

            var freezeSkill = new CharacterFreezeSkill(this);
            _state.AddState(freezeSkill.GetState(), freezeSkill);

            var turretSkill = new CharacterTurretSkill(this);
            _state.AddState(turretSkill.GetState(), turretSkill);

            var laserSkill = new CharacterLaserBeamSkill(this);
            _state.AddState(laserSkill.GetState(), laserSkill);

            var companionSkill = new CharacterCompanionSkill(this);
            _state.AddState(companionSkill.GetState(), companionSkill);

            var meteorSkill = new CharacterMeteorSkill(this);
            _state.AddState(meteorSkill.GetState(), meteorSkill);

            var chainLightning = new CharacterMultipleLightningSkill(this);
            _state.AddState(chainLightning.GetState(), chainLightning);

            var skyLight = new CharacterSkyLightSkill(this);
            _state.AddState(skyLight.GetState(), skyLight);

            var timeBomb = new CharacterTimebombSkill(this);
            _state.AddState(timeBomb.GetState(), timeBomb);
            //스킬

            _state.ChangeState(eActorState.Idle);

            _state.StateStop(false);

            Main().Forget();
            Player.Unit.userUnit = this;
            Battle.Field.UnitStop += UnitStopCallback;
            Battle.Field.UnitRestart += UnitRestartCallback;

            normalScale = _view.transform.localScale;
            awakeScale=new Vector3(_view.transform.localScale.x*1.5f, _view.transform.localScale.y*1.5f, _view.transform.localScale.z*1.5f);

            //soul unit setting
            Player.Unit.usersubUnit= new ControllerSubUnitInGame(parent, cts);

            Player.Unit.syncHpUI += SyncHp;

            lvUpEffect_0 = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.lvUpEffect_0);
            lvUpEffect_0.gameObject.SetActive(false);
            lvUpEffect_1 = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.lvUpEffect_1);
            lvUpEffect_1.gameObject.SetActive(false);
            Player.Level.levelupCallBack += LvupEvent;
            Player.Unit.buffUpdate += BuffIconUpdate;

            shieldObj = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.shieldObj);
            shieldObj.transform.SetParent(_view.transform);
            shieldObj.transform.localPosition = Vector3.zero;

            Player.Unit.shieldStateChangeCallback += ShieldStateChange;

            if (Player.Unit.Shield >= (Player.Unit.MaxShield / 2))
            {
                shieldObj.SetActive(true);
            }
            else
            {
                shieldObj.SetActive(false);
            }

            BuffIconUpdate();

            _view.textTypeWriter.onMessage.AddListener(TextTypeWritingEnd);

            dialogTime = 3;// Random.Range(minTime, maxTime);
            dialogUpdateType = DialogUpdateType.Wait;
            //CharacterDialogUpdate().Forget();
        }

        const float maxTime = 17;//120;
        const float minTime = 15;//40;
        float currentTime;
        float dialogTime;
        DialogUpdateType dialogUpdateType;
        int speechRandomIndex;
        int speechCurrentStepIndex;
        enum DialogUpdateType
        {
            Wait,
            Progress,
            EndGoWait,
        }
        async UniTaskVoid CharacterDialogUpdate()
        {
            while (true)
            {
                switch (dialogUpdateType)
                {
                    case DialogUpdateType.Wait:
                        currentTime += Time.deltaTime;
                        if (currentTime >= dialogTime)
                        {
                            _view.speechBubbleObj.SetActive(true);
                            speechRandomIndex = Random.Range(0, StaticData.Wrapper.speechBubbleData.Length);
                            speechCurrentStepIndex = 0;
                            LocalizeDescKeys descKey= StaticData.Wrapper.speechBubbleData[speechRandomIndex].speechBubbleArray[speechCurrentStepIndex];
                            string dialog =  StaticData.Wrapper.localizeddesclist[(int)descKey].StringToLocal+"<?endMsg>";
                            _view.speechBubleText.text = dialog;
                            await UniTask.Yield();
                            _view.textTypeWriter.ShowText(dialog);

                            dialogUpdateType = DialogUpdateType.Progress;
                        }
                        break;
                    case DialogUpdateType.Progress:
                        break;
                    case DialogUpdateType.EndGoWait:
                        currentTime += Time.deltaTime;
                        if(currentTime>=3.0f)
                        {
                            currentTime = 0;
                            dialogTime = Random.Range(minTime, maxTime);
                            dialogUpdateType = DialogUpdateType.Wait;
                            _view.speechBubbleObj.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
               
                await UniTask.Yield(_cts.Token);
            }
        }

        void TextTypeWritingEnd(Febucci.UI.Core.Parsing.EventMarker eventMarker)
        {
            switch (eventMarker.name)
            {
                case "endMsg":
                    //Debug.Log("대화 끝!");
                    if(StaticData.Wrapper.speechBubbleData[speechRandomIndex].speechBubbleArray.Length <= speechCurrentStepIndex+1)
                    {
                        currentTime = 0;
                        dialogUpdateType = DialogUpdateType.EndGoWait;
                    }
                    else
                    {
                        WaitAndNextDialog().Forget();
                   
                    }
                    break;
                default:
                    break;
            }
        }

        async UniTaskVoid WaitAndNextDialog()
        {
            await UniTask.Delay(3000);

            speechCurrentStepIndex++;
            LocalizeDescKeys descKey = StaticData.Wrapper.speechBubbleData[speechRandomIndex].speechBubbleArray[speechCurrentStepIndex];
            string dialog = StaticData.Wrapper.localizeddesclist[(int)descKey].StringToLocal + "<?endMsg>";
            _view.speechBubleText.text = dialog;
            await UniTask.Yield();
            _view.textTypeWriter.ShowText(dialog);

        }

        void ShieldStateChange(ShieldState shieldState)
        {
            switch (shieldState)
            {
                case ShieldState.Idle:
                    shieldObj.SetActive(true);
                    break;
                case ShieldState.Recovering:
                    shieldObj.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void SyncHp()
        {
            _view.SyncHpUI();
        }


        void LvupEvent()
        {
            lvUpEffect_0.gameObject.SetActive(true);
            lvUpEffect_0.Play();
            lvUpEffect_1.gameObject.SetActive(true);
            lvUpEffect_1.Play();

            lvUpEffect_0.transform.position =_view.transform.position;
            lvUpEffect_1.transform.position = _view.transform.position;

            AudioManager.Instance.Play(AudioSourceKey.LvupConglatulation);

            string localizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_LevelUp].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
        }
       
        public void BuffIconUpdate()
        {
            bool isatkbuff = Player.Unit.IsSkillActive(SkillKey.IncreaseAttack);
            bool ismovebuff = Player.Unit.IsSkillActive(SkillKey.IncreaseMoving);
            bool ishpbuff = Player.Unit.IsSkillActive(SkillKey.RecoverHpTick);
            bool isshieldbuff = Player.Unit.IsSkillActive(SkillKey.RecoverShield);
            bool isPetshieldbuff = Player.Pet.IsSkillActive(PetSkillKey.ShieldRecover);

            if (isatkbuff)
            {
                if(_view.buffObject[(int)buffType.Attack].activeInHierarchy==false)
                {
                    _view.buffObject[(int)buffType.Attack].SetActive(true);
                    _view.buffObjectParticle[(int)buffType.Attack].Play();
                }
            }
            else
            {
                _view.buffObject[(int)buffType.Attack].SetActive(false);
            }

            if (ismovebuff)
            {
                if (_view.buffObject[(int)buffType.MoveSpeed].activeInHierarchy == false)
                {
                    _view.buffObject[(int)buffType.MoveSpeed].SetActive(true);
                    _view.buffObjectParticle[(int)buffType.MoveSpeed].Play();
                }
            }
            else
            {
                _view.buffObject[(int)buffType.MoveSpeed].SetActive(false);
            }

            if (ishpbuff)
            {
                if (_view.buffObject[(int)buffType.HpRecover].activeInHierarchy == false)
                {
                    _view.buffObject[(int)buffType.HpRecover].SetActive(true);
                    _view.buffObjectParticle[(int)buffType.HpRecover].Play();
                }
            }
            else
            {
                _view.buffObject[(int)buffType.HpRecover].SetActive(false);
            }

            if (isshieldbuff || isPetshieldbuff)
            {
                if (_view.buffObject[(int)buffType.ShieldRecover].activeInHierarchy == false)
                {
                    _view.buffObject[(int)buffType.ShieldRecover].SetActive(true);
                    _view.buffObjectParticle[(int)buffType.ShieldRecover].Play();
                }
            }
            else
            {
                _view.buffObject[(int)buffType.ShieldRecover].SetActive(false);
            }

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
                        if (_state.IsCurrentState(eActorState.Move)|| _state.IsCurrentState(eActorState.Idle))
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
                }
                else
                {
                  
                }

                if (Battle.Field.IsFightScene)
                {
                    if (_state.IsCurrentState(eActorState.Move) || _state.IsCurrentState(eActorState.Idle) || _state.IsCurrentState(eActorState.Attack))
                    {
                        currentskillGlobalCooltime += Time.deltaTime;
                        //등록된 스킬 사용
                        var indexes = Player.Skill.GetAllRegisterSkillInput();
                        Definition.SkillKey _skillhashkey = Definition.SkillKey.None;
                        foreach (var index in indexes)
                        {
                            var catkey = (Definition.SkillKey)index;
                            if (catkey != Definition.SkillKey.None)
                            {
                                _skillhashkey = catkey;
                                break;
                            }
                        }
                        if (_skillhashkey != Definition.SkillKey.None)
                        {
                            currentskillGlobalCooltime = 0.0f;
                            switch (_skillhashkey)
                            {
                                case Definition.SkillKey.None:
                                    break;
                                case Definition.SkillKey.SwordExplode:
                                    _state.ChangeState(eActorState.SwordExplode);
                                    break;
                                case Definition.SkillKey.MultipleFireball:
                                    _state.ChangeState(eActorState.MultipleFireMissile);
                                    break;
                                case Definition.SkillKey.IncreaseAttack:
                                    _state.ChangeState(eActorState.IncreaseAttack);
                                    break;
                                case Definition.SkillKey.FireRain:
                                    _state.ChangeState(eActorState.FireRain);
                                    break;
                                case Definition.SkillKey.GuidedMissile:
                                    _state.ChangeState(eActorState.GuidedMissile);
                                    break;
                                case Definition.SkillKey.ExplodePoisonCloud:
                                    _state.ChangeState(eActorState.ExplodePoisonCloud);
                                    break;
                                case Definition.SkillKey.IncreaseMoving:
                                    _state.ChangeState(eActorState.IncreaseMoving);
                                    break;
                                case Definition.SkillKey.SwordFewHitFire:
                                    _state.ChangeState(eActorState.SwordFewHitFire);
                                    break;
                                case Definition.SkillKey.AbsorbLife:
                                    _state.ChangeState(eActorState.AbsorbLife);
                                    break;
                                case Definition.SkillKey.RecoverHpTick:
                                    _state.ChangeState(eActorState.RecoverHpTick);
                                    break;
                                case Definition.SkillKey.GodMode:
                                    _state.ChangeState(eActorState.GodMode);
                                    break;
                                case Definition.SkillKey.RangeStun:
                                    _state.ChangeState(eActorState.RangeStun);
                                    break;
                                case Definition.SkillKey.LightningForSeconds:
                                    _state.ChangeState(eActorState.LightningForSeconds);
                                    break;
                                case Definition.SkillKey.NoveForSeconds:
                                    _state.ChangeState(eActorState.NoveForSeconds);
                                    break;
                                case Definition.SkillKey.BigFireballForSeconds:
                                    _state.ChangeState(eActorState.BigFireballForSeconds);
                                    break;
                                case Definition.SkillKey.RecoverShield:
                                    _state.ChangeState(eActorState.RecoverShield);
                                    break;
                                case Definition.SkillKey.SummonSubunit:
                                    _state.ChangeState(eActorState.SummonSubunit);
                                    break;
                                case Definition.SkillKey.MagicFewHitFire:
                                    _state.ChangeState(eActorState.MagicFewHitFire);
                                    break;
                                case Definition.SkillKey.FarEnemyFreeze:
                                    _state.ChangeState(eActorState.FarEnemyFreeze);
                                    break;
                                case Definition.SkillKey.SetTurret:
                                    _state.ChangeState(eActorState.SetTurret);
                                    break;
                                case Definition.SkillKey.LaserBeam:
                                    _state.ChangeState(eActorState.LaserBeam);
                                    break;
                                case Definition.SkillKey.CompanionSpawn:
                                    _state.ChangeState(eActorState.CompanionSpawn);
                                    break;
                                case Definition.SkillKey.SpawnMeteor:
                                    _state.ChangeState(eActorState.SpawnMeteor);
                                    break;
                                case Definition.SkillKey.MultipleElectric:
                                    _state.ChangeState(eActorState.MultipleElectric);
                                    break;
                                case Definition.SkillKey.SkyLight:
                                    _state.ChangeState(eActorState.SkyLight);
                                    break;
                                case Definition.SkillKey.TimeBomb:
                                    _state.ChangeState(eActorState.TimeBomb);
                                    break;
                                case Definition.SkillKey.End:
                                    break;
                                default:
                                    break;
                            }
                            var skillcache = Player.Skill.Get(_skillhashkey);
                            skillcache.elapsedCooltime = 0;
                            skillcache.waitForuseSkill = false;
                            Player.Skill.RemoveSkillInput((int)_skillhashkey);
                        }
                    }


                    
                }

                //스킬 등록
                var skillEquiplist = Player.Skill.currentSkillContainer();
                foreach (var skillkey in skillEquiplist)
                {
                    if (skillkey == Definition.SkillKey.None)
                        continue;
                    var skilldata = Player.Skill.Get(skillkey);
                    if (skilldata.IsEquiped && _state.stop == false)
                    {
                        if (skilldata.tabledataSkill.skillType == SkillType.Passive)
                            continue;

                        if (Player.Unit.awakeState == CharacterAwakeState.Normal)
                        {
                            skilldata.elapsedCooltime += Time.deltaTime;
                        }
                        else
                        {
                            skilldata.elapsedCooltime += Time.deltaTime*1.5f;
                        }
                            
                        if (skilldata.leftCooltime > 0)
                        {
                                
                        }
                        else
                        {
                            if (skilldata.waitForuseSkill == false && Player.Cloud.skilldata.isAutoSkill && Battle.Field.IsFightScene)
                            {
                                if (skillkey == Definition.SkillKey.SwordExplode || skillkey == Definition.SkillKey.MultipleFireball)
                                {
                                    if (_state.IsCurrentState(eActorState.Attack))
                                    {
                                        skilldata.waitForuseSkill = true;
                                        Player.Skill.RegisterSkillInput((int)skilldata.tabledataSkill.skillKey);
                                    }
                                }
                                else
                                {
                                    skilldata.waitForuseSkill = true;
                                    Player.Skill.RegisterSkillInput((int)skilldata.tabledataSkill.skillKey);
                                }

                            }
                        }
                    }
                }

                if (_view.triggeredEnemy != null)
                {
                    if (_view.triggeredEnemy._state.IsCurrentState(eActorState.Die) || _view.triggeredEnemy._state.IsCurrentState(eActorState.InActive)
                        || _view.triggeredEnemy._view.gameObject.activeInHierarchy == false)
                    {
                        //Debug.Log("지금 적 업다");
                        _view.triggeredEnemy = null;
                        //target = null;
                        // _state.ChangeState(eActorState.Idle);
                    }
                }
                await UniTask.Yield(_cts.Token);


                _state.StateStop(Battle.Field.unitActivePause);
                if(_state.stop)
                {
                    await UniTask.WaitUntil(()=> Battle.Field.unitActivePause == false);
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

        public bool isUnitArriveTarget()
        {
            return ai.reachedDestination;
        }

        #region attack
        public class CharacterAttack : CharacterState
        {
            ControllerUnitInGame _unit;
            int attackSpeedFrame=60;
            int attackCountForFewhitFireSkill = 0;
            int attackCountForCompanionSkill = 0;
            public CharacterAttack(ControllerUnitInGame unit)
            {
                _unit = unit;
            }

            bool targetDie;
            List<ViewPowerFullGodAttack> godAttackObjList = new List<ViewPowerFullGodAttack>();
            CriticalType criType;
            public virtual void Attack()
            {
                targetDie = false;
                var enemy = _unit.target.GetComponent<ViewEnemy>();
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                    if (enemycon == null)
                        return;
                    double dmg = Player.Unit.SwordAtk;

                    float randomcri = Random.Range(0, 100);
                    bool isCri = (randomcri <= Player.Unit.CriRate) && Player.Unit.CriRate>0;
                    bool isSuper = false;
                    bool isMega = false;
                    criType = CriticalType.None;
                    if (isCri)
                    {
                        dmg *= (1+Player.Unit.CriDmg/100.0f);
                        criType = CriticalType.Cri;
                        float randomsuper = Random.Range(0, 100);
                        isSuper = (randomsuper <= Player.Unit.SuperRate)&& Player.Unit.SuperRate>0;

                        if(isSuper)
                        {
                            criType = CriticalType.Super;
                            dmg *= (1 + Player.Unit.SuperDmg / 100.0f);

                            float randommega = Random.Range(0, 100);

                            isMega = (randommega <= Player.Unit.MegaRate) && Player.Unit.MegaRate > 0;
                            if (isMega)
                            {
                                criType = CriticalType.Mega;
                                dmg *= (1 + Player.Unit.MegaDmg / 100.0f);
                            }
                        }
                    }

                    if (enemycon.enemyType != EnemyType.Boss)
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                    }
                    else
                    {
                        dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                    }
                    enemycon.DecreaseHp(dmg, UserDmgType.Normal);
                    //Player.Skill.skillDamageList[(int)SkillKey.End] += dmg;
                    if (enemycon.hp<=0)
                    {
                        targetDie = true;
                    }

                    enemycon._view.SetHitEffectOn();
                    var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, enemy.hitPos.position);
                    _hitEffect.On();

                    switch (criType)
                    {
                        case CriticalType.None:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg,criType, Color.white);
                            break;
                        case CriticalType.Cri:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.yellow);
                            break;
                        case CriticalType.Super:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.red);
                            break;
                        case CriticalType.Mega:
                            WorldUIManager.Instance.InstatiateCriticalFont(enemy.transform.position, dmg, criType, Color.blue);
                            break;
                        default:
                            break;
                    }
                 

                    if (Player.Unit.IsSkillActive(Definition.SkillKey.GodMode))
                    {
                        if (Player.Skill.Get(SkillKey.GodMode).userSkilldata.AwakeLv >= 2)
                        {
                            ViewPowerFullGodAttack tempObj = null;
                            for (int i = 0; i < godAttackObjList.Count; i++)
                            {
                                if (godAttackObjList[i].gameObject.activeInHierarchy == false)
                                {
                                    tempObj = godAttackObjList[i];
                                    break;
                                }
                            }
                            if (tempObj == null)
                            {
                                tempObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.godAttackSkill);
                                godAttackObjList.Add(tempObj);
                            }
                            tempObj.particle.Stop();
                            tempObj.Activate();
                            tempObj.gameObject.SetActive(true);
                            tempObj.transform.position = _unit.target.position;
                            tempObj.particle.Play();
                        }
                    }

                    
                    if (Player.Skill.Get(SkillKey.SwordFewHitFire).IsEquiped)
                    {
                        attackCountForFewhitFireSkill++;
                        if (attackCountForFewhitFireSkill >= CharacterSwordFewHitFireSkill.FireAttackCount())
                        {
                            attackCountForFewhitFireSkill = 0;

                            Player.Skill.RegisterSkillInput((int)SkillKey.SwordFewHitFire);
                       
                        }
                    }
                    if (Player.Skill.Get(SkillKey.CompanionSpawn).IsEquiped)
                    {
                        attackCountForCompanionSkill++;
                        if (attackCountForCompanionSkill >= CharacterCompanionSkill.FireAttackCount())
                        {
                            attackCountForCompanionSkill = 0;
                            Player.Skill.RegisterSkillInput((int)SkillKey.CompanionSpawn);

                        }
                    }
                }
            }

            public override eActorState GetState()
            {
                return eActorState.Attack;
            }

            UnitAnimSprtieType attackanimType;
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(Mathf.Clamp(attackSpeedFrame-(int)((Player.Unit.AtkSpeed-1)* attackSpeedFrame),10, attackSpeedFrame));
                _unit.animindex = 0;
                int tempvalue = Random.Range(0, 2);
                if(tempvalue==1)
                {
                    attackanimType = UnitAnimSprtieType.Attack_0;
                }
                else
                {
                    attackanimType = UnitAnimSprtieType.Attack_1;
                }
            }

            protected override void OnExit()
            {
            }
            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(attackanimType, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                    if(targetDie)
                    {
                        _unit._state.ChangeState(eActorState.Idle);
                    }
                    else
                    {
                        if (attackanimType == UnitAnimSprtieType.Attack_0)
                        {
                            attackanimType = UnitAnimSprtieType.Attack_1;
                        }
                        else
                        {
                            attackanimType = UnitAnimSprtieType.Attack_0;
                        }
                    }
                }
                _unit._view.SetSpriteImage(attackanimType, _unit.animindex);

                Vector2 dir = ((Vector2)_unit.target.position - (Vector2)_unit._view.transform.position).normalized;

                float localscalex = Mathf.Abs(_unit._view.spriteTransform.localScale.x);
                float x = localscalex;
                if (dir.x >= 0)
                {
                    x = -localscalex;
                }
                else
                {
                    x = localscalex;
                }
                _unit._view.spriteTransform.localScale = new Vector2(x, _unit._view.spriteTransform.localScale.y);

                var animarray = _unit._view.animspriteinfo[attackanimType].eventFrame;
                for (int i = 0; i < animarray.Length; i++)
                {
                    if (animarray[i] == _unit.animindex)
                    {
                        Attack();
                        //AudioManager.Instance.Play(AudioSourceKey.swordHit);
                    }

                    if(_unit.animindex==2)
                    {
                        int index = Random.Range((int)AudioSourceKey.swing_sword_0, (int)(AudioSourceKey.swing_sword_1) + 1);
                        AudioManager.Instance.Play((AudioSourceKey)index);
                 
                    }
                }
            }
        }
        #endregion
        #region idle

        public class CharacterIdle : CharacterState
        {
            ControllerUnitInGame _unit;
            public CharacterIdle(ControllerUnitInGame unit)
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
            ControllerUnitInGame _unit;
            Vector2 targetposition;
            public CharacterMove(ControllerUnitInGame unit)
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
                _unit.ai.maxSpeed =(float)Player.Unit.MoveSpeed;

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

        public class CharacterDie: CharacterState
        {
            ControllerUnitInGame _unit;
            public CharacterDie(ControllerUnitInGame unit)
            {
                _unit = unit;
            }
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;
                _unit._view.gameObject.SetActive(false);
                Battle.Field.unitDieEvent?.Invoke();
            }

            protected override void OnExit()
            {
            }

            protected override void OnUpdate()
            {
                //_unit.animindex++;
                //if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Idle, _unit.animindex) == false)
                //{
                //    _unit.animindex = 0;
                //}
                //_unit._view.SetSpriteImage(UnitAnimSprtieType.Idle, _unit.animindex);
            }
            public override eActorState GetState()
            {
                return eActorState.Die;
            }
        }
        #endregion
    }



}
