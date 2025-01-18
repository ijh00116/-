using BlackTree.Core;
using BlackTree.Definition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Model;
using BlackTree.Bundles;

namespace BlackTree
{
    //public class CharacterForSkillUI : MonoBehaviour
    //{
    //    [SerializeField] public Transform powerfulAtkPos;

    //    [SerializeField] public SpriteRenderer cahracterRenderer;
    //    [SerializeField] public AnimSpriteInfo[] animSpriteinfoList;

    //    Dictionary<UnitAnimSprtieType, SpriteAnimInfo> animspriteinfo = new Dictionary<UnitAnimSprtieType, SpriteAnimInfo>();
    //    int animindex = 0;

    //    Coroutine skillAction;

    //    UnitAnimSprtieType currentAnimType;
    //    Player.Skill.SkillCacheData currentSkillData=null;

    //    WaitForSeconds wait = new WaitForSeconds(0.05f);

    //    //skill ฐüทร Object
    //    List<ViewPowerfullAttack> powerfullattackobjList = new List<ViewPowerfullAttack>();
    //    int skillPrefabIndex = 0;

    //    ViewMultipleMissileSkill explodeskillobj;
    //    List<ViewMultipleMissile> missilePool = new List<ViewMultipleMissile>();

    //    ViewAtkIncreaseSkill atkIncreaseSkillview;

    //    ViewFireRainSkill fireRainSkillEffect;

    //    ViewShootWave frontWave;
    //    ViewShootWave backWave;
    //    ViewMissileFireEffect fireEffect;

    //    List<ViewAllDamage> explodeSkillObj = new List<ViewAllDamage>();

    //    ViewMoveIncreaseSkill moveIncreaseSkillview;

    //    ViewPoolingTrapSkill poolingTrapSkill;

    //    ViewAbsorbLifeSkill viewabsorbLife;

    //    ViewRecoverHpTickSkill viewRecovertickHp;

    //    ViewCallGodSkill viewCallGodSkill;
    //    public void Init()
    //    {
    //        for (int i = 0; i < animSpriteinfoList.Length; i++)
    //        {
    //            animspriteinfo.Add(animSpriteinfoList[i].spriteType, animSpriteinfoList[i].animInfo);
    //        }

    //        for (int i = 0; i < 3; i++)
    //        {
    //            var powerfullattackobj = Object.Instantiate(SkillResourcesBundle.Loaded.powerAttackSkillview[i]);
    //            powerfullattackobj.particle.Stop();
    //            powerfullattackobj.gameObject.SetActive(false);
    //            powerfullattackobjList.Add(powerfullattackobj);
    //        }

    //        Player.Skill.SlotTouched += skillkeyparam =>
    //        {
    //            //if (currentSkillData != null)
    //            //{
    //            //    if (currentSkillData.tabledataSkill.skillKey == skillkeyparam)
    //            //    {
    //            //        return;
    //            //    }
    //            //    else
    //            //    {
    //            //        SkillActivate(skillkeyparam);
    //            //    }
    //            //}
    //            //else
    //            //{
    //            //    SkillActivate(skillkeyparam);
    //            //}
    //        };

    //        //skill object instantiate
    //        viewabsorbLife = Object.Instantiate(SkillResourcesBundle.Loaded.absorbLifeskill);
    //        viewabsorbLife.transform.SetParent(InGameObjectManager.Instance.transform, false);

    //        fireEffect = Object.Instantiate(SkillResourcesBundle.Loaded.viewShotEffect);
    //        frontWave = Object.Instantiate(SkillResourcesBundle.Loaded.viewHommingMissile_0);
    //        backWave = Object.Instantiate(SkillResourcesBundle.Loaded.viewHommingMissile_1);
    //        fireEffect.gameObject.SetActive(false);
    //        frontWave.gameObject.SetActive(false);
    //        backWave.gameObject.SetActive(false);

    //        poolingTrapSkill = Object.Instantiate(SkillResourcesBundle.Loaded.poolingTrapSkillView);
    //        poolingTrapSkill.gameObject.SetActive(false);

