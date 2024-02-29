using System;
using System.Collections;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Skills;
using UnityEngine;

namespace InventorySystem.Items
{
	[CreateAssetMenu(fileName = nameof(Weapon), menuName = "InventorySystem/Items/" + nameof(Weapon))]
	public class Weapon : UniqueItem
	{
		[Header("Weapon Options")] public WeaponType weaponType;
		public AmmunitionType ammunitionType;
		public Ammunition ammunition;
		public int magazineCapacity;
		public int baseDamage;
		public float attackSpeed;
		public float reloadSpeed;
		public float baseAccuracy;
		public int effectiveRange;

		public GunController weaponGameObject;

		public override UniqueItemInstance CreateInstance(int itemLvl)
		{
			WeaponInstance instance = new();
			instance.item = this;
			instance.SetItemLvl(itemLvl);
			return instance;
		}
	}

	[System.Serializable]
	public class WeaponInstance : UniqueItemInstance
	{
		public int currentAmmo;
		[NonSerialized] public int damage;
		[NonSerialized] public float accuracy;

		public SkillSlot[] skillSlots = new SkillSlot[2];

		public void SetSkillSlot(int skillId, ActivatedSkill skill)
		{
			skillSlots[skillId] = new SkillSlot();
			skillSlots[skillId].skill = skill;
			skillSlots[skillId].isActive = false;
			skillSlots[skillId].cooldown = skill.cooldown;
		}

		public override void SetItemLvl(int iLvl)
		{
			if (iLvl > UniqueItem.s_MaxItemLevel)
			{
				Debug.LogError($"Cannot upgrade item above {UniqueItem.s_MaxItemLevel}");
				return;
			}
			itemLvl = iLvl;
			damage = GetWeapon().baseDamage + (GetWeapon().baseDamage * itemLvl / 10);
			accuracy = GetWeapon().baseAccuracy + (itemLvl - 1) / 550f;
			accuracy = Mathf.Clamp(accuracy, 0, 1f);

			sellPrice = GetSellPrice(iLvl);
			buyPrice = sellPrice * 2;
		}

		public int GetWeaponTypeInt()
		{
			return (int)((Weapon)item).weaponType;
		}

		public string GetWeaponTypeString()
		{
			return Enum.GetName(typeof(WeaponType), (int)((Weapon)item).weaponType);
		}

		public int GetWeaponMagazineCapacity()
		{
			return ((Weapon)item).magazineCapacity;
		}

		public Weapon GetWeapon()
		{
			return (Weapon)item;
		}

		public override int GetUpgradeCost()
		{
			return 10 + Math.Abs(GetSellPrice(itemLvl) - GetSellPrice(itemLvl+1));
		}
	}

	public enum WeaponType
	{
		LONG_WEAPON,
		SHORT_WEAPON
	}

	[Serializable]
	public class SkillSlot
	{
		public float cooldown;
		public bool isActive;
		[NonSerialized] public ActivatedSkill skill;
		public string skillId;


		[OnSerializing()]
		public void OnSerializing(StreamingContext context)
		{
			skillId = skill.name;
		}

		[OnDeserialized()]
		public void OnDeserialize(StreamingContext context)
		{
			skill = Resources.Load<ActivatedSkill>($"Items/Skills/{skillId}");
		}
	}
}