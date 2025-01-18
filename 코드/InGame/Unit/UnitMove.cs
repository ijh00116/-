using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace BlackTree
{
    public class UnitMove : MonoBehaviour
    {
        public IAstarAI ai;

        public void Init()
        {

        }
        private void OnEnable()
        {
            if (ai == null)
                ai = GetComponent<IAstarAI>();
            ai.onSearchPath += Update;
        }

        private void OnDisable()
        {
            if(ai!=null)
            ai.onSearchPath -= Update;
        }

        private void Update()
        {
            if (ai != null)
                ai.destination =Model.Player.Unit.userUnit._view.transform.position;
        }
    }

}