    //        viewCallGodSkill = Object.Instantiate(SkillResourcesBundle.Loaded.viewCallGodSkill);
    //        viewCallGodSkill.gameObject.SetActive(false);

    //        skillAction = StartCoroutine(ActivateSkillAction(UnitAnimSprtieType.Idle));

    //        currentSkillData = null;
    //    }

    //    public void SkillActivate(SkillKey skillkey)
    //    {
    //        for (int i = 0; i < missilePool.Count; i++)
    //        {
    //            missilePool[i].gameObject.SetActive(false);
    //        }
    //        fireEffect.gameObject.SetActive(false);
    //        frontWave.gameObject.SetActive(false);
    //        backWave.gameObject.SetActive(false);
    //        for (int i = 0; i < explodeSkillObj.Count; i++)
    //        {
    //            explodeSkillObj[i].gameObject.SetActive(false);
    //        }
    //        poolingTrapSkill.gameObject.SetActive(false);
    //        viewCallGodSkill.gameObject.SetActive(false);

    //        currentAnimType = UnitAnimSprtieType.Skill_0;
    //        switch (skillkey)
    //        {
    //            case SkillKey.None:
    //                break;
    //            case SkillKey.ThreeComboAttack:
    //                currentAnimType = UnitAnimSprtieType.Skill_0;
    //                skillPrefabIndex = 0;
    //                break;
    //            case SkillKey.MultipleFireball:
    //                currentAnimType = UnitAnimSprtieType.Skill_1;
                   
    //                break;
    //            case SkillKey.IncreaseAttack:
    //                currentAnimType = UnitAnimSprtieType.BuffSkill;
    //                break;
    //            case SkillKey.FireRain:
    //                currentAnimType = UnitAnimSprtieType.Skill_1;
    //                break;
    //            case SkillKey.GuidedMissile:
                    
    //                currentAnimType = UnitAnimSprtieType.Skill_1;
    //                break;
    //            case SkillKey.Explode:
                   
    //                currentAnimType = UnitAnimSprtieType.Skill_0;
    //                break;
    //            case SkillKey.IncreaseMoving:
    //                currentAnimType = UnitAnimSprtieType.BuffSkill;
    //                break;
    //            case SkillKey.PoolingPortal:
                 
    //                currentAnimType = UnitAnimSprtieType.Skill_1;
    //                break;
    //            case SkillKey.AbsorbLife:
    //                currentAnimType = UnitAnimSprtieType.BuffSkill;
    //                break;
    //            case SkillKey.RecoverHpTick:
    //                currentAnimType = UnitAnimSprtieType.Skill_1;
    //                break;
    //            case SkillKey.GodMode:
               
    //                currentAnimType = UnitAnimSprtieType.Skill_1;
    //                break;
    //            case SkillKey.End:
    //                break;
    //            default:
    //                break;
    //        }
    //        currentSkillData = Model.Player.Skill.Get(skillkey);
    //        animindex = 0;
    //        if(skillAction!=null)
    //        {
    //            StopCoroutine(skillAction);
    //            skillAction = null;
    //        }
            

    //        skillAction=StartCoroutine(ActivateSkillAction(currentAnimType));
    //    }
    //    int idleLoopCount = 0;
    //    IEnumerator ActivateSkillAction(UnitAnimSprtieType skillanimType)
    //    {
    //        while (true)
    //        {
    //            animindex++;
    //            if (IsInSpriteRange(skillanimType, animindex) == false)
    //            {
    //                if(skillanimType!=UnitAnimSprtieType.Idle)
    //                {
    //                    SkillAnimEnd();
    //                    break;
    //                }
    //                else
    //                {
    //                    if(currentSkillData!=null)
    //                    {
    //                        idleLoopCount++;
    //                        if(idleLoopCount>2)
    //                        {
    //                            idleLoopCount = 0;
    //                            SkillActivate(currentSkillData.tabledataSkill.skillKey);
    //                        }
                            
    //                    }
    //                }
    //                animindex = 0;
    //            }
    //            SetSpriteImage(skillanimType, animindex);

