using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Tile_StairsWide : Tile, Dungeon.ILinkCreator
    {
        public void CreateLinks()
        {
            Transform t = transform;

            for (int i = 1; i <= 2; ++i)
            {
                Dungeon.Node source = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, t.position - t.forward * 3 - t.right * i + t.up * 2, 1.0f);
                Dungeon.Node target = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, t.position + t.forward * 1 - t.right * i, 1.0f);
                if (source != null && target != null)
                {
                    source.AddLink(new Link_Stair(source, target));
                    target.AddLink(new Link_Stair(target, source));
                }
            }
        }
    }
}