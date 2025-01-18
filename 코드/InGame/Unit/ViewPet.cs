using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;
using Pathfinding;

namespace BlackTree.Bundles
{
    public class ViewPet : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer characterRenderer;
        ControllerPetUnitInGame _petUnit;
        public Transform spriteTransform;
        public IAstarAI ai;

        public ControllerEnemyInGame triggeredEnemy;
        public LayerMask targetLayer;

        public int attackEventFrame;
        public void Init(ControllerPetUnitInGame unit)
        {
            _petUnit = unit;
            ai= GetComponent<IAstarAI>();
        }

        public void SetSpriteImage(UnitAnimSprtieType _type,int petID ,int index)
        {
            if (petID < 0)
                return;
            var petImage = PetResourcesBundle.Loaded.petImage[petID];
            int typeIndex = -1;
            if (_type==UnitAnimSprtieType.Idle)
            {
                typeIndex = 0;
            }
            else if(_type == UnitAnimSprtieType.Move)
            {
                typeIndex = 1;
            }
            else if (_type == UnitAnimSprtieType.Attack_0)
            {
                typeIndex = 2;
            }
            if(typeIndex>=0)
            {
                characterRenderer.sprite = petImage.animInfo[typeIndex].animInfo.spriteList[index];
            }
        }

        public bool IsInSpriteRange(UnitAnimSprtieType _type, int petID , int index)
        {
            if (petID < 0)
                return false;
            var petImage = PetResourcesBundle.Loaded.petImage[petID];
            int typeIndex = -1;
            if (_type == UnitAnimSprtieType.Idle)
            {
                typeIndex = 0;
            }
            else if (_type == UnitAnimSprtieType.Move)
            {
                typeIndex = 1;
            }
            else if (_type == UnitAnimSprtieType.Attack_0)
            {
                typeIndex = 2;
            }

            if(typeIndex>=0)
            {
                if(index >= petImage.animInfo[typeIndex].animInfo.spriteList.Length)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if ((targetLayer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
                return;

            var enemy = collision.GetComponent<ViewEnemy>();

            if (enemy != null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);

                if (enemycon != null)
                {
                    if (enemycon._state.IsCurrentState(eActorState.Die) == false && enemycon._state.IsCurrentState(eActorState.InActive) == false && enemycon._state.stop == false)
                    {
                        if (triggeredEnemy == null)
                        {
                            triggeredEnemy = enemycon;
                        }
                    }
                }
            }
        }

        
    }
}

