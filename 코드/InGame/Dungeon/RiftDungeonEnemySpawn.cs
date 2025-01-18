using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;
using DG.Tweening;
using BlackTree.Bundles;

namespace BlackTree.Core
{
    public class RiftDungeonEnemySpawn
    {
        private readonly CancellationTokenSource _cts;
        private readonly Transform _parent;

        int currentLevel;

        DungeonWaveUp dungeonwaveup;
        ParticleSystem landingDust;
        public RiftDungeonEnemySpawn(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;

            dungeonwaveup = new DungeonWaveUp();
            dungeonwaveup.dungeonType = DungeonType.Rift;
            Battle.Dungeon.DungeonStart += DungeonStart;

            landingDust = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.landingDustExplosion);
            landingDust.gameObject.SetActive(false);
            Battle.Field.unitDieEvent += DieEvent;
        }

        void DungeonStart(DungeonType dungeonType)
        {
            if (dungeonType != DungeonType.Rift)
                return;

            Init();
            currentLevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel;
            DungeonSpawnStart().Forget();
        }
        void Init()
        {
            currentLevel = 0;
            Battle.Dungeon.dungeonEnd = false;
            //Debug.Log("균열 던전 초기화");
        }
        async UniTaskVoid DungeonSpawnStart()
        {
            while (true)
            {
                for (int i = 0; i < Battle.Dungeon.RiftDungeonEnemyCount; i++)
                {
                    ControllerEnemyInGame enemycontroller;

                    int randomenemyindex = Random.Range(0, Battle.Field.GetStage().enemyPrefab.Length);
                    var enemytype = Battle.Field.GetStage().enemyPrefab[randomenemyindex].enemyType;
                    enemycontroller = Battle.Enemy.GetInActiveEnemyController(enemytype);

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
                    enemycontroller.InitinDungeon(currentLevel*2, eSceneState.RiftDungeon);

                    if (Battle.Field.currentSceneState != eSceneState.RiftDungeon && Battle.Field.currentSceneState != eSceneState.RiftDungeonPause 
                        && Battle.Field.currentSceneState!=eSceneState.RiftBossEvent)
                    {
                        Battle.Dungeon.dungeonEnd = true;
                        Battle.Field.UnitStop?.Invoke();
                        Battle.Field.MainEnemyDeleteAction?.Invoke();
                        break;
                    }

                    if (TimeOutCheck())
                    {
                        break;
                    }
                    await UniTask.WaitUntil(() => Battle.Field.currentSceneState == eSceneState.RiftDungeon);
                }

                while (true)
                {
                    if (IsCurrentWaveClear())
                    {
                        break;
                    }
                    if (TimeOutCheck())
                    {
                        break;
                    }
                    if (Battle.Dungeon.dungeonEnd)
                    {
                        break;
                    }
                    await UniTask.Yield(_cts.Token);
                }

                if (TimeOutCheck())
                {
                    break;
                }
                if (Battle.Dungeon.dungeonEnd)
                {
                    break;
                }

                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentWave++;
                Message.Send<DungeonWaveUp>(dungeonwaveup);

                //await fadeinout

                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentWave >= Battle.Dungeon.riftDungeonMaxWave)
                {
                    DieMainEnemies();
                    Battle.Field.ChangeSceneState(eSceneState.RiftBossEvent);
                    Player.Unit.userUnit.target = Battle.Field.GetStage().playerPos_inBoss;
                    Player.Unit.userUnit.ai.destination = Player.Unit.userUnit.target.position;
                    Player.Unit.userUnit._state.ChangeState(eActorState.Move);
                    BossEventProcess().Forget();
                   
                    break;
                }
                await UniTask.Yield(_cts.Token);
            }

