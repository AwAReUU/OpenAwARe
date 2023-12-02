using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.DataTypes;
using IngredientLists;
using RoomScan;

namespace AwARe.DataStructures
{
    public interface IStorage
    {
        public IngredientList ActiveIngredientList { get; set; }
        public Room ActiveRoom { get; set; }
    }

    public class Storage
    {
        public Storage() { }

        public IngredientList ActiveIngredientList { get; set; }
        public Room ActiveRoom { get; set; }

    }

}