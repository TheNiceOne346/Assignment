using Game.Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

namespace Game
{
    public abstract class HeroController : Controller
    {
        static HeroController sm_instance;

        #region Properties

        public override float MovementSpeed => 2.5f;

        public static HeroController Instance => sm_instance;

        #endregion

        protected override void Start()
        {
            base.Start();
            sm_instance = this;
        }

        public override void OnDeath()
        {
            base.OnDeath();
            sm_instance = (sm_instance == this ? null : sm_instance);
        }
    }
}