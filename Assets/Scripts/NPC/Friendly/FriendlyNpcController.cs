using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Interactions;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;
using UnityEngine.Serialization;
using World.TownGeneration;
using Random = UnityEngine.Random;

namespace NPC.Friendly
{
	public class FriendlyNpcController : MonoBehaviour, ISaveData<FriendlyNpcData>
	{
		[NonSerialized] public FriendlyNpcData friendlyNpcData;
		
		public string npcName;

		[NonSerialized] public HashSet<INpcDialogueAction> dialogueActions = new();

		[SerializeField] private QuestGiverController m_QuestGiver;
		[SerializeField] private TraderController m_Trader;
		[SerializeField] private Upgrader m_Upgrader;
		[SerializeField] private Wanderer m_Wanderer;
		[SerializeField] private Healer m_Healer;
		[SerializeField] private StorylineNpc m_StorylineNpc;
		[SerializeField] private Talkable m_Talkable;

		[FormerlySerializedAs("objectNameTag")]
		[SerializeField] private ObjectNameTagController m_ObjectNameTagController;
		private bool m_NameTagPermanent;

		[SerializeField] private EquipmentController m_EquipmentController;

		public string dialogueText = "";

		private void MakeNpcImportant()
		{
			if (m_NameTagPermanent == false)
			{
				m_ObjectNameTagController.ShowNpcName(npcName, Color.white);
				m_NameTagPermanent = true;
			}

			m_Talkable.outlineEnabled = true;
		}

		private void Start()
		{
			if (m_EquipmentController != null && m_EquipmentController.InventoryDictionary.UniqueItemCount() == 0)
			{
				FitNPC();
			}

			if (friendlyNpcData == null)
			{
				if (m_Trader != null)
				{
					Debug.Log($"{npcName} becomes trader");
					BecomeTrader(null, m_Trader.traderType);
				}
				
				if (m_StorylineNpc != null)
				{
					dialogueActions.Clear();
					BecomeStorylineNpc();
				}
			}
		}

		public void SetData(FriendlyNpcData npcData)
		{
			friendlyNpcData = npcData;
			npcName = npcData.npcName;

			if (npcData.questGiverData != null) BecomeQuestGiver(npcData.questGiverData);
			if (npcData.traderData != null) BecomeTrader(npcData.traderData, npcData.traderData.traderType);

			if ((npcData.npcType & NpcType.upgrader) == NpcType.upgrader) BecomeUpgrader();
			if ((npcData.npcType & NpcType.healer) == NpcType.healer) BecomeHealer();
			if ((npcData.npcType & NpcType.storylineNpc) == NpcType.storylineNpc) BecomeStorylineNpc();
			if ((npcData.npcType & NpcType.wanderer) == NpcType.wanderer)
				BecomeWanderer(TownGenerator.Instance.housesDict[npcData.houseData].m_WandererPoints);

			if (m_EquipmentController)
			{
				if (friendlyNpcData.equipmentSaveData != null)
				{
					m_EquipmentController.SetData(friendlyNpcData.equipmentSaveData);
				}
				else
				{
					friendlyNpcData.equipmentSaveData = new EquipmentSaveData();
					FitNPC();
				}
			}
		}

		public void BecomeQuestGiver(QuestGiverData questGiverData)
		{
			if (m_QuestGiver == null)
			{
				QuestGiverController v = gameObject.AddComponent<QuestGiverController>();

				if (questGiverData != null)
				{
					v.SetData(questGiverData);
				}

				v.npc = this;
				dialogueActions.Add(v);
				m_QuestGiver = v;
				gameObject.name += "QuestGiver ";

				MakeNpcImportant();
			}
		}

		public void BecomeTrader(TraderData traderData, TraderType traderType)
		{
			if (m_Trader == null)
			{
				m_Trader = gameObject.AddComponent<TraderController>();
			}

			if (traderData != null)
			{
				m_Trader.SetData(traderData);
			}

			dialogueActions.Add(m_Trader);
			m_Trader.PrepareItemsToSell(traderType);

			gameObject.name = "Trader " + traderType + " ";

			MakeNpcImportant();
		}

		public void BecomeUpgrader()
		{
			if (m_Trader == null)
			{
				var v = gameObject.AddComponent<Upgrader>();
				dialogueActions.Add(v);
				m_Upgrader = v;
				gameObject.name += "Upgrader ";
			}

			MakeNpcImportant();
		}

		public void BecomeWanderer(Transform[] points)
		{
			if (m_Wanderer == null)
			{
				var v = gameObject.AddComponent<Wanderer>();
				v.SetWandererPoints(points);
				m_Wanderer = v;
				gameObject.name = "Wanderer ";
				dialogueText = $"Welcome to {friendlyNpcData.villageData.villageName} traveller.";
			}
		}

		public void BecomeHealer()
		{
			if (m_Healer == null)
			{
				var v = gameObject.AddComponent<Healer>();
				dialogueActions.Add(v);
				m_Healer = v;
				gameObject.name = "Healer ";
				dialogueText = m_Healer.interactionText;
			}

			MakeNpcImportant();
		}

		public void BecomeStorylineNpc()
		{
			if (m_StorylineNpc == null)
			{
				m_StorylineNpc = gameObject.AddComponent<StorylineNpc>();
			}

			dialogueActions.Add(m_StorylineNpc);
			gameObject.name = "Storyline ";

			MakeNpcImportant();
			
			
			
			
			// if (m_StorylineNpc == null)
			// {
			// 	var v = gameObject.AddComponent<StorylineNpc>();
			// 	m_StorylineNpc = v;
			// 	v.npc = this;
			// }
			// dialogueActions.Clear();
			// dialogueActions.Add(m_StorylineNpc);
			// gameObject.name = "Storyline ";
			// dialogueText = "";
			//
			// EnablePermanentNameTag(true);
		}


		private void FitNPC()
		{
			List<Armor> legs = ItemIndex.GetArmorBySlot(ArmorSlot.LEGS);
			int randLegs = Random.Range(0, legs.Count);
			UniqueItem legsItem = legs[randLegs];
			FitUniqueItem(legsItem);

			List<Armor> torso = ItemIndex.GetArmorBySlot(ArmorSlot.TORSO);
			int randTorso = Random.Range(0, torso.Count);
			UniqueItem torsoItem = torso[randTorso];
			FitUniqueItem(torsoItem);

			if (Random.value >= 0.75)
			{
				List<Armor> head = ItemIndex.GetArmorBySlot(ArmorSlot.HEAD);
				int randHead = Random.Range(0, head.Count);
				UniqueItem headItem = head[randHead];
				FitUniqueItem(headItem);				
			}
		}

		private void FitUniqueItem(UniqueItem item)
		{
			UniqueItemInstance itemInstance = item.CreateInstance(1);
			m_EquipmentController.AddItem(itemInstance, 1);
			m_EquipmentController.EquipItem(itemInstance);
		}


		private void OnValidate()
		{
			if (m_Upgrader == null) m_Upgrader = GetComponent<Upgrader>();
			if (m_Healer == null) m_Healer = GetComponent<Healer>();
			if (m_Trader == null) m_Trader = GetComponent<TraderController>();
			if (m_QuestGiver == null) m_QuestGiver = GetComponent<QuestGiverController>();
			if (m_EquipmentController == null) m_EquipmentController = GetComponent<EquipmentController>();
			if (m_Talkable == null) m_Talkable = GetComponent<Talkable>();
			//if (!m_Ani) m_Animator = GetComponentInChildren<Animator>();
		}

		public FriendlyNpcData GetData()
		{
			throw new NotImplementedException();
		}
	}
}