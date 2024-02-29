using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using Skills;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace HealthSystem
{
    public class HealthController : MonoBehaviour
    {

        public bool godMode;
        
        // public BodyPartPreset head;
        // public BodyPartPreset stomach;
        // public BodyPartPreset leftArm;
        // public BodyPartPreset rightArm;
        // public BodyPartPreset leftLeg;
        // public BodyPartPreset rightLeg;
        // [NonSerialized] public BodyPartPreset[] bodyPartPresets;

        public CharacterHealthSkeleton<BodyPartPreset> bodyPartPresets;
        public CharacterHealthSkeleton<int> currentHp;
        public bool healOnStart = true;

        // private HealthData m_HealthData;
        //
        // public HealthData HealthData
        // {
        //     get => m_HealthData;
        //     set
        //     {
        //         m_HealthData = value;
        //         OnDebuffChanged.Invoke(this);
        //         OnHeal.Invoke(this);
        //     }
        // }

        public List<PassiveSkill> debuffs = new List<PassiveSkill>();


        public UnityEvent<HealthController> OnHeal;
        public UnityEvent<HealthController> OnDamageReceived;
        public UnityEvent<HealthController> OnDebuffChanged;
        public UnityEvent<HealthController> OnDeath;

        private EquipmentController m_EquipmentController;

        [NonSerialized] public int lastReceivedDamage;

        private void Awake()
        {
            // bodyPartPresets = new BodyPartPreset[] { head, stomach, leftArm, rightArm, leftLeg, rightLeg };

            m_EquipmentController = GetComponent<EquipmentController>();
        }

        private void Start()
        {
            // HealthData ??= new HealthData();
            if (healOnStart)
            {
                HealAll();
            }
        }

        public int ApplyDamage(int dmg) //, (BodyPart, int) bonusAccuracy)
        {
            if (godMode)
            {
                return 0;
            }
            
            lastReceivedDamage = 0;
            //Suma wag czesci ciala
            int totalWeight = bodyPartPresets.Sum(part => part.Item2.weight);
            int rand = Random.Range(0, totalWeight);
            int usedWeight = 0;

            foreach (CharacterBodyPart bodyPart in (CharacterBodyPart[])Enum.GetValues(typeof(CharacterBodyPart)))
            {
                //sprawdzenie w co trafil gracz
                if (usedWeight + bodyPartPresets[bodyPart].weight /*+ bonusWeight*/ > rand)
                {
                    //rozlozyc dmg na wszystkie zywe czesci
                    if (currentHp[bodyPart] <= 0)
                    {
                        SpreadDamage(dmg);
                    }
                    else
                    {
                        //sprawdzic czy dmg zniszczy czesc ciala i jezeli zostanie jakas reszta dmg to rozlozyc ja po czesciach ciala
                        int dmgAfterArmorReduction = (DamageReducedByArmor(dmg, bodyPart)); //przemnozyc dmg przez armorPoints / 100;
                        currentHp[bodyPart] -= dmgAfterArmorReduction;
                        lastReceivedDamage += dmgAfterArmorReduction;

                        OnDamageReceived.Invoke(this);
                        if (currentHp[bodyPart] < 0)
                        {
                            AddDebuff(bodyPart);
                            SpreadDamage(-currentHp[bodyPart]);
                        }
                    }

                    break;
                }

                usedWeight += bodyPartPresets[bodyPart].weight;
            }

            if (currentHp.head <= 0)
            {
                Alive = false;
                OnDeath.Invoke(this);
            }

            return lastReceivedDamage;
        }

        private int DamageReducedByArmor(int dmg, CharacterBodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case CharacterBodyPart.RightLeg:
                case CharacterBodyPart.LeftLeg:
                    if (m_EquipmentController.equippedArmors[(int)ArmorSlot.LEGS] != null)
                    {
                        return dmg - (dmg * m_EquipmentController.equippedArmors[(int)ArmorSlot.LEGS].armorPoints / 100);
                    }
                    return dmg;
                case CharacterBodyPart.RightArm:
                case CharacterBodyPart.LeftArm:
                case CharacterBodyPart.Torso:
                    if (m_EquipmentController.equippedArmors[(int)ArmorSlot.TORSO] != null)
                    {
                        return dmg - (dmg * m_EquipmentController.equippedArmors[(int)ArmorSlot.LEGS].armorPoints / 100);
                    }
                    return dmg;
                    //return dmg - (dmg * m_EquipmentController.equippedArmors[(int)ArmorSlot.TORSO].armorPoints / 100);
                case CharacterBodyPart.Head:
                    if (m_EquipmentController.equippedArmors[(int)ArmorSlot.HEAD] != null)
                    {
                        return dmg - (dmg * m_EquipmentController.equippedArmors[(int)ArmorSlot.LEGS].armorPoints / 100);
                    }
                    return dmg;
                    //return dmg - (dmg * m_EquipmentController.equippedArmors[(int)ArmorSlot.HEAD].armorPoints / 100);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bodyPart), bodyPart, null);
            }
        }

        private void SpreadDamage(int dmg)
        {
            int aliveBodyPartsCount = currentHp.Count(x => x.Item2 > 0);
            // int aliveBodyPartsCount = 0;
            // for (int i = 0; i < bodyPartPresets.Length; i++)
            // {
            //     if (HealthData[i] > 0)
            //     {
            //         aliveBodyPartsCount++;
            //     }
            // }

            // for (int i = 0; i < bodyPartPresets.Length; i++)
            // {
            //     if (HealthData[i] > 0)
            //     {
            //         int dmgAfterArmorReduction = (int)(DamageReducedByArmor(dmg, bodyPartPresets[i]) / aliveBodyPartsCount);
            //         HealthData[i] -= dmgAfterArmorReduction;
            //         lastReceivedDamage += dmgAfterArmorReduction;
            //
            //         if (HealthData[i] <= 0)
            //         {
            //             AddDebuff(bodyPartPresets[i]);
            //         }
            //     }
            // }

            foreach (CharacterBodyPart bodyPart in (CharacterBodyPart[])Enum.GetValues(typeof(CharacterBodyPart)))
            {
                if (currentHp[bodyPart] > 0)
                {
                    int dmgAfterArmorReduction = (int)(DamageReducedByArmor(dmg, bodyPart) / aliveBodyPartsCount);
                    currentHp[bodyPart] -= dmgAfterArmorReduction;
                    lastReceivedDamage += dmgAfterArmorReduction;

                    if (currentHp[bodyPart] <= 0)
                    {
                        AddDebuff(bodyPart);
                    }
                }
            }
        }

        private void AddDebuff(CharacterBodyPart bodyPart)
        {
            if (bodyPartPresets[bodyPart].debuff != null && debuffs.Count(x => x == bodyPartPresets[bodyPart].debuff) < 2)
            {
                debuffs.Add(bodyPartPresets[bodyPart].debuff);
                OnDebuffChanged.Invoke(this);
            }
        }

        private void RemoveDebuff(CharacterBodyPart bodyPart)
        {
            if (bodyPartPresets[bodyPart].debuff != null)
            {
                debuffs.Remove(bodyPartPresets[bodyPart].debuff);
                OnDebuffChanged.Invoke(this);
            }
        }

        // private void HealPart(BodyPart bodyPart, int value)
        // {
        //     bodyPart.health = Mathf.Max(bodyPart.health + value, bodyPart.bodyPart.maxHealth);
        //     RemoveDebuff(bodyPart);
        //     OnHeal.Invoke(this);
        // }

        public void HealParts(int value)
        {
            // BodyPart[] damagedBodyParts = bodyPartPresets.Where(x => x.health < x.bodyPart.maxHealth).ToArray();
            // foreach (BodyPart bodyPart in damagedBodyParts)
            // {
            //     bodyPart.health = Mathf.Clamp(bodyPart.health + (value / damagedBodyParts.Length), 0, bodyPart.bodyPart.maxHealth);
            //     RemoveDebuff(bodyPart);
            // }
            //
            // OnHeal.Invoke(this);

            CharacterBodyPart[] damagedBodyParts = currentHp.Where(x => x.Item2 < bodyPartPresets[x.Item1].maxHealth).Select(x => x.Item1).ToArray();

            foreach (CharacterBodyPart bodyPartId in damagedBodyParts)
            {
                currentHp[bodyPartId] = Mathf.Clamp(currentHp[bodyPartId] + (value / damagedBodyParts.Length), 0, bodyPartPresets[bodyPartId].maxHealth);
                RemoveDebuff(bodyPartId);
            }

            OnHeal.Invoke(this);
        }

        public void HealAll()
        {
            // for (int i = 0; i < bodyPartPresets.Length; i++)
            // {
            //     HealthData[i] = bodyPartPresets[i].maxHealth;
            //     RemoveDebuff(bodyPartPresets[i]);
            // }

            foreach (CharacterBodyPart bodyPart in Enum.GetValues(typeof(CharacterBodyPart)))
            {
                currentHp[bodyPart] = bodyPartPresets[bodyPart].maxHealth;
                RemoveDebuff(bodyPart);
            }

            OnHeal.Invoke(this);
        }

        // public BodyPart GetBodyPartByName(string name)
        // {
        //     return bodyPartPresets.FirstOrDefault(x => x.bodyPart.name == name);
        // }
        
        public bool Alive { get; private set; }
    }
}