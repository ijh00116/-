using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;

public class ViewMeteorSkill : MonoBehaviour
{
    Vector3 dir = new Vector3(1, -1, 0);
    float currentTime = 0;

    Vector2 endPos;
    [SerializeField] float movespeed = 5f;



    public bool isActive = false;

    Player.Skill.SkillCacheData skillcache;

    WitchSkillType witchSkilltype;
    [SerializeField] ParticleSystem particle;
    public void Fire(Vector2 _startPos, Vector2 _endPos, Player.Skill.SkillCacheData _skillcache, WitchSkillType witchtype)
    {
        particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
        isActive = true;
        endPos = _endPos;
        skillcache = _skillcache;

        this.transform.position = _startPos;

        dir = (endPos - _startPos).normalized;

        witchSkilltype = witchtype;
        currenttime = 0;


    }

    float currenttime = 0;
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        currenttime += Time.deltaTime;
        if(currenttime>=3)
        {
            isActive = false;
            this.gameObject.SetActive(false);
            currenttime = 0;
        }
        //lerp·Î º¯°æ
        transform.Translate(dir * Time.deltaTime * (float)movespeed * 5);

        var distance = Vector2.Distance(this.transform.position, endPos);
        
        if (distance <= 2.0f)
        {
            currentTime = 0;
            var explode = PoolManager.Pop(SkillResourcesBundle.Loaded.meteorExplode, InGameObjectManager.Instance.transform, this.transform.position);
            var fire= PoolManager.Pop(SkillResourcesBundle.Loaded.meteorFire, InGameObjectManager.Instance.transform, this.transform.position);
            
            explode.Fire(skillcache, witchSkilltype);
            fire.Fire(skillcache, witchSkilltype);

            explode.transform.position = this.transform.position;
            fire.transform.position = this.transform.position;

            this.gameObject.SetActive(false);

         

            InGameObjectManager.Instance.ShakeCam(CamShakeType.Skill_2);
            isActive = false;
        }
    }
}
