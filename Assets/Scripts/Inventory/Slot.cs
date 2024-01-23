using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    public int itemCount;
    public bool isSelected;
    public Item item;
}
