using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using AssetKits.ParticleImage;
using BlackTree.Definition;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class BattlePassSlot : MonoBehaviour
    {
        public BTButton rewardBtn;
        public Image iconFrame;
        public Image icon;
        public Image skillicon;
        public TMP_Text itemCount;

        public GameObject purchasedLock;
        public GameObject checkedRewardObj;
        public GameObject Locked;
        public TMP_Text lockedLevel;

        bool isImNormal;
        BattlePassData passData;
        public void Init(BattlePassData _passData,int tier,bool isnormal)
        {
            passData = _passData;
            isImNormal = isnormal;

            rewardBtn.onClick.RemoveAllListeners();
            rewardBtn.onClick.AddListener(()=> {
                Model.Player.Pass.GiveReward(passData.lockType, tier, passData.index, isImNormal);
            });

            BattlsPassHistory myRewardHistory = null;
            List<bool> myPurchaseHistory = null;
            switch (passData.lockType)
            {
                case ContentLockType.UnitLevel:
                    if(tier==0)
                    {
                        myRewardHistory = Player.Cloud.battlepassHistory.levelPassHistory_0;
                    }
                    if (tier == 1)
                    {
                        myRewardHistory = Player.Cloud.battlepassHistory.levelPassHistory_1;
                    }
                    if (tier == 2)
                    {
                        myRewardHistory = Player.Cloud.battlepassHistory.levelPassHistory_2;
                    }
                    myPurchaseHistory = Player.Cloud.battlepassPurchaseHistory.levelPassPurchased;
                    break;
                case ContentLockType.ChapterLevel:
                    if (tier == 0)
                    {
                        myRewardHistory = Player.Cloud.battlepassHistory.chapterPassHistory_0;
                    }
                    if (tier == 1)
                    {
                        myRewardHistory = Player.Cloud.battlepassHistory.chapterPassHistory_1;
                    }
                    if (tier == 2)
                    {
                        myRewardHistory = Player.Cloud.battlepassHistory.chapterPassHistory_2;
                    }
                    myPurchaseHistory = Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased;
                    break;
                default:
                    break;
            }





            if (isnormal)
            {
                bool isSkill = isRewardTypeSkill(passData.goodskey_normal);
                if (isSkill)
                {
                    icon.gameObject.SetActive(false);
                    skillicon.gameObject.SetActive(true);
                    skillicon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)passData.goodskey_normal];
                }
                else
                {
                    icon.gameObject.SetActive(true);
                    skillicon.gameObject.SetActive(false);
                    icon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)passData.goodskey_normal];
                }
                iconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[(int)passData.goodskey_normal];
                itemCount.text = passData.goodsAmount_normal.ToString();
                purchasedLock.SetActive(false);
                rewardBtn.enabled = false;

                bool isLocked = IsLockedCurrent(passData.lockType,passData.unLockLevel);
                if(isLocked)
                {
                    checkedRewardObj.SetActive(false);
                    Locked.SetActive(true);
                    if(passData.lockType==ContentLockType.ChapterLevel)
                    {
                        lockedLevel.text = string.Format("{0}-1\nChapter", (int)(passData.unLockLevel/5));
                    }
                    else
                    {
                        lockedLevel.text = string.Format("{0}\nLevel",passData.unLockLevel);
                    }
                }
                else
                {
                    Locked.SetActive(false);
                    if (myRewardHistory.normalRecieved[passData.index])
                    {
                        checkedRewardObj.SetActive(true);
                    }
                    else
                    {
                        checkedRewardObj.SetActive(false);
                        rewardBtn.enabled = true;
                    }
                    if (passData.lockType == ContentLockType.ChapterLevel)
                    {
                        lockedLevel.text = string.Format("{0}-1\nChapter", (int)(passData.unLockLevel / 5));
                    }
                    else
                    {
                        lockedLevel.text = string.Format("{0}\nLevel", passData.unLockLevel);
                    }
                }
            }
            else
            {
                bool isSkill = isRewardTypeSkill(passData.goodskey_premium);
                if(isSkill)
                {
                    icon.gameObject.SetActive(false);
                    skillicon.gameObject.SetActive(true);
                    skillicon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)passData.goodskey_premium];
                }
                else
                {
                    icon.gameObject.SetActive(true);
                    skillicon.gameObject.SetActive(false);
                    icon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)passData.goodskey_premium];
                }
                
                iconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[(int)passData.goodskey_premium];
                itemCount.text = passData.goodsAmount_premium.ToString();
                purchasedLock.SetActive(!myPurchaseHistory[tier]);
                rewardBtn.enabled = false;
                bool isLocked = IsLockedCurrent(passData.lockType, passData.unLockLevel);
                if (isLocked)
                {
                    checkedRewardObj.SetActive(false);
                    Locked.SetActive(true);
                    if (passData.lockType == ContentLockType.ChapterLevel)
                    {
                        lockedLevel.text = string.Format("{0}-1\nChapter", (int)(passData.unLockLevel / 5));
                    }
                    else
                    {
                        lockedLevel.text = string.Format("{0}\nLevel", passData.unLockLevel);
                    }
                }
                else
                {
                    Locked.SetActive(false);
                    if (myRewardHistory.premiumRecieved[passData.index])
                    {
                        checkedRewardObj.SetActive(true);
                    }
                    else
                    {
                        rewardBtn.enabled = myPurchaseHistory[tier];
                        checkedRewardObj.SetActive(false);
                        
                    }
                    if (passData.lockType == ContentLockType.ChapterLevel)
                    {
                        lockedLevel.text = string.Format("{0}-1\nChapter", (int)(passData.unLockLevel / 5));
                    }
                    else
                    {
                        lockedLevel.text = string.Format("{0}\nLevel", passData.unLockLevel);
                    }
                }
            }
            
        }

        bool isRewardTypeSkill(RewardTypes _type)
        {
            bool isSkill = false;
            switch (_type)
            {
                case RewardTypes.package_swordfewhit:
                case RewardTypes.package_magicfewhit:
                case RewardTypes.package_setturret:
                case RewardTypes.package_companionspawn:
                case RewardTypes.package_guidedmissile:
                case RewardTypes.package_godmode:
                case RewardTypes.package_summon:
                case RewardTypes.package_nova:
                case RewardTypes.package_meteor:
                case RewardTypes.package_multielectric:
                case RewardTypes.skillAwakeStone:
                case RewardTypes.package_skyLight:
                    isSkill = true;
                    break;
                default:
                    break;
            }
            return isSkill;
        }



        bool IsLockedCurrent(ContentLockType lockType,int goal)
        {
            bool isLocked = true;

            int currentChapter = (Player.Cloud.field.bestChapter+1) * 5 + Player.Cloud.field.bestStage;
            int currentLevel = Player.Cloud.userLevelData.currentLevel;
            switch (lockType)
            {
                case ContentLockType.UnitLevel:
                    if(currentLevel>=goal)
                    {
                        isLocked = false;
                    }
                    break;
                case ContentLockType.ChapterLevel:
                    if (currentChapter >= goal)
                    {
                        isLocked = false;
                    }
                    break;
                default:
                    break;
            }

            return isLocked;
        }
    }

}
