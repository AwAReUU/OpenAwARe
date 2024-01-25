// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                new Model( 1, ResourceType.Water,  @"Water/Water_bottle",    0, 0, 0.3f, 0, 0),
                new Model( 2, ResourceType.Animal, @"Animals/CowBlW",        0, 0, 0, 0, 0),
                new Model( 3, ResourceType.Animal, @"Animals/ChickenBrown",  0, 0, 0.5f, 0, 0),
                new Model( 4, ResourceType.Animal, @"Animals/Pig",           0, 0, 0.94f, 0, 0),
                new Model( 5, ResourceType.Animal, @"Animals/DuckWhite",     0, 0, 0.39f, 0, 0),
                new Model( 6, ResourceType.Plant,  @"Plants/grap",            0, 0, 1f, 0, 0),
                new Model( 7, ResourceType.Plant,  @"Plants/wheat1",          0, 0, 1.2f, 0, 0),
                new Model( 8, ResourceType.Animal, @"Water/Milk_carton",     0, 0, 0.2f, 0, 0),
                new Model( 9, ResourceType.Plant,  @"Plants/potato",          0, 0, 0.4f, 0, 0),
                new Model(10, ResourceType.Plant,  @"Plants/beet", 0, 0, 0.2f, 0, 0)
            };
        }

        ///<inheritdoc cref="IModelDatabase.GetModel"/>
        public Task<Model> GetModel(int id)
        {
            return Task.Run(() => modelTable.First(x => x.ID == id));
        }

        ///<inheritdoc cref="IModelDatabase.GetModels"/>
        public Task<List<Model>> GetModels(IEnumerable<int> ids)
        {
            IEnumerable<Model> models =
                from id in ids
                join model in modelTable on id equals model.ID
                select model;
            return Task.Run(() => models.ToList());
        }
    }
}

