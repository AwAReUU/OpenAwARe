using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.DataTypes;
using IngredientLists;

namespace AwARe.DataStructures
{
    public interface IStorage
    {
        public IngredientList ActiveIngredientList { get; set; }
    }

    public class Storage
    {
        public Storage() { }

        public IngredientList ActiveIngredientList { get; set; }

    }

}