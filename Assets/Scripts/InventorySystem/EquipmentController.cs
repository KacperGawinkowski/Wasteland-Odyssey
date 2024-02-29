using System;
using System.Collections.Generic;
using InventorySystem.Items;
using TestyAndStareRzeczy.sus_test;
using UnityEngine;

namespace InventorySystem
{
    public class EquipmentController : BasicInventory
    {
        [NonSerialized] public EquipmentSaveData equipmentSaveData;

        [NonSerialized] public WeaponInstance currentlyHeldWeapon;
        [SerializeReference] public WeaponInstance[] equippedWeapons;
        [SerializeReference] public ArmorInstance[] equippedArmors;

        [SerializeField] private IKControl m_IKControl;
        [SerializeField] private Animator m_Animator;
        [SerializeField] public GameObject m_weaponParentObject;
        [NonSerialized] public GunController weaponObject;

        [SerializeField] private MeshFilter m_HelmetMeshFilter;
        [SerializeField] private MeshRenderer m_HelmetMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer m_TorsoMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer m_LegsMeshRenderer;

        [SerializeField] private SkinnedMeshRenderer m_DefaultTorso;
        [SerializeField] private SkinnedMeshRenderer m_DefaultLegs;

        public bool isPlayer;

        [SerializeField] public int maxWeight;
        [SerializeField] public int currentWeight;

        // public InventoryDictionary InventoryDictionary { get; set; } = new();

        private void Awake()
        {
            // stworzenie sus tablic 
            equippedWeapons = new WeaponInstance[Enum.GetNames(typeof(WeaponType)).Length];
            equippedArmors = new ArmorInstance[Enum.GetNames(typeof(ArmorSlot)).Length];

            SaveSystem.OnUpdateSaveContent += SaveInventory;
        }

        public void LoadWeapon()
        {
            int ammoNeeded = currentlyHeldWeapon.GetWeaponMagazineCapacity() - currentlyHeldWeapon.currentAmmo;
            //int ammoInBackpack = inventory.GetAmount(currentlyHeldWeapon.GetWeapon().ammunition);

            int ammoInBackpack = GetAmount(currentlyHeldWeapon.GetWeapon().ammunition);

            //brak amunicji w plecaku
            if (ammoInBackpack <= 0)
            {
                print("No more ammunition in backpack");
                //zwrocic ze nie ma amunicji tego typu w plecaku
            }
            //magazynek juz pelny
            else if (ammoNeeded == 0)
            {
                print("No need for reload");
                //zwrocic ze nie trzeba przeladowac
            }
            //ilosc amunicji w placaku jest mniejsza niz potrzebna ilosc amunicji do pelnego magazynka
            else if (ammoInBackpack < ammoNeeded)
            {
                currentlyHeldWeapon.currentAmmo += ammoInBackpack;
                RemoveItem(currentlyHeldWeapon.GetWeapon().ammunition, ammoNeeded);
            }
            //przeladowanie broni do pelna
            else
            {
                currentlyHeldWeapon.currentAmmo += ammoNeeded;
                RemoveItem(currentlyHeldWeapon.GetWeapon().ammunition, ammoNeeded);
            }
        }

        public void SetWeaponInHand(int weaponSlot)
        {
            if (equippedWeapons[weaponSlot] != null)
            {
                SetBodyModelObject(equippedWeapons[weaponSlot]);
                currentlyHeldWeapon = equippedWeapons[weaponSlot];
                if (isPlayer)
                {
                    CanvasController.Instance.ammoInterface.SetAmmoCanvas(currentlyHeldWeapon);
                    CanvasController.Instance.skillsCanvasController.SetSkillsInterface(currentlyHeldWeapon);
                }
            }
            else
            {
                //Add some popup that there is no weapon in this slot
                Debug.Log("NO WEAPON IN THIS SLOT");
            }
        }

        public void HideWeapon()
        {
            if (weaponObject != null)
            {
                Destroy(weaponObject.gameObject);
                if (m_Animator)
                {
                    m_Animator.SetBool(AnimatorVariables.WeaponMode, false);
                }
            }

            currentlyHeldWeapon = null;
            m_IKControl.ClearIK();
            CanvasController.Instance.ammoInterface.SetAmmoCanvas(null);
        }

