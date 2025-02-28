using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void AppleModifier()
    {
        //
    }
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>();
    }

    
    void Update()
    {
        
    }
}
