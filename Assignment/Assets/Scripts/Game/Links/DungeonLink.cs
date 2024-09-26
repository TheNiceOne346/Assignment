using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class DungeonLink : Link
    {
        #region Properties

        public new Dungeon.Node Source => base.Source as Dungeon.Node;

        public new Dungeon.Node Target => base.Target as Dungeon.Node;

        public virtual float AdditionalCost => 0.0f;

        public Vector3 Direction => Vector3.Normalize(Target.WorldPosition - Source.WorldPosition);

        #endregion

        public DungeonLink(Dungeon.Node source, Dungeon.Node target) : base(source, target)
        {
        }

        public abstract IEnumerator MoveController(Controller controller);
    }
}