    //            var animarray = animspriteinfo[skillanimType].eventFrame;
    //            for (int i = 0; i < animarray.Length; i++)
    //            {
    //                if (animarray[i] == animindex)
    //                {
    //                    CreateSkillEffect();
    //                }
    //            }
    //            yield return wait;
    //        }
    //    }
    //    void SkillAnimEnd()
    //    {
    //        if (skillAction != null)
    //        {
    //            StopCoroutine(skillAction);
    //            skillAction = null;
    //        }
    //        if (currentSkillData.tabledataSkill.skillKey == SkillKey.GodMode)
    //        {
    //            cahracterRenderer.material = InGameResourcesBundle.Loaded.defaultMat;
    //        }
    //        animindex = 0;
    //        skillAction =StartCoroutine(ActivateSkillAction(UnitAnimSprtieType.Idle));
    //    }


    //    void CreateSkillEffect()
    //    {
    //        var enemy = InGameObjectManager.Instance.transform;
    //        Vector3 rotation = Vector3.zero;
    //        float rotz = 0;
    //        switch (currentSkillData.tabledataSkill.skillKey)
    //        {
    //            case SkillKey.None:
    //                break;
    //            case SkillKey.ThreeComboAttack:
    //                var powerfullattackobj = powerfullattackobjList[skillPrefabIndex];

    //                powerfullattackobj.particle.Stop();
    //                powerfullattackobj.gameObject.SetActive(true);
    //                powerfullattackobj.transform.position = powerfulAtkPos.position;

    //                powerfullattackobj.particle.Play();

    //                rotation = enemy.transform.position - this.transform.position;

    //                rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

    //                powerfullattackobj.transform.rotation = Quaternion.Euler(0, 0, rotz);

    //                skillPrefabIndex++;
    //                break;
    //            case SkillKey.MultipleFireball:
    //                if (explodeskillobj == null)
    //                {
    //                    explodeskillobj = SkillResourcesBundle.Loaded.multipleFireSkillView;
    //                }

    //                for (int i = 0; i < 10; i++)
    //                {
    //                    ViewMultipleMissile missileobj = null;
    //                    if (i >= missilePool.Count)
    //                    {
    //                        missileobj = Object.Instantiate(explodeskillobj.viewExplodeMissile);
    //                        missilePool.Add(missileobj);
    //                    }
    //                    else
    //                    {
    //                        missileobj = missilePool[i];
    //                    }
    //                    missileobj.particle.Stop();
    //                    missileobj.gameObject.SetActive(true);
    //                    missileobj.transform.position = powerfulAtkPos.position;
    //                    missileobj.particle.Play();

    //                    rotation = enemy.transform.position - this.transform.position;

    //                    rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg + ((i - 4) * 5.0f);

    //                    Vector3 dirVector = ConvertAngleToVector(rotz);
    //                    missileobj.MoveToArr(dirVector,0);
    //                }
    //                break;
    //            case SkillKey.IncreaseAttack:
    //                if (atkIncreaseSkillview == null)
    //                {
    //                    atkIncreaseSkillview = Object.Instantiate(SkillResourcesBundle.Loaded.atkIncreaseSkillview);
    //                }
    //                atkIncreaseSkillview.gameObject.SetActive(true);
    //                atkIncreaseSkillview.transform.position = this.transform.position;
    //                atkIncreaseSkillview.particle.Play();
    //                break;
    //            case SkillKey.FireRain:
    //                if (fireRainSkillEffect == null)
    //                {
    //                    fireRainSkillEffect = Object.Instantiate(SkillResourcesBundle.Loaded.viewFireRainSkill);
    //                }

    //                rotation = this.transform.position - enemy.transform.position;

    //                fireRainSkillEffect.gameObject.SetActive(true);
    //                fireRainSkillEffect.Fire(new Vector2(enemy.transform.position.x, enemy.transform.position.y+2), enemy.transform.position,0,0);
    //                break;
    //            case SkillKey.GuidedMissile:
    //                fireEffect.gameObject.SetActive(true);
    //                fireEffect.particle.Play();

