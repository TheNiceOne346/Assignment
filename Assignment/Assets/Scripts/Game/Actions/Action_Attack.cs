using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions
{
    public class Action_Attack : ControllerAction
    {
        private Controller      m_target;

        public Action_Attack(Controller controller, Controller target) : base(controller)
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
                Controller.transform.rotation = Quaternion.Slerp(Controller.transform.rotation, Quaternion.LookRotation(vForward.normalized), Time.deltaTime * Controller.MovementSpeed * 2.0f);
                yield return null;
            }

            // bounce = attack
            for (float f = 0.0f; f < 1.0f; f += Time.deltaTime)
            {
                Controller.Bounce();
                yield return null;
            }

            // deal damage?
            if (Controller.IsNeighbor(m_target))
            {
                m_target.TakeDamage(1);
            }

            // down time
            yield return new WaitForSeconds(1.5f);
        }
    }
}