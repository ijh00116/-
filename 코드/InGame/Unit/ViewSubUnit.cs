using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewSubUnit : MonoBehaviour
    {
        [HideInInspector] public int hash;
        [SerializeField] public AnimSpriteInfo[] animSpriteinfoList;
        public Transform stunObjectParent;

        ControllerSubUnitInGame _unit;

        public Dictionary<UnitAnimSprtieType, SpriteAnimInfo> animspriteinfo = new Dictionary<UnitAnimSprtieType, SpriteAnimInfo>();

        [SerializeField]public SpriteRenderer spriteRenderer;

        public Rigidbody2D rb;
        public float rotateSpeed = 200f;
        public float movespeed = 5f;
        public void Init(ControllerSubUnitInGame unit)
        {
            _unit = unit;
            hash = GetHashCode();
            rb = this.GetComponent<Rigidbody2D>();
            spriteRenderer = unit._renderView.spriteRenderer;
            stunObjectParent= unit._renderView.stunObjectParent;

            for (int i = 0; i < animSpriteinfoList.Length; i++)
            {
                animspriteinfo.Add(animSpriteinfoList[i].spriteType, animSpriteinfoList[i].animInfo);
            }
        }

        private void Update()
        {
            _unit._renderView.transform.position = this.transform.position;


        }
        public void SetSpriteImage(UnitAnimSprtieType _type, int index)
        {
            if (animspriteinfo.ContainsKey(_type))
            {
                spriteRenderer.sprite = animspriteinfo[_type].spriteList[index];
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
