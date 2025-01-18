using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using BlackTree.Model;
using BlackTree.Bundles;

namespace BlackTree.Core
{
    public enum eSceneState
    {
        WaitForMainIdle=0,
        MainIdle,
        MainPause, 
        MainBossEvent,
        ReadyForNextStage,
     

        WaitForRPDungeon,
        RPDungeon,
        RPDungeonPause,
        RPDungeonEnd,

        WaitForEXPDungeon,
        EXPDungeon,
        EXPDungeonPause,
        EXPDungeonEnd,

        WaitForAwakeDungeon,
        AwakeDungeon,
        AwakeDungeonPause,
        AwakeDungeonEnd,

        WaitForRiftDungeon,
        RiftDungeon,
        RiftDungeonPause,
        RiftDungeonEnd,
        RiftBossEvent,

        WaitForClickerDungeon,
        ClickerDungeon,
        ClickerDungeonPause,
        ClickerDungeonEnd,

        WaitForRaidDungeon,
        RaidDungeon,
        RaidDungeonPause,
        RaidDungeonEnd,

        WaitForRuneDungeon,
        RuneDungeon,
        RuneDungeonPause,
        RuneDungeonEnd,

        End
    }
    #region interface class
    public abstract class SceneStatedata
    {
        public eSceneState _currentstate;
        public StateMachine<eSceneState> _state;
        private readonly CancellationTokenSource _cts;

        public SceneStatedata(CancellationTokenSource cts)
        {
            _cts = cts;
        }
    }

    public abstract class SceneState : IStateCallback
    {
        public Action onEnter => OnEnter;
        public Action onUpdate => OnUpdate;
        public Action onExit => OnExit;

        public float updateWaitFrame => waitframe;

        protected abstract void OnEnter();
        protected abstract void OnUpdate();
        protected abstract void OnExit();

        public abstract eSceneState GetState();

