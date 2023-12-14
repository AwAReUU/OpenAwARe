using System;

namespace AwARe.ResourcePipeline.Logic
{
    /// <summary>
    /// Class <c>ModelCalculator</c> is a class containing methods to convert resources to models.
    /// </summary>
    internal class ModelCalculator
    {
        /// <summary>
        /// Calculate how many models we need to render for the given resource
        /// </summary>
        /// <param name="resource">The resource we want to calculate the modelQuantity of</param>
        /// <param name="resourceQuantity">The quantity of the resource</param>
        /// <returns></returns>
        public int CalculateModelQuantity(Resource resource, float resourceQuantity)
        {
            if (resource.GramsPerModel != null)
                return (int)Math.Ceiling((double)(resourceQuantity / resource.GramsPerModel));
            return resource.Name switch
            {
                "Water" => (int)Math.Ceiling(resourceQuantity / 1500), 
                "Milk" => (int)Math.Ceiling(resourceQuantity / 1000),
                _ => 1 //otherwise
            };
        }
    }
}