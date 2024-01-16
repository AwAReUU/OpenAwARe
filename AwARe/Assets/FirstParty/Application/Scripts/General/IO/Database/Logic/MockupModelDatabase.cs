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
                // All distances are 'placeholder = 0', except for real-life heights (meters)
                new Model( 1, ResourceType.Water,  @"Water/Water_bottle",    0, 0, 0.3f,  0, 0),
                new Model( 2, ResourceType.Animal, @"Animals/CowBlW",        0, 0, 1.5f,  0, 0),
                new Model( 3, ResourceType.Animal, @"Animals/ChickenBrown",  0, 0, 0.5f,  0, 0), 
                new Model( 4, ResourceType.Animal, @"Animals/Pig",           0, 0, 1f,    0, 0),       
                new Model( 5, ResourceType.Animal, @"Animals/DuckWhite",     0, 0, 0.39f, 0, 0), 
                new Model( 6, ResourceType.Plant,  @"Plants/grap",           0, 0, 1f,    0, 0),
                new Model( 7, ResourceType.Plant,  @"Plants/wheat1",         0, 0, 1.2f,  0, 0),
                new Model( 8, ResourceType.Animal, @"Water/Milk_carton",     0, 0, 0.2f,  0, 0),
                new Model( 9, ResourceType.Plant,  @"Plants/potato",         0, 0, 0.4f,  0, 0),
                new Model(10, ResourceType.Plant,  @"Plants/beet",           0, 0, 0.2f,  0, 0),
                new Model(11, ResourceType.Plant,  @"Plants/artichoke",      0, 0, 1.5f,  0, 0),
                new Model(12, ResourceType.Plant,  @"Plants/brokoly",        0, 0, 0.4f,  0, 0),
                new Model(13, ResourceType.Plant,  @"Plants/cabbage",        0, 0, 0.3f,  0, 0),
                new Model(14, ResourceType.Plant,  @"Plants/carrot",         0, 0, 0.2f,  0, 0),
                new Model(15, ResourceType.Plant,  @"Plants/corn",           0, 0, 1.8f,  0, 0),
                new Model(16, ResourceType.Plant,  @"Plants/corn2",          0, 0, 1.8f,  0, 0),
                new Model(17, ResourceType.Plant,  @"Plants/cucumber",       0, 0, 1.5f,  0, 0),
                new Model(18, ResourceType.Plant,  @"Plants/eggplant",       0, 0, 0.3f,  0, 0),
                new Model(19, ResourceType.Plant,  @"Plants/garlic",         0, 0, 0.2f,  0, 0),
                new Model(20, ResourceType.Plant,  @"Plants/grap2",          0, 0, 2.1f,  0, 0),
                new Model(21, ResourceType.Plant,  @"Plants/onion",          0, 0, 0.3f,  0, 0),
                new Model(22, ResourceType.Plant,  @"Plants/pepper",         0, 0, 0.7f,  0, 0),
                new Model(23, ResourceType.Plant,  @"Plants/poppy",          0, 0, 0.6f,  0, 0),
                new Model(24, ResourceType.Plant,  @"Plants/pumpkin",        0, 0, 0.3f,  0, 0), 
                new Model(25, ResourceType.Plant,  @"Plants/sunflower",      0, 0, 2f,    0, 0),
                new Model(26, ResourceType.Plant,  @"Plants/tomato",         0, 0, 0.6f,  0, 0),
                new Model(27, ResourceType.Plant,  @"Plants/wheat2",         0, 0, 0.6f,  0, 0),
                new Model(28, ResourceType.Animal, @"Animals/SheepWhite",    0, 0, 1f,    0, 0)
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

