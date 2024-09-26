using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Node_Trap : Node_Normal
    {
        #region Properties

        public override float AdditionalCost => 4;

        #endregion

        public Node_Trap(Vector3Int vPosition, Tile tile) : base(vPosition, tile)
        {
        }

        public override void OnEnter(Controller controller)
        {
            base.OnEnter(controller);

            if (m_tile is Tile_Trap trap)
            {
                trap.Trigger();
            }           
        }
    }
}
