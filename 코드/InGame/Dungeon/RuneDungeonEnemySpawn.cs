using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;

namespace BlackTree.Core
{
    public class RuneDungeonEnemySpawn 
    {
        private readonly CancellationTokenSource _cts;
        private readonly Transform _parent;

        CancellationTokenSource dungeonspawnCanceller;

        public RuneDungeonEnemySpawn(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;

            Battle.Dungeon.DungeonStart += DungeonStart;
            Battle.Field.unitDieEvent += DieEvent;
        }

        void DungeonStart(DungeonType dungeonType)
        {
            if (dungeonType != DungeonType.Rune)
                return;
            Init();
            Battle.Dungeon.RuneDungeonCurrentLevel = 1;
            dungeonspawnCanceller = new CancellationTokenSource();
            DungeonSpawnStart().Forget();
            TimeOutUpdate().Forget();
        }

        void Init()
        {
            Battle.Dungeon.RuneDungeonCurrentLevel = 1;
            Battle.Dungeon.dungeonEnd = false;
        }

        async UniTaskVoid TimeOutUpdate()
        {
            while (true)
            {
                if (Battle.Dungeon.DungeonPlayingTime >= Battle.Dungeon.DungeonMaxTimeInRune)
                {
                    break;
                }
                await UniTask.Yield();
            }
            if(Battle.Dungeon.dungeonEnd==false)
            {
                if (dungeonspawnCanceller != null)
                {
                    dungeonspawnCanceller.Cancel();
                    dungeonspawnCanceller = null;
                }

                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, true);

            }
           
        }

        async UniTaskVoid DungeonSpawnStart()
        {
            while (true)
            {
                for (int i = 0; i < Battle.Dungeon.RuneDungeonEnemyCount; i++)
                {
                    if (Battle.Dungeon.dungeonEnd)//Dungeon End
                    {
                        break;
                    }

                    if (Battle.Field.currentSceneState != eSceneState.RuneDungeon && Battle.Field.currentSceneState != eSceneState.RuneDungeonPause)
                    {
                        Battle.Dungeon.dungeonEnd = true;
                        Battle.Field.UnitStop?.Invoke();
                        Battle.Field.MainEnemyDeleteAction?.Invoke();
                        break;
                    }

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
                    enemycontroller.InitinDungeon(Battle.Dungeon.RuneDungeonCurrentLevel, eSceneState.RuneDungeon);
               
                    //await UniTask.WaitUntil(() => Battle.Field.currentSceneState == eSceneState.RuneDungeon);
                }

                //boss
                ControllerEnemyInGame bosscontroller;
                bosscontroller = Battle.Enemy.GetInActiveEnemyController(EnemyType.Boss);
                int randomPosindex = Random.Range(0, Battle.Field.GetStage().enemyPos.Length);
                if (bosscontroller == null)
                {
                    var _prefab = Battle.Field.GetStage().bossPrefab;

                    var tempEnemy = Object.Instantiate(_prefab);

                    tempEnemy.gameObject.SetActive(false);
                    bosscontroller = new ControllerEnemyInGame(tempEnemy, _cts);
                    tempEnemy.Init(bosscontroller);

                    Battle.Enemy.AddEnemyControllerList(bosscontroller._view.hash, bosscontroller);
                }
                else
                {
                    Battle.Enemy.AddEnemyHash(bosscontroller._view.hash);
                }

                bosscontroller._view.transform.position = new Vector2(Battle.Field.GetStage().enemyPos[randomPosindex].position.x,
                     Battle.Field.GetStage().enemyPos[randomPosindex].position.y);
                bosscontroller.InitinDungeonBoss(Battle.Dungeon.RuneDungeonCurrentLevel, eSceneState.RuneDungeon);


                if (Battle.Dungeon.dungeonEnd)//Dungeon End
                {
                    break;
                }

                await UniTask.WaitUntil(() => CurrentEnemyCountLow() == true, cancellationToken: dungeonspawnCanceller.Token);

                if(Battle.Dungeon.DungeonPlayingTime>=8)
                {
                    Battle.Dungeon.RuneDungeonCurrentLevel += 5;
                }
                else if (Battle.Dungeon.DungeonPlayingTime >= 5)
                {
                    Battle.Dungeon.RuneDungeonCurrentLevel += 10;
                }
                else if (Battle.Dungeon.DungeonPlayingTime >= 3)
                {
                    Battle.Dungeon.RuneDungeonCurrentLevel += 17;
                }
                else
                {
                    Battle.Dungeon.RuneDungeonCurrentLevel += 25;
                }
              
                Battle.Dungeon.DungeonPlayingTime = 0;
            }
        }

        void DieEvent()
        {
            if (Battle.Dungeon.dungeonEnd)
                return;
            if (Battle.Field.currentSceneState == eSceneState.RuneDungeon)
            {
                dungeonspawnCanceller.Cancel();
                dungeonspawnCanceller = null;

                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, true);
            }
        }


        private bool CurrentEnemyCountLow()
        {
            var enemycount = Battle.Enemy.GetActiveEnemyControllerCount();
            return enemycount <= 0;
        }
    }
}
