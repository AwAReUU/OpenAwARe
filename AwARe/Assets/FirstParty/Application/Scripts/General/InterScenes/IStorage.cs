using Rooms = AwARe.RoomScan.Polygons.Logic;
using  Ingredients = AwARe.IngredientList.Logic;

namespace AwARe.InterScenes
{
    public interface IStorage
    {
        public Ingredients.IngredientList ActiveIngredientList { get; set; }
        public Rooms.Room ActiveRoom { get; set; }
    }
}