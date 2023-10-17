using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IngredientResourceDatabaseMock : MonoBehaviour
{
    Dictionary<int, objectingredient> ingredientDBMock = new Dictionary<int, objectingredient>()
    { { 1, new objectingredient("Apple", "fruit", "beet.fbx") }, { 1, new objectingredient("Beef", "animal", "CowBIW.prefab") } };

    Dictionary<int, objectplant> plantDBMock = new Dictionary<int, objectplant>() 
    { { 1, new objectplant(1100, 1100) } };

    Dictionary<int, objectanimal> animalDBMock = new Dictionary<int, objectanimal>()
    { { 2, new objectanimal(124098) } };

    //<prefabId, quantity>
    private Dictionary<int, int> spawnDictMock = new Dictionary<int, int>() { { 0, 2 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 1 } };

    private string findPrefabName(int id) 
    {
        return ingredientDBMock[id].prefName;
    }

    private GameObject FindGameObject() 
    {
        string foodType = FirstLetterToUpper("crops");
        string prefName = "garlic".ToLower();
        string prefabPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Prefabs", foodType, prefName);
        Debug.Log(prefabPath);
    }

    public string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

}

public class objectingredient
{
    public objectingredient(string name, string type, string prefabName) 
    {
        ingredientName = name;
        prefName = prefabName;
        FoodType = type;
    }
    public string ingredientName { get; set; }
    public string prefName { get; set; }
    public string FoodType { get; set; }
}

public class objectplant
{
    public objectplant(float plants, float rows) 
    {
        spacePlants = plants;
        spaceRows = rows;
    } 
    public float spacePlants { get; set; }
    public float spaceRows { get; set; }
}
public class objectanimal
{
    public objectanimal(float w) 
    {
        water = w;
    }
    public float water { get; set; }

}