        private void SetBodyModelObject(IItem item)
        {
            if (item is WeaponInstance weapon)
            {
                // if (weaponObject == null)
                // {
                //     if (m_Animator != null)
                //     {
                //         m_Animator.SetBool(AnimatorVariables.WeaponMode, false);
                //     }
                //
                //     return;
                // }

                if (m_Animator != null)
                {
                    m_Animator.SetBool(AnimatorVariables.WeaponMode, true);
                }

                if (weapon == currentlyHeldWeapon && weaponObject != null) return;

                if (weaponObject != null)
                {
                    Destroy(weaponObject.gameObject);
                }

                weaponObject = Instantiate(weapon.GetWeapon().weaponGameObject, m_weaponParentObject.transform, false);
                m_IKControl.ClearIK();
                m_IKControl.rightHandObj = weaponObject.grip.transform;
                m_IKControl.leftHandObj = weaponObject.foreGrip.transform;
            }

            if (item is ArmorInstance armor) //&& isPlayer == false)
            {
                switch (armor.GetArmor().armorSlot)
                {
                    case ArmorSlot.HEAD:
                        if (m_HelmetMeshFilter != null && m_HelmetMeshRenderer != null)
                        {
                            m_HelmetMeshFilter.gameObject.SetActive(true);
                            m_HelmetMeshFilter.mesh = armor.GetArmor().meshFilter.sharedMesh;
                            m_HelmetMeshRenderer.sharedMaterials = armor.GetArmor().meshRenderer.sharedMaterials;
                        }

                        break;
                    case ArmorSlot.TORSO:
                        if (m_TorsoMeshRenderer != null)
                        {
                            m_TorsoMeshRenderer.sharedMesh = armor.GetArmor().skinnedMeshRenderer.sharedMesh;
                            m_TorsoMeshRenderer.sharedMaterials = armor.GetArmor().skinnedMeshRenderer.sharedMaterials;
                        }

                        break;
                    case ArmorSlot.LEGS:
                        if (m_LegsMeshRenderer != null)
                        {
                            m_LegsMeshRenderer.sharedMesh = armor.GetArmor().skinnedMeshRenderer.sharedMesh;
                            m_LegsMeshRenderer.sharedMaterials = armor.GetArmor().skinnedMeshRenderer.sharedMaterials;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DeleteBodyModelObject(IItem itemInstance)
        {
            if (itemInstance is WeaponInstance)
            {
                if (weaponObject != null)
                {
                    Destroy(weaponObject.gameObject);
                }
            }

            if (itemInstance is ArmorInstance armor) //&& isPlayer == false)
            {
                switch (armor.GetArmor().armorSlot)
                {
                    case ArmorSlot.HEAD:
                        if (m_HelmetMeshFilter != null && m_HelmetMeshRenderer != null)
                        {
                            m_HelmetMeshFilter.gameObject.SetActive(false);
                            // m_HelmetMeshFilter.mesh = null;
                            // m_HelmetMeshRenderer.sharedMaterials = null;
                        }

                        break;
                    case ArmorSlot.TORSO:
                        if (m_TorsoMeshRenderer != null)
                        {
                            m_TorsoMeshRenderer.sharedMesh = m_DefaultTorso.sharedMesh;
                            m_TorsoMeshRenderer.sharedMaterials = m_DefaultTorso.sharedMaterials;
                        }

                        break;
                    case ArmorSlot.LEGS:
                        if (m_LegsMeshRenderer != null)
                        {
                            m_LegsMeshRenderer.sharedMesh = m_DefaultLegs.sharedMesh;
                            m_LegsMeshRenderer.sharedMaterials = m_DefaultLegs.sharedMaterials;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool CheckIfItemIsEquipped(IItem item)
        {
            // Debug.Log("CheckIfItemIsEquipped");
            if (item is WeaponInstance weapon)
            {
                if (equippedWeapons[weapon.GetWeaponTypeInt()] == weapon)
                {
                    return true;
                }
            }

            if (item is ArmorInstance armor)
            {
                if (equippedArmors[armor.GetArmorTypeInt()] == armor)
                {
                    return true;
                }
            }

            return false;
        }

        public void CheckAllEquippedItems()
        {
            foreach (var armor in equippedArmors)
            {
                if (GetAmount(armor) == 0)
                {
                    TakeOffItem(armor);
                }
            }

            foreach (var weapon in equippedWeapons)
            {
                if (GetAmount(weapon) == 0)
                {
                    TakeOffItem(weapon);
                }
            }
        }

        public void EquipItem(IItem item)
        {
            if (item is WeaponInstance weapon)
            {
                equippedWeapons[weapon.GetWeaponTypeInt()] = weapon;
                //jezeli gracz nie ma zadnej broni w ręce to automatycznie ekwipuje mu do ręki broń albo jezeli zmienil bron tego samego typu
                if (currentlyHeldWeapon == null ||
                    currentlyHeldWeapon.GetWeapon().weaponType == weapon.GetWeapon().weaponType)
                {
                    SetWeaponInHand(weapon.GetWeaponTypeInt());
                }
            }
            else if (item is ArmorInstance armor)
            {
                equippedArmors[armor.GetArmorTypeInt()] = armor;
                SetBodyModelObject(armor);
            }
        }

        public void TakeOffItem(IItem item)
        {
            if (item is WeaponInstance weapon)
            {
                equippedWeapons[weapon.GetWeaponTypeInt()] = null;
                if (currentlyHeldWeapon == item) //jezeli to byla bron ktora gracz mial w rece to usuwa jej model
                {
                    // DeleteBodyModelObject(weapon);
                    currentlyHeldWeapon = null;
                    CanvasController.Instance.ammoInterface.SetAmmoCanvas(currentlyHeldWeapon);
                    CanvasController.Instance.skillsCanvasController.SetSkillsInterface(null);
                    if (m_Animator)
                    {
                        m_Animator.SetBool(AnimatorVariables.WeaponMode, false);
                    }
                }
            }

            if (item is ArmorInstance armor)
            {
                equippedArmors[armor.GetArmorTypeInt()] = null;
                // print("kiedyś trzeba dodać zdejmowanie itemów"); // dodałem
            }

            DeleteBodyModelObject(item);
        }

        public override void AddItem(IItem item, int amount)
        {
            if (isPlayer)
            {
                if (currentWeight + item.Weight * amount > maxWeight)
                {
                    Debug.Log("Overweight");
                }
            }

            InventoryDictionary.AddItem(item, amount);
            currentWeight += item.Weight * amount;
        }

        public override void RemoveItem(IItem item, int amount)
        {
            InventoryDictionary.RemoveItem(item, amount);
            currentWeight -= item.Weight * amount;

            if (CheckIfItemIsEquipped(item))
            {
                TakeOffItem(item);
                m_IKControl.ClearIK();
            }
        }

        // public override int GetAmount(IItem item)
        // {
        // 	return InventoryDictionary.GetAmount(item);
        // }

        public void ClearInventory()
        {
            InventoryDictionary.Clear();
            currentWeight = 0;

            currentlyHeldWeapon = null;
            for (int i = 0; i < equippedArmors.Length; i++)
            {
                equippedArmors[i] = null;
            }

            for (int i = 0; i < equippedWeapons.Length; i++)
            {
                equippedWeapons[i] = null;
            }
        }

        public int GetMoney()
        {
            return InventoryDictionary.GetAmount(ItemIndex.currencyItem);
        }

        public void SetMoney(int amount)
        {
            InventoryDictionary.SetAmount(ItemIndex.currencyItem, amount);
        }


        public void SetData(EquipmentSaveData data)
        {
            equipmentSaveData = data;

            //InventoryDictionary = new InventoryDictionary();

            if (data.items != null)
            {
                foreach (ItemSaveData item in data.items)
                {
                    BaseItem baseItem = ItemIndex.GetById(item.itemId);

                    if (baseItem is StackableItem stackableItem)
                    {
                        //InventoryDictionary.AddItem(stackableItem, item.count);
                        AddItem(stackableItem, item.count);
                    }

                    if (baseItem is UniqueItem uniqueItem)
                    {
                        for (int i = 0; i < item.count; i++)
                        {
                            item.uniqueItemInstance.item = uniqueItem;
                            item.uniqueItemInstance.SetItemLvl(item.uniqueItemInstance.ItemLvl);
                            //InventoryDictionary.AddItem(item.uniqueItemInstance, 1);
                            AddItem(item.uniqueItemInstance, 1);
                        }
                    }
                }

                foreach (ItemSaveData item in data.equippedItems)
                {
                    if (item != null)
                    {
                        EquipItem(item.uniqueItemInstance);
                    }
                }
            }
        }

        private void SaveInventory()
        {
            if (equipmentSaveData != null)
            {
                equipmentSaveData.items = new ItemSaveData[InventoryDictionary.UniqueItemCount()];

                int i = 0;
                foreach (KeyValuePair<IItem, int> item in InventoryDictionary)
                {
                    equipmentSaveData.items[i] = new ItemSaveData
                    {
                        itemId = item.Key.ItemName,
                        count = item.Value,
                        uniqueItemInstance = (item.Key is UniqueItemInstance ii ? ii : null)
                    };
                    i++;
                }

                int equipedItemIndex = 0;
                foreach (ArmorInstance armor in equippedArmors)
                {
                    if (armor != null)
                    {
                        equipmentSaveData.equippedItems[equipedItemIndex] = new ItemSaveData
                        {
                            itemId = armor.ItemName,
                            uniqueItemInstance = armor
                        };
                    }

                    equipedItemIndex++;
                }

                foreach (WeaponInstance weapon in equippedWeapons)
                {
                    if (weapon != null)
                    {
                        equipmentSaveData.equippedItems[equipedItemIndex] = new ItemSaveData
                        {
                            itemId = weapon.ItemName,
                            uniqueItemInstance = weapon
                        };
                    }

                    equipedItemIndex++;
                }
            }
        }

        private void OnDestroy()
        {
            SaveInventory();

            SaveSystem.OnUpdateSaveContent -= SaveInventory;
        }

        private void OnValidate()
        {
            if (!m_Animator) m_Animator = GetComponentInChildren<Animator>(true);
        }
    }
}