using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions
{
    public class Action_MoveTowards : ControllerAction
    {
        private DungeonLink     m_moveLink;

        public Action_MoveTowards(Controller controller, Controller target) : this(controller, target.Node != null ? target.Node : GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, target.transform.position))
        {            
        }

        public Action_MoveTowards(Controller controller, Vector3 vTarget) : this(controller, GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, vTarget))
        {
        }

        public Action_MoveTowards(Controller controller, Dungeon.Node targetNode) : base(controller)
        {
            // get closest node
            if (targetNode != null && 
                targetNode.Owner != null && 
                targetNode.Owner != controller)
            {
                float fClosestNode = float.MaxValue;
                Dungeon.Node newTarget = null;
                foreach (ILink link in targetNode.Links)
                {
                    if (link is Link_Normal &&
                        link.Target is Dungeon.Node target &&
                        (target.Owner == null || target.Owner == controller))
                    {
                        float fDistance = Vector3.Distance(controller.transform.position, target.WorldPosition);
                        if (fDistance < fClosestNode)
                        {
                            fClosestNode = fDistance;
                            newTarget = target;
                        }
                    }
                }

                targetNode = newTarget;
            }

            if (targetNode != null)
            {
                GraphAlgorithms.Path path = Dungeon.Instance.GetShortestPath(controller, controller.Node, targetNode);
                if (path != null && path.Count > 0)
                {
                    Controller.LastPath = path;
                    m_moveLink = path[0] as DungeonLink;
                }
            }
        }

        public override IEnumerator PerformAction()
        {
            if (m_moveLink != null)
            {
                // claim target node
                (m_moveLink.Target as Dungeon.Node).Owner = Controller;

                // do move
                yield return m_moveLink.MoveController(Controller);

                // move unit node
                Controller.Node = m_moveLink.Target;                
            }
        }
    }
}