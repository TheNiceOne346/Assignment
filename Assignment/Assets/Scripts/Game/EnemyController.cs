using Game.Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Game
{
    public class EnemyController : Controller
    {
        public static List<EnemyController>     AllEnemies = new List<EnemyController>();

        #region Properties

        public override float MovementSpeed => 1.5f;

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            AllEnemies.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (AllEnemies.Contains(this))
            {
                AllEnemies.Remove(this);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (AllEnemies.Contains(this))
            {
                AllEnemies.Remove(this);
            }
        }

        public override ControllerAction Think()
        {
            // move towards hero and attack when neighbor
            if (HeroController.Instance != null &&
                HeroController.Instance.IsAlive)
            {
                if (IsNeighbor(HeroController.Instance))
                {
                    return new Action_Attack(this, HeroController.Instance);
                }
                else
                {
                    return new Action_MoveTowards(this, HeroController.Instance);
                }
            }

            // otherwise wait around...
            return new Action_Wait(this, 1.0f);
        }
    }
}