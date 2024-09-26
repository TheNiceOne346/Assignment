using Game.GUI;
using Game.Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;
using AI;

namespace Game
{
    public abstract class Controller : Brain
    {
        private Dungeon.Node            m_node;
        private Transform               m_meshTransform;
        private float                   m_fBounce;
        private int                     m_iHP;
        private HPBar                   m_hpBar;

        #region Properties

        public int HP => m_iHP;

        public int MaxHP => this is EnemyController ? 2 : 5;

        public bool IsAlive => m_iHP > 0;

        public Dungeon.Node Node 
        {
            get => m_node;
            set
            {
                if (m_node != value)
                {
                    if (m_node != null)
                    {
                        m_node.Owner = null;
                    }

                    m_node = value;

                    if (m_node != null)
                    {
                        m_node.Owner = this;
                        m_node.OnEnter(this);

                        // pickup heart?
                        Heart heart = Heart.AllHearts.Find(h => h.Node == m_node);
                        if (heart != null)
                        {
                            Destroy(heart.gameObject);
                            OnHeartPickup();
                        }
                    }
                }
            }
        }

        public GraphAlgorithms.Path LastPath { get; set; }

        public Transform MeshTransform => m_meshTransform;

        public abstract float MovementSpeed { get; }

        public Vector3 MessagePosition => transform.position + Vector3.up * 1.6f;

        #endregion

        protected virtual void OnEnable()
        {
            StartCoroutine(ControllerBrain());
        }

        protected virtual void OnDisable()
        {
        }

        protected override void Start()
        {
            base.Start();
            m_meshTransform = transform.Find("Mesh");
            m_iHP = MaxHP;
            m_hpBar = HPBar.Create(this);            
        }

        public void Bounce()
        {
            // add some bounce!
            m_fBounce += Time.deltaTime * 12.0f;
            m_meshTransform.localPosition = Vector3.up * Mathf.Abs(Mathf.Sin(m_fBounce)) * 0.2f;
            m_meshTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Cos(m_fBounce) * 10.0f);
        }

        public virtual void TakeDamage(int iDMG)
        {
            if (!IsAlive)
            {
                return;
            }

            m_iHP -= iDMG;
            if (m_iHP <= 0)
            {
                OnDeath();
                DungeonCanvas.Instance.AddMessage(name + " died", MessagePosition, Color.red);
            }
            else
            {
                DungeonCanvas.Instance.AddMessage("-" + iDMG + " HP", MessagePosition, Color.red);
            }
        }

        public void OnHeartPickup()
        {
            // is damaged?
            if (m_iHP < MaxHP)
            {
                m_iHP++;
                DungeonCanvas.Instance.AddMessage("+1 HP", MessagePosition, Color.green);
            }
        }

        public virtual void OnDeath()
        {
            StopAllCoroutines();

            if (m_hpBar != null)
            {
                Destroy(m_hpBar.gameObject);
                m_hpBar = null;
            }

            StartCoroutine(Death());
        }

        public bool IsNeighbor(Controller other)
        {
            if (other != null && other != this)
            {
                List<ILink> links = new List<ILink>(Node.Links);
                if (links.FindIndex(l => l is Link_Normal && l.Target == other.Node) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual IEnumerator ControllerBrain()
        {
            // wait for dungeon initialization
            yield return new WaitForSeconds(0.1f);

            if (Node == null)
            {
                Node = GraphAlgorithms.GetClosestNode<Dungeon.Node>(Dungeon.Instance, transform.position);
            }
            transform.position = Node.Position;

            // think up a new action to do?
            yield return new WaitForSeconds(0.1f);
            while (IsAlive)
            {
                ControllerAction action = Think();
                if (action != null)
                {
                    yield return action.PerformAction();
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        protected IEnumerator Death()
        {
            // disable light
            GetComponentInChildren<Light>().enabled = false;

            // fall forward
            Quaternion qSource = transform.rotation;
            Quaternion qTarget = qSource * Quaternion.Euler(90.0f, 0.0f, 0.0f);
            for (float f = 0.0f; f <= 1.0f; f += Time.deltaTime)
            {
                transform.rotation = Quaternion.Slerp(qSource, qTarget, f);
                yield return null;
            }

            // sink down
            Vector3 vSource = transform.position;
            Vector3 vTarget = transform.position + Vector3.down;
            for (float f = 0.0f; f <= 1.0f; f += Time.deltaTime * 0.25f)
            {
                transform.position = Vector3.Lerp(vSource, vTarget, f);
                yield return null;
            }

            // self destruct
            Node = null;
            Destroy(gameObject);
        }

        public abstract ControllerAction Think();

        protected virtual void OnDrawGizmosSelected()
        {
            if (LastPath != null && LastPath.Count > 0)
            {
                Vector3 vOffset = Vector3.up * 0.1f;

                // draw first/current link
                Gizmos.color = Color.red;
                Gizmos.DrawLine((LastPath[0].Source as Dungeon.Node).WorldPosition + vOffset, (LastPath[0].Target as Dungeon.Node).WorldPosition + vOffset);

                // draw remaining path
                Gizmos.color = Color.yellow;
                for (int i = 1; i < LastPath.Count; i++)
                {
                    Gizmos.DrawLine((LastPath[i].Source as Dungeon.Node).WorldPosition + vOffset, (LastPath[i].Target as Dungeon.Node).WorldPosition + vOffset);
                }
            }
        }
    }
}