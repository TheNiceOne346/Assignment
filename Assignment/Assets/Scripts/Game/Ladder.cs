using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Ladder : MonoBehaviour, Dungeon.ILinkCreator
    {
        public void CreateLinks()
        {
            Dungeon.Node source = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, transform.position, 1.5f);
            Dungeon.Node target = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, transform.position - transform.forward + Vector3.up * 2.0f, 1.5f);

            if (source != null && target != null)
            {
                source.AddLink(new Link_Ladder(source, target, -transform.forward));
                target.AddLink(new Link_Ladder(target, source, -transform.forward));
            }
        }
    }
}