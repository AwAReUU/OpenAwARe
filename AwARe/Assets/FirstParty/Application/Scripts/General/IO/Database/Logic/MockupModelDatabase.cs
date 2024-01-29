// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;

using AwARe.ResourcePipeline.Logic;

namespace AwARe.Database.Logic
{
    /// <summary>
    /// Implementation of the Model Database interface, that uses mock database using a locally saved table.
    /// The Mock Database can be used for testing purposes.
    /// </summary>
    public class MockupModelDatabase : IModelDatabase
    {
        readonly List<Model> modelTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockupModelDatabase"/> class.
        /// </summary>
        public MockupModelDatabase()
        {
            modelTable = new()
            {
                new Model( 1, ResourceType.Water,    @"Water/WaterCube", 0.3f ),
                new Model( 2, ResourceType.Animal, @"Animals/CowBlW",       1.5f ),
                new Model( 3, ResourceType.Animal, @"Animals/ChickenBrown", 0.5f ), 
                new Model( 4, ResourceType.Animal, @"Animals/Pig",          1f   ),       
                new Model( 5, ResourceType.Animal, @"Animals/DuckWhite",    0.39f), 
                new Model( 6, ResourceType.Plant,   @"Plants/grap",         1f   ),
                new Model( 7, ResourceType.Plant,   @"Plants/wheat1",       1.2f ),
                new Model( 8, ResourceType.Animal,   @"Water/Milk_carton",  0.2f ),
                new Model( 9, ResourceType.Plant,   @"Plants/potato",       0.4f ),
                new Model(10, ResourceType.Plant,   @"Plants/beet",         0.2f ),
                new Model(11, ResourceType.Plant,   @"Plants/artichoke",    1.5f ),
                new Model(12, ResourceType.Plant,   @"Plants/brokoly",      0.4f ),
                new Model(13, ResourceType.Plant,   @"Plants/cabbage",      0.3f ),
                new Model(14, ResourceType.Plant,   @"Plants/carrot",       0.2f ),
                new Model(15, ResourceType.Plant,   @"Plants/corn",         1.8f ),
                new Model(16, ResourceType.Plant,   @"Plants/corn2",        1.8f ),
                new Model(17, ResourceType.Plant,   @"Plants/cucumber",     1.5f ),
                new Model(18, ResourceType.Plant,   @"Plants/eggplant",     0.3f ),
                new Model(19, ResourceType.Plant,   @"Plants/garlic",       0.2f ),
                new Model(20, ResourceType.Plant,   @"Plants/grap2",        2.1f ),
                new Model(21, ResourceType.Plant,   @"Plants/onion",        0.3f ),
                new Model(22, ResourceType.Plant,   @"Plants/pepper",       0.7f ),
                new Model(23, ResourceType.Plant,   @"Plants/poppy",        0.6f ),
                new Model(24, ResourceType.Plant,   @"Plants/pumpkin",      0.3f ), 
                new Model(25, ResourceType.Plant,   @"Plants/sunflower",    2f   ),
                new Model(26, ResourceType.Plant,   @"Plants/tomato",       0.6f ),
                new Model(27, ResourceType.Plant,   @"Plants/wheat2",       0.6f ),
                new Model(28, ResourceType.Animal, @"Animals/SheepWhite",   1f   )
            };
        }

        ///<inheritdoc cref="IModelDatabase.GetModel"/>
        public Model GetModel(int id)
        {
            return modelTable.First(x => x.ID == id);
        }

        ///<inheritdoc cref="IModelDatabase.GetModels"/>
        public List<Model> GetModels(IEnumerable<int> ids)
        {
            IEnumerable<Model> models =
                from id in ids
                join model in modelTable on id equals model.ID
                select model;
            return models.ToList();
        }
    }
}

