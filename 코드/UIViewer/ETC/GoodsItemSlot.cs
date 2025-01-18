using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class GoodsItemSlot : MonoBehaviour
    {
        public TMP_Text goodsName;
        public TMP_Text goodsAmount;
        public Image goodsImage;
        public TMP_Text goodsDesc;

        public void Set(DataGood goodsData)
        {
            goodsImage.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)goodsData.key];
            goodsAmount.text = Player.ControllerGood.GetValue(goodsData.key).ToNumberString();

            string _name = StaticData.Wrapper.localizednamelist[(int)goodsData.nameLmk].StringToLocal;
            string _desc = StaticData.Wrapper.localizeddesclist[(int)goodsData.descLmk].StringToLocal;

            goodsName.text = _name;
            goodsDesc.text = _desc;
        }
    }

}
