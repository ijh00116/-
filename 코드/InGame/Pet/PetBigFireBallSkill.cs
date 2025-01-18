using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetBigFireBallSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;

        List<ViewPetBigFireSkill> missilePool = new List<ViewPetBigFireSkill>();

        public PetBigFireBallSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;

            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();
        }


        public void CreateSkillEffect()
        {
            var randomenemy = Battle.Enemy.GetRandomEnemyController();
            if (randomenemy == null)
                return;

            ViewPetBigFireSkill rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.petBigFireSkill);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos=_unit._view.transform.position;
          
            Vector2 endpos = randomenemy._view.transform.position;

            rainObj.Fire(startpos, endpos, _unit.petCache);

        }

        public override eActorState GetState()
        {
            return eActorState.PetBigFire;
        }
        protected override void OnEnter()
        {
            Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.BigFireBall, true);
            _unit._state.ChangeState(eActorState.Idle);
        }

        private void SkillActivate(Definition.PetSkillKey _skillkey, bool active)
        {
            if (_unit.petCache == null)
                return;
            if (_skillkey != _unit.petCache.tabledata.petskillKey)
                return;
            if (active == false)
            {
                Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.BigFireBall, false);
                for (int i = 0; i < missilePool.Count; i++)
                {
                    missilePool[i].gameObject.SetActive(false);
                }
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.BigFireBall == _unit.petCache.tabledata.petskillKey)
                    {
                        if (Player.Pet.IsSkillActive(Definition.PetSkillKey.BigFireBall))
                        {
                            if (_unit._state.stop == false)
                            {
                                currentTime += Time.deltaTime;
                                if (currentTime > 0.8f)
                                {
                                    if (Battle.Field.IsFightScene)
                                    {
                                        currentTime = 0;
                                        CreateSkillEffect();
                                    }
                                }
                                if (_unit.petCache.elapsedCooltime >= _unit.petCache.tabledata.skillEffectTime)
                                {
                                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.BigFireBall, false);
                                }
                            }
                        }
                    }
                    
                }
            
                await UniTask.Yield(_unit._cts.Token);
            }
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {

        }
    }

}
