using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;

namespace BlackTree.Core
{
    public class GoldDungeonEnemySpawn
    {
        private readonly CancellationTokenSource _cts;
        private readonly Transform _parent;

        int currentLevel;

        DungeonWaveUp dungeonwaveup;
        public GoldDungeonEnemySpawn(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;

            dungeonwaveup = new DungeonWaveUp();
            Battle.Dungeon.DungeonStart += DungeonStart;
            Battle.Field.unitDieEvent += DieEvent;
        }

        void DungeonStart(DungeonType dungeonType)
        {
            if (dungeonType != DungeonType.Research)
                return;

            Init();
            currentLevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel;
            DungeonTimeCheck().Forget();
            DungeonSpawnStart().Forget();
        }
        void Init()
        {
            Battle.Dungeon.dungeonEnd = false;
            //Debug.Log("골드 던전 초기화");
        }
        async UniTaskVoid DungeonTimeCheck()
        {
            while (true)
            {
                if(TimeOutCheck())
                {
                    break;
                }
                if(Battle.Dungeon.dungeonEnd)
                {
                    break;
                }
                await UniTask.Yield(_cts.Token);

            }

            if (Battle.Dungeon.dungeonEnd == false)//dungeon cant clear for timeout
            {
                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
            }
        }
        async UniTaskVoid DungeonSpawnStart()
        {
            while (true)
            {
                if(CurrentEnemyCountLow())
                {
                    for (int i = 0; i < Battle.Dungeon.GoldDungeonEnemyCount; i++)
                    {
                        ControllerEnemyInGame enemycontroller;
                        enemycontroller = Battle.Enemy.GetInActiveEnemyController();
                        int randomindex = Random.Range(0, Battle.Field.GetStage().enemyPos.Length);
                        if (enemycontroller == null)
                        {
                            var _prefab = Battle.Field.GetStage().enemyPrefab[Random.Range(0, Battle.Field.GetStage().enemyPrefab.Length)];

                            var tempEnemy = Object.Instantiate(_prefab);

                            tempEnemy.gameObject.SetActive(false);
                            enemycontroller = new ControllerEnemyInGame(tempEnemy, _cts);
                            tempEnemy.Init(enemycontroller);

                            Battle.Enemy.AddEnemyControllerList(enemycontroller._view.hash, enemycontroller);
                        }
                        else
                        {
                            Battle.Enemy.AddEnemyHash(enemycontroller._view.hash);
                        }

                        enemycontroller._view.transform.position = new Vector2(Battle.Field.GetStage().enemyPos[randomindex].position.x,
                             Battle.Field.GetStage().enemyPos[randomindex].position.y);
                        enemycontroller.InitinDungeon(currentLevel*2, eSceneState.RPDungeon);

                        //시간 초과로 인한 던전 시간 종료
                        if (TimeOutCheck())
                        {
                            break;
                        }
                        if (Battle.Field.currentSceneState != eSceneState.RPDungeon && Battle.Field.currentSceneState != eSceneState.RPDungeonPause)
                        {
                            Battle.Dungeon.dungeonEnd = true;
                            Battle.Field.UnitStop?.Invoke();
                            Battle.Field.MainEnemyDeleteAction?.Invoke();
                            break;
                        }
                        await UniTask.WaitUntil(() => Battle.Field.currentSceneState == eSceneState.RPDungeon);
                    }
                }
               
                //시간 초과로 인한 던전 시간 종료
                if (TimeOutCheck())
                {
                    Battle.Dungeon.dungeonEnd = true;
                    break;
                }

                //성공으로 인한 콜백
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentKillEnemy>=Battle.Dungeon.GoldDungeonClearEnemyCount)
                {
                    Battle.Dungeon.dungeonEnd = true;
                    Battle.Field.UnitStop?.Invoke();
                    Battle.Field.MainEnemyStop?.Invoke();
                    Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType,true);
                    break;
                }
                if (Battle.Dungeon.dungeonEnd)
                    break;

                await UniTask.Yield(_cts.Token);
            }
        }

        bool TimeOutCheck()
        {
            if (Battle.Dungeon.DungeonPlayingTime >= Battle.Dungeon.DungeonMaxTimeInGold)
            {
                return true;
            }

            return false;
        }
        private bool CurrentEnemyCountLow()
        {
            var enemycount = Battle.Enemy.GetActiveEnemyControllerCount();
            return enemycount <= 6;
        }

        void DieEvent()
        {
            if (Battle.Dungeon.dungeonEnd)
                return;
            //fail dungeon
            if (Battle.Field.currentSceneState == eSceneState.RPDungeon)
            {
                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
            }
        }
    }

}
