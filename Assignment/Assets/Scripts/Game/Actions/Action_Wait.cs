using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions
{
    public class Action_Wait : ControllerAction
    {
        private float m_fWaitTime;

        public Action_Wait(Controller controller, float fWaitTime) : base(controller)
        {
            m_fWaitTime = fWaitTime;
        }

        public override IEnumerator PerformAction()
        {
            yield return new WaitForSeconds(m_fWaitTime);
        }
    }
}