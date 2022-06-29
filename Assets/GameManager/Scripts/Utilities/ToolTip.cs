using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TMP_Text   toolTipText;

    Button       button;
    private Loot _loot;

    private void Start()
    {
        button = GetComponent<Button>();
        TryGetComponent(out Loot loot);
        _loot = loot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_loot == null)
        {
            return;
        }

        ShowToolTip(_loot.GetItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideToolTip();
    }

    void ShowToolTip(Item item)
    {
        toolTipText.text = $"{item.ItemDescription}";
        toolTip.SetActive(true);
    }

    void HideToolTip()
    {
        toolTip.SetActive(false);
    }
}