            if ( TimeOutCheck() &&Battle.Dungeon.dungeonEnd == false)//dungeon cant clear for timeout
            {
                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
            }
        }

        void DieMainEnemies()
        {
            Battle.Enemy.SetStateCurrentEnemies(eActorState.Die);
        }

        async UniTaskVoid BossEventProcess()
        {
            while (true)
            {
                if (Player.Unit.userUnit.isUnitArriveTarget())
                {
                    await BossSpawnEvent();
                    if (Battle.Field.currentSceneState == eSceneState.RiftBossEvent)
                    {
                        Battle.Field.ChangeSceneState(eSceneState.RiftDungeon);
                        CheckBossClear().Forget();
                        break;
                    }
                    else
                    {
                        InActiveMainEnemies();
                        break;
                    }
                }
                await UniTask.Yield(_cts.Token);
            }
        }

        async UniTask BossSpawnEvent()
        {
            ControllerEnemyInGame enemycontroller;
            enemycontroller = Battle.Enemy.GetInActiveEnemyController(EnemyType.Boss);
            int randomindex = Random.Range(0, Battle.Field.GetStage().enemyPos.Length);
            if (enemycontroller == null)
            {
                var _prefab = Battle.Field.GetStage().bossPrefab;

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

            Vector2 spawnPos = new Vector2(Battle.Field.GetStage().bossPos_inBoss.position.x,
                 Battle.Field.GetStage().bossPos_inBoss.position.y + 20);
            Vector2 endPos = new Vector2(Battle.Field.GetStage().bossPos_inBoss.position.x,
                 Battle.Field.GetStage().bossPos_inBoss.position.y);

            enemycontroller._view.transform.position = spawnPos;

            bool bossEventComplete = false;
            enemycontroller._view.transform.DOMove(endPos, 0.8f).SetEase(Ease.InQuart).OnComplete(() => {
                bossEventComplete = true;
                landingDust.gameObject.SetActive(true);
                landingDust.transform.position = endPos;
                landingDust.Play();
            });
            enemycontroller.InitinDungeonBoss(currentLevel, eSceneState.RiftDungeon);

            while (true)
            {
                if (bossEventComplete)
                {
                    break;
                }
                if (Battle.Field.currentSceneState != eSceneState.RiftBossEvent)
                {
                    break;
                }
                await UniTask.Yield(_cts.Token);
            }
        }


        async UniTask CheckBossClear()
        {
            while (true)
            {
                if (Battle.Field.currentSceneState == eSceneState.RiftDungeon)
                {
                    if (IsClear())
                    {
                        Battle.Dungeon.dungeonEnd = true;
                        Battle.Field.UnitStop?.Invoke();
                        Battle.Field.MainEnemyStop?.Invoke();
                        Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, true);
                        break;
                    }
                    else
                    {
                        if (TimeOutCheck())//dungeon cant clear for timeout
                        {
                            Battle.Dungeon.dungeonEnd = true;
                            Battle.Field.UnitStop?.Invoke();
                            Battle.Field.MainEnemyStop?.Invoke();
                            Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
                        }
                    }
                }
                else
                {
                    Battle.Dungeon.dungeonEnd = true;
                    Battle.Field.UnitStop?.Invoke();
                    Battle.Field.MainEnemyStop?.Invoke();
                    Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
                    break;
                }
                await UniTask.Yield(_cts.Token);
            }

        }

        private bool IsClear()
        {
            bool isEnemycleaer = true;

            var enemylist = Battle.Enemy.GetCurrentAllEnemylist();
            foreach (var enemy in enemylist)
            {
                if (enemy.Value._view == null)
                    continue;
                if (enemy.Value._state.IsCurrentState(eActorState.Die) || enemy.Value._state.IsCurrentState(eActorState.InActive))
                    continue;
                if (enemy.Value._view.gameObject.activeInHierarchy)
                {
                    isEnemycleaer = false;
                    break;
                }
            }
            return isEnemycleaer;
        }

        void InActiveMainEnemies()
        {
            Battle.Enemy.SetInActiveCurrentEnemies();
        }


        bool TimeOutCheck()
        {
            if (Battle.Dungeon.DungeonPlayingTime >= Battle.Dungeon.DungeonMaxTimeInRift)
            {
                return true;
            }

            return false;
        }

        private bool IsCurrentWaveClear()
        {
            var enemy = Battle.Enemy.GetActiveEnemyController();
            return enemy == null;
        }

        void DieEvent()
        {
            if (Battle.Dungeon.dungeonEnd)
                return;
            //fail dungeon
            if (Battle.Field.currentSceneState == eSceneState.RiftDungeon)
            {
                Battle.Dungeon.dungeonEnd = true;
                Battle.Field.UnitStop?.Invoke();
                Battle.Field.MainEnemyStop?.Invoke();
                Battle.Dungeon.DungeonEnd?.Invoke(Battle.Dungeon.currentDungeonType, false);
            }
        }
    }
}
