using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Model;
using BlackTree.Definition;
using BlackTree.Bundles;
using DG.Tweening;

namespace BlackTree.Core
{
    public class ControllerEnemySpawn
    {
        private readonly CancellationTokenSource _cts;
        private readonly Transform _parent;

        ParticleSystem landingDust;

        float currentEliteTime;
        const float EliteSpawnTime = 900;
        bool canEliteSpawn = false;

        float currentEliteAtkBuffTime;
        const float maxEliteAtkBuffTime = 30;
        public ControllerEnemySpawn(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = new CancellationTokenSource();

            Init();

            Battle.Field.MainEnemyDeleteAction += InActiveMainEnemies;

            landingDust = UnityEngine.Object.Instantiate(InGameResourcesBundle.Loaded.landingDustExplosion);
            landingDust.gameObject.SetActive(false);

            Battle.Field.enemySpawnStart += () => { _stageState = StageState.NormalFight; };
            //StageSpawnStart();
            _stageState = StageState.NormalFight;

            canEliteSpawn = false;
            currentEliteTime = 0;

            currentEliteAtkBuffTime = 0;

            EnemySpawn().Forget();
            EliteMonsterTimer().Forget();
        }
        void Init()
        {
            //Debug.Log("스테이지 플러스");
        }

        enum StageState
        {
            NormalFight,
            bossEvent,
            bossFight,
            End
        }
        StageState _stageState;
        async UniTask EnemySpawn()
        {
            while (true)
            {
                switch (_stageState)
                {
                    case StageState.NormalFight:
                        await UniTask.WaitUntil(() => Battle.Field.enemyActivePause == false);
                        if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                        {
                            int leftEnemyCount = Battle.Enemy.GetActiveEnemyControllerCount();
                            int spawnStartEnemyCount = 20;
                            if(Battle.Field.CurrentFieldChapter<=2)
                            {
                                spawnStartEnemyCount = 10;
                            }
                            else
                            {
                                spawnStartEnemyCount = 20;
                            }
                            if(Player.Unit.awakeChange==CharacterAwakeChange.MonsterGenTwice)
                            {
                                if (Battle.Field.CurrentFieldChapter <= 2)
                                {
                                    spawnStartEnemyCount = 20;
                                }
                                else
                                {
                                    spawnStartEnemyCount = 40;
                                }
                                
                            }
                            if (leftEnemyCount <= spawnStartEnemyCount)
                            {
                                ControllerEnemyInGame enemycontroller;
                                int randomenemyindex = Random.Range(0, Battle.Field.GetStage().enemyPrefab.Length);
                                var enemytype = Battle.Field.GetStage().enemyPrefab[randomenemyindex].enemyType;
                                enemycontroller =  Battle.Enemy.GetInActiveEnemyController(enemytype);
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
                                if(canEliteSpawn)
                                {
                                    enemycontroller.isElite = true;
                                    enemycontroller._view.mat.SetFloat("_OutlineAlpha", 1);
                                    canEliteSpawn = false;
                                    currentEliteTime = 0;
                                }
                                else
                                {
                                    enemycontroller.isElite = false;
                                    enemycontroller._view.mat.SetFloat("_OutlineAlpha", 0);
                                }
                                enemycontroller.Init();
                            }
                            if (Player.Cloud.optiondata.autoChallengeBoss)
                            {
                                if (Battle.Field.leftEnemy <= 0)
                                {
                                    DieMainEnemies();
                                    //Debug.Log("보스 자리로 타겟 설정");
                                    Battle.Field.ChangeSceneState(eSceneState.MainBossEvent);
                                    Player.Unit.userUnit.target = Battle.Field.GetStage().playerPos_inBoss;
                                    Player.Unit.userUnit._state.ChangeState(eActorState.Move);
                                    _stageState = StageState.bossEvent;
                                }
                            }
                        }
                        break;
                    case StageState.bossEvent:
                        if (Player.Unit.userUnit.isUnitArriveTarget())
                        {
                            await BossSpawnEvent();
                            //Debug.Log("보스 전투 시작");
                            Battle.Field.ChangeSceneState(eSceneState.MainIdle);
                            _stageState = StageState.bossFight;
                        }
                        await UniTask.Yield(_cts.Token);
                        break;
                    case StageState.bossFight:
                        if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                        {
                            if (IsClear())
                            {
                                Battle.Field.stageStart = false;
                                Battle.Field.CurrentFieldStage++;
                                Player.Pass.chapterCallBack?.Invoke();
                                if (Battle.Field.CurrentFieldStage >= Battle.Field.stagecount)
                                {
                                    Battle.Field.CurrentFieldChapter++;
                                    Battle.Field.CurrentFieldStage = 0;

                                    Player.Option.ContentUnlockUpdate?.Invoke();

                                    if (Player.Cloud.field.chapter >= Player.Cloud.field.bestChapter)
                                    {
                                        Player.Cloud.field.bestChapter = Player.Cloud.field.chapter;
                                        Player.Cloud.field.bestStage = 0;
#if UNITY_EDITOR
                                        //BackEnd.Param param = new BackEnd.Param();
                                        //param.Add("Chapter", Player.Cloud.field.bestChapter);
                                        //Player.BackendData.LogEvent("ChapterInfo", param);
#elif UNITY_ANDROID
                                        BackEnd.Param param = new BackEnd.Param();
                                        param.Add("Chapter", Player.Cloud.field.bestChapter);
                                        Player.BackendData.LogEvent("ChapterInfo", param);
#else
#endif
                                    }
                                    Player.Products.FreeShopAwakeStoneSync?.Invoke();
                                }
                                else
                                {
                                    if (Player.Cloud.field.stage >= Player.Cloud.field.bestStage)
                                    {
                                        Player.Cloud.field.bestStage = Player.Cloud.field.stage;
                                    }
                                }

                                if(Battle.Field.CurrentFieldChapter>= StaticData.Wrapper.chapterRewardTableDatas.Length)
                                {
                                    Battle.Field.CurrentFieldChapter = StaticData.Wrapper.chapterRewardTableDatas.Length-1;
                                    Battle.Field.CurrentFieldStage = Battle.Field.stagecount-1;

                                    Player.Cloud.field.bestChapter = Battle.Field.CurrentFieldChapter;
                                    Player.Cloud.field.bestStage = Battle.Field.CurrentFieldStage;
                                }

                                Player.Quest.TryCountUp(QuestType.UserStage, 1);
                                LocalSaveLoader.SaveUserCloudData();

                                if (Player.Cloud.field.bestChapterForBackEndRanking*100+ Player.Cloud.field.bestStageForBackEndRanking < Player.Cloud.field.chapter*100+ Player.Cloud.field.stage)
                                {
                                    Player.Cloud.field.bestChapterForBackEndRanking = Player.Cloud.field.chapter;
                                    Player.Cloud.field.bestStageForBackEndRanking = Player.Cloud.field.stage;
                                }

                                if (Player.Cloud.field.bestChapter >= Battle.Field.RankingUnlockedChapterIndex)
                                {
                                    Player.BackendData.NormalRankingUpdate(StaticData.Wrapper.ingameRankingNameData[(int)Player.BackendData.NormalRankingType.StageRanking].titleName);
                                }

                                _stageState = StageState.NormalFight;
                                Battle.Field.ChangeSceneState(eSceneState.ReadyForNextStage);
                            }
                        }
                        await UniTask.Yield(_cts.Token);
                        break;
                    case StageState.End:
                        break;
                    default:
                        break;
                }

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
            enemycontroller.InitBoss();

            Vector2 spawnPos= new Vector2(Battle.Field.GetStage().bossPos_inBoss.position.x,
                 Battle.Field.GetStage().bossPos_inBoss.position.y + 20);
            Vector2 endPos = new Vector2(Battle.Field.GetStage().bossPos_inBoss.position.x,
                 Battle.Field.GetStage().bossPos_inBoss.position.y);

            enemycontroller._view.transform.position = spawnPos;
            enemycontroller._view.collider.enabled = false;

            bool bossEventComplete = false;
            enemycontroller._view.transform.DOMove(endPos, 0.8f).SetEase(Ease.InQuart).OnComplete(()=> {
                bossEventComplete = true;
                landingDust.gameObject.SetActive(true);
                enemycontroller._view.collider.enabled = true;
                landingDust.transform.position = endPos;
                landingDust.Play();
            });
            //Debug.Log("보스 나옴");

            while (true)
            {
                if(bossEventComplete)
                {
                    break;
                }
             
                await UniTask.Yield(_cts.Token);
            }
        }

        async UniTaskVoid EliteMonsterTimer()
        {
            while (true)
            {
                if(canEliteSpawn==false)
                {
                    currentEliteTime += Time.deltaTime;
                    if (currentEliteTime >= EliteSpawnTime)
                    {
                        canEliteSpawn = true;
                        currentEliteTime = 0;
                    }
                }
                
                if(Player.Unit.isEliteAtkBuff)
                {
                    currentEliteAtkBuffTime+= Time.deltaTime;
                    if (currentEliteAtkBuffTime >= maxEliteAtkBuffTime)
                    {
                        Player.Unit.isEliteAtkBuff = false;
                        currentEliteAtkBuffTime = 0;
                    }
                }
                 

                await UniTask.Yield(_cts.Token);
            }
        }
        void DieMainEnemies()
        {
            //Debug.Log("남은적 없애기");
            Battle.Enemy.SetStateCurrentEnemies(eActorState.Die);
        }
        void InActiveMainEnemies()
        {
            Battle.Enemy.SetInActiveCurrentEnemies();
        }

        private bool IsClear()
        {
            bool isEnemycleaer = true;

            var enemylist = Battle.Enemy.GetCurrentAllEnemylist();
            foreach (var enemy in enemylist)
            {
                if (enemy.Value._view == null)
                    continue;
                if(enemy.Value._state.IsCurrentState(eActorState.Die)|| enemy.Value._state.IsCurrentState(eActorState.InActive))
                    continue;
                if (enemy.Value._view.gameObject.activeInHierarchy)
                {
                    isEnemycleaer = false;
                    break;
                }
            }
            return isEnemycleaer;
        }
    }

}
