using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace BlackTree.Core
{
    public class ControllerSubUnitInGame : Character
    {
        public ViewSubUnit _view;
        public ViewSubUnitRenderer _renderView;
        public ControllerEnemyInGame target;
        public int animindex = 0;
        public ParticleSystem stunParticle;
        public ParticleSystem recoverParticle;

        const string BlendGhost = "_GhostBlend";

        public Vector3 normalScale;
        public Vector3 awakeScale;

        ParticleSystem awakeParticle;

        enum MatPhase
        {
            normal,alpha
        }
        float currentMatconst=0;
        float currentMatTime = 0;
        MatPhase matPhase=MatPhase.alpha;

        GameObject shieldObj;
        int characterAwakeUnlockLv;

        public ParticleSystem touchParticle;
        public ParticleSystem touchParticleToSubUnit;
        public ControllerSubUnitInGame(Transform parent,CancellationTokenSource cts):base(cts)
        {
            if (_view == null)
            {
                _view = Object.Instantiate(InGameResourcesBundle.Loaded.subunit, parent);
            }
            if (_renderView == null)
            {
                _renderView = Object.Instantiate(InGameResourcesBundle.Loaded.subunitRender, parent);
            }

            _view.Init(this);

            _state = new StateMachine<eActorState>(true, cts);

            var idlestate = new CharacterIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var attackstate = new CharacterAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);

            var stunState = new CharacterStun(this);
            _state.AddState(stunState.GetState(), stunState);
            var recoverState = new CharacterRecover(this);
            _state.AddState(recoverState.GetState(), recoverState);

            if (Player.Unit.Shield >= (Player.Unit.MaxShield / 2))
            {
                Player.Unit.shieldState = ShieldState.Idle;
            }
            else
            {
                Player.Unit.shieldState = ShieldState.Recovering;
            }
                
            _state.StateStop(false);

            characterAwakeUnlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.CharacterAwakeQuestUnlock].unLockLevel;

            touchParticle=UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.touchParticle);
            touchParticle.gameObject.SetActive(false);

            touchParticleToSubUnit= UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.touchParticleToSubUnit);
            touchParticleToSubUnit.transform.SetParent(_renderView.transform, false);
            touchParticleToSubUnit.transform.localPosition = Vector3.zero;
            touchParticleToSubUnit.gameObject.SetActive(false);
            Main().Forget();
            TouchProcess().Forget();

            Player.Unit.shieldStateChangeCallback += ShieldStateChange;

            Battle.Field.UnitStop += UnitStopCallback;
            Battle.Field.UnitRestart += UnitRestartCallback;

            Player.Unit.characterAwakeCallback += CharacterAwake;

            normalScale = _renderView.transform.localScale;
            awakeScale = new Vector3(_renderView.transform.localScale.x * 1.5f, _renderView.transform.localScale.y * 1.5f, _renderView.transform.localScale.z * 1.5f);

            shieldObj = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.shieldObj);
            shieldObj.transform.SetParent(_renderView.transform);
            shieldObj.transform.localPosition = Vector3.zero;
            shieldObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            if (Player.Unit.Shield >= (Player.Unit.MaxShield / 2))
            {
                shieldObj.SetActive(true);
            }
            else
            {
                shieldObj.SetActive(false);
            }
        }

        void CharacterAwake(CharacterAwakeState state)
        {
            if(state==CharacterAwakeState.Normal)
            {
                Player.Unit.awakeState = CharacterAwakeState.Normal;
                Player.Unit.awakeChange= CharacterAwakeChange.None;
                Player.Unit.characterAwakeTouchCount = 0;

                Player.Unit.usersubUnit._renderView.transform.DOScale(Player.Unit.usersubUnit.normalScale, 0.1f);
                Player.Unit.userUnit._view.transform.DOScale(Player.Unit.userUnit.normalScale, 0.1f);

                Player.Unit.StatusSync();
            }
            else if (state == CharacterAwakeState.ToAwake)
            {
                Player.Unit.awakeState = CharacterAwakeState.ToAwake;
                Player.Quest.TryCountUp(QuestType.AwakeWitch, 1);
                ChangeToAwake();
            }
            else
            {
                Player.Unit.currentAwakeTime = 0;
                Player.Unit.currentAwakeTimeForQuest = 0;
                Player.Unit.awakeState = CharacterAwakeState.Awake;

                int randomChange = Random.Range(0, (int)CharacterAwakeChange.End);

                randomChange = 3;
                Player.Unit.awakeChange = (CharacterAwakeChange)randomChange;
            
                switch (Player.Unit.awakeChange)
                {
                    case CharacterAwakeChange.HpOne:
                        break;
                    case CharacterAwakeChange.ExpTwice:
                        break;
                    case CharacterAwakeChange.GoldTwice:
                        break;
                    case CharacterAwakeChange.MonsterGenTwice:
                        break;
                    default:
                        break;
                }

                Player.Unit.characterAwakeChangeCallback?.Invoke();
                Player.Unit.StatusSync();
            }
        }

        void ChangeToAwake()
        {
            if (awakeParticle == null)
            {
                awakeParticle = Object.Instantiate(InGameResourcesBundle.Loaded.awakeParticle);
                awakeParticle.transform.SetParent(Player.Unit.userUnit._view.transform, false);
                awakeParticle.transform.localPosition=Vector3.zero;
                
            }
            awakeParticle.gameObject.SetActive(true);
            awakeParticle.Play();

            Player.Unit.usersubUnit._renderView.transform.DOScale(Player.Unit.usersubUnit.awakeScale,0.5f);
            Player.Unit.userUnit._view.transform.DOScale(Player.Unit.userUnit.awakeScale, 0.5f).OnComplete(() =>
            {
                Player.Unit.characterAwakeCallback?.Invoke(CharacterAwakeState.Awake);
            });
        }

        float rotateSpeed = 100f;
        float movespeed = 5f;
        Vector2 dir;
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Battle.Field.IsFightScene)
                {
                    if(_state.IsCurrentState(eActorState.Idle) || _state.IsCurrentState(eActorState.Attack))
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
                    if (_state.IsCurrentState(eActorState.Stun)==false && _state.IsCurrentState(eActorState.Recover)==false)
                    {
                        target = null;
                        _state.ChangeState(eActorState.Idle);
                    }
                }
                //move
                float distance = Vector2.Distance(Player.Unit.userUnit._view.transform.position, _view.transform.position);
                if(distance>5)
                {
                    _view.transform.rotation = Quaternion.Slerp(_view.transform.rotation, Quaternion.LookRotation(_view.transform.position- Player.Unit.userUnit._view.transform.position),_view.rotateSpeed*Time.deltaTime);
                    _view.transform.position += -_view.transform.forward * _view.movespeed * Time.deltaTime;
   
                }
                else
                {
         
                }
                
                //move
                if(target!=null)
                {
                    Vector2 targetdir = ((Vector2)target._view.transform.position - (Vector2)_renderView.transform.position).normalized;

                    float localscalex = Mathf.Abs(_renderView.transform.localScale.x);
                    float x = localscalex;
                    if (targetdir.x >= 0)
                    {
                        x = -localscalex;
                    }
                    else
                    {
                        x = localscalex;
                    }
                    _renderView.transform.localScale = new Vector3(x, _renderView.transform.localScale.y, _renderView.transform.localScale.z);

                    float viewlocalscalex = Mathf.Abs(_renderView.atkSpeedCanvas.transform.localScale.x);
                    if (x<0)
                    {
                        _renderView.atkSpeedCanvas.transform.localScale = new Vector3(-viewlocalscalex, _renderView.atkSpeedCanvas.transform.localScale.y, _renderView.transform.localScale.z);
                    }
                    else
                    {
                        _renderView.atkSpeedCanvas.transform.localScale = new Vector3(viewlocalscalex, _renderView.atkSpeedCanvas.transform.localScale.y, _renderView.transform.localScale.z);
                    }
                    
                }

                if (Player.Unit.shieldState == ShieldState.Idle)
                {
                    if(currentMatconst>0)
                    {
                        currentMatconst = 0;
                        _view.spriteRenderer.material.SetFloat(BlendGhost, currentMatconst);
                    }
                }
                else
                {
                    switch (matPhase)
                    {
                        case MatPhase.normal:
                            currentMatTime += Time.deltaTime;
                            currentMatconst=Mathf.Lerp(1,0, currentMatTime);
                            _view.spriteRenderer.material.SetFloat(BlendGhost, currentMatconst);
                            if(currentMatTime>=1)
                            {
                                currentMatTime = 0;
                                matPhase = MatPhase.alpha;
                            }
                            break;
                        case MatPhase.alpha:
                            currentMatTime += Time.deltaTime;
                            currentMatconst = Mathf.Lerp(0,1, currentMatTime);
                            _view.spriteRenderer.material.SetFloat(BlendGhost, currentMatconst);
                            if (currentMatTime >= 1)
                            {
                                currentMatTime = 0;
                                matPhase = MatPhase.normal;
                            }
                            break;
                        default:
                            break;
                    }
                }

                await UniTask.Yield(_cts.Token);
            }
        }
        float intervalWithTouch = 10.0f;
        float touchSpeed;
        int delayFrame = 1;
        const int normalAttackSpeed = 70;

        Touch touch;
        async UniTaskVoid TouchProcess()
        {
            while (true)
            {
                if (_state.IsCurrentState(eActorState.Attack))
                {
#if UNITY_EDITOR
                    if (EventSystem.current.IsPointerOverGameObject() == false)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (Player.Quest.mainQuestCurrentId >= characterAwakeUnlockLv)
                            {
                                AudioManager.Instance.Play(AudioSourceKey.TouchForAtkSpeed);

                                Vector2 pos = InGameObjectManager.Instance.ingamecamera.ScreenToWorldPoint(Input.mousePosition);

                                if (touchParticle.gameObject.activeInHierarchy == false)
                                    touchParticle.gameObject.SetActive(true);
                                touchParticle.transform.position = pos;
                                touchParticle.Play();

                                if (touchParticleToSubUnit.gameObject.activeInHierarchy == false)
                                    touchParticleToSubUnit.gameObject.SetActive(true);
                                touchParticleToSubUnit.Stop();
                                touchParticleToSubUnit.Play();

                                intervalWithTouch -= 0.9f;
                                if (Player.Unit.awakeState == CharacterAwakeState.Awake)
                                {
                                    if (intervalWithTouch <= 4)
                                    {
                                        intervalWithTouch = 4;
                                    }
                                }
                                else
                                {
                                    if (intervalWithTouch <= 5)
                                    {
                                        intervalWithTouch = 5;
                                    }

                                }

                                if (Player.Unit.awakeState == CharacterAwakeState.Normal)
                                {
                                    Player.Unit.characterAwakeTouchCount++;
                                    if (Player.Unit.characterAwakeTouchCount >= Player.Unit.characterAwakeMaxTouchCount)
                                    {
                                        Player.Unit.characterAwakeCallback?.Invoke(CharacterAwakeState.ToAwake);

                                        Player.Guide.StartTutorial(Definition.TutorialType.CharacterAwake);
                                    }
                                }
                            }
                        }
                    }

#else
                    if (Input.touchCount > 0)
                    {
                        if (Player.Quest.mainQuestCurrentId >= characterAwakeUnlockLv)
                        {
                            for (int i = 0; i < Input.touchCount; i++)
                            {
                                touch = Input.GetTouch(i);
                                if (!IsPointerOverUIObject(Input.GetTouch(i).position))
                                {
                                    if (touch.phase == TouchPhase.Began)
                                    {
                                        AudioManager.Instance.Play(AudioSourceKey.TouchForAtkSpeed);
                                        Vector2 pos = InGameObjectManager.Instance.ingamecamera.ScreenToWorldPoint(touch.position);

                                        if (touchParticle.gameObject.activeInHierarchy == false)
                                            touchParticle.gameObject.SetActive(true);
                                        touchParticle.transform.position = pos;
                                        touchParticle.Play();

                                        if (touchParticleToSubUnit.gameObject.activeInHierarchy == false)
                                            touchParticleToSubUnit.gameObject.SetActive(true);
                                        touchParticleToSubUnit.Play();

                                        intervalWithTouch -= 0.2f;
                                        if (Player.Unit.awakeState == CharacterAwakeState.Awake)
                                        {
                                            if (intervalWithTouch <= 4)
                                            {
                                                intervalWithTouch = 4;
                                            }
                                        }
                                        else
                                        {
                                            if (intervalWithTouch <= 5)
                                            {
                                                intervalWithTouch = 5;
                                            }

                                        }

                                        if (Player.Unit.awakeState == CharacterAwakeState.Normal)
                                        {
                                            Player.Unit.characterAwakeTouchCount++;
                                            if (Player.Unit.characterAwakeTouchCount >= Player.Unit.characterAwakeMaxTouchCount)
                                            {
                                                Player.Unit.characterAwakeCallback?.Invoke(CharacterAwakeState.ToAwake);

                                                Player.Guide.StartTutorial(Definition.TutorialType.CharacterAwake);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                       
#endif
                    double defaultAtkSpeed = ((Player.Unit.maxWitchAttackSpeed) / ( intervalWithTouch));

                    int atkSpeed = (int)(5000 / ((Player.Unit.maxWitchAttackSpeed) /
                        (intervalWithTouch)));

                    if(Player.Unit.awakeState == CharacterAwakeState.Awake)
                    {
                        if(intervalWithTouch>=7)
                        {
                            intervalWithTouch = 7;
                        }
                    }
                   
                    _state.SetUpdateFrameDelay(atkSpeed);
                    //Debug.Log("¸¶³à °ø¼Ó:" + atkSpeed.ToString());

                    _renderView.atkSpeedSlider.value = (10.0f - intervalWithTouch) * 0.2f;
                    _renderView.atkSpeedText.text = string.Format("{0:F0}", defaultAtkSpeed);

                    intervalWithTouch += Time.deltaTime * 0.7f;
                    if (intervalWithTouch >= 10)
                        intervalWithTouch = 10;

                    if (Player.Unit.awakeState == CharacterAwakeState.Awake)
                    {
                        Player.Unit.currentAwakeTime += Time.deltaTime;
                        Player.Unit.currentAwakeTimeForQuest += Time.deltaTime;
                        //double awakeTime = Player.Unit.AwakeTime + Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCharacterAwakeTime);
                        if (Player.Unit.currentAwakeTime >= Player.Unit.AwakeTime)
                        {
                            Player.Unit.characterAwakeCallback?.Invoke(CharacterAwakeState.Normal);
                        }

                        if(Player.Unit.currentAwakeTimeForQuest>=1)
                        {
                            Player.Unit.currentAwakeTimeForQuest = 0;
                            Player.Quest.TryCountUp(QuestType.AwakeTime, 1);
                        }
                    }
                    else
                    {

                    }
                }
                await UniTask.Yield(_cts.Token);
            }
        }

        public bool IsPointerOverUIObject(Vector2 touchPos)
        {
            PointerEventData eventDataCurrentPosition
                = new PointerEventData(EventSystem.current);

            eventDataCurrentPosition.position = touchPos;

            List<RaycastResult> results = new List<RaycastResult>();


            EventSystem.current
            .RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        void UnitStopCallback()
        {
            _state.ChangeState(eActorState.Idle);
            target = null;
        }
        void UnitRestartCallback()
        {

        }

        private void ShieldStateChange(ShieldState s)
        {
            switch (s)
            {
                case ShieldState.Idle:
                    _state.ChangeState(eActorState.Recover);
                    shieldObj.SetActive(true);
                    break;
                case ShieldState.Recovering:
                    _state.ChangeState(eActorState.Stun);
                    shieldObj.SetActive(false);
                    break;
                default:
                    break;
            }
        }

#region attack
        public class CharacterAttack : CharacterState
        {
            ControllerSubUnitInGame _unit;
            int attackCountForFewhitFireSkill = 0;
      
            public CharacterAttack(ControllerSubUnitInGame unit)
            {
                _unit = unit;
            }

            public virtual void Attack()
            {
                var enemy = _unit.target._view;
                if (enemy != null)
                {
                    var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                    double dmg = Player.Unit.BowAtk;

                    float randomcri = Random.Range(0, 100);
                    bool isCri = (randomcri <= Player.Unit.CriRate)&& Player.Unit.CriRate>0;
                    bool isSuper = false;
                    bool isMega = false;
                    CriticalType critype=CriticalType.None;
                    if (isCri)
                    {
                        dmg *= (1 + Player.Unit.CriDmg / 100.0f);

                        float randomsuper = Random.Range(0, 100);
                        isSuper = (randomsuper <= Player.Unit.SuperRate)&& Player.Unit.SuperRate>0;
                        critype = CriticalType.Cri;
                        if (isSuper)
                        {
                            dmg *= (1 + Player.Unit.SuperDmg / 100.0f);

                            critype = CriticalType.Super;
                            float randommega = Random.Range(0, 100);

                            isMega = (randommega <= Player.Unit.MegaRate) && Player.Unit.MegaRate > 0;
                            if(isMega)
                            {
                                critype = CriticalType.Mega;
                                dmg *= (1 + Player.Unit.MegaDmg / 100.0f);
                            }
                        }
                    }

                    ViewBulletForSoulUnit bullet= null;
                    bullet = PoolManager.Pop(InGameResourcesBundle.Loaded.bullet, null, _unit._view.transform.position,10);

                    bullet.Shoot(_unit._view.transform.position, _unit.target, dmg, Definition.WitchSkillType.Witch, critype);


                    attackCountForFewhitFireSkill++;
                    if (Player.Skill.Get(SkillKey.MagicFewHitFire).IsEquiped)
                    {
                        if (attackCountForFewhitFireSkill >= CharacterMagicFewHitFireSkill.FireAttackCount())
                        {
                            attackCountForFewhitFireSkill = 0;

                            Player.Skill.RegisterSkillInput((int)SkillKey.MagicFewHitFire);
                        }
                    }
                }
            }

            public override eActorState GetState()
            {
                return eActorState.Attack;
            }

            protected override void OnEnter()
            {
                //_unit._state.SetUpdateFrameDelay(normalAttackSpeed-(int)Player.Unit.currentWitchAttackSpeed);
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
                _unit._view.transform.localScale = new Vector3(x, _unit._view.transform.localScale.y, _unit._view.transform.localScale.z);

                var animarray = _unit._view.animspriteinfo[UnitAnimSprtieType.Attack_0].eventFrame;
                for (int i = 0; i < animarray.Length; i++)
                {
                    if (animarray[i] == _unit.animindex)
                    {
                        Attack();
                        //AudioManager.Instance.Play(AudioSourceKey.witchFire);
                    }
                }
            }
        }
#endregion
#region idle

        public class CharacterIdle : CharacterState
        {
            ControllerSubUnitInGame _unit;
            public CharacterIdle(ControllerSubUnitInGame unit)
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

#region Stun
        public class CharacterStun : CharacterState
        {
            ControllerSubUnitInGame _unit;
            public CharacterStun(ControllerSubUnitInGame unit)
            {
                _unit = unit;
                if(_unit.stunParticle==null)
                {
                    _unit.stunParticle = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.soulStunParticle);
                }
                _unit.stunParticle.transform.SetParent(_unit._renderView.transform, false);
                _unit.stunParticle.transform.position = _unit._renderView.stunObjectParent.position;
                _unit.stunParticle.gameObject.SetActive(false);

            }
            protected override void OnEnter()
            {
            
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;
                _unit.stunParticle.gameObject.SetActive(true);
                _unit.stunParticle.Play();

                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ShieldDissapear].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade_ShieldStat(localizedvalue);
            }

            protected override void OnExit()
            {
                //_unit.stunParticle.gameObject.SetActive(false);
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Stun, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                    _unit._state.ChangeState(eActorState.Idle);
                    return;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Stun, _unit.animindex);
            }
            public override eActorState GetState()
            {
                return eActorState.Stun;
            }
        }
#endregion

#region Recover
        public class CharacterRecover : CharacterState
        {
            ControllerSubUnitInGame _unit;
            public CharacterRecover(ControllerSubUnitInGame unit)
            {
                _unit = unit;

                if (_unit.recoverParticle == null)
                {
                    _unit.recoverParticle = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.soulRecoverParticle);
                }
                _unit.recoverParticle.transform.SetParent(_unit._renderView.transform, false);
                _unit.recoverParticle.transform.position = _unit._renderView.stunObjectParent.position;
                _unit.recoverParticle.gameObject.SetActive(false);
            }
            protected override void OnEnter()
            {
                _unit._state.SetUpdateFrameDelay(50);
                _unit.animindex = 0;

                _unit.recoverParticle.gameObject.SetActive(true);
                _unit.recoverParticle.Play();

                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Shieldapear].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade_ShieldStat(localizedvalue);
            }

            protected override void OnExit()
            {
                //_unit.recoverParticle.gameObject.SetActive(false);
            }

            protected override void OnUpdate()
            {
                _unit.animindex++;
                if (_unit._view.IsInSpriteRange(UnitAnimSprtieType.Recover, _unit.animindex) == false)
                {
                    _unit.animindex = 0;
                    _unit._state.ChangeState(eActorState.Idle);
                    return;
                }
                _unit._view.SetSpriteImage(UnitAnimSprtieType.Recover, _unit.animindex);
            }
            public override eActorState GetState()
            {
                return eActorState.Recover;
            }
        }
#endregion
    }
}