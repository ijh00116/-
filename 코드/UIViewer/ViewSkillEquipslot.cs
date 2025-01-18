using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewSkillEquipslot : ViewBase
    {
        public Image FrameImage;
        public Image skillIcon;
        public Image cooltimeImage;

        public GameObject locked;
        public GameObject canPlusObj;

        public BTButton skillInfoBtn;
        public TMP_Text leftTimeText;
        public void Init(Model.Player.Skill.SkillCacheData skillCache , ContentState lockState)
        {
            SyncInfo(skillCache, lockState);
        }

        public void SyncInfo(Model.Player.Skill.SkillCacheData skillCache, ContentState lockState)
        {
            switch (lockState)
            {
                case ContentState.Locked:
                    skillIcon.gameObject.SetActive(false);
                    locked.SetActive(true);
                    canPlusObj.SetActive(false);
                    break;
                case ContentState.UnLocked:
                    locked.SetActive(false);
                    if (skillCache == null)
                    {
                        skillIcon.gameObject.SetActive(false);
                        canPlusObj.SetActive(true);
                        cooltimeImage.gameObject.SetActive(false);
                        leftTimeText.gameObject.SetActive(false);
                        FrameImage.sprite= InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[0];
                    }
                    else
                    {
                        skillIcon.gameObject.SetActive(true);
                        canPlusObj.SetActive(false);
                        skillIcon.gameObject.SetActive(skillCache.IsEquiped);
                        skillIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skillCache.tabledataSkill.skillKey];
                        FrameImage.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[(int)skillCache.tabledataSkill.grade-1];
                    }
                    break;
                default:
                    break;
            }
        }

     

        enum shakestate
        {
            goright, left, right, gomid
        }
        float currenttime;
        float speed = 14.0f;
        Vector3 originalangle = Vector3.zero;
        Coroutine enduceanim;

        public void StartEnduceaction()
        {
            transform.rotation = Quaternion.Euler(originalangle);
            if (enduceanim == null)
            {
                enduceanim = StartCoroutine(Shake());
            }
        }
        WaitForSeconds waitshake = new WaitForSeconds(0.5f);
        IEnumerator Shake()
        {
            shakestate _state = shakestate.left;
            currenttime = 0;
            var lefttargetangle = new Vector3(originalangle.x, originalangle.y, originalangle.z - 4.0f);
            var righttargetangle = new Vector3(originalangle.x, originalangle.y, originalangle.z + 4.0f);
            int count = 0;
            while (true)
            {

                switch (_state)
                {
                    case shakestate.goright:
                        transform.rotation = Quaternion.Euler(Vector3.Lerp(originalangle, righttargetangle, currenttime));
                        currenttime += Time.deltaTime * speed;
                        if (currenttime >= 1)
                        {
                            currenttime = 0;
                            _state = shakestate.left;
                        }
                        break;
                    case shakestate.left:
                        transform.rotation = Quaternion.Euler(Vector3.Lerp(righttargetangle, lefttargetangle, currenttime));
                        currenttime += Time.deltaTime * speed;
                        if (currenttime >= 1)
                        {
                            currenttime = 0;
                            _state = shakestate.right;
                        }
                        break;
                    case shakestate.right:
                        transform.rotation = Quaternion.Euler(Vector3.Lerp(lefttargetangle, righttargetangle, currenttime));
                        currenttime += Time.deltaTime * speed;
                        if (currenttime >= 1)
                        {
                            currenttime = 0;
                            _state = shakestate.left;
                            count++;
                        }
                        if (count >= 2)
                        {
                            currenttime = 0;
                            _state = shakestate.gomid;
                        }
                        break;
                    case shakestate.gomid:
                        transform.rotation = Quaternion.Euler(Vector3.Lerp(righttargetangle, originalangle, currenttime));
                        currenttime += Time.deltaTime * speed * 2;
                        if (currenttime >= 1)
                        {
                            count = 0;
                            yield return waitshake;
                            _state = shakestate.goright;
                        }
                        break;
                    default:
                        break;
                }
                yield return null;
            }
        }

        public void StopEnduceaction()
        {
            if (enduceanim != null)
            {
                StopCoroutine(enduceanim);
                enduceanim = null;
                transform.rotation = Quaternion.Euler(originalangle);
            }


        }
    }

}
