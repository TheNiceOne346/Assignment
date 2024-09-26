using Game.Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

namespace Game
{
    public class Heart : MonoBehaviour
    {
        Transform                   m_mesh;
        Dungeon.Node                m_node;

        public static List<Heart>   AllHearts = new List<Heart>();

        #region Properties

        public Dungeon.Node Node => m_node;

        #endregion

        private void OnEnable()
        {
            AllHearts.Add(this);
            m_mesh = transform.Find("Mesh");
            m_node = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, transform.position);
            transform.position = m_node.Position;
        }

        private void OnDisable()
        {
            AllHearts.Remove(this);
        }

        private void Update()
        {
            // spin & bounce
            m_mesh.Rotate(Vector3.up, Time.deltaTime * 90.0f);
            m_mesh.localPosition = Vector3.up * Mathf.Lerp(0.1f, 0.5f, Mathf.Abs(Mathf.Sin(Time.time * 4.0f)));
        }
    }
}