    //                fireEffect.transform.position = transform.position;

    //                frontWave.gameObject.SetActive(true);
    //                //frontWave.Shoot(powerfulAtkPos.position,enemy.transform);

    //                backWave.gameObject.SetActive(true);
    //                //backWave.Shoot(powerfulAtkPos.position, enemy.transform);
    //                break;
    //            case SkillKey.Explode:

    //                ViewAllDamage effectObj = null;
    //                for (int i = 0; i < explodeSkillObj.Count; i++)
    //                {
    //                    if (explodeSkillObj[i].gameObject.activeInHierarchy == false)
    //                    {
    //                        effectObj = explodeSkillObj[i];
    //                        break;
    //                    }
    //                }
    //                if (effectObj == null)
    //                {
    //                    effectObj = Object.Instantiate(SkillResourcesBundle.Loaded.viewAllDmg);
    //                    explodeSkillObj.Add(effectObj);
    //                }

    //                effectObj.transform.position = enemy.transform.position;
    //                effectObj.gameObject.SetActive(true);
    //                effectObj.particle.Play();

    //                break;
    //            case SkillKey.IncreaseMoving:
    //                if (moveIncreaseSkillview == null)
    //                {
    //                    moveIncreaseSkillview = Object.Instantiate(SkillResourcesBundle.Loaded.moveIncreaseSkillView);
    //                }
    //                moveIncreaseSkillview.gameObject.SetActive(true);
    //                moveIncreaseSkillview.transform.position =this.transform.position;
    //                moveIncreaseSkillview.particle.Play();

    //                break;
    //            case SkillKey.PoolingPortal:
    //                poolingTrapSkill.gameObject.SetActive(true);
    //                poolingTrapSkill.transform.position = enemy.transform.position;
    //                break;
    //            case SkillKey.AbsorbLife:
    //                var absorbEffect = PoolManager.Pop<AbsorbLifeEffect>(viewabsorbLife.particleEffect, InGameObjectManager.Instance.transform, enemy.transform.position);
    //                absorbEffect.transform.position = enemy.transform.position;
    //                absorbEffect.particle.Play();
    //                absorbEffect.gameObject.SetActive(true);

    //                //var absorbBullet = PoolManager.Pop<AbsorbLifeBullet>(viewabsorbLife.particleBullet, InGameObjectManager.Instance.transform, enemy.transform.position);
    //                //absorbBullet.gameObject.SetActive(true);
    //                //absorbBullet.Shoot(enemy.transform.position,this.transform.position);
    //                break;
    //            case SkillKey.RecoverHpTick:
    //                if (viewRecovertickHp == null)
    //                {
    //                    viewRecovertickHp = Object.Instantiate(SkillResourcesBundle.Loaded.viewRecovertickHp);
    //                }
    //                viewRecovertickHp.gameObject.SetActive(true);
    //                viewRecovertickHp.transform.position = this.transform.position;
    //                viewRecovertickHp.particle.Play();
    //                break;
    //            case SkillKey.GodMode:
    //                viewCallGodSkill.gameObject.SetActive(true);
    //                viewCallGodSkill.transform.position =this.transform.position;
    //                viewCallGodSkill.particle.Play();

    //                cahracterRenderer.material = InGameResourcesBundle.Loaded.GodModeMat;
    //                break;
    //            case SkillKey.End:
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    Vector3 ConvertAngleToVector(float _deg)
    //    {
    //        var rad = _deg * Mathf.Deg2Rad;
    //        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    //    }

    //    void SetSpriteImage(UnitAnimSprtieType _type, int index)
    //    {
    //        if (animspriteinfo.ContainsKey(_type))
    //        {
    //            cahracterRenderer.sprite = animspriteinfo[_type].spriteList[index];
    //        }
    //    }

    //    bool IsInSpriteRange(UnitAnimSprtieType _type, int index)
    //    {
    //        if (animspriteinfo.ContainsKey(_type))
    //        {
    //            if (index >= animspriteinfo[_type].spriteList.Length)
    //            {
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            return false;
    //        }

    //        return true;
    //    }
    //}
}
