// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.Database.Logic;
using AwARe.ResourcePipeline.Logic;
using NUnit.Framework;

namespace AwARe.Tests.ResourcePipeline
{
    public class IngredientListPipelineEditModeTests
    {
        [Test, Description("Test whether all resource to model conversions result in atleast 1 model." +
             " Even if the quantity of the resource is just one.")]
        public async void Test_No_Resources_Conversion_Zero_Models()
        {
            //Arrange: Create a mock resource database and retrieve its elements.
            const int MOCKUP_DATABASE_SIZE = 19;
            const int RESOURCE_QUANTITY = 1;
            ModelCalculator modelCalculator = new();
            MockupResourceDatabase mockupResourceDatabase = new();
            List<Resource> resources = await mockupResourceDatabase.GetResources(Enumerable.Range(1, MOCKUP_DATABASE_SIZE));

            //Act: Calculate model quantities for all resources in mockdatabase.
            IEnumerable<int> quantitiesList = Enumerable.Range(0, MOCKUP_DATABASE_SIZE)
                .Select(x => modelCalculator.CalculateModelQuantity(resources[x], RESOURCE_QUANTITY));

            //Assert: All modelQuantities should be at least one.
            Assert.True(quantitiesList.All(x => x > 0));
        }

        [Test, Description("Tests whether the constructor initializes the models dictionary.")]
        public void Test_ModelDictionaryConstructor_Initializes_ModelsDictionary()
        {
            //Arrange & Act: Construct a model dictionary.
            var modelDictionary = new ModelDictionary();

            //Assert: Models is initialized.
            Assert.IsNotNull(modelDictionary.Models);
        }

        [Test, Description("Tests whether the constructor initializes the models dictionary.")]
        public async void Test_ModelDictionary_Add_Works()
        {
            //Arrange: Construct the modelDictionary and obtain a model.
            MockupModelDatabase mockupModelDatabase = new();
            var modelDictionary = new ModelDictionary();
            Model mockModel = await mockupModelDatabase.GetModel(1);

            //Act: Add the model to the dictionary.
            modelDictionary.AddModel(mockModel, 10);

            //Assert: Models is initialized.
            Assert.True(modelDictionary.NumberOfModels() == 1);
        }
    }
}
