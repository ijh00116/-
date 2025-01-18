using UnityEngine;

namespace BlackTree.Bundles
{
    public class BTSelector : MonoBehaviour
    {
        public GameObject[] panels = null;
        public GameObject[] inactivepanels = null;

        public void Show(int index)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].SetActive(index == i);
                if(i<inactivepanels.Length)
                    inactivepanels[i].SetActive(index != i);
            }
        }
    }
}
