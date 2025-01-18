using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewCanvasMainTop : ViewCanvas
    {
        //ÇÁ·ÎÇÊ
        public BTButton ProfileButton;

        public BTButton menuButton;
        public GameObject menuRedDot;
        public GameObject[] redDotList;
        public GameObject questRedDot;
        public RectTransform menuWindow;
        public Vector3 menuOpenPos = new Vector3(0,-88,0);
        public Vector3 menuClosePos = new Vector3(0, 420, 0);
        public BTButton menuClose;

        public BTButton optionButton;
        public BTButton MailBtn;
        public BTButton InformBtn;
        public BTButton sleepmodeBtn;
        public BTButton attendBtn;
        public BTButton rankingBtn;
        public BTButton inventoryBtn;
        public BTButton[] goodsBtn;

        public BTButton questBtn;

        public TMP_Text optionicontxt;
        public TMP_Text mailicontxt;
        public TMP_Text savingicontxt;
        public TMP_Text rankicontxt;
        public TMP_Text attentionicontxt;
        public TMP_Text informicontxt;
        public TMP_Text inventoryicontxt;
        public TMP_Text questicontxt;

        [Header("Money")]
        [SerializeField] private ObtainGood[] _obtainGoods;

        [Header("profile")]
        [SerializeField] public TMP_Text nickName;
        [SerializeField] public TMP_Text userLv;
        [SerializeField] public TMP_Text userCombatValue;
        [SerializeField] public Image expfillAmount;
        [SerializeField] public Slider hpSlider;
        [SerializeField] public TMP_Text hpText;
        
        [SerializeField] public Slider shieldSlider;
        [SerializeField] public TMP_Text shieldText;

        [Header("goodsParticle")]
        [SerializeField] public Transform goodsParticleTarget;
        [SerializeField] public Transform goodsParticleParent;
        [SerializeField] GoodsParticleAttraction[] goodsParticlePrefab;
        public ObtainGood[] ObtainGoods => _obtainGoods;
        public void Init()
        {
            foreach (var obtain in _obtainGoods)
            {
                obtain.Init();
            }
            optionicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Option].StringToLocal;
            mailicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Mail].StringToLocal;
            savingicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Saving].StringToLocal;
            rankicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Ranking].StringToLocal;
            attentionicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Attention].StringToLocal;
            informicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Inform].StringToLocal;
            inventoryicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Inventory].StringToLocal;
            questicontxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Quest].StringToLocal;

            Battle.Field.goodsParticleEffect += ActivateParticle;
        }

        public void ActivateParticle(Vector2 startPoint,Definition.RewardTypes rewardType )
        {
            var goodsUIParticle = PoolManager.Pop<GoodsParticleAttraction>(goodsParticlePrefab[(int)rewardType], goodsParticleParent, Vector2.zero);
            goodsUIParticle.ActivateGoodsParticle(startPoint, goodsParticleTarget);
        }
    }

}
