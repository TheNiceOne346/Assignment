using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Tile : MonoBehaviour
    {
        [System.Serializable]
        public class NodeData
        {
            [SerializeField]
            public Vector3Int   m_vLocalPosition;

            [SerializeField]
            public string       m_nodeType = "Node_Normal";
        }

        [SerializeField, HideInInspector]
        public List<NodeData>   m_nodeData = new List<NodeData>();

        public virtual IEnumerable<Dungeon.Node> CreateNodes()
        {
            foreach (NodeData nodeData in m_nodeData)
            {
                // get type
                System.Type type = typeof(Dungeon).Assembly.GetType("Game." + nodeData.m_nodeType);
                if (type != null && typeof(Dungeon.Node).IsAssignableFrom(type))
                {
                    Vector3 vWP = transform.TransformPoint(nodeData.m_vLocalPosition);
                    Vector3Int vPosition = new Vector3Int(Mathf.RoundToInt(vWP.x), Mathf.RoundToInt(vWP.y), Mathf.RoundToInt(vWP.z));
                    Dungeon.Node node = System.Activator.CreateInstance(type, new object[] { vPosition, this }) as Dungeon.Node;

                    if (node != null)
                    {
                        yield return node;
                    }
                }
            }
        }
    }
}