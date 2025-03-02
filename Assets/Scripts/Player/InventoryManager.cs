using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //dont use dictionary because unity dont have it on inspector  

    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];

    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] passiveItemLevels = new int[6];


    public void AddWeapon(int slotIndex, WeaponController weapon) //adiciona uma arma a um slot especifico 
    {
        weaponSlots[slotIndex] = weapon;
    }

    public void addPassiveItem(int slotIndex, PassiveItem passiveItem) //adiciona um item passivo a um slot especifico
    {
        passiveItemSlots[slotIndex] = passiveItem;      
    }

    //level up de itens 
    public void LevelUpWeapon(int slotIndex)
    {

    }

    public void LevelUpPassiveItem(int slotIndex)
    {

    }
}
