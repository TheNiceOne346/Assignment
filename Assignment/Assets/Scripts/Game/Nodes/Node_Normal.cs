using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

namespace Game
{
    public class Node_Normal : Dungeon.Node
    {
        static Vector3Int[] sm_directions = new Vector3Int[]
        {
            new Vector3Int(-1, 0, -1),  new Vector3Int(0, 0, -1),   new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, 0),                               new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 1),   new Vector3Int(0, 0, 1),    new Vector3Int(1, 0, 1)
        };

        static Vector3Int[] sm_jumpDirections = new Vector3Int[]
        {
            new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1),
        };

        public Node_Normal(Vector3Int vPosition, Tile tile) : base(vPosition, tile)
        {
        }

        public override void CreateLinks()
        {
            m_links.Clear();

            // normal links
            foreach (Vector3Int vDir in sm_directions)
            {
                Vector3Int vTarget = m_vPosition + vDir;
                Dungeon.Node target = Dungeon.Instance[vTarget];

                if (target != null && target is Node_Normal)
                {
                    // check access to target
                    bool bGoodLink = true;
                    for (int z = Mathf.Min(m_vPosition.z, vTarget.z); z <= Mathf.Max(m_vPosition.z, vTarget.z) && bGoodLink; z++)
                    {
                        for (int x = Mathf.Min(m_vPosition.x, vTarget.x); x <= Mathf.Max(m_vPosition.x, vTarget.x) && bGoodLink; x++)
                        {
                            Dungeon.Node node = Dungeon.Instance[x, m_vPosition.y, z];
                            if (node == null)
                            {
                                bGoodLink = false;
                                break;
                            }
                        }
                    }

                    if (bGoodLink)
                    {
                        m_links.Add(new Link_Normal(this, target));
                    }
                }
            }

            // jump down links
            foreach (Vector3Int vDir in sm_jumpDirections)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Vector3Int vJumpGoal = m_vPosition + vDir + Vector3Int.down * i;
                    Dungeon.Node targetNode = Dungeon.Instance[vJumpGoal];
                    if (targetNode != null)
                    {
                        m_links.Add(new Link_JumpDown(this, targetNode));
                    }
                }
            }
        }
    }
}
