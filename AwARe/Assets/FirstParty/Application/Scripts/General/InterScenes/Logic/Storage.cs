using Rooms = AwARe.RoomScan.Polygons.Logic;
using  Ingredients = AwARe.IngredientList.Logic;
using System.Collections.Generic;

namespace AwARe.InterScenes.Logic
{
    public class Storage
    {
        public Storage() { }

        public Ingredients.IngredientList ActiveIngredientList { get; set; }
        public Rooms.Room ActiveRoom { get; set; }
  
        public string SavedAnchorId { get; set; }
        public Dictionary<int, string> SavedPolygons { get; set; }
        public string SavedPolygonKey { get; set; }

    }

}