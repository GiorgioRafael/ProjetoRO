using UnityEngine;
using UnityEditor;

public static class RectTransformAnchorSetter
{
    [MenuItem("Tools/UI/Set Anchors to Corners")]
    public static void SetAnchorsToCorners()
    {
        // Get the currently selected GameObject
        GameObject selectedGameObject = Selection.activeGameObject;

        if (selectedGameObject == null)
        {
            Debug.LogWarning("No GameObject selected. Please select a UI element.");
            return;
        }

        RectTransform rt = selectedGameObject.GetComponent<RectTransform>();

        if (rt == null)
        {
            Debug.LogWarning("Selected GameObject does not have a RectTransform component.");
            return;
        }

        // Get the parent's RectTransform
        RectTransform parentRt = null;
        if (rt.parent != null)
        {
            parentRt = rt.parent.GetComponent<RectTransform>();
        }

        // If no parent RectTransform, anchors are relative to the Canvas.
        // We'll treat the Canvas's RectTransform as the effective parent for calculation.
        if (parentRt == null)
        {
            Canvas canvas = rt.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                parentRt = canvas.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogError("RectTransform has no parent RectTransform and is not part of a Canvas hierarchy. Cannot set anchors relative to parent.");
                return;
            }
        }

        // Use Undo.RecordObject to allow undoing the action
        Undo.RecordObject(rt, "Set Anchors to Corners");

        // Calculate the current corners of the RectTransform in its own local space
        // These are (0,0) for bottom-left, (width, height) for top-right, relative to its pivot
        // We need to get these values in the PARENT's local space.

        // Get the current local corners of the child in its own local space (relative to its pivot)
        Vector2 rtRectMin = rt.rect.min; // Bottom-left corner in local space of rt
        Vector2 rtRectMax = rt.rect.max; // Top-right corner in local space of rt

        // Convert these local points of the child to the parent's local space
        Vector2 localPointInParentMin = parentRt.InverseTransformPoint(rt.TransformPoint(rtRectMin));
        Vector2 localPointInParentMax = parentRt.InverseTransformPoint(rt.TransformPoint(rtRectMax));

        // Get the parent's rectangle information
        Vector2 parentLocalRectMin = parentRt.rect.min;
        Vector2 parentLocalRectSize = parentRt.rect.size;

        // Calculate the new anchor values (normalized relative to parent's size and min/max)
        Vector2 anchorMin = new Vector2(
            (localPointInParentMin.x - parentLocalRectMin.x) / parentLocalRectSize.x,
            (localPointInParentMin.y - parentLocalRectMin.y) / parentLocalRectSize.y
        );

        Vector2 anchorMax = new Vector2(
            (localPointInParentMax.x - parentLocalRectMin.x) / parentLocalRectSize.x,
            (localPointInParentMax.y - parentLocalRectMin.y) / parentLocalRectSize.y
        );
        
        // Clamp anchor values to ensure they are within [0, 1]
        anchorMin.x = Mathf.Clamp01(anchorMin.x);
        anchorMin.y = Mathf.Clamp01(anchorMin.y);
        anchorMax.x = Mathf.Clamp01(anchorMax.x);
        anchorMax.y = Mathf.Clamp01(anchorMax.y);

        // Set the new anchors
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;

        // After setting anchors, reset offsets to maintain current visual position
        // This is key to making the element "snap" to the anchors and have offsets (0,0)
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Debug.Log("Anchors set to corners for: " + selectedGameObject.name);
    }
}