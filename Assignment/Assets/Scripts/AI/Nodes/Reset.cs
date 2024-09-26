using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Nodes
{
    public class Reset : DecoratorNode
    {
        protected override void OnStop()
        {
            base.OnStop();

            if (m_state == State.Failure)
            {
                // reset all children
                Tree.Traverse(this, (n) => 
                {
                    if (n != this)
                    {
                        n.m_bStarted = false; n.m_state = State.Running;
                    }
                });
            }
        }

        protected override State OnUpdate()
        {
            // updating child
            if (m_child != null)
            {
                m_state = m_child.Update();
            }

            return m_state;
        }
    }
}