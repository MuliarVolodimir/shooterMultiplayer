using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotView : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void UpdateSlotView(Item item, int count)
    {
        if (item != null)
            Text.text = $"{item.itemName}:{count}";
        else
            Text.text = $"Slot empty";
    }
    public void SwitchSelected(bool isSelected)
    {
        if (isSelected)
        {
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.gray;
        }
    }
}
