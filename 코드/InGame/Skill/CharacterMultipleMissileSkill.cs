using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    #region explode
    public class CharacterMultipleMissileSkill : CharacterState
    {
        ControllerUnitInGame _unit;
        ViewMultipleMissileSkill explodeskillobj;

        Player.Skill.SkillCacheData skillCache;

        Color skillColor = new Color(174f / 255f, 231f / 255f, 143f / 255f);

        List<ViewMultipleMissile> missilePool =new List<ViewMultipleMissile>();

        const int missileDefaultNum = 5;

        float skillvalue_0 = 0;
        float skillvalue_1 = 0;
        float skillvalue_2 = 0;
        public CharacterMultipleMissileSkill(ControllerUnitInGame unit)
        {
            _unit = unit;

            skillCache = Player.Skill.skillCaches[Definition.SkillKey.MultipleFireball];
        }

        public void CreateSkillEffect()
        {
            if (_unit._state.IsCurrentState(GetState()) == false)
                return;

            var subUnitskillcache = Player.Skill.Get(Definition.SkillKey.SummonSubunit);
            if (Player.Unit.IsSkillActive(Definition.SkillKey.SummonSubunit))
            {
                if (subUnitskillcache.userSkilldata.AwakeLv >= 2)
                {
                    CreateSkillView(Definition.WitchSkillType.SkillWitch);
                }
            }
            CreateSkillView(Definition.WitchSkillType.Witch);

            AudioManager.Instance.Play(AudioSourceKey.Skill_MultiFireball);
        }

        void CreateSkillView(Definition.WitchSkillType witchtype)
        {
            if (_unit.target == null)
                return;
            var enemy = _unit.target.GetComponent<ViewEnemy>();
            if (enemy != null)
            {
                if (explodeskillobj == null)
                {
                    explodeskillobj = SkillResourcesBundle.Loaded.multipleFireSkillView;
                }

                int missileNum = missileDefaultNum + (int)skillvalue_1;
                for (int i = 0; i < missileNum; i++)
                {
                    ViewMultipleMissile missileobj = null;
                    if (i >= missilePool.Count)
                    {
                        missileobj = Object.Instantiate(explodeskillobj.viewExplodeMissile);
                        missilePool.Add(missileobj);
                    }
                    else
                    {
                        missileobj = missilePool[i];
                    }
                    missileobj.particle.Stop();
                    missileobj.gameObject.SetActive(true);
                    if (witchtype == Definition.WitchSkillType.Witch)
                    {
                        missileobj.transform.position = Player.Unit.usersubUnit._view.transform.position;
                    }
                    else
                    {
                        missileobj.transform.position = Player.Skill.skillSubUnitObject._view.transform.position;
                    }

                    InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_1);

                    missileobj.particle.Play();

                    Vector2 rotation;
                    if (witchtype == Definition.WitchSkillType.Witch)
                        rotation = _unit.target.position - Player.Unit.usersubUnit._view.transform.position;
                    else
                        rotation = _unit.target.position - Player.Skill.skillSubUnitObject._view.transform.position;

                    float bulletscale = (1 + skillvalue_2 * 0.01f);
                    float bulletAngleBetween = ((i - (int)(missileNum / 2)) * missileNum) * (bulletscale) * 2;
                    missileobj.transform.localScale = new Vector3(bulletscale, bulletscale, bulletscale);

                    float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg + bulletAngleBetween;

                    Vector3 dirVector = ConvertAngleToVector(rotz);
                    missileobj.MoveToArr(dirVector, skillvalue_0, witchtype);
                }
            }
        }

        Vector3 ConvertAngleToVector(float _deg)
        {
            var rad = _deg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        public override eActorState GetState()
        {
            return eActorState.MultipleFireMissile;
        }

        protected override void OnEnter()
        {
            _unit.animindex = 0;

            skillvalue_0 = Player.Skill.Get(Definition.SkillKey.MultipleFireball).SkillValue(0);
            skillvalue_1 = Player.Skill.Get(Definition.SkillKey.MultipleFireball).SkillValue(1);
            skillvalue_2 = Player.Skill.Get(Definition.SkillKey.MultipleFireball).SkillValue(2);

            CreateSkillEffect();

            _unit._state.ChangeState(eActorState.Idle);
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {

        }
    }
    #endregion
}