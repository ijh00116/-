using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using BlackTree.Model;
using BlackTree.Bundles;
using Cysharp.Threading.Tasks;
using BackEnd;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerRaidDungeonInGame 
    {
        private ViewCanvasRaidDungeon _view;
        private CancellationTokenSource _cts;

        private readonly Transform _parent;

        ControllerRaidEnemyInGame bossEnemycontroller;
        public ControllerRaidDungeonInGame(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;

            _view = ViewCanvas.Create<ViewCanvasRaidDungeon>(_parent);
            _view.exitTotalWindow.SetActive(false);

            _view.endClickerBtn.onClick.AddListener(()=> OpenExitWindow(false));

            for (int i = 0; i < _view.OutBtn.Length; i++)
            {
                int index = i;
                _view.OutBtn[index].onClick.AddListener(ExitRaidContent);
            }
            for (int i = 0; i < _view.CancelOutBtn.Length; i++)
            {
                int index = i;
                _view.CancelOutBtn[index].onClick.AddListener(CloseExitWindow);
            }
            _view.Init();

            Battle.Raid.DungeonStart += IntroStart;
            Battle.Raid.DungeonEnd += OpenExitWindow;
            Battle.Raid.timeMinusEvent += UpdateLeftTime;
        }

        void UpdateLeftTime(double minusTime)
        {
            Battle.Raid.DungeonPlayingTime += (float)minusTime;
        }
        void IntroStart()
        {
            _view.exitTotalWindow.SetActive(false);
            BossDescSet();
            Battle.Raid.isDungeonEnd = false;
            _view.SetVisible(true);
            IntroFlow().Forget();
        }

        void BossDescSet()
        {
            if (Battle.Raid.raidBossIndex == 0)
            {
                _view.raidBossDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.raidBoss_0_Desc].StringToLocal);
            }
            if (Battle.Raid.raidBossIndex == 1)
            {
                _view.raidBossDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.raidBoss_1_Desc].StringToLocal);
            }
        }
        int currentRaidLevel;
        async UniTaskVoid IntroFlow()
        {
            float currentTime = 0;
            float maxtime = 1.0f;

            //not start delay 1sec
            while (currentTime < maxtime)
            {
                currentTime += Time.deltaTime;
                await UniTask.Yield(_cts.Token);
            }
            Battle.Raid.DungeonPlayingTime = 0;


            currentRaidLevel = (int)(Player.Cloud.field.bestChapter / Player.BackendData.indexConstForraidRanking);

            UIUpdate().Forget();
            DungeonSpawnStart().Forget();
            DungeonUpdate().Forget();
            DungeonEventStart().Forget();
        }
        async UniTaskVoid UIUpdate()
        {
            while (Battle.Raid.isDungeonEnd == false)
            {
                if (Battle.Raid.isDungeonEnd)
                    break;

                float leftTime = Battle.Raid.DungeonMaxTime- Battle.Raid.DungeonPlayingTime;
                _view.currentTimeSlider.value = leftTime / Battle.Raid.DungeonMaxTime;
                _view.currentTime.text = string.Format("{0:F1}", leftTime);
                _view.currentDamage.text = string.Format("{0}", Battle.Raid.currentRaidDungeonDMG.ToNumberString());

                await UniTask.Yield(_cts.Token);
            }
        }
        async UniTaskVoid DungeonSpawnStart()
        {
            await UniTask.WaitUntil(() => Battle.Raid.isDungeonEnd == false);

            if (bossEnemycontroller == null)
            {
                var _prefab = Battle.Field.GetStage().bossPrefab;
                var tempEnemy = UnityEngine.Object.Instantiate(_prefab);
                
                tempEnemy.gameObject.SetActive(false);
                bossEnemycontroller = new ControllerRaidEnemyInGame(tempEnemy, _cts);
                tempEnemy.Init(bossEnemycontroller);

                Battle.Enemy.AddEnemyControllerList(bossEnemycontroller._view.hash, bossEnemycontroller);
            }
            else
            {
                Battle.Enemy.AddEnemyHash(bossEnemycontroller._view.hash);
            }

            Transform bosspos = Battle.Field.GetStage().bossPos_inBoss;
            bossEnemycontroller._view.transform.position = new Vector2(bosspos.position.x,
                 bosspos.position.y);

            int chapterLevel = 0;
            if(currentRaidLevel==0)
            {
                chapterLevel = 50;
            }
            else
            {
                chapterLevel = Player.BackendData.indexConstForraidRanking * currentRaidLevel;
            }
            bossEnemycontroller.InitinDungeon(chapterLevel, eSceneState.RaidDungeon);

            Battle.Field.UnitRestart();

            await UniTask.Yield(_cts.Token);
        }

        float currentRaidTime=0;
        async UniTaskVoid DungeonEventStart()
        {
            currentRaidTime = 0;
            while (true)
            {
                if (Battle.Raid.raidBossIndex==1)
                {
                    currentRaidTime += Time.deltaTime;
                    if(currentRaidTime>=2)
                    {
                        currentRaidTime = 0;
                        for (int i = 0; i < Battle.Raid.raidSecondBossMonsterCount; i++)
                        {
                            ControllerRaidSubEnemyInGame enemycontroller;
                            enemycontroller = Battle.Enemy.GetInActiveEnemyController() as ControllerRaidSubEnemyInGame;
                            int randomindex =UnityEngine.Random.Range(0, Battle.Field.GetStage().enemyPos.Length);
                            if (enemycontroller == null)
                            {
                                var _prefab = Battle.Field.GetStage().enemyPrefab[UnityEngine.Random.Range(0, Battle.Field.GetStage().enemyPrefab.Length)];
                                var tempEnemy = UnityEngine.Object.Instantiate(_prefab);

                                tempEnemy.gameObject.SetActive(false);
                                enemycontroller = new ControllerRaidSubEnemyInGame(tempEnemy, _cts);
                                tempEnemy.Init(enemycontroller);

                                Battle.Enemy.AddEnemyControllerList(enemycontroller._view.hash, enemycontroller);
                            }
                            else
                            {
                                Battle.Enemy.AddEnemyHash(enemycontroller._view.hash);
                            }

                            enemycontroller._view.transform.position = new Vector2(Battle.Field.GetStage().enemyPos[randomindex].position.x,
                                 Battle.Field.GetStage().enemyPos[randomindex].position.y);

                            int chapterLevel = 0;
                            if (currentRaidLevel == 0)
                            {
                                chapterLevel = 50;
                            }
                            else
                            {
                                chapterLevel = Player.BackendData.indexConstForraidRanking * currentRaidLevel;
                            }

                            enemycontroller.InitinDungeon(chapterLevel, eSceneState.RPDungeon);
                        }
                    }
                }
                if (Battle.Raid.isDungeonEnd)
                    break;

                await UniTask.Yield(_cts.Token);
            }
        }

        async UniTaskVoid DungeonUpdate()
        {
            while (true)
            {
                Battle.Raid.DungeonPlayingTime += Time.deltaTime;
                if (Battle.Raid.DungeonPlayingTime >= Battle.Raid.DungeonMaxTime)
                {
                    break;
                }
                await UniTask.Yield(_cts.Token);
            }

            if (Battle.Field.currentSceneState == eSceneState.RaidDungeon)
            {
                Battle.Raid.isDungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Raid.DungeonEnd?.Invoke(true);
            }
        }
  
        bool isRaidComplete = false;
        void OpenExitWindow(bool isComplete)
        {
            Battle.Field.ChangeSceneState(eSceneState.RaidDungeonPause);

            isRaidComplete = isComplete;
          
            _view.exitTotalWindow.SetActive(true);
            _view.exitWindow.gameObject.SetActive(true);

            _view.exitWindowBlackBG.PopupOpenColorFade();
            _view.exitWindow.CommonPopupOpenAnimationUp();

            if (isRaidComplete)
            {
                string localizedesc_0 = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RaidComplete].StringToLocal;
                _view.completeDesc.text = localizedesc_0;
            }
            else
            {
                string localizedesc_1 = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RaidOutDesc].StringToLocal;
                _view.completeDesc.text = localizedesc_1;
            }
            string localizedesc_2 = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RaidResultDmg].StringToLocal;
            _view.raidBestDmg_ExitWindow.text = string.Format(localizedesc_2, Battle.Raid.currentRaidDungeonDMG.ToNumberString());

            _view.currentRaidCount_ExitWindow.text = string.Format("{0}/{1}", Battle.Raid.maxRaidParticipateMaxCount - (Player.Cloud.userRaidData.todayParticipateCount+1), Battle.Raid.maxRaidParticipateMaxCount);
        }
        void CloseExitWindow()
        {
            if (isRaidComplete)
                return;

            Battle.Field.ChangeSceneState(eSceneState.RaidDungeon);

            _view.exitWindowBlackBG.PopupCloseColorFade();
            _view.exitWindow.CommonPopupCloseAnimationDown(() => {
                _view.exitTotalWindow.SetActive(false);
            });
        }


        void ExitRaidContent()
        {
            Battle.Clicker.isDungeonEnd = true;

            bossEnemycontroller._state.StateStop(true);
            bossEnemycontroller._view.gameObject.SetActive(false);

            //우선 현재 랭킹정보에 내가 있는지 검사(없다면 랭킹 초기화 됐다는 뜻이므로 유저의 bestdamgae정보도 초기화 해줘야 함)
            Player.BackendData.SetMyRaidRank();
            if(Player.BackendData.myRaidRankInfo==null)
            {
                Player.Cloud.userRaidData.bestDamage = 0;
            }

            Player.Cloud.userRaidData.bestDamage += Battle.Raid.currentRaidDungeonDMG;

            //랭킹 갱신
            double calculScore = Player.Cloud.userRaidData.bestDamage / Battle.Raid.divideValue;
            Player.BackendData.RaidRankingUpdate(calculScore);
            //랭킹 갱신
            Battle.Raid.currentRankInitTime = Battle.Raid.raidInitRankTime;

            Player.Cloud.userRaidData.todayParticipateCount++;

            Player.Cloud.userRaidData.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
            Player.SaveUserDataToFirebaseAndLocal().Forget();
         


            _view.SetVisible(false);
            Battle.Field.ChangeSceneState(eSceneState.RaidDungeonEnd);
        }
        //UI
    }
}
