using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace Game
{
    public class Link_Stair : DungeonLink
    {
        #region Properties

        #endregion

        public Link_Stair(Dungeon.Node source, Dungeon.Node target) : base(source, target)
        {
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
                    controller.transform.position = Vector3.MoveTowards(controller.transform.position, Target.WorldPosition, Time.deltaTime * controller.MovementSpeed * 0.8f);

                    // rotate towards to face forward 
                    Vector3 vForward = Direction;
                    vForward.y = 0.0f;
                    controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, Quaternion.LookRotation(vForward.normalized), Time.deltaTime * 4.0f);

                    yield return null;
                }
            }
        }

    }
}