using UnityEngine;

/// <summary>
/// This is a class that can be subclassed by any other class to make the sprites
/// of the class automatically sort themselves by the y-axis.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{

    SpriteRenderer sorted;
    public bool sortingActive = true; // Allows us to deactivate this on certain objects.
    public const float MIN_DISTANCE = 0.2f;
    int lastSortOrder = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sorted = GetComponent<SpriteRenderer>();    
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        if (!sorted) return;
        int newSortOrder = (int)(-transform.position.y / MIN_DISTANCE);
        if (lastSortOrder != newSortOrder) {
            lastSortOrder = sorted.sortingOrder;
            sorted.sortingOrder = newSortOrder;
        }
    }
}