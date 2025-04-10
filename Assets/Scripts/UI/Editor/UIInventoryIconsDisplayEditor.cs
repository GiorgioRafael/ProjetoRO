using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIInventoryIconsDisplay))]
public class UIInventoryIconsDisplayEditor : Editor
{

    UIInventoryIconsDisplay display;
    int targetedItemListIndex = 0;
    string[] itemListOptions;

    // This fires whenever we select a GameObject containing the
    // UIInventoryIconsDisplay component. The function scans the PlayerInventory
    // script to find all variables of the type List<PlayerInventory.Slot>.
    private void OnEnable()
    {
        // Get access to the component, as we will need to set
        // the targetedItemList variable on it.
        display = target as UIInventoryIconsDisplay;

        // Get the Type object for the PlayerInventory class
        Type playerInventoryType = typeof(PlayerInventory);

        // Get all fields of the PlayerInventory class
        FieldInfo[] fields = playerInventoryType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        // List to store variables of type List<PlayerInventory.Slot>
        // Use LINQ to filter fields of type List<PlayerInventory.Slot> and select their names
        List<string> slotListNames = fields
            .Where(field => field.FieldType.IsGenericType &&
                             field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
                             field.FieldType.GetGenericArguments()[0] == typeof(PlayerInventory.Slot))
            .Select(field => field.Name)
            .ToList();

        slotListNames.Insert(0, "None");
        itemListOptions = slotListNames.ToArray();

        // Ensure that we are using the correct weapon subtype.
        targetedItemListIndex = Math.Max(0, Array.IndexOf(itemListOptions, display.targetedItemList));
    }

    // This function draws the inspector.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck(); // Begin checking for changes

        // Draw a dropdown in the Inspector
        targetedItemListIndex = EditorGUILayout.Popup("Targeted Item List", Mathf.Max(0, targetedItemListIndex), itemListOptions);

        if (EditorGUI.EndChangeCheck())
        {
            display.targetedItemList = itemListOptions[targetedItemListIndex].ToString();
            EditorUtility.SetDirty(display); // Marks the object to save.
        }

        if (GUILayout.Button("Generate Icons")) RegenerateIcons();
    }

    // Regenerate the icons based on the slotTemplate.
    // Fires when the Generate Icons button is clicked on the Inspector.
    void RegenerateIcons()
    {
        display = target as UIInventoryIconsDisplay;

        // Register the entire function call as undoable
        Undo.RegisterCompleteObjectUndo(display, "Regenerate Icons");

        if(display.slots.Length > 0)
        {
            // Destroy all the children in the previous slots.
            foreach (GameObject g in display.slots)
            {
                if (!g) continue; // If the slot is empty, ignore it.

                // Otherwise remove it and record it as an undoable action.
                if (g != display.slotTemplate)
                    Undo.DestroyObjectImmediate(g);
            }
        }

        // Destroy all other children except for the slot template.
        for (int i = 0; i < display.transform.childCount; i++)
        {
            if (display.transform.GetChild(i).gameObject == display.slotTemplate) continue;
            Undo.DestroyObjectImmediate(display.transform.GetChild(i).gameObject);
            i--;
        }

        if (display.maxSlots <= 0) return; // Terminate if there are no slots.

        // Create all the new children.
        display.slots = new GameObject[display.maxSlots];
        display.slots[0] = display.slotTemplate;
        for (int i = 1; i < display.slots.Length; i++)
        {
            display.slots[i] = Instantiate(display.slotTemplate, display.transform);
            display.slots[i].name = display.slotTemplate.name;
        }
    }
}