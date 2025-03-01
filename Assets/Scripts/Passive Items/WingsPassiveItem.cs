using UnityEngine;

public class WingsPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.currentMoveSpeed *= 1 + passiveItemData.Multipler / 100f; //modify the player based on the multiplier
    }
}
