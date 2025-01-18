using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class PetFireRainSkill : CharacterState
    {
        ControllerPetUnitInGame _unit;

        ViewPetFireRainSkill fireRainViewEffect;

        List<ViewPetFireRainSkill> missilePool = new List<ViewPetFireRainSkill>();

        List<Vector2> randomPos = new List<Vector2>();
        const float randomDistance = 3.0f;

        float spawnRainTime = 0.1f;


        public PetFireRainSkill(ControllerPetUnitInGame unit)
        {
            _unit = unit;


            Player.Pet.SkillActivate += SkillActivate;
            Main().Forget();
            
   
        }

        public void CreateSkillEffect()
        {
            ViewPetFireRainSkill rainObj = null;
            rainObj = missilePool.Find(o => o.isActive == false);
            if (rainObj == null)
            {
                rainObj = Object.Instantiate(SkillResourcesBundle.Loaded.petFireRainSkill);
                missilePool.Add(rainObj);
            }
            rainObj.gameObject.SetActive(true);

            Vector2 startpos;
            Vector2 endpos;

            int rainindex = Random.Range(0, randomPos.Count);
            startpos = new Vector2(randomPos[rainindex].x, randomPos[rainindex].y + 7);
            endpos = randomPos[rainindex];




            rainObj.Fire(startpos, endpos, _unit.petCache);

        }

        public override eActorState GetState()
        {
            return eActorState.PetFireRain;
        }
        protected override void OnEnter()
        {
            _unit._state.SetUpdateFrameDelay(1);
            _unit.animindex = 0;

            randomPos.Clear();

            AudioManager.Instance.Play(AudioSourceKey.Skill_FireRain);

            var enemy = Battle.Enemy.GetClosedEnemyController(Player.Unit.usersubUnit._view.transform);
            var otherenemy = Battle.Enemy.GetClosedEnemyController(Player.Skill.skillSubUnitObject._view.transform);

            if (enemy == null)
            {
                _unit._state.ChangeState(eActorState.Idle);
                return;
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomposition = Random.insideUnitCircle;
                var _random = (Vector2)enemy._view.transform.position + (new Vector2(randomposition.x, 0) * randomDistance);
                randomPos.Add(_random);
            }

            _unit._state.ChangeState(eActorState.Idle);


            Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.FireRain, true);

            spawnRainTime = 0.1f;

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);
        }
        private void SkillActivate(Definition.PetSkillKey _skillkey, bool active)
        {
            if (_unit.petCache == null)
                return;
            if (_skillkey != _unit.petCache.tabledata.petskillKey)
                return;
            if (active == false)
            {
                for (int i = 0; i < missilePool.Count; i++)
                {
                    missilePool[i].gameObject.SetActive(false);
                }
                Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.FireRain, false);
            }
        }
        async UniTaskVoid Main()
        {
            float currentTime = 0;
            while (true)
            {
                if (_unit.petCache != null)
                {
                    if (Definition.PetSkillKey.FireRain== _unit.petCache.tabledata.petskillKey)
                    {
                        if (Player.Pet.IsSkillActive(Definition.PetSkillKey.FireRain))
                        {
                            if (_unit._state.stop == false)
                            {
                                currentTime += Time.deltaTime;
                                if (currentTime > spawnRainTime)
                                {
                                    currentTime = 0;

                                    CreateSkillEffect();
                                }
                                if (_unit.petCache.elapsedCooltime >= _unit.petCache.tabledata.skillEffectTime)
                                {
                                    for (int i = 0; i < missilePool.Count; i++)
                                    {
                                        missilePool[i].gameObject.SetActive(false);
                                    }
                                    // viewFireRainRangeObject.SetActive(false);
                                    Player.Pet.SkillActiveUpdate(Definition.PetSkillKey.FireRain, false);
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
