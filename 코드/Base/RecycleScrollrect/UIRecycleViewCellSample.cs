using UnityEngine;
using UnityEngine.UI;

namespace BlackTree
{
    public class UICellSampleData
    {
        public int index;
        public string name;
    }

    public class UIRecycleViewCellSample : UIRecycleViewCell<UICellSampleData>
    {
        [SerializeField]
        private Text nIndex;
        [SerializeField]
        private Text txtName;

        public override void UpdateContent(UICellSampleData itemData)
        {
            nIndex.text = itemData.index.ToString();
            txtName.text = itemData.name;
        }

        public void onClickedButton()
        {
            //Debug.Log(nIndex.text.ToString());
        }
    }
}