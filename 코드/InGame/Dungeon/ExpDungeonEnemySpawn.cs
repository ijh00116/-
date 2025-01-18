using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;

namespace BlackTree.Core
{
    public class ExpDungeonEnemySpawn
    {
        private readonly CancellationTokenSource _cts;
        private readonly Transform _parent;

        int currentLevel;

        DungeonLevelUp msgDungeonlvUp;
        CancellationTokenSource dungeonspawnCanceller;
        public ExpDungeonEnemySpawn(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;

            Battle.Dungeon.DungeonStart += DungeonStart;
            Battle.Field.unitDieEvent += DieEvent;
            msgDungeonlvUp = new DungeonLevelUp();
        }

        void DungeonStart(DungeonType dungeonType)
        {
            if (dungeonType != DungeonType.Exp)
                return;
            Init();
            currentLevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel;
            dungeonspawnCanceller = new CancellationTokenSource();
            DungeonSpawnStart().Forget();
            TimeOutUpdate().Forget();
        }
        void Init()
        {
            currentLevel = 0;
            Battle.Dungeon.dungeonEnd = false;
            //Debug.Log("경험치 던전 초기화");
        }
        async UniTaskVoid TimeOutUpdate()
        {
            while (true)
            {
                if (Battle.Dungeon.DungeonPlayingTime >= Battle.Dungeon.DungeonMaxTimeInExp)
                {
                    break;
                }
                await UniTask.Yield();
            }
            dungeonspawnCanceller.Cancel();
            dungeonspawnCanceller = null;

            Battle.Dungeon.dungeonEnd = true;
            Battle.Field.UnitStop?.Invoke();
            Battle.Field.MainEnemyStop?.Invoke();
            Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, true);
        }
        async UniTaskVoid DungeonSpawnStart()
        {
            while (true)
            {
                for (int i = 0; i < Battle.Dungeon.ExpDungeonEnemyCount; i++)
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
                    enemycontroller.InitinDungeon(currentLevel,eSceneState.EXPDungeon);

                    if(Battle.Dungeon.dungeonEnd)//Dungeon End
                    {
                        break;
                    }

                    if (Battle.Field.currentSceneState != eSceneState.EXPDungeon && Battle.Field.currentSceneState != eSceneState.EXPDungeonPause)
                    {
                        Battle.Dungeon.dungeonEnd = true;
                        Battle.Field.UnitStop?.Invoke();
                        Battle.Field.MainEnemyDeleteAction?.Invoke();
                        break;
                     }

                    await UniTask.WaitUntil(() => Battle.Field.currentSceneState==eSceneState.EXPDungeon);
                }

                if (Battle.Dungeon.dungeonEnd)//Dungeon End
                {
                    break;
                }

                await UniTask.WaitUntil(()=> CurrentEnemyCountLow()==true,cancellationToken: dungeonspawnCanceller.Token);

            }

            //if (Battle.Dungeon.dungeonEnd == false)//dungeon clear for timeout(in expdungeon time end=>dungeon clear)
            //{
            //    Battle.Dungeon.dungeonEnd = true;
            //    Battle.Field.UnitStop?.Invoke();
            //    Battle.Field.MainEnemyStop?.Invoke();
            //    Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, true);
            //}
        }

        void DieEvent()
        {
            if (Battle.Dungeon.dungeonEnd)
                return;
            //fail dungeon
            if (Battle.Field.currentSceneState == eSceneState.EXPDungeon)
            {
                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
            }
        }


        private bool CurrentEnemyCountLow()
        {
            var enemycount = Battle.Enemy.GetActiveEnemyControllerCount();
            return enemycount <= 5;
        }
    }
}
