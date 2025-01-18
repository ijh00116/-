using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class CharacterLaserBeamSkill : CharacterState
    {
        ControllerUnitInGame _unit;

        ViewLaserBeamSkill laserbeamSkillView;
        ViewLaserBeamSkill laserbeamSkillView_forSubunitSkill;
        Player.Skill.SkillCacheData skillCache;

        float skillValue_0;
        float skillValue_1;
        float skillValue_2;

        public CharacterLaserBeamSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.LaserBeam];
            Player.Skill.SkillActivate += SkillActivate;

            Main().Forget();
        }

        private void SkillActivate(Definition.SkillKey _skillkey, bool active)
        {
            if (_skillkey != skillCache.tabledataSkill.skillKey)
                return;
            if (active == false)
            {
                if(laserbeamSkillView!=null)
                {
                    laserbeamSkillView.InActivate();
                    laserbeamSkillView_forSubunitSkill.InActivate();
                    Player.Unit.SkillActiveUpdate(Definition.SkillKey.LaserBeam, false);

                    Player.Rune.SyncAllData();
                }
            }
        }

        public void CreateSkillEffect(Definition.WitchSkillType witchtype)
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;
            if(laserbeamSkillView==null)
            {
                laserbeamSkillView= UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.laserSkillView);
            }
            if (laserbeamSkillView_forSubunitSkill == null)
            {
                laserbeamSkillView_forSubunitSkill = UnityEngine.Object.Instantiate(SkillResourcesBundle.Loaded.laserSkillView);
            }
            if(witchtype==Definition.WitchSkillType.Witch)
            {
                laserbeamSkillView.Activate(skillCache, witchtype);
            }
            else
            {
                laserbeamSkillView_forSubunitSkill.Activate(skillCache, witchtype);
            }
            
        }

        public override eActorState GetState()
        {
            return eActorState.LaserBeam;
        }

        protected override void OnEnter()
        {
            skillValue_0 = Player.Skill.Get(Definition.SkillKey.LaserBeam).SkillValue(0);
            skillValue_1 = Player.Skill.Get(Definition.SkillKey.LaserBeam).SkillValue(1);
            skillValue_2 = Player.Skill.Get(Definition.SkillKey.LaserBeam).SkillValue(2);

            if(laserbeamSkillView!=null)
                laserbeamSkillView.InActivate();
            if (laserbeamSkillView_forSubunitSkill != null)
                laserbeamSkillView_forSubunitSkill.InActivate();

            var subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
            {
                if (subUnitskillcache.userSkilldata.AwakeLv >= 2)
                {
                    CreateSkillEffect(Definition.WitchSkillType.SkillWitch);
                }
            }
            CreateSkillEffect(Definition.WitchSkillType.Witch);

            AudioManager.Instance.Play(AudioSourceKey.Skill_LaserBeam);
            _unit._state.ChangeState(eActorState.Idle);

            Player.Unit.SkillActiveUpdate(Definition.SkillKey.LaserBeam, true);
            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);

            Player.Rune.SyncAllData();
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {

        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                if (Player.Unit.IsSkillActive(Definition.SkillKey.LaserBeam))
                {
                    if (_unit._state.stop == false)
                    {
                        if (skillCache.elapsedCooltime >= skillCache.tabledataSkill.effectTime + skillValue_1)
                        {
                            laserbeamSkillView.InActivate();
                            laserbeamSkillView_forSubunitSkill.InActivate();
                            Player.Unit.SkillActiveUpdate(Definition.SkillKey.LaserBeam, false);

                            Player.Rune.SyncAllData();
                        }

                        if(laserbeamSkillView_forSubunitSkill.gameObject.activeInHierarchy)
                        {
                            if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit)==false)
                            {
                                laserbeamSkillView_forSubunitSkill.InActivate();
                            }
                        }
                       
                    }
                }
                await UniTask.Yield(_unit._cts.Token);
            }
        }
    }
}
