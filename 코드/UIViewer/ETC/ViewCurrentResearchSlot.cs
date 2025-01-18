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
    public class ViewCurrentResearchSlot : MonoBehaviour
    {
        public GameObject inProgress;
        public GameObject notProgress;

        public Image Icon;
        public TMP_Text level;
        public TMP_Text title;
        public TMP_Text desc;

        public TMP_Text leftTime_progressing;
        public Slider leftTimeBar;

        public TMP_Text completeResearchCost;
        public BTButton completeResearchBtn;

        [HideInInspector]
        public ResearchUpgradeKey currentResearch;
        ResearchTableData tabledata;
        System.DateTime exitTime;

        double diaForJustComplete;

        public TMP_Text startResearch;
        public TMP_Text researchCompleteNowTxt;
        UserResearchUpgradeData UpgradeUserData
        {
            get
            {
                return Player.Cloud.researchData.data[tabledata.index];
            }
        }
        private void Awake()
        {
            completeResearchBtn.onClick.AddListener(CompleteUpgrade);
            startResearch.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ResearchSlot].StringToLocal;
            researchCompleteNowTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_NowComplete].StringToLocal;
        }

        public void Init(ResearchUpgradeKey researchKey)
        {
            currentResearch = researchKey;

            if (currentResearch==ResearchUpgradeKey.None)
            {
                inProgress.SetActive(false);
                notProgress.SetActive(true);
                this.transform.SetAsLastSibling();
            }
            else
            {
                tabledata = Player.Research.GetTableData(researchKey);

                inProgress.SetActive(true);
                notProgress.SetActive(false);

                Icon.sprite = InGameResourcesBundle.Loaded.researchIconSprites[tabledata.index];
                int lv = Player.Research.GetLevel(researchKey);
                level.text = string.Format("{0}><color=green>{1}</color>", lv, lv+1);
                title.text = string.Format(StaticData.Wrapper.localizednamelist[(int)tabledata.nameLmk].StringToLocal, Player.Research.GetValue(tabledata.researchTypeKey));
                desc.text = string.Format("{0} > {1}", Player.Research.GetValue(researchKey).ToNumberString(), Player.Research.GetNextValue(researchKey).ToNumberString());

                exitTime = Extension.GetDateTimeByIsoString(Player.Cloud.researchData.data[tabledata.index].scheduledExittime);
                this.transform.SetAsFirstSibling();
            }

        }

        void CompleteUpgrade()
        {
            if(Player.Good.Get(GoodsKey.Dia)>= diaForJustComplete)
            {
                Player.ControllerGood.Consume(GoodsKey.Dia, diaForJustComplete);
                Player.Research.CompleteUpgrade(tabledata.researchTypeKey);
            }
            else
            {
                string desc = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DiaNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(desc);
            }
            
        }

        private void Update()
        {
            if (currentResearch == ResearchUpgradeKey.None)
                return;
            if (UpgradeUserData.upgradeState == ResearchUpgradeStateKey.Ready)
                return;
            var currenttime = Player.Research.currentTime;
            var _leftTime = exitTime - currenttime;
            if (_leftTime.TotalSeconds > 0)
            {
                diaForJustComplete = (_leftTime.TotalSeconds/60.0f)*40;
                completeResearchCost.text = string.Format(diaForJustComplete.ToNumberString());
                leftTimeBar.value = currenttime.Ticks / exitTime.Ticks;
                leftTime_progressing.text = string.Format("{0:D2}:{1:D2}:{2:D2}",_leftTime.Hours,_leftTime.Minutes,_leftTime.Seconds);
            }
        }
    }

}
