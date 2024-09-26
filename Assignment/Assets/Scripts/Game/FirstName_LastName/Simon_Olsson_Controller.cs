using Game;
using Game.Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Replace the FirstName and LastName with your name and last name :)
/// </summary>
namespace Simon_Olsson
{
    public class Simon_Olsson_Controller : HeroController
    {
        public override ControllerAction Think()
        {
           EnemyController enemy = EnemyController.AllEnemies.Find(e=>IsNeighbor(e));
            Heart heart = Heart.AllHearts.Count > 0 ? Heart.AllHearts[0] : null;

            if (enemy != null)
            {

                if (this.HP > 3)
                {
                    return new Action_Attack(this, enemy);
                } 
                else
                {
                    return new Action_Kick(this, enemy);
                }
                

            }
            if (heart != null)
            {
                if(this.HP < this.MaxHP)
                {
                    return new Action_MoveTowards(this, heart.Node);
                }
            }
           
            return new Action_Wait(this, 1.0f);
        }
    }
}