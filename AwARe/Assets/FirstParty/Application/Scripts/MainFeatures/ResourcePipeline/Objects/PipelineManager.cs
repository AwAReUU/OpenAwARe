// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*        


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using AwARe.Database;
using AwARe.Database.Logic;
using AwARe.ObjectGeneration;
using AwARe.ResourcePipeline.Logic;

using UnityEngine;

namespace AwARe.ResourcePipeline.Objects
{
    /// <summary>
    /// Class <c>PipeLineManager</c> is responsible for managing the entire pipeline from ingredientList
    /// to List of renderables.
    /// </summary>
    public class PipelineManager
    {
        private readonly IModelDatabase modelDatabase;
        /// <summary>
        /// Constructor. It sets which implementation of the IModelDatabase we will use.
        /// </summary>
        public PipelineManager()
        {
            modelDatabase = new MockupModelDatabase();
        }


        /// <summary>
        /// "Main method" of the pipeline. Given a <seealso cref="IngredientList"/>, manage the entire pipeline,
        /// and return the resulting List of renderables.
        /// </summary>
        /// <param name="selectedList">IngredientList to convert to list of renderables.</param>
        /// <returns></returns>
        public async Task<List<Renderable>> GetRenderableList(IngredientList.Logic.IngredientList selectedList)
        {
            ResourceDictionary resourceList = await IngredientListToResourceList(selectedList);
            Dictionary<int, int> modelQuantities = ResourceListToModelQuantities(resourceList);
            return await QuantityDictToRenderables(modelQuantities);
        }

        /// <summary>
        /// Converts an <see cref="IngredientList"/> to a <see cref="ResourceDictionary"/>
        /// </summary>
        /// <returns>Model list</returns>
        private Task<ResourceDictionary> IngredientListToResourceList(IngredientList.Logic.IngredientList selectedList)
        {
            ResourceCalculator resourceCalculator = new ResourceCalculator();
            return resourceCalculator.IngredientsToResources(selectedList);
        }

        /// <summary>
        /// Convert <see cref="ResourceDictionary"/> to ModelDict(which is a Dictionary of modelIds to quantities).
        /// </summary>
        /// <param name="resourceList">The resource list to convert.</param>
        /// <returns>modelDict</returns>
        public Dictionary<int, int> ResourceListToModelQuantities(ResourceDictionary resourceList)
        {
            ModelCalculator modelCalculator = new();

            Dictionary<int, int> modelQuantities = new();
            foreach ((Resource resource, float quantityGrams) in resourceList.Resources)
            {
                int modelID = resource.ModelID;
                modelQuantities[modelID] = modelCalculator.CalculateModelQuantity(resource, quantityGrams);
            }
            return modelQuantities;
        }

        /// <summary>
        /// Given a dictionary of (modelId, quantity), create a <see cref="Renderable"/> for each modelId.
        /// </summary>
        /// <param name="quantityDictionary"></param>
        /// <returns></returns>
        private async Task<List<Renderable>> QuantityDictToRenderables(Dictionary<int, int> quantityDictionary)
        {
            const float sizeMultiplier = 1;
            List<Renderable> Renderables = new();
            foreach ((int modelId, int quantity) in quantityDictionary)
            {
                GameObject prefab = GetPrefabFromPath(@"Models/" + (await modelDatabase.GetModel(modelId)).PrefabPath);
                Vector3 halfExtents = GetHalfExtents(prefab);

                //dirty temp code so that water does not have size 0.
                float realHeight = (await modelDatabase.GetModel(modelId)).RealHeight;
                if (realHeight == 0)
                    realHeight = 1;

                float modelSizeMultiplier = realHeight / (2 * halfExtents.y) * sizeMultiplier;
                ResourceType resourceType = (await modelDatabase.GetModel(modelId)).Type;
                if (resourceType != ResourceType.Water)
                    halfExtents *= modelSizeMultiplier;
                Renderable renderable = new(prefab, halfExtents, quantity, modelSizeMultiplier, resourceType);
                Renderables.Add(renderable);
            }
            Renderables = Renderable.SetSurfaceRatios(Renderables);
            return Renderables;
        }

        /// <summary>
        /// Get the <see cref="GameObject"/> prefab from modelPath.
        /// If no <see cref="GameObject"/> is found at that path, return a placeholder <see cref="GameObject"/>.
        /// <para>It is basically a safe Resources.Load() that always returns a GameObject.</para>
        /// </summary>
        /// <param name="modelPath"></param>
        /// <returns>A <see cref="GameObject"/> prefab found at modelPath. If not found, a placeholder <see cref="GameObject"/>.</returns>
        private GameObject GetPrefabFromPath(string modelPath)
        {
            GameObject model = Resources.Load<GameObject>(modelPath);
            if (model == null) //model not found, use placeholder model.
            {
                modelPath = @"Prefabs/Shapes/Cube";
                model = Resources.Load<GameObject>(modelPath);
            }
            return model;
        }

        /// <summary>
        /// HalfExtents are distances from center to bounding box walls.
        /// </summary>
        /// <param name="prefab">GameObject to get the halfExtents from.</param>
        /// <returns>halfExtents of the given prefab</returns>
        public static Vector3 GetHalfExtents(GameObject prefab)
        {
            //Temporarily instantiate the object to get the BoxCollider size
            GameObject tempObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            BoxCollider tempCollider = tempObj.AddComponent<BoxCollider>();
            Vector3 halfExtents = tempCollider.size / 2;
            Object.Destroy(tempObj);

            return halfExtents;
        }
    }
}
