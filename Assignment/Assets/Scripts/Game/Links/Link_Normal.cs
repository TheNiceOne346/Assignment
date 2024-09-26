using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.UI.CanvasScaler;

namespace Game
{
    public class Link_Normal : DungeonLink
    {
        #region Properties

        #endregion

        public Link_Normal(Dungeon.Node source, Dungeon.Node target) : base(source, target)
        {
        }

        public IEnumerator MoveUnit(Controller controller, Vector3 vTarget)
        {
            while (true)
            {
                // reached target?
                if (Vector3.Distance(controller.transform.position, vTarget) < 0.01f)
                {
                    // done!
                    break;
                }
                else
                {
                    // move towards next node
                    controller.transform.position = Vector3.MoveTowards(controller.transform.position, vTarget, Time.deltaTime * controller.MovementSpeed);

                    // rotate towards target (in 2D)
                    Vector3 vForward = vTarget - controller.transform.position;
                    if (vForward.magnitude > 0.001f)
                    {
                        vForward.y = 0.0f;
                        vForward = vForward.normalized;
                        controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, Quaternion.LookRotation(vForward), Time.deltaTime * 4.0f);
                    }

                    // add some bounce!
                    controller.Bounce();

                    yield return null;
                }
            }
        }

        public override IEnumerator MoveController(Controller controller)
        {
            yield return MoveUnit(controller, Target.WorldPosition);
        }
    }
}