using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Tile_StairsThin : Tile, Dungeon.ILinkCreator
    {
        public void CreateLinks()
        {
            Transform t = transform;

            Dungeon.Node source = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, t.position - t.forward + t.right, 1.0f);
            Dungeon.Node target = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, t.position - t.forward - t.right * 3 + t.up * 2, 1.0f);
            if (source != null && target != null)
            {
                source.AddLink(new Link_Stair(source, target));
                target.AddLink(new Link_Stair(target, source));
            }
        }
    }
}