using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BlackTree.Bundles
{
    public class AnimatorEvent : MonoBehaviour
    {
        private Action attackEvent;
        private Action animEndEvent;

        public void SetEndCallback(Action callback)
        {
            animEndEvent = callback;
        }
        public void SetAttackCallback(Action callback)
        {
            attackEvent = callback;
        }

        public void TriggerAttackCallback()
        {
            attackEvent?.Invoke();
        }
        public void TriggerAttackEndCallback()
        {
            animEndEvent?.Invoke();
        }
    }
}
