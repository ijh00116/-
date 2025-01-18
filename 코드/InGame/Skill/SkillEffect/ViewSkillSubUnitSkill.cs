using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewSkillSubUnitSkill : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer characterRenderer;
        [SerializeField] public AnimSpriteInfo[] animSpriteinfoList;

        public Dictionary<UnitAnimSprtieType, SpriteAnimInfo> animspriteinfo = new Dictionary<UnitAnimSprtieType, SpriteAnimInfo>();
        public void Init()
        {
            for (int i = 0; i < animSpriteinfoList.Length; i++)
            {
                animspriteinfo.Add(animSpriteinfoList[i].spriteType, animSpriteinfoList[i].animInfo);
            }
        }

        private void Update()
        {
            this.transform.position = new Vector2(Player.Unit.userUnit._view.transform.position.x - 2.5f, Player.Unit.userUnit._view.transform.position.y + 1.5f);
        }
        public void SetSpriteImage(UnitAnimSprtieType _type, int index)
        {
            if (animspriteinfo.ContainsKey(_type))
            {
                characterRenderer.sprite = animspriteinfo[_type].spriteList[index];
            }
        }

        public bool IsInSpriteRange(UnitAnimSprtieType _type, int index)
        {
            if (animspriteinfo.ContainsKey(_type))
            {
                if (index >= animspriteinfo[_type].spriteList.Length)
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
    }

}