        protected float waitframe = -1;
    }
    #endregion
    public class SceneStateController : SceneStatedata
    {
        public CancellationTokenSource _cts;
        public SceneStateController(Transform parent, CancellationTokenSource cts) : base(cts)
        {
            _cts = cts;
            _state = new StateMachine<eSceneState>(true, cts);

            //main
            var waitforidlestate = new WaitforMainSceneIdle(this);
            _state.AddState(waitforidlestate.GetState(), waitforidlestate);
            var idlestate = new MainSceneIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var nextstageState = new MainSceneReadyForStage(this);
            _state.AddState(nextstageState.GetState(), nextstageState);
            var pauseState = new MainScenePause(this);
            _state.AddState(pauseState.GetState(), pauseState);
            var bossEventState = new MainSceneBossEvent(this);
            _state.AddState(bossEventState.GetState(), bossEventState);
            
            //goldDungeon
            var waitbossstate = new WaitforGoldDungeonIdle(this);
            _state.AddState(waitbossstate.GetState(), waitbossstate);
            var bossstate = new GoldDungeonIdle(this);
            _state.AddState(bossstate.GetState(), bossstate);
            var bossExitstate = new GoldDungeonPause(this);
            _state.AddState(bossExitstate.GetState(), bossExitstate);
            var bossPausestate = new GoldDungeonExit(this);
            _state.AddState(bossPausestate.GetState(), bossPausestate);

            var waitexpstate= new WaitforEXPDungeonIdle(this);
            _state.AddState(waitexpstate.GetState(), waitexpstate);
            var expdungeonidle = new EXPDungeonIdle(this);
            _state.AddState(expdungeonidle.GetState(), expdungeonidle);
            var expdungeonpause = new EXPDungeonPause(this);
            _state.AddState(expdungeonpause.GetState(), expdungeonpause);
            var expdungeonexit = new EXPDungeonExit(this);
            _state.AddState(expdungeonexit.GetState(), expdungeonexit);

            var waitAwakestate = new WaitforAwakeDungeonIdle(this);
            _state.AddState(waitAwakestate.GetState(), waitAwakestate);
            var Awakedungeonidle = new AwakeDungeonIdle(this);
            _state.AddState(Awakedungeonidle.GetState(), Awakedungeonidle);
            var Awakedungeonpause = new AwakeDungeonPause(this);
            _state.AddState(Awakedungeonpause.GetState(), Awakedungeonpause);
            var Awakedungeonexit = new AwakeDungeonExit(this);
            _state.AddState(Awakedungeonexit.GetState(), Awakedungeonexit);

            var waitRiftstate = new WaitforRiftDungeonIdle(this);
            _state.AddState(waitRiftstate.GetState(), waitRiftstate);
            var Riftdungeonidle = new RiftDungeonIdle(this);
            _state.AddState(Riftdungeonidle.GetState(), Riftdungeonidle);
            var Riftdungeonpause = new RiftDungeonPause(this);
            _state.AddState(Riftdungeonpause.GetState(), Riftdungeonpause);
            var Riftdungeonexit = new RiftDungeonExit(this);
            _state.AddState(Riftdungeonexit.GetState(), Riftdungeonexit);
            var RiftDungeonbossEvent = new RiftDungeonBossEvent(this);
            _state.AddState(RiftDungeonbossEvent.GetState(), RiftDungeonbossEvent);

            var waitClickerstate = new WaitforClickerDungeonIdle(this);
            _state.AddState(waitClickerstate.GetState(), waitClickerstate);
            var Clickerdungeonidle = new ClickerDungeonIdle(this);
            _state.AddState(Clickerdungeonidle.GetState(), Clickerdungeonidle);
            var Clickerdungeonpause = new ClickerDungeonPause(this);
            _state.AddState(Clickerdungeonpause.GetState(), Clickerdungeonpause);
            var Clickerdungeonexit = new ClickerDungeonExit(this);
            _state.AddState(Clickerdungeonexit.GetState(), Clickerdungeonexit);

            var waitRaidstate = new WaitforRaidDungeonIdle(this);
            _state.AddState(waitRaidstate.GetState(), waitRaidstate);
            var Raiddungeonidle = new RaidDungeonIdle(this);
            _state.AddState(Raiddungeonidle.GetState(), Raiddungeonidle);
            var Raiddungeonpause = new RaidDungeonPause(this);
            _state.AddState(Raiddungeonpause.GetState(), Raiddungeonpause);
            var Raiddungeonexit = new RaidDungeonExit(this);
            _state.AddState(Raiddungeonexit.GetState(), Raiddungeonexit);

            var waitRunestate = new WaitforRuneDungeonIdle(this);
            _state.AddState(waitRunestate.GetState(), waitRunestate);
            var Runedungeonidle = new RuneDungeonIdle(this);
            _state.AddState(Runedungeonidle.GetState(), Runedungeonidle);
            var Runedungeonpause = new RuneDungeonPause(this);
            _state.AddState(Runedungeonpause.GetState(), Runedungeonpause);
            var Runedungeonexit = new RuneDungeonExit(this);
            _state.AddState(Runedungeonexit.GetState(), Runedungeonexit);

            Battle.Field.ChangeSceneState += ChangeState;
            _state.StateStop(false);

            ChangeState(eSceneState.WaitForMainIdle);
        }

        private void ChangeState(eSceneState scenetype)
        {
            Battle.Field.currentSceneState = scenetype;
            _state.ChangeState(scenetype);
        }
    }

