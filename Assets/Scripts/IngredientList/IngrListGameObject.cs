using IngredientLists;
using UnityEngine;

public class IngrListGameObject : MonoBehaviour 
{
    public IngredientList selectedList;

    private void Awake() => DontDestroyOnLoad(gameObject);

}