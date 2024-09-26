using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Tile_Trap : Tile
    {
        private Transform       m_spikes;
        private Coroutine       m_triggerRoutine;
        private List<Node_Trap> m_trapNodes;

        private void OnEnable()
        {
            m_spikes = transform.Find("Spikes");
        }

        public override IEnumerable<Dungeon.Node> CreateNodes()
        {
            // store trap nodes
            m_trapNodes = new List<Node_Trap>();
            foreach (Dungeon.Node node in base.CreateNodes())
            {
                if (node is Node_Trap trapNode)
                {
                    m_trapNodes.Add(trapNode);
                }

                yield return node;
            }
        }

        public void Trigger()
        {
            if (m_triggerRoutine == null)
            {
                m_triggerRoutine = StartCoroutine(TriggerLogic());
            }
        }

        IEnumerator TriggerLogic()
        {
            // BAM!
            for (float f = 0.0f; f < 1.0f; f += Time.deltaTime * 6.0f)
            {
                m_spikes.localPosition = new Vector3(0.0f, f, 0.0f);
                yield return null;
            }

            // deal damage
            HashSet<Controller> processedControllers = new HashSet<Controller>();
            foreach (Node_Trap node in m_trapNodes)
            {
                if (node.Owner != null && !processedControllers.Contains(node.Owner))
                {
                    node.Owner?.TakeDamage(1);
                    processedControllers.Add(node.Owner);
                }
            }

            // wait a while
            yield return new WaitForSeconds(2.0f);

            // slowly retract
            while (m_spikes.localPosition.y > 0.0f)
            {
                Vector3 v = m_spikes.localPosition;
                v.y -= Time.deltaTime * 0.25f;
                m_spikes.localPosition = v;
                yield return null;
            }

            // done
            m_spikes.localPosition = Vector3.zero;
            m_triggerRoutine = null;
        }
    }
}