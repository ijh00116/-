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

namespace BlackTree.Core
{
    public class ControllerDummyEnemyInGame : ControllerEnemyInGame
    {
        public ControllerDummyEnemyInGame(ViewEnemy view, CancellationTokenSource cts):base(view,cts)
        {

        }

        public override void Init()
        {
            base.Init();
        }

        public override void IncreaseHp(double increasehpValue)
        {
            if(Battle.Field.currentSceneState==eSceneState.AwakeDungeon)
            {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage -= increasehpValue;

                Vector2 myUIPoint = InGameObjectManager.Instance.ingamecamera.WorldToScreenPoint(_view.transform.position);
                Battle.Field.goodsParticleEffect?.Invoke(myUIPoint,RewardTypes.AwakeStone);
            }
        }

        public override void DecreaseHp(double increasehpValue, UserDmgType dmgType)
        {
            if (Battle.Field.currentSceneState == eSceneState.AwakeDungeon)
            {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage += increasehpValue;

                Vector2 myUIPoint = InGameObjectManager.Instance.ingamecamera.WorldToScreenPoint(_view.transform.position);
                Battle.Field.goodsParticleEffect?.Invoke(myUIPoint, RewardTypes.AwakeStone);
            }
            else if (Battle.Field.currentSceneState == eSceneState.RaidDungeon)
            {
                Battle.Raid.currentRaidDungeonDMG += increasehpValue;

                //Vector2 myUIPoint = InGameObjectManager.Instance.ingamecamera.WorldToScreenPoint(_view.transform.position);
                //Battle.Field.goodsParticleEffect?.Invoke(myUIPoint);
            }
        }
        public override async UniTaskVoid Main()
        {
            await UniTask.Yield();
        }
    }
}

