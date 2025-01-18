using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections;

namespace BlackTree.Bundles
{
    public class ViewCanvasExit : ViewCanvas
    {
        public BTButton[] CancelExit;
        public BTButton ExitGame;
        public Image animImage;
        public Sprite[] sprites;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.SetVisible(true);
            }
        }
    }
}

