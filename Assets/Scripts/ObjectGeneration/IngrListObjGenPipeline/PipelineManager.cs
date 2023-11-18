using Databases;
using IngredientLists;
using ResourceLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ObjectGeneration
{
    class PipelineManager
    {
        public List<Renderable> GetRenderableDict(IngredientList selectedList)
        {
            Dictionary<int, int> quantityDict = IngredientListToQuantityDict(selectedList);

            MockupModelDatabase modelDatabase = new MockupModelDatabase();
            return QuantityDictToRenderables(quantityDict, modelDatabase);
        }

        /// <summary>
        /// Converts an ingredientlist to resource list to model list.
        /// </summary>
        /// <returns>Model list</returns>
        private Dictionary<int, int> IngredientListToQuantityDict(IngredientList selectedList)
        {
            ResourceCalculator resourceCalculator = new ResourceCalculator();
            ResourceList resourceList = resourceCalculator.IngredientsToResources(selectedList);
            Dictionary<int, int> modelList = ResourceListToModelList(resourceList);
            return modelList;
        }

        /// <summary> Converts the dictionary from the form (resourceID, Quantity) to the form (modelID, Quantity) </summary>
        public Dictionary<int, int> MockResourceListToModelList(
            Dictionary<int, int> resourceList,
            MockupResourceDatabase resourceDatabase)
        {
            var modelList = new Dictionary<int, int>();
            foreach (var obj in resourceList)
            {
                int modelID = resourceDatabase.GetResource(obj.Key).ModelID;
                modelList[modelID] = obj.Value;
            }
            return modelList;
        }

        /// <summary>
        /// Convert resourceList to ModelList (list of <id,quantity> of items we need to render)
        /// </summary>
        /// <param name="resourceList"></param>
        /// <param name="resourceDatabase"></param>
        /// <returns>modelList</returns>
        public Dictionary<int, int> ResourceListToModelList(ResourceList resourceList)
        {
            ModelCalculator modelCalculator = new ModelCalculator();
            var modelList = new Dictionary<int, int>();
            foreach (var obj in resourceList.Resources)
            {
                Resource resource = obj.Key;
                float quantityGrams = obj.Value;

                int modelID = resource.ModelID;
                modelList[modelID] = modelCalculator.CalculateModelQuantity(resource, quantityGrams);
            }
            return modelList;
        }

        private List<Renderable> QuantityDictToRenderables(
            Dictionary<int, int> idQtyDictionary,
            MockupModelDatabase modelDatabase)
        {
            float sizeMultiplier = 1;
            List<Renderable> Renderables = new();
            foreach ((int modelId, int quantity) in idQtyDictionary)
            {
                Renderable renderable = new();
                string modelpath = @"Prefabs/" + modelDatabase.GetModel(modelId).PrefabPath;
                GameObject model = Resources.Load<GameObject>(modelpath);
                Vector3 halfExtents = ObjectCreationManager.GetHalfExtents(model);

                //dirty temp code so that water doesnt have size 0.
                float realHeight = modelDatabase.GetModel(modelId).RealHeight;
                if (realHeight == 0)
                    realHeight = 1;

                float modelSizeMultiplier = realHeight / (2 * halfExtents.y) * sizeMultiplier;
                renderable.prefab = model;
                renderable.quantity = quantity;
                renderable.scaling = modelSizeMultiplier;
                renderable.halfExtents = halfExtents * modelSizeMultiplier;
                Renderables.Add(renderable);
            }

            Renderables = SetSurfaceRatios(Renderables);
            return Renderables;
        }

        /// <summary>
        /// For each unique object, find out the percentage of space it will need.
        /// </summary>
        /// <param name="spawnDict"></param>
        /// <returns></returns>
        private List<Renderable> SetSurfaceRatios(List<Renderable> renderables)
        {
            //compute the sum of area of all gameobjects that will be spawned.
            float sumArea = 0;
            for(int i = 0; i < renderables.Count; i++)
            {
                int quantity = renderables[i].quantity;
                Vector3 halfExtents = renderables[i].halfExtents;
                float areaPerClone = halfExtents.x * halfExtents.z * 4;
                float areaClonesSum = areaPerClone * quantity;

                renderables[i].allowedSurfaceUsage = areaClonesSum;
                sumArea += areaClonesSum;
            }
            for(int i = 0; i < renderables.Count; i++)
                renderables[i].allowedSurfaceUsage /= sumArea;

            return renderables;
        }
    }
}
