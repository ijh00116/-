using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree.Core
{
    public static class BTHelper 
    {
        public static double Lerp(double a, double b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }
    }
}
