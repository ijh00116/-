using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerGameExit
    {
        ViewCanvasExit viewcanvasExit;
        CancellationTokenSource _cts;

        public ControllerGameExit(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;

            viewcanvasExit = ViewCanvas.Create<ViewCanvasExit>(parent);
            for(int i=0; i< viewcanvasExit.CancelExit.Length; i++)
            {
                viewcanvasExit.CancelExit[i].onClick.AddListener(() => {
                    viewcanvasExit.SetVisible(false);
                });
            }
            
            viewcanvasExit.ExitGame.onClick.AddListener(OutGame);

            viewcanvasExit.SetVisible(false);

            currentIndex = 0;
            Main().Forget();
        }

        int currentIndex;

        async UniTaskVoid Main()
        {
            while (true)
            {
                if(viewcanvasExit.IsVisible)
                {
                    viewcanvasExit.animImage.sprite = viewcanvasExit.sprites[currentIndex];
                    currentIndex++;
                    if (currentIndex >= viewcanvasExit.sprites.Length)
                    {
                        currentIndex = 0;
                    }
                }
                await UniTask.DelayFrame(3);
            }
        }

        void OutGame()
        {
            Player.SaveUserDataToFirebaseAndLocal().Forget();
            Application.Quit();
        }
    }


}
