using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]

public class Item : ScriptableObject
{
    public string itemName;
    public int ItemIndex;
    public bool CanBeCompose;
    public GameObject itemPrefab; 
}
