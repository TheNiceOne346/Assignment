using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace Game
{
    public class Link_Ladder : DungeonLink
    {
        private Vector3 m_vForward;

        #region Properties

        #endregion

        public Link_Ladder(Dungeon.Node source, Dungeon.Node target, Vector3 vForward) : base(source, target)
        {
            m_vForward = vForward;
        }

        public override IEnumerator MoveController(Controller controller)
        {
            while (true)
            {
                // reached target?
                if (Vector3.Distance(controller.transform.position, Target.WorldPosition) < 0.01f)
                {
                    // done!
                    break;
                }
                else
                {
                    // move towards next node
                    controller.transform.position = Vector3.MoveTowards(controller.transform.position, Target.WorldPosition, Time.deltaTime * controller.MovementSpeed * 0.6f);

                    // rotate towards to face ladder
                    controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, Quaternion.LookRotation(m_vForward), Time.deltaTime * 4.0f);

                    yield return null;
                }
            }
        }

    }
}