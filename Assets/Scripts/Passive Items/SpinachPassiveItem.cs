using UnityEngine;

public class SpinachPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.currentMight *= 1 + passiveItemData.Multipler / 100f;
    }
}
