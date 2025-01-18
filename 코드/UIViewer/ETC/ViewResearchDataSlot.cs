using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewResearchDataSlot : MonoBehaviour
    {
        [SerializeField] Image Icon;
        [SerializeField] TMP_Text level;
        [SerializeField] TMP_Text title;

        [SerializeField] BTButton StartResearchBtn;
        [SerializeField] TMP_Text researchTime;
        [SerializeField] TMP_Text researchCost;

        [SerializeField] TMP_Text maxLevel;

        [SerializeField] GameObject sliderObject;
        [SerializeField] Slider timeSlider;
        [SerializeField] TMP_Text lefttime_Progressing;
        [SerializeField] GameObject InProgressBtnImage;
        
        [SerializeField] GameObject lockedObject;

        [SerializeField] GameObject maxLevelObj;

        ResearchTableData tabledata;
        System.DateTime exitTime;

        [SerializeField] TMP_Text maxLevelTxt;
        public ResearchUpgradeKey researchType
        {
            get
            {
                return tabledata.researchTypeKey;
            }
        }
        public void Init(ResearchUpgradeKey key)
        {
            tabledata=Player.Research.GetTableData(key);

            bool isUgrading = UpgradeUserData.upgradeState != ResearchUpgradeStateKey.Ready;
            if (isUgrading)
            {
                exitTime = Extension.GetDateTimeByIsoString(Player.Cloud.researchData.data[tabledata.index].scheduledExittime);
            }
            maxLevelTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_maxLevel].StringToLocal;

            UISettingForUpgradeType();
        }

        public void AddOpenDetailResearchWindowActionBtn(UnityAction callback)
        {
            StartResearchBtn.onClick.AddListener(callback);
        }

      
        public ViewResearchDataSlot SetIcon()
        {
            Icon.sprite = InGameResourcesBundle.Loaded.researchIconSprites[tabledata.index];
            return this;
        }

        public ViewResearchDataSlot SetName()
        {
            title.text=string.Format(StaticData.Wrapper.localizednamelist[(int)tabledata.nameLmk].StringToLocal, Player.Research.GetValue(tabledata.researchTypeKey));
            return this;
        }
        public ViewResearchDataSlot SetLevel()
        {
            level.text =Player.Research.GetLevel(tabledata.researchTypeKey).ToString();

            maxLevel.text = string.Format("MAX:{0}", StaticData.Wrapper.researchTableSequence[tabledata.index].maxLevel);
            return this;
        }

     
        public ViewResearchDataSlot SetCost()
        {
            researchCost.text = Player.Research.GetCost(tabledata.researchTypeKey).ToNumberString();
            return this;
        }

        public ViewResearchDataSlot SetProgressState()
        {
            exitTime = Extension.GetDateTimeByIsoString(Player.Cloud.researchData.data[tabledata.index].scheduledExittime);
            UISettingForUpgradeType();
            return this;
        }
        UserResearchUpgradeData UpgradeUserData
        {
            get
            {
                return Player.Cloud.researchData.data[tabledata.index];
            }
        }

        private void Update()
        {
            if (UpgradeUserData.upgradeState != ResearchUpgradeStateKey.Progressing)
                return;

            var currenttime = Player.Research.currentTime;
            var _leftTime = exitTime - currenttime;
            if(_leftTime.TotalSeconds>0)
            {
                timeSlider.value = currenttime.Ticks / exitTime.Ticks;
                lefttime_Progressing.text = string.Format("{0:D2}:{1:D2}:{2:D2}", _leftTime.Hours, _leftTime.Minutes, _leftTime.Seconds);
            }
            else
            {
                Player.Research.CompleteUpgrade(tabledata.researchTypeKey);
            }
        }

        void UISettingForUpgradeType()
        {
            StartResearchBtn.gameObject.SetActive(false);
            InProgressBtnImage.gameObject.SetActive(false);

            maxLevel.gameObject.SetActive(false);
            sliderObject.SetActive(false);
            int level = Player.Research.GetLevel(tabledata.researchTypeKey);

            switch (UpgradeUserData.upgradeState)
            {
                case ResearchUpgradeStateKey.Ready:
                    StartResearchBtn.gameObject.SetActive(true);
                    maxLevel.gameObject.SetActive(true);

                    double min;
                    if (tabledata.researchTypeKey >= ResearchUpgradeKey.SwordAttackIncrease_2)
                    {
                        min = Player.Research.GetTime(tabledata.researchTypeKey);
                    }
                    else
                    {
                        min = StaticData.Wrapper.researchTableSequence[tabledata.index].ResearchTime_min;
                    }


                    int hour = (int)(min / 60);
                    int minute = (int)(min % 60);
                    researchTime.text = string.Format("{0:D2}:{1:D2}:00",hour,minute);
                    break;
                case ResearchUpgradeStateKey.Progressing:
                    InProgressBtnImage.SetActive(true);
                    sliderObject.SetActive(true);
                    break;
                default:
                    break;
            }

            if(Player.Research.isMaxLevel(researchType))
            {
                StartResearchBtn.enabled = false;
                maxLevelObj.SetActive(true);
            }
            else
            {
                StartResearchBtn.enabled = true;
                maxLevelObj.SetActive(false);
            }

        }
    }

    
}
