using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeBeansData", menuName = "Scriptable Objects/CoffeeBeansData")]
public class CoffeeBeansData : ScriptableObject
{
    public List<CoffeeBeans> beans;
}
