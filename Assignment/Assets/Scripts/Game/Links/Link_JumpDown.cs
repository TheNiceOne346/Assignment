using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace Game
{
    public class Link_JumpDown : DungeonLink
    {
        #region Properties

        public override float AdditionalCost => 5;

        #endregion

        public Link_JumpDown(Dungeon.Node source, Dungeon.Node target) : base(source, target)
        {
        }

        public override IEnumerator MoveController(Controller controller)
        {
            AnimationCurve jumpCurve = new AnimationCurve(new Keyframe[]{
                new Keyframe(0.0f, 0.0f),
                new Keyframe(0.2f, 1.5f),
                new Keyframe(1.0f, 0.0f)
            });

            for (float f = 0.0f; f <= 1.0f; f += Time.deltaTime)
            {
                controller.transform.position = Vector3.Lerp(Source.Position, Target.Position, f) +
                                                             jumpCurve.Evaluate(f) * Vector3.up;
                yield return null;
            }
        }
    }
}