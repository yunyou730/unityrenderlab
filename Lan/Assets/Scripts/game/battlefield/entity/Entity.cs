using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace lan.game.battlefield
{
    public class Entity
    {
        public readonly int _id;

        public int ID()
        {
            return _id;
        }

        public Entity(int id)
        {
            _id = id;
        }
           
    }
   
}
