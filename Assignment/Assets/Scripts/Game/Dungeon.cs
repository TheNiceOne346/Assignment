using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class Dungeon : MonoBehaviour, ISearchableGraph
    {
        public interface ILinkCreator
        {
            void CreateLinks();
        }

        public abstract class Node : IPositionNode, ILinkCreator
        {
            public DungeonLink      m_parentLink;
            public float            m_fDistance;

            protected Vector3Int    m_vPosition;
            protected List<Link>    m_links = new List<Link>();
            protected Tile          m_tile;

            #region Properties

            public Vector3Int Position => m_vPosition;

            public Vector3 WorldPosition => m_vPosition;

            public IEnumerable<ILink> Links
            {
                get
                {
                    for (int i = m_links.Count - 1; i >= 0; i--)
                    {
                        yield return m_links[i];
                    }
                }
            }

            public virtual float AdditionalCost => 0.0f;

            public Controller Owner { get; set; }

            #endregion

            public Node(Vector3Int vPosition, Tile tile)
            {
                m_vPosition = vPosition;
                m_tile = tile;
            }

            public void AddLink(Link link)
            {
                if (link != null && !m_links.Contains(link))
                {
                    m_links.Add(link);
                }
            }

            public void RemoveLink(Link link)
            {
                if (link != null && m_links.Contains(link))
                {
                    m_links.Remove(link);
                }
            }

            public abstract void CreateLinks();

            public void ResetPathfinding()
            {
                m_fDistance = float.MaxValue;
                m_parentLink = null;
            }

            public virtual void OnEnter(Controller controller)
            {
            }
        }

        private Dictionary<Vector3Int, Node>    m_nodes = new Dictionary<Vector3Int, Node>();
        private Bounds                          m_bounds;

        private static Dungeon                  sm_instance;

        #region Properties

        public IEnumerable<INode> Nodes => m_nodes.Values;

        public Bounds Bounds => m_bounds;

        public bool IsReady => m_nodes.Count > 0;

        public Node this[int x, int y, int z] => this[new Vector3Int(x, y, z)];

        public Node this[Vector3Int v]
        {
            get
            {
                Node node = null;
                m_nodes.TryGetValue(v, out node);
                return node;
            }

            set
            {
                if (m_nodes.ContainsKey(v))
                {
                    Node nodeToRemove = m_nodes[v];
                    m_nodes.Remove(v);

                    foreach (Dungeon.Node n in Nodes)
                    {
                        foreach (Link link in n.Links)
                        {
                            if (link.Target == nodeToRemove)
                            {
                                n.RemoveLink(link);
                            }
                        }
                    }
                }

                if (value != null)
                {
                    m_nodes[v] = value;
                }
            }
        }

        public static Dungeon Instance => sm_instance;

        #endregion

        protected virtual void OnEnable()
        {
            sm_instance = this;
        }

        protected virtual void Start()
        {
            CreateNodes();
        }

        protected void CreateNodes()
        {
            // get nodes from child tiles (and calculate bounds)
            Vector3 vMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);
            Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_nodes.Clear();

            foreach (Tile tile in GetComponentsInChildren<Tile>())
            {
                foreach (Node node in tile.CreateNodes())
                {
                    Vector3 vWP = node.WorldPosition;
                    Vector3Int v = new Vector3Int(Mathf.RoundToInt(vWP.x), Mathf.RoundToInt(vWP.y), Mathf.RoundToInt(vWP.z));
                    
                    vMax.x = Mathf.Max(vMax.x, vWP.x);
                    vMax.y = Mathf.Max(vMax.y, vWP.y);
                    vMax.z = Mathf.Max(vMax.z, vWP.z);

                    vMin.x = Mathf.Min(vMin.x, vWP.x);
                    vMin.y = Mathf.Min(vMin.y, vWP.y);
                    vMin.z = Mathf.Min(vMin.z, vWP.z);

                    m_nodes[v] = node;
                }
            }

            // set bounds
            m_bounds = new Bounds((vMax + vMin) * 0.5f, vMax - vMin);

            // create links
            foreach (Node node in m_nodes.Values)
            {
                node.CreateLinks();
            }

            // let objects create links
            foreach (ILinkCreator linkCreator in GetComponentsInChildren<ILinkCreator>())
            {
                linkCreator.CreateLinks();
            }
        }

        public virtual Node GetRandomNode()
        {
            List<INode> nodes = new List<INode>(Nodes);
            return nodes[Random.Range(0, nodes.Count)] as Node;
        }

        public float Heuristic(INode start, INode goal)
        {
            if (start is IPositionNode startNode &&
                goal is IPositionNode goalNode)
            {
                return Vector3.Distance(startNode.WorldPosition, goalNode.WorldPosition);
            }

            return 1.0f;
        }
    }
}