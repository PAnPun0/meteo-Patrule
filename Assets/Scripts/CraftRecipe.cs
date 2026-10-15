using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    public string materialName; // "Wool", "Membrane", "Fleece"
    public int amount;
}

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Patrol/Craft Recipe")]
public class CraftRecipe : ScriptableObject
{
    public EquipmentItem resultItem;
    public List<Ingredient> ingredients = new List<Ingredient>();
}