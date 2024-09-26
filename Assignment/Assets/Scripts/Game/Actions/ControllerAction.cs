using AI;
using AI.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions
{
    public abstract class ControllerAction
    {
        private Controller      m_controller;

        #region Properties

        public Controller Controller => m_controller;

        #endregion

        public ControllerAction(Controller controller)
        {
            m_controller = controller;
        }

        public abstract IEnumerator PerformAction();
    }
}