    #region 메인 인게임
    public class WaitforMainSceneIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforMainSceneIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForMainIdle;
        }

        protected override void OnEnter()
        {
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.SimpleAction(() => {
                Battle.Clicker.ClickDungeonEndGotoMain?.Invoke();
                ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(false);
                ViewCanvas.Get<ViewCanvasUserDiePopup>().SetVisible(false);


                MainNav.MainUISetting(true);
                MainNav.MainSkillUISetting(true);

                if(Battle.Field.GetStage()!=null)
                {
                    Battle.Field.MainEnemyDeleteAction?.Invoke();
                    Battle.Field.skillCompanionDelete?.Invoke();
                }

                int chapterindex = (Player.Cloud.field.chapter%100) / 25;
                int phaseindex= Player.Cloud.field.chapter / 100;
                ViewDefaultStage[] stageList;
                if(phaseindex==0)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_0;
                }
                else if (phaseindex == 1)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_1;
                }
                else if (phaseindex == 2)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_2;
                }
                else if (phaseindex == 3)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_3;
                }
                else
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_4;
                }

                Battle.Field.SetStage(stageList[chapterindex]);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Player.Unit.userUnit._view.gameObject.SetActive(true);
                //Battle.Field.currentKillEnemy = 0;

                Battle.Field.UnitRestart?.Invoke();
                Battle.Field.MainEnemyRestart?.Invoke();
                Battle.Field.onChangeStage?.Invoke();
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Field.ChangeSceneState(eSceneState.MainIdle);

                if(Battle.Field.leftEnemy<=0)
                {
                    Battle.Field.currentKillEnemy = Battle.Field.TotalCountEnemyinStage - 2;
                }

                Battle.Field.stageStart = true;
                Battle.Field.enemySpawnStart?.Invoke();

                InGameObjectManager.Instance.proCam.AddCameraTarget(Player.Unit.userUnit._view.transform);

            }, _scenecontroller._cts).Forget();
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    public class MainSceneIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public MainSceneIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }

        float currentTime = 0;
        public override eSceneState GetState()
        {
            return eSceneState.MainIdle;
        }

        protected override void OnEnter()
        {
            currentTime = 0;
            Battle.Field.mainIdleSafeState = false;
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {
            if (Battle.Field.mainIdleSafeState)
                return;

            currentTime += Time.deltaTime;
            if(currentTime>=1)
            {
                Battle.Field.mainIdleSafeState = true;
            }
        }
    }

    public class MainScenePause : SceneState
    {
        SceneStateController _scenecontroller;
        public MainScenePause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.MainPause;
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
    }

    public class MainSceneReadyForStage : SceneState
    {
        SceneStateController _scenecontroller;
        public MainSceneReadyForStage(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.ReadyForNextStage;
        }

        protected override void OnEnter()
        {
            Player.Unit.userUnit.target = Battle.Field.GetStage().waveEndPos;
            Player.Unit.userUnit._state.ChangeState(eActorState.Move);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {
            if(Player.Unit.userUnit.isUnitArriveTarget())
            {
                ChangeToPause();
            }
        }

        void ChangeToPause()
        {
            Player.Unit.userUnit._state.ChangeState(eActorState.Idle);
            Battle.Field.ChangeSceneState(eSceneState.MainPause);

            
            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.SimpleAction(() => {

                Battle.Field.MainEnemyDeleteAction?.Invoke();
                Battle.Field.skillCompanionDelete?.Invoke();

                MainNav.MainUISetting(true);
                MainNav.MainSkillUISetting(true);

                int chapterindex = (Player.Cloud.field.chapter % 100) / 25;
                int phaseindex = Player.Cloud.field.chapter / 100;
                ViewDefaultStage[] stageList;
                if (phaseindex == 0)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_0;
                }
                else if (phaseindex == 1)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_1;
                }
                else if (phaseindex == 2)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_2;
                }
                else if (phaseindex == 3)
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_3;
                }
                else
                {
                    stageList = InGameResourcesBundle.Loaded.mapForchapter_4;
                }

                Battle.Field.SetStage(stageList[chapterindex]);

   
                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;

                Battle.Field.currentKillEnemy = 0;
                Battle.Field.UnitRestart?.Invoke();
                Battle.Field.MainEnemyRestart?.Invoke();
                Battle.Field.onChangeStage?.Invoke();
                Battle.Field.ChangeSceneState(eSceneState.MainIdle);
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Field.stageStart = true;
                Battle.Field.enemySpawnStart?.Invoke();

            }, _scenecontroller._cts,99).Forget();
        }
    }

    public class MainSceneBossEvent : SceneState
    {
        SceneStateController _scenecontroller;
        public MainSceneBossEvent(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.MainBossEvent;
        }

        protected override void OnEnter()
        {
    
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {
            if (Player.Unit.userUnit.isUnitArriveTarget())
            {
                Player.Unit.userUnit._state.ChangeState(eActorState.Idle);
            }
        }

    }
    #endregion
    #region 골드 던전
    public class WaitforGoldDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforGoldDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForRPDungeon;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("골드던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();
            Battle.Field.MainEnemyDeleteAction?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainSkillUISetting(true);
                MainNav.MainUISetting(false);
            
                var stage = InGameResourcesBundle.Loaded.goldDungeon;
                Battle.Field.SetStage(stage);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentKillEnemy = 0;
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentWave = 0;
                Battle.Field.MainEnemyRestart();

                Battle.Dungeon.DungeonReady?.Invoke(Model.DungeonType.Research);
                ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(true);
                Battle.Field.ChangeSceneState(eSceneState.RPDungeon);

            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class GoldDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public GoldDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            ////Debug.Log("골드던전 진입");
            return eSceneState.RPDungeon;
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
    }

    public class GoldDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public GoldDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RPDungeonPause;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("골드던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class GoldDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public GoldDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RPDungeonEnd;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("골드던전 종료");

            Battle.Field.MainEnemyDeleteAction?.Invoke();
            Player.Unit.BackToMainfromDungeon();
            ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    #endregion
    #region 경험치 던전
    public class WaitforEXPDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforEXPDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForEXPDungeon;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("경치던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainSkillUISetting(true);
                MainNav.MainUISetting(false);

                Battle.Field.MainEnemyDeleteAction?.Invoke();
                var stage = InGameResourcesBundle.Loaded.EXPDungeon;
                Battle.Field.SetStage(stage);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentKillEnemy = 0;
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDamage = 0;
                Battle.Field.MainEnemyRestart();

                Battle.Dungeon.DungeonReady?.Invoke(Model.DungeonType.Exp);
                ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(true);
                Battle.Field.ChangeSceneState(eSceneState.EXPDungeon);

            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class EXPDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public EXPDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.EXPDungeon;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("경치던전 진입");
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class EXPDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public EXPDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.EXPDungeonPause;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class EXPDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public EXPDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.EXPDungeonEnd;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 종료");
            Battle.Field.MainEnemyDeleteAction?.Invoke();
            Player.Unit.BackToMainfromDungeon();
            ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    #endregion

    #region 균열 던전
    public class WaitforRiftDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforRiftDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForRiftDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("균열 던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainSkillUISetting(true);
                MainNav.MainUISetting(false);

                Battle.Field.MainEnemyDeleteAction?.Invoke();
                var stage = InGameResourcesBundle.Loaded.RiftDungeon;
                Battle.Field.SetStage(stage);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentKillEnemy = 0;
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentWave = 0;
                Battle.Field.MainEnemyRestart();

                Battle.Dungeon.DungeonReady?.Invoke(Model.DungeonType.Rift);
                ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(true);
                Battle.Field.ChangeSceneState(eSceneState.RiftDungeon);

            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RiftDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public RiftDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RiftDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 진입");
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RiftDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public RiftDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RiftDungeonPause;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class RiftDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public RiftDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RiftDungeonEnd;
        }

        protected override void OnEnter()
        {
            //Debug.Log("균열 던전 종료");
            Battle.Field.MainEnemyDeleteAction?.Invoke();
            Player.Unit.BackToMainfromDungeon();
            ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RiftDungeonBossEvent: SceneState
    {
        SceneStateController _scenecontroller;
        public RiftDungeonBossEvent(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RiftBossEvent;
        }

        protected override void OnEnter()
        {
            //Debug.Log("균열던전 보스전 종료");
            Player.Unit.userUnit.target = Battle.Field.GetStage().playerPos_inBoss;
            Player.Unit.userUnit._state.ChangeState(eActorState.Move);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {
            if (Player.Unit.userUnit.isUnitArriveTarget())
            {
                Player.Unit.userUnit._state.ChangeState(eActorState.Idle);
            }
        }
    }

    #endregion


    #region 각성석 던전
    public class WaitforAwakeDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforAwakeDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForAwakeDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("각성던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainSkillUISetting(true);
                MainNav.MainUISetting(false);

                Battle.Field.MainEnemyDeleteAction?.Invoke();
                var stage = InGameResourcesBundle.Loaded.AwakeDungeon;
                Battle.Field.SetStage(stage);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentKillEnemy = 0;
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage = 0;

                Battle.Field.MainEnemyRestart();

                Battle.Dungeon.DungeonReady?.Invoke(Model.DungeonType.Awake);
                ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(true);
                Battle.Field.ChangeSceneState(eSceneState.AwakeDungeon);

            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class AwakeDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public AwakeDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.AwakeDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 진입");
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class AwakeDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public AwakeDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.AwakeDungeonPause;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class AwakeDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public AwakeDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.AwakeDungeonEnd;
        }

        protected override void OnEnter()
        {
            //Debug.Log("각성 던전 종료");
            Battle.Field.MainEnemyDeleteAction?.Invoke();
            Player.Unit.BackToMainfromDungeon();
            ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    #endregion

    #region 클리커 던전
    public class WaitforClickerDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforClickerDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForClickerDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("클리커 던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainUISetting(false);
                MainNav.MainSkillUISetting(false);

                ViewCanvas.Get<ViewCanvasEnterClickerDungeon>().SetVisible(false);
                Battle.Field.MainEnemyDeleteAction?.Invoke();

                Battle.Clicker.DungeonReady?.Invoke();
            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class ClickerDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public ClickerDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.ClickerDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("클리커던전 진입");
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class ClickerDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public ClickerDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.ClickerDungeonPause;
        }

        protected override void OnEnter()
        {
            //Debug.Log("클리커 던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class ClickerDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public ClickerDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.ClickerDungeonEnd;
        }

        protected override void OnEnter()
        {
            //Debug.Log("클리커 던전 종료");
            
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    #endregion

    #region 레이드 던전
    public class WaitforRaidDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforRaidDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForRaidDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("레이드 던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainSkillUISetting(true);
                MainNav.MainUISetting(false);

                Battle.Field.MainEnemyDeleteAction?.Invoke();
                var stage = InGameResourcesBundle.Loaded.RaidDungeon[Battle.Raid.raidBossIndex];
                Battle.Field.SetStage(stage);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Battle.Field.PetPositionDecide?.Invoke();

                Player.Unit.userUnit.target = null;
                Player.Unit.usersubUnit.target = null;
                Battle.Raid.currentRaidDungeonDMG = 0;
                Battle.Field.MainEnemyRestart();

                var enterRaidDungeonCanvas = ViewCanvas.Get<ViewCanvasEnterRaidDungeon>();
                enterRaidDungeonCanvas.blackBG.PopupCloseColorFade();
                enterRaidDungeonCanvas.Wrapped.CommonPopupCloseAnimationUp(() => {
                    enterRaidDungeonCanvas.SetVisible(false);
                });

                ViewCanvas.Get<ViewCanvasRaidDungeon>().SetVisible(true);
                Battle.Raid.DungeonStart?.Invoke();
                Battle.Field.ChangeSceneState(eSceneState.RaidDungeon);
            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RaidDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public RaidDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RaidDungeon;
        }

        protected override void OnEnter()
        {
            //Debug.Log("레이드 던전 진입");
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RaidDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public RaidDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RaidDungeonPause;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class RaidDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public RaidDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RaidDungeonEnd;
        }

        protected override void OnEnter()
        {
            //Debug.Log("레이드 던전 종료");
            Battle.Field.MainEnemyDeleteAction?.Invoke();
            ViewCanvas.Get<ViewCanvasRaidDungeon>().SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    #endregion



    #region 룬 던전
    public class WaitforRuneDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public WaitforRuneDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.WaitForRuneDungeon;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("경치던전 진입대기");
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();

            var fade = ViewCanvas.Get<ViewLoading>();
            fade.SetInvokeTime(1);
            fade.Action(() => {
                MainNav.MainSkillUISetting(true);
                MainNav.MainUISetting(false);

                Battle.Field.MainEnemyDeleteAction?.Invoke();
                var stage = InGameResourcesBundle.Loaded.RuneDungeon;
                Battle.Field.SetStage(stage);

                Player.Unit.userUnit._view.transform.position = Battle.Field.currentMap.playerPos.position;
                Player.Unit.usersubUnit._view.transform.position = Battle.Field.currentMap.witchPos.position;
                Battle.Field.PetPositionDecide?.Invoke();

                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rune].currentKillEnemy = 0;
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rune].currentDamage = 0;
                Battle.Field.MainEnemyRestart();

                Battle.Dungeon.DungeonReady?.Invoke(Model.DungeonType.Rune);
                ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(true);
                Battle.Field.ChangeSceneState(eSceneState.RuneDungeon);

            }, _scenecontroller._cts).Forget();

        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RuneDungeonIdle : SceneState
    {
        SceneStateController _scenecontroller;
        public RuneDungeonIdle(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RuneDungeon;
        }

        protected override void OnEnter()
        {
            ////Debug.Log("룬던전 진입");
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }

    public class RuneDungeonPause : SceneState
    {
        SceneStateController _scenecontroller;
        public RuneDungeonPause(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RuneDungeonPause;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 멈춤");
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();
        }

        protected override void OnExit()
        {
            Battle.Field.MainEnemyRestart();
            Battle.Field.UnitRestart();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class RuneDungeonExit : SceneState
    {
        SceneStateController _scenecontroller;
        public RuneDungeonExit(SceneStateController scenecontroller)
        {
            _scenecontroller = scenecontroller;
        }
        public override eSceneState GetState()
        {
            return eSceneState.RuneDungeonEnd;
        }

        protected override void OnEnter()
        {
            //Debug.Log("경치던전 종료");
            Battle.Field.MainEnemyDeleteAction?.Invoke();
            Player.Unit.BackToMainfromDungeon();
            ViewCanvas.Get<ViewCanvasDungeonIngame>().SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
    #endregion
}
