using UnityEngine;

public class ExperienceGem : Pickup, ICollectable
{
    public int experienceGranted;
    public override void Collect()
    {
        if(hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        player.IncreaseExperience(experienceGranted);
    }

}
