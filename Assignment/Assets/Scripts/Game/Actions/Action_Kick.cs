using Game.GUI;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Game.Actions
{
    public class Action_Kick : ControllerAction
    {
        private Controller      m_target;

        public Action_Kick(Controller controller, Controller target) : base(controller)
        {
            m_target = target;
        }

        public override IEnumerator PerformAction()
        {
            // face target
            while (Vector3.Angle(Controller.transform.forward, Vector3.Normalize(m_target.transform.position - Controller.transform.position)) > 5.0f)
            {
                Vector3 vForward = m_target.transform.position - Controller.transform.position;
                vForward.y = 0.0f;
                Controller.transform.rotation = Quaternion.Slerp(Controller.transform.rotation, Quaternion.LookRotation(vForward.normalized), Time.deltaTime * Controller.MovementSpeed * 3.0f);
                yield return null;
            }

            // bounce = attack
            for (float f = 0.0f; f < 0.5f; f += Time.deltaTime)
            {
                Controller.Bounce();
                yield return null;
            }

            // deal damage?
            if (m_target.IsAlive)
            {
                Dungeon.Instance.StartCoroutine(DoKnockback(Controller, m_target));
                DungeonCanvas.Instance.AddMessage("Kick", Controller.MessagePosition, Color.white);
            }

            // down time
            yield return new WaitForSeconds(0.5f);
        }

        static IEnumerator DoKnockback(Controller source, Controller target)
        {
            // find best knockback tile
            Vector3 vDirection = target.transform.position - source.transform.position;
            vDirection.y = 0.0f;
            vDirection = Vector3.Normalize(vDirection);

            float fBestDot = 0.1f;
            DungeonLink bestLink = null;
            foreach (DungeonLink link in target.Node.Links)
            {
                if (link.Target.Position.y > link.Source.Position.y)
                {
                    continue;
                }

                float fDot = Vector3.Dot(link.Direction, vDirection);
                if (fDot > fBestDot) 
                {
                    fBestDot = fDot;
                    bestLink = link;
                }
            }

            if (bestLink != null)
            {
                target.enabled = false;
                target.StopAllCoroutines();
                
                // knockback, knockback?
                if (bestLink.Target.Owner != null)
                {
                    Dungeon.Instance.StartCoroutine(DoKnockback(target, bestLink.Target.Owner));
                }

                // claim node
                yield return null;
                target.Node = bestLink.Target;

                while (Vector3.Distance(target.transform.position, bestLink.Target.WorldPosition) > 0.01f)
                {
                    // move towards next node
                    target.transform.position = Vector3.MoveTowards(target.transform.position, bestLink.Target.WorldPosition, Time.deltaTime * 5.0f);

                    // rotate towards source
                    Vector3 vForward = source.transform.position - target.transform.position;
                    if (vForward.magnitude > 0.001f)
                    {
                        vForward.y = 0.0f;
                        vForward = vForward.normalized;
                        target.transform.rotation = Quaternion.Slerp(target.transform.rotation, Quaternion.LookRotation(vForward), Time.deltaTime * 4.0f);
                    }

                    yield return null;
                }

                // let target resume thinking
                target.enabled = true;
            }
        }
    }
}