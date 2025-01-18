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
using System;

namespace BlackTree.Core
{
    public enum EnemyType
    {
        melee,
        AD,
        Boss,
    }
    public class ControllerEnemyInGame : Character
    {
        public ViewEnemy _view;
        public double hp;
        public double maxhp;
        public double atk;
        public int animindex = 0;
        public IAstarAI ai;

        public bool isCollidingWithOthers;

        public static float hp_eff_chapter = 1.15f;
        protected static float atk_eff_chapter = 1.11f;

        protected static float AgroDistance = 50.0f;

        protected static float enemyHpConst= 30.0f;

        public float currentStunTime=0.0f;
        public float currentFreezeTime = 0.0f;
        public EnemyType enemyType;

        public GameObject stunDebuffObject;
        public GameObject freezeDebuffObject;

        public bool isSlow = false;
        public float slowPercent = 0;
        public float slowCurrentTime = 0;
        public  float maxSlowTime = 2;

        public bool isPoisoned= false;
        public float poisonDmgTickTimer=0;
        public float poisonCurrentTime = 0;
        public float maxPoisonTime = 5;

        public bool ismeteorFire = false;
        public WitchSkillType witchskillType=WitchSkillType.None;
        public float meteorFireDmgTickTimer = 0;
        public float meteorFireCurrentTime = 0;
        public float maxmeteorFire = 3;

        public bool isHaveBomb = false;
        public float BombMaxTime = 3;
        public float BombCurrentTime = 0;
        public ViewTimeBombUI bombUIObj;

        protected Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        public bool isElite = false;
        public ControllerEnemyInGame(ViewEnemy view,CancellationTokenSource cts) : base(cts)
        {
            _view = view;
            _state = new StateMachine<eActorState>(true, cts);
            ai = _view.GetComponent<IAstarAI>();

            enemyType = view.enemyType;

            var idlestate = new EnemyIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var movestate = new EnemyMove(this);
            _state.AddState(movestate.GetState(), movestate);
            var attackstate = new EnemyAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);
            var diestate = new EnemyDie(this);
            _state.AddState(diestate.GetState(), diestate);
            var inActivestate = new EnemyInActive(this);
            _state.AddState(inActivestate.GetState(), inActivestate);

            var stunState = new EnemyStun(this);
            _state.AddState(stunState.GetState(), stunState);

            var freeze = new EnemyFreeze(this);
            _state.AddState(freeze.GetState(), freeze);

            _state.StateStop(false);
            _state.ChangeState(eActorState.Idle);

            isCollidingWithOthers = false;
            Main().Forget();
        }

       
        public virtual void Init()
        {
            if(isElite)
            {
                hp = Battle.Field.CalculateHp(Player.Cloud.field.chapter, Player.Cloud.field.stage) *
                (1 - Player.Pet._equipAbilitycaches[EquipAbilityKey.MonstaerhpDecrease] * 0.01f) *10;
                maxhp = hp;
                atk = Battle.Field.CalculateAtk(Player.Cloud.field.chapter) *
                    (1 - (Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg) * 0.01f + Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg_2) * 0.01f)) * 7;
            }
            else
            {
                hp = Battle.Field.CalculateHp(Player.Cloud.field.chapter, Player.Cloud.field.stage) *
               (1 - Player.Pet._equipAbilitycaches[EquipAbilityKey.MonstaerhpDecrease] * 0.01f);
                maxhp = hp;
                atk = Battle.Field.CalculateAtk(Player.Cloud.field.chapter) *
                    (1 - (Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg) * 0.01f + Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg_2) * 0.01f));
            }

            _view.gameObject.SetActive(true);
            _view.SyncHpUI();

            _view.mat.SetFloat("_DistortAmount", 0);
            _view.characterRenderer.color = Color.white;
            if(freezeDebuffObject!=null)
            freezeDebuffObject.SetActive(false);

            _view.collider.enabled = true;

            isPoisoned = false;
            ismeteorFire = false;
            isSlow = false;
            isHaveBomb = false;
            if(bombUIObj==null)
            {
                bombUIObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.timeBombUIObj);
                bombUIObj.transform.SetParent(_view.transform, false);
                bombUIObj.transform.position = _view.hitPos.position;
            }
            bombUIObj.gameObject.SetActive(false);

            _state.ChangeState(eActorState.Idle);
        }

     

        public virtual void InitBoss()
        {
            hp = (Battle.Field.CalculateHp(Player.Cloud.field.chapter, Player.Cloud.field.stage) * 17)
                * (1 - Player.Pet._equipAbilitycaches[EquipAbilityKey.BosshpDecrease] * 0.01f ) ;
            maxhp = hp;
            atk = (Battle.Field.CalculateAtk(Player.Cloud.field.chapter) * 9)
                * (1 - (Player.Research.GetValue(ResearchUpgradeKey.DecreaseBossMonsterDmg) * 0.01f+ Player.Research.GetValue(ResearchUpgradeKey.DecreaseBossMonsterDmg_2) * 0.01f));

            _view.gameObject.SetActive(true);
            _view.SyncHpUI();

            _view.mat.SetFloat("_DistortAmount", 0);
            _view.characterRenderer.color = Color.white;
            if (freezeDebuffObject != null)
                freezeDebuffObject.SetActive(false);
            _view.collider.enabled = true;

            isPoisoned = false;
            ismeteorFire = false;
            isSlow = false;
            isHaveBomb = false;
            if (bombUIObj == null)
            {
                bombUIObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.timeBombUIObj);
                bombUIObj.transform.SetParent(_view.transform, false);
                bombUIObj.transform.position = _view.hitPos.position;
            }
            bombUIObj.gameObject.SetActive(false);
            _state.ChangeState(eActorState.Idle);
        }

        public void InitinDungeon(int level,eSceneState scenestate)
        {
            var defaultHp = Battle.Field.CalculateHp(level,2);
            var defaultAtk = Battle.Field.CalculateAtk(level);

            if (scenestate == eSceneState.RaidDungeon)
            {
                hp = defaultHp;
                atk = defaultAtk*6;
                maxhp = hp;
            }
            else
            {
                hp = defaultHp;
                atk = defaultAtk;
                maxhp = hp;
            }

            _view.gameObject.SetActive(true);
            _view.SyncHpUI();

            _view.mat.SetFloat("_DistortAmount", 0);
            _view.characterRenderer.color = Color.white;
            if (freezeDebuffObject != null)
                freezeDebuffObject.SetActive(false);

            _view.collider.enabled = true;

            isPoisoned = false;
            ismeteorFire = false;
            isSlow = false;
            isHaveBomb = false;
            if (bombUIObj == null)
            {
                bombUIObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.timeBombUIObj);
                bombUIObj.transform.SetParent(_view.transform, false);
                bombUIObj.transform.position = _view.hitPos.position;
            }
            bombUIObj.gameObject.SetActive(false);

            _state.ChangeState(eActorState.Idle);
        }

        public void InitinDungeonBoss(int level, eSceneState scenestate)
        {
            var defaultHp = Battle.Field.CalculateHp(level,2)*10;
            var defaultAtk = Battle.Field.CalculateAtk(level)*3;
        


            if (scenestate == eSceneState.RaidDungeon)
            {
                hp = 50000;
                atk = 1;
                maxhp = hp;
            }
            else
            {
                hp = defaultHp;
                atk = defaultAtk;
                maxhp = hp;
            }

            _view.gameObject.SetActive(true);
            _view.SyncHpUI();

            _view.mat.SetFloat("_DistortAmount", 0);
            _view.characterRenderer.color = Color.white;
            if (freezeDebuffObject != null)
                freezeDebuffObject.SetActive(false);

            _view.collider.enabled = true;

            isPoisoned = false;
            ismeteorFire = false;
            isSlow = false;
            isHaveBomb = false;
            if (bombUIObj == null)
            {
                bombUIObj = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.timeBombUIObj);
                bombUIObj.transform.SetParent(_view.transform, false);
                bombUIObj.transform.position = _view.hitPos.position;
            }
            bombUIObj.gameObject.SetActive(false);

            _state.ChangeState(eActorState.Idle);
        }

        public void SlowSetting(bool slowActive, float _slowPercent)
        {
            if (enemyType == EnemyType.Boss)
                return;
            if (slowActive)
            {
                isSlow = true;
                slowPercent = _slowPercent;
                _view.characterRenderer.color = Color.blue;
                slowCurrentTime = 0;
            }
            else
            {
                _view.characterRenderer.color = Color.white;
                isSlow = false;
            }
        }

        public void PoisonSetting(bool poisonActive)
        {
            if (enemyType == EnemyType.Boss)
                return;
            if (poisonActive)
            {
                isPoisoned = true;
                _view.characterRenderer.color = Color.green;
                poisonCurrentTime = 0;
            }
            else
            {
                _view.characterRenderer.color = Color.white;
                isPoisoned = false;
            }
        }

        public void MeteorFireSetting(bool meteorFireActive,WitchSkillType _witchSkilltype)
        {
            if (enemyType == EnemyType.Boss)
                return;
            if (meteorFireActive)
            {
                ismeteorFire = true;
                _view.characterRenderer.color = Color.red;
                meteorFireCurrentTime = 0;

                witchskillType = _witchSkilltype;
            }
            else
            {
                _view.characterRenderer.color = Color.white;
                ismeteorFire = false;
            }
        }

        public void TimeBombSetting(bool bombStart)
        {
            if (bombStart)
            {
                isHaveBomb = true;
                bombUIObj.gameObject.SetActive(true);
                BombCurrentTime = 0;
            }
            else
            {
                isHaveBomb = false;
                bombUIObj.gameObject.SetActive(false);

            }
        }

        public virtual void IncreaseHp(double increasehpValue)
        {
            hp += increasehpValue;
           
            _view.SyncHpUI();
        }

        public virtual void DecreaseHp(double increasehpValue, UserDmgType dmgType)
        {
            //if(Battle.Field.currentSceneState==eSceneState.RaidDungeon)
            //{
            //    if (Battle.Raid.raidBossIndex == 0)
            //    {
            //        if (dmgType == UserDmgType.Normal)
            //        {
            //            increasehpValue = increasehpValue * 1.2f;
            //        }
            //        if (dmgType == UserDmgType.SkillMissile || dmgType == UserDmgType.SkillNormal || dmgType == UserDmgType.SkillRange)
            //        {
            //            increasehpValue = increasehpValue * 0.5f;
            //        }

            //    }
            //    else if (Battle.Raid.raidBossIndex == 1)
            //    {
            //        if (dmgType == UserDmgType.SkillMissile)
            //        {
            //            increasehpValue = increasehpValue * 0.6f;
            //        }
            //        if (dmgType == UserDmgType.SkillRange)
            //        {
            //            increasehpValue = increasehpValue * 1.2f;
            //        }
            //    }
            //    Battle.Raid.currentRaidDungeonDMG += increasehpValue;
            //}
           

            //hp -= maxhp / 10.0f;
            if (isPoisoned)
            {
                hp -= (increasehpValue) * (1 + 0.2f);
            }
            else
            {
                hp -= increasehpValue;
            }
            
            if(_state.IsCurrentState(eActorState.Freeze))
            {
                _state.ChangeState(eActorState.Idle);
            }

            _view.SyncHpUI();
        }

        public double GetHpPercentageToValue(double percent)
        {
            var dataValue = maxhp * percent * 0.01f;
            return dataValue;
        }
        public virtual void DecreaseHpPercentageForAbsorbSkill(double percentage)
        {
            var dataValue = maxhp * percentage * 0.01f;
            hp -= dataValue;

            Player.Skill.skillDamageList[(int)Definition.SkillKey.AbsorbLife] += dataValue;

            _view.SyncHpUI();
        }
        public virtual void InstantDie()
        {
            hp =-1;

            _view.SyncHpUI();
        }

        public virtual async UniTaskVoid Main()
        {
            while (true)
            {
                await UniTask.Yield(_cts.Token);
                if (hp<=0)
                {
                    if(_state.IsCurrentState(eActorState.Die)==false&& _state.IsCurrentState(eActorState.InActive) == false)
                    {
                        _state.ChangeState(eActorState.Die);
                        if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                        {
                            var getexp = Battle.Field.GetRewardExpForEnemy() * Player.AdsBuff.GetBuffValueData(AdsBuffType.ExpGetIncrease);
                            Player.Level.ExpUpAndLvUp(getexp);

                            double coinValue = Battle.Field.GetRewardGoldForEnemy() * Player.AdsBuff.GetBuffValueData(AdsBuffType.CoinGetIncrease);
                            if (Battle.Field.IsFeverTime)
                            {
                                coinValue = coinValue * Battle.Field.goldexpRate;
                            }
                            
                            Player.ControllerGood.Earn(GoodsKey.Coin, coinValue);
                            Battle.Field.SetRewardWhenKill(isElite);

                            if(isElite)
                            {
                                Player.Unit.isEliteAtkBuff = true;

                            }

                            WorldUIManager.Instance.InstatiateGoldFont(_view.transform.position,Player.ControllerGood.CoinCalculatedData(coinValue), false, Color.yellow);

                            AudioManager.Instance.Play(AudioSourceKey.CoinDrop);

                            Battle.Field.currentKillEnemy++;
                            Battle.Field.CurrentEnemyStateCallback?.Invoke();

                            Battle.Field.currentGetExpAfterSleep += getexp;
                            Battle.Field.currentGetGoldAfterSleep += coinValue;
                            //ÀçÈ­ ¶³±À
                            //WorldUIManager.Instance.DropGoodEvent(Definition.GoodsKey.Coin, (int)(10*Mathf.Pow(1.02f,Player.Cloud.field.stage))
                            //    , _view.transform.position, ViewCanvas.Get<ViewCanvasMainTop>().ObtainGoods[0].transform.position);

                            Player.Quest.TryCountUp(QuestType.KillEnemy, 1);

                            Player.Unit.IncreaseHp(Player.Unit.HpRecover);
                            Player.Unit.IncreaseShield(Player.Unit.ShieldRecover);
                        }
                        else if (Battle.Field.currentSceneState == eSceneState.RPDungeon || Battle.Field.currentSceneState == eSceneState.EXPDungeon
                            || Battle.Field.currentSceneState == eSceneState.AwakeDungeon || Battle.Field.currentSceneState == eSceneState.RiftDungeon
                             || Battle.Field.currentSceneState == eSceneState.RuneDungeon)
                        {
                            Message.Send<KillDungeonEnemy>(null);
                            //Player.Quest.TryCountUp(QuestType.KillEnemy, 1);
                            //Player.Unit.IncreaseShield(atk);
                        }
                 
                        if(isHaveBomb)
                        {
                            isHaveBomb = false;
                            BombCurrentTime = 0;
                            bombUIObj.gameObject.SetActive(false);

                            ViewTimeBomb bombeffect = PoolManager.Pop(SkillResourcesBundle.Loaded.timeBombObj, null, this._view.hitPos.position);
                            bombeffect.FireStart(false);
                        }
                    }
                }
                else
                {
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

                        if (_state.IsCurrentState(eActorState.Stun) == false&& _state.IsCurrentState(eActorState.Freeze) == false)
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
                        if(poisonDmgTickTimer>=0.7f)
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

                            DecreaseHp(dmg,UserDmgType.DebuffDmg);
                            Player.Skill.skillDamageList[(int)Definition.SkillKey.ExplodePoisonCloud] += dmg;

                            WorldUIManager.Instance.InstatiateFont(this._view.transform.position, dmg, false, false,skillColor);
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
                        meteorFireDmgTickTimer+= Time.deltaTime;
                        if (meteorFireDmgTickTimer >= 0.5f)
                        {
                            var skilldata = Player.Skill.Get(SkillKey.SpawnMeteor);
                            var dmg = Player.Unit.BowAtk* (skilldata.SkillValue(0,1) * 0.01f) * (Player.Unit.GetSkillIncreaseValue());
                  
                            if(enemyType!=EnemyType.Boss)
                            {
                                dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                            }
                            else
                            {
                                dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                            }

                            if (witchskillType == Definition.WitchSkillType.Witch)
                            {
                                DecreaseHp(dmg, UserDmgType.DebuffDmg);
                                Player.Skill.skillDamageList[(int)Definition.SkillKey.SpawnMeteor] += dmg;
                            }
                            else
                            {
                                var subunitskillcache = Player.Skill.Get(SkillKey.SummonSubunit);
                                dmg = dmg * subunitskillcache.SkillValue(2) * 0.01f;
                                DecreaseHp(dmg, UserDmgType.DebuffDmg);

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
                    if(isSlow)
                    {
                        slowCurrentTime += Time.deltaTime;
                        if (slowCurrentTime >= maxSlowTime)
                        {
                            isSlow = false;
                            _view.characterRenderer.color = Color.white;
                            slowCurrentTime = 0;
                        }
                    }

                    if(isHaveBomb)
                    {
                        BombCurrentTime += Time.deltaTime;
                        if(BombCurrentTime>=BombMaxTime)
                        {
                            isHaveBomb = false;
                            BombCurrentTime = 0;
                            bombUIObj.gameObject.SetActive(false);

                            ViewTimeBomb bombeffect = PoolManager.Pop(SkillResourcesBundle.Loaded.timeBombObj, null, this._view.hitPos.position);
                            bombeffect.FireStart(true);
                        }
                    }

                    if(_state.IsCurrentState(eActorState.Stun))
                    {
                        currentStunTime -= Time.deltaTime;
                        //Debug.Log(string.Format("{0}:{1}", _unit._view.hash, _unit.currentStunTime));
                        if (currentStunTime <= 0)
                        {
                            _state.ChangeState(eActorState.Idle);
                        }
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

        public void SetStunState(float stunTime,bool adstun=false)
        {
            if (enemyType == EnemyType.Boss)
                return;
            if (enemyType == EnemyType.AD&& adstun==false)
            {
                return;
            }
            if (stunTime <= 0)
                return;
            if(stunDebuffObject==null)
            {
                stunDebuffObject = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.stunDebuffObject);
                stunDebuffObject.transform.SetParent(_view.debuffParent, false);
            }
            stunDebuffObject.SetActive(true);

            currentStunTime = stunTime;
            _state.ChangeState(eActorState.Stun);
        }

        public void SetFreezeState(float freezeTime)
        {
            if (freezeTime <= 0)
                return;
            if (freezeDebuffObject == null)
            {
                freezeDebuffObject = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.freezeDebuffObject);
                freezeDebuffObject.transform.SetParent(_view.transform, false);
            }
            freezeDebuffObject.SetActive(true);

            currentFreezeTime = freezeTime;
            _state.ChangeState(eActorState.Freeze);
        }

        protected bool IsInDistanceWithPlayer()
        {
            if (Vector2.Distance(Player.Unit.userUnit._view.transform.position, _view.transform.position) < 7)
            {
                return true;
            }
            return false;
        }
       
    }

    #region attack
    public class EnemyAttack : CharacterState
    {

        ControllerEnemyInGame _unit;
        public EnemyAttack(ControllerEnemyInGame unit)
        {
            _unit = unit;
        }

        public virtual void Attack(Vector2 targetPos)
        {

        }

        public override eActorState GetState()
        {
            return eActorState.Attack;
        }

        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(70);
        }

        protected override void OnExit()
        {
        }
        void Attack()
        {
            if(_unit._view.isMelee)
            {
                int randomIndex = UnityEngine.Random.Range(0, Player.Unit.userUnit._view.hitTransform.Length);
                var _hitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, Player.Unit.userUnit._view.hitTransform[randomIndex].position);
                _hitEffect.On();

                double realdmg = _unit.atk;
                if(_unit.enemyType!=EnemyType.Boss)
                {
                    realdmg = _unit.atk * (1 - Player.Unit.DecreaseDmgFromMonster);
                }

                if (_unit.enemyType != EnemyType.Boss)
                {
                    realdmg = realdmg * Player.Unit.IncreaseAtkToNormalMonster;
                }
                else
                {
                    realdmg = realdmg * Player.Unit.IncreaseAtkToBossMonster;
                }
                if (_unit.isPoisoned)
                {
                    Player.Unit.DecreaseHp(realdmg * (1.0f-0.2f));
                }
                else
                {
                    Player.Unit.DecreaseHp(realdmg);
                }
                
                
                if(Player.Skill.Get(SkillKey.RecoverShield).userSkilldata.AwakeLv >= 1)
                {
                    if (Player.Unit.IsSkillActive(Definition.SkillKey.RecoverShield))
                    {
                        float skillvalue_1 = Player.Skill.Get(SkillKey.RecoverShield).SkillValue(1);
                        double dmg = Player.Unit.SwordAtk * (skillvalue_1 * 0.01f);

                        if (_unit.enemyType != EnemyType.Boss)
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToNormalMonster;
                        }
                        else
                        {
                            dmg = dmg * Player.Unit.IncreaseAtkToBossMonster;
                        }
                        _unit.DecreaseHp(dmg, UserDmgType.SkillNormal);

                        _unit._view.SetHitEffectOn();
                        var _mirrorhitEffect = PoolManager.Pop<HitEffect>(InGameResourcesBundle.Loaded.swordHitEffect, null, _unit._view.hitPos.position);
                        _mirrorhitEffect.On();

                        WorldUIManager.Instance.InstatiateFont(_unit._view.transform.position, dmg, false, false, Color.cyan);
                    }
                }
            }
            else
            {
                var bullet = PoolManager.Pop<ViewBullet>(_unit._view.bullet, null, _unit._view.firePos.transform.position);
                bullet.Shoot(_unit._view.firePos.transform.position, Player.Unit.userUnit._view.transform, _unit);
            }
            
        }
        protected override void OnUpdate()
        {
            _unit.animindex++;
            if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Attack_0, _unit.animindex) == false)
            {
                _unit.animindex = 0;
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
    #region idle

    public class EnemyIdle : CharacterState
    {
        ControllerEnemyInGame _unit;
        public EnemyIdle(ControllerEnemyInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(_unit._view.animationDelayFram);
            _unit.animindex = 0;
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (_unit._view.gameObject.activeInHierarchy == false)
                return;

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

    public class EnemyMove : CharacterState
    {
        ControllerEnemyInGame _unit;

        public EnemyMove(ControllerEnemyInGame unit)
        {
            _unit = unit;
          
        }
        protected override void OnEnter()
        {
            //_unit.ai.onSearchPath += OnUpdate;
            _unit.ai.canMove = true;
            _unit._state.SetUpdateFrameDelay(_unit._view.animationDelayFram);
            _unit.animindex = 0;
        }

        protected override void OnExit()
        {
            // _unit.ai.onSearchPath -= OnUpdate;
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
            float mySpeed=0;
            if(_unit.isSlow)
            {
                _unit.slowCurrentTime += Time.deltaTime;
                if(_unit.slowCurrentTime>=_unit.maxSlowTime)
                {
                    _unit.isSlow = false;
                    _unit._view.characterRenderer.color = Color.white;
                    _unit.slowCurrentTime = 0;
                }
                mySpeed = _unit._view.speed * (1 - _unit.slowPercent * 0.01f);
            }
            else
            {
                mySpeed = _unit._view.speed;
            }
            _unit.ai.maxSpeed = mySpeed;
            
            if(Player.Unit.IsSkillActive(Definition.SkillKey.SwordFewHitFire))
            {

            }
            else
            {
                _unit.ai.destination = Player.Unit.userUnit._view.transform.position;
            }
            

            float localscalex = Mathf.Abs(_unit._view.transform.localScale.x);
            float x = localscalex;

            var unitdir = _unit.ai.velocity;
            if (unitdir.x >= 0 ||Mathf.Abs(unitdir.x)<=0.1f)
            {
                x = localscalex;
            }
            else
            {
                x = -localscalex;
            }

            _unit._view.transform.localScale = new Vector2(x, _unit._view.transform.localScale.y);

            //logic delete
            //_unit.ai.canMove = !_unit.isCollidingWithOthers;
        }



        public override eActorState GetState()
        {
            return eActorState.Move;
        }
    }
    #endregion
    #region Die

    public class EnemyDie : CharacterState
    {
        ControllerEnemyInGame _unit;
        float currentDistortAmount = 0;
        Color defaultcolor = new Color(1, 1, 1, 1);
        Color alphacolor = new Color(1, 1, 1, 0);
        public EnemyDie(ControllerEnemyInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            _unit._view.collider.enabled = false;
            _unit._state.SetUpdateFrameDelay(_unit._view.animationDelayFram);
            _unit.animindex = 0;
            currentDistortAmount = 0;
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
            _unit._view.characterRenderer.color = Color.Lerp(defaultcolor, alphacolor, currentDistortAmount * 3);

            //_unit._view.mat.SetFloat("_DistortAmount", currentDistortAmount);
            
            currentDistortAmount += Time.deltaTime;
            if (currentDistortAmount >= 0.3f)
            {
                //_unit._state.ChangeState(eActorState.InActive);
            }

        }
        public override eActorState GetState()
        {
            return eActorState.Die;
        }
    }
    #endregion

    public class EnemyInActive : CharacterState
    {
        ControllerEnemyInGame _unit;
        public EnemyInActive(ControllerEnemyInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(_unit._view.animationDelayFram);
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



    public class EnemyStun : CharacterState
    {
        ControllerEnemyInGame _unit;

        public EnemyStun(ControllerEnemyInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(_unit._view.animationDelayFram);
        }

        protected override void OnExit()
        {
            _unit.stunDebuffObject.SetActive(false);
        }

        protected override void OnUpdate()
        {
            if (_unit._view.gameObject.activeInHierarchy == false)
                return;

            _unit.animindex++;
            if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Stun, _unit.animindex) == false)
            {
                _unit.animindex = 0;
            }
            _unit._view.SetSpriteImage(UnitAnimSprtieType.Stun, _unit.animindex);


            //_unit.currentStunTime -= Time.deltaTime;
            //Debug.Log(string.Format("{0}:{1}", _unit._view.hash, _unit.currentStunTime));
            //if (_unit.currentStunTime <= 0)
            //{
            //    _unit._state.ChangeState(eActorState.Idle);
            //}
        }
        public override eActorState GetState()
        {
            return eActorState.Stun;
        }
    }

    public class EnemyFreeze : CharacterState
    {
        ControllerEnemyInGame _unit;

        public EnemyFreeze(ControllerEnemyInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            //_unit._state.SetUpdateFrameDelay(_unit._view.animationDelayFram);
        }

        protected override void OnExit()
        {
            //_unit.freezeDebuffObject.SetActive(false);
        }

        protected override void OnUpdate()
        {
            if (_unit._view.gameObject.activeInHierarchy == false)
                return;

            _unit.currentFreezeTime -= Time.deltaTime;
            if (_unit.currentFreezeTime <= 0)
            {
                _unit._state.ChangeState(eActorState.Idle);
                if (_unit.freezeDebuffObject != null)
                    _unit.freezeDebuffObject.SetActive(false);
            }
        }
        public override eActorState GetState()
        {
            return eActorState.Freeze;
        }
    }
}
