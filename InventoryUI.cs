using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Very simple UI for SimpleInventory.
/// Place this on a GameObject with a GridLayoutGroup, and assign:
/// - inventory: reference to SimpleInventory
/// - slotPrefab: a prefab containing an Image for icon and Text for count
/// </summary>
public class SimpleInventoryUI : MonoBehaviour
{
    [Header("Inventory Source")]
    public Inventory inventory;
    public GameObject inventoryPanel;

    [Header("UI Setup")]
    public GameObject slotPrefab;           // prefab with Icon + Count
    public GridLayoutGroup gridLayout;      // parent container

    [Header("Options")]
    public bool refreshOnStart = true;


    private readonly List<GameObject> slotInstances = new List<GameObject>();

    private void Reset()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        if (refreshOnStart)
            RefreshUI();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // 1..5 map to modes
        if (kb.iKey.wasPressedThisFrame) ShowHideInventory();

    }

    public void ShowHideInventory()
    {
        if (inventoryPanel.activeSelf) inventoryPanel.SetActive(false);
        else inventoryPanel.SetActive(true);
    }

    /// <summary>
    /// Rebuilds the entire grid based on the inventory slots.
    /// Call this after you change inventory contents.
    /// </summary>
    public void RefreshUI()
    {
        if (inventory == null)
        {
            Debug.LogWarning("SimpleInventoryUI: No inventory assigned.");
            return;
        }

        if (gridLayout == null)
        {
            Debug.LogWarning("SimpleInventoryUI: No GridLayoutGroup assigned.");
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogWarning("SimpleInventoryUI: No slotPrefab assigned.");
            return;
        }

        ClearOldSlots();

        var slots = inventory.GetSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            var slotData = slots[i];

            GameObject slotGO = Instantiate(slotPrefab, gridLayout.transform);
            slotInstances.Add(slotGO);

            // Find icon & text components in prefab
            Image icon = null;
            Text countText = null;

            // You can also do named lookups if you prefer:
            // icon = slotGO.transform.Find("Icon").GetComponent<Image>();
            // countText = slotGO.transform.Find("Count").GetComponent<Text>();

            foreach (var img in slotGO.GetComponentsInChildren<Image>())
            {
                if (icon == null && img.gameObject.name.ToLower().Contains("icon"))
                    icon = img;
            }

            foreach (var txt in slotGO.GetComponentsInChildren<Text>())
            {
                if (countText == null && txt.gameObject.name.ToLower().Contains("count"))
                    countText = txt;
            }

            // Set visuals
            if (slotData.IsEmpty)
            {
                // Empty slot: clear icon & text
                if (icon != null) icon.sprite = null;
                if (icon != null) icon.color = new Color(1, 1, 1, 0); // hide icon
                if (countText != null) countText.text = "";
            }
            else
            {
                var def = inventory.GetDef(slotData.id);

                if (icon != null)
                {
                    icon.sprite = def != null ? def.icon : null;
                    icon.color = (icon.sprite != null) ? Color.white : new Color(1, 1, 1, 0);
                }

                if (countText != null)
                {
                    countText.text = (slotData.quantity > 1) ? slotData.quantity.ToString() : "";
                }
            }
        }
    }

    private void ClearOldSlots()
    {
        for (int i = 0; i < slotInstances.Count; i++)
        {
            if (slotInstances[i] != null)
                DestroyImmediate(slotInstances[i]);
        }
        slotInstances.Clear();
    }
}
