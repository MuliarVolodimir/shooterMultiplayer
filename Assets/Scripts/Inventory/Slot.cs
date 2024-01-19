using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    public int itemCount;
    public bool isSelected;
    public Item item;

    public void UpdateSlot(Item item, int itemCount)
    {
        this.item = item;
        this.itemCount = itemCount;

        if (item != null) _text.text = item.itemName + ":" + itemCount;
        else _text.text = "";
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            this.GetComponent<Image>().color = Color.white;
        }
        else
        {
            this.GetComponent <Image>().color = Color.gray;
        }
        isSelected = selected;
    }
}
