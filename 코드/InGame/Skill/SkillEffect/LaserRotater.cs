using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRotater : MonoBehaviour
{
    Player.Skill.SkillCacheData skillCache;
    public void Activate(Player.Skill.SkillCacheData _skillCache)
    {
        skillCache = _skillCache;
    }

    private void FixedUpdate()
    {
        //endLaser 이동
        transform.RotateAround(Player.Unit.usersubUnit._renderView.transform.position, Vector3.back, Time.deltaTime * 30);
        //endLaser 이동
    }
}
