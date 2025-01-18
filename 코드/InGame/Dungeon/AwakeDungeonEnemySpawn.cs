using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;

namespace BlackTree.Core
{
    public class AwakeDungeonEnemySpawn
    {
        private readonly CancellationTokenSource _cts;
        private readonly Transform _parent;

        int currentLevel;

        DungeonWaveUp dungeonwaveup;

        public AwakeDungeonEnemySpawn(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;

            dungeonwaveup = new DungeonWaveUp();
            Battle.Dungeon.DungeonStart += DungeonStart;
        }

        void DungeonStart(DungeonType dungeonType)
        {
            if (dungeonType != DungeonType.Awake)
                return;
            Init();
            DungeonSpawnStart().Forget();
            DungeonUpdate().Forget();
        }

        void Init()
        {
            currentLevel = 0;
            Battle.Dungeon.dungeonEnd = false;
            //Debug.Log("각성석 던전 초기화");
        }
        async UniTaskVoid DungeonSpawnStart()
        {
            await UniTask.WaitUntil(() => Battle.Dungeon.dungeonEnd == false);

            ControllerEnemyInGame enemycontroller;
            enemycontroller = Battle.Enemy.GetInActiveEnemyController();
            int randomindex = Random.Range(0, Battle.Field.GetStage().enemyPos.Length);
            if (enemycontroller == null)
            {
                var _prefab = Battle.Field.GetStage().enemyPrefab[Random.Range(0, Battle.Field.GetStage().enemyPrefab.Length)];

                var tempEnemy = Object.Instantiate(_prefab);

                tempEnemy.gameObject.SetActive(false);
                enemycontroller = new ControllerDummyEnemyInGame(tempEnemy, _cts);
                tempEnemy.Init(enemycontroller);

                Battle.Enemy.AddEnemyControllerList(enemycontroller._view.hash, enemycontroller);
            }
            else
            {
                Battle.Enemy.AddEnemyHash(enemycontroller._view.hash);
            }

            enemycontroller._view.transform.position = new Vector2(Battle.Field.GetStage().enemyPos[randomindex].position.x,
                 Battle.Field.GetStage().enemyPos[randomindex].position.y);
            enemycontroller.InitinDungeon(currentLevel, eSceneState.AwakeDungeon);

            await UniTask.Yield(_cts.Token);
        }

        async UniTaskVoid DungeonUpdate()
        {
            while (true)
            {
                if (Battle.Dungeon.DungeonPlayingTime >= Battle.Dungeon.DungeonMaxTimeInAwake)
                {
                    break;
                }
                if (Battle.Field.currentSceneState != eSceneState.AwakeDungeon && Battle.Field.currentSceneState != eSceneState.AwakeDungeonPause)
                {
                    Battle.Dungeon.dungeonEnd = true;
                    Battle.Field.UnitStop?.Invoke();
                    Battle.Field.MainEnemyDeleteAction?.Invoke();
                    break;
                }

                await UniTask.Yield(_cts.Token);
            }

            if (Battle.Field.currentSceneState == eSceneState.AwakeDungeon && Battle.Dungeon.dungeonEnd==false)
            {
                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, true);
            }
        }
     
    }
}
