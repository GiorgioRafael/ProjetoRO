using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void ApplyModifier()
    {
        //aplica o modificador no passive item script
    }
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>();
        ApplyModifier();
    }

    
    void Update()
    {
        
    }
}
