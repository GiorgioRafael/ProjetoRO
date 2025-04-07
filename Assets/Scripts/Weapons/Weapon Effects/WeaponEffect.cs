using UnityEngine;

/// <summary>
/// A GameObject that is spawned as an effect of a weapon firing, e.g. projectiles, auras, pulses.
/// </summary>
public abstract class WeaponEffect : MonoBehaviour
{

    [HideInInspector] public PlayerStats owner;
    [HideInInspector] public Weapon weapon;

    // Makes it possible to access owner using capital letter as well.
    // This maintains consistency between naming conventions across
    // different classes.
    public PlayerStats Owner { get { return owner; } }

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}