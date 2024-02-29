using HealthSystem;
using InventorySystem;
using InventorySystem.Items;
using Player;
using UnityEngine;
using World;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
	public GameObject globalMapPlayer;
	public GameObject localMapPlayer;

	public static PlayerController Instance;
	public PlayerControllerWSAD playerControllerLocal;
	public PlayerControllerGlobal playerControllerGlobal;

	public EquipmentController equipmentController;
	public HealthController healthController;

	public float randomEncounterChance = 0f;

	public RandomEventData randomEventData;

	[SerializeField] private EquipmentEntry[] m_StartingEquipment;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		LoadSavedInventory();
		
		if (SaveSystem.saveContent.playerHp != null)
		{
			healthController.currentHp = SaveSystem.saveContent.playerHp;
			healthController.OnDebuffChanged.Invoke(healthController);
			CanvasController.Instance.healthHUDController.UpdateHealthHUD(healthController);
		}
		else
		{
			SaveSystem.saveContent.playerHp = healthController.currentHp;
			healthController.HealAll();
			CanvasController.Instance.healthHUDController.UpdateHealthHUD(healthController);
		}
		
		if (SaveSystem.saveContent.questLog.storylineQuestsFinished == 0 && SaveSystem.saveContent.questLog.storylineQuests.Count == 0)
		{
			StorylineQuestController.SetFirstStorylineQuest();
		}
		else if (SaveSystem.saveContent.questLog.storylineQuestsFinished == SaveSystem.saveContent.questLog.storylineQuests.Count - 1)
		{
			StorylineQuestController.s_StorylineQuest = SaveSystem.saveContent.questLog.storylineQuests[^1];
		}
		else
		{
			StorylineQuestController.s_StorylineQuest = null;
		}
		
		playerControllerLocal.interactionCoroutine.SetMonoBehavior(PlayerController.Instance);
		playerControllerLocal.qSkillCoroutine.SetMonoBehavior(this);
		playerControllerLocal.eSkillCoroutine.SetMonoBehavior(this);
	}

	private void LoadSavedInventory()
	{
		if (SaveSystem.saveContent.playerInventory != null)
		{
			equipmentController.SetData(SaveSystem.saveContent.playerInventory);
		}
		else
		{
			EquipmentSaveData equipmentSaveData = new();
			SaveSystem.saveContent.playerInventory = equipmentSaveData;
			equipmentController.SetData(equipmentSaveData);

			foreach (EquipmentEntry entry in m_StartingEquipment)
			{
				BaseItem item = entry.item;
				if (item is StackableItem stackableItem)
				{
					equipmentController.AddItem(stackableItem, entry.amount);
				}

				if (item is UniqueItem uniqueItem)
				{
					for (int i = 0; i < entry.amount; i++)
					{
						UniqueItemInstance itemInstance = uniqueItem.CreateInstance(entry.ilvl);
						equipmentController.AddItem(itemInstance, 1);

						if (itemInstance is WeaponInstance weapon)
						{
							equipmentController.equippedWeapons[weapon.GetWeaponTypeInt()] = weapon;
							equipmentController.currentlyHeldWeapon = weapon;
						}
						
						if (itemInstance is ArmorInstance armor)
						{
							// equipmentController.equippedArmors[armor.GetArmorTypeInt()] = armor;
							equipmentController.EquipItem(armor);
						}
						
					}
				}
			}
		}
	}

	public void EnterLocation()
	{
		//GlobalMapController.Instance.RemoveLocationNameTextXD();
		CanvasController.Instance.enterLeaveInterfaceController.Clear();
		CanvasController.Instance.ammoInterface.SetAmmoCanvas(equipmentController.currentlyHeldWeapon);
			//ammoInterface.gameObject.SetActive(equipmentController.weaponObject != null);
		CanvasController.Instance.skillsCanvasController.gameObject.SetActive(true);
		globalMapPlayer.SetActive(false);

		if (playerControllerGlobal.currentLocation is VillageData village)
		{
			CanvasController.Instance.clockInterface.UpdateLocationName(village.villageName);
			GameLoader.Instance.LoadVillage();
		}
		else if (playerControllerGlobal.currentLocation is StorylineQuest storylineQuest)
		{
			CanvasController.Instance.clockInterface.UpdateLocationName(storylineQuest.questLocation.locationName);
			playerControllerLocal.locationBoundary.hasToKillEnemies = true;
			GameLoader.Instance.LoadQuest(storylineQuest);
		}
		else if (playerControllerGlobal.currentLocation is Quest quest)
		{
			CanvasController.Instance.clockInterface.UpdateLocationName(quest.questLocation.locationName);
			GameLoader.Instance.LoadQuest(quest);
		}
		CanvasController.Instance.questObjectiveTracker.SetObjectives();
	}

	private void PrepareForLocation(Vector3 spawnPoint)
	{
		playerControllerLocal.playerVelocity = Vector3.zero;
		playerControllerLocal.characterController.enabled = false;
		playerControllerLocal.transform.position = spawnPoint;
		playerControllerLocal.characterController.enabled = true;
		globalMapPlayer.SetActive(false);
		localMapPlayer.SetActive(true);
	}
	
	public void PrepareForHostileLocation(Vector3 spawnPoint)
	{
		PrepareForLocation(spawnPoint);
		equipmentController.SetWeaponInHand(1);
		//CanvasController.Instance.ammoInterface.gameObject.SetActive(equipmentController.weaponObject != null);
	}
	
	public void PrepareForFriendlyLocation(Vector3 spawnPoint)
	{
		PrepareForLocation(spawnPoint);
		equipmentController.HideWeapon();
		//CanvasController.Instance.ammoInterface.gameObject.SetActive(equipmentController.weaponObject != null);
	}

	public void LeaveLocation()
	{
		GameLoader.Instance.UnloadScene();
		
		//sus?
		//CanvasController.Instance.enterLeaveInterfaceController.ShowLeaveLocationInterface(playerControllerGlobal.currentLocation); //leaveLocationButton.gameObject.SetActive(false);
		//sus?
		
		CanvasController.Instance.tutorialCanvasController.gameObject.SetActive(false);
		CanvasController.Instance.ammoInterface.SetAmmoCanvas(null);
		//CanvasController.Instance.ammoInterface.gameObject.SetActive(false);
		CanvasController.Instance.skillsCanvasController.gameObject.SetActive(false);

		if (playerControllerGlobal.currentLocation is Quest quest)
		{
			if (quest.questStatus == QuestStatus.InProgress)
			{
				quest.questStatus = QuestStatus.Failed;
				CanvasController.Instance.m_DialoguePanel.SetActive(false);
				CanvasController.Instance.dialogueCanvasController.gameObject.SetActive(false);
			}
			GlobalMapController.Instance.RemoveLocation(quest);
		}

		if (playerControllerGlobal.agent.CurrentTarget != playerControllerGlobal.agent.CurrentTile)
		{
			TimeController.Instance.timeSpeedMultiplier = 100;
		}
		else
		{
			TimeController.Instance.timeSpeedMultiplier = 1;
		}

		randomEventData = null;
		playerControllerLocal.locationBoundary.hasToKillEnemies = false;
		localMapPlayer.SetActive(false);
		globalMapPlayer.SetActive(true);
		CanvasController.Instance.clockInterface.UpdateLocationName("World");
		CanvasController.Instance.questObjectiveTracker.SetObjectives();
	}

	public void RandomEncounter(Vector2Int position)
	{
		if (!GlobalMapController.Instance.IsPositionALocation(position)) return;

		float randomNumberForIncreasingEncounterChance = Random.value;
		if (randomNumberForIncreasingEncounterChance < 0.05f) randomEncounterChance += 0.01f;

		float randomNumber = Random.value;
		if (randomNumber < randomEncounterChance)
		{
			randomEventData = new RandomEventData();
			CanvasController.Instance.randomEventInterfaceController.ShowInterface(randomEventData);
			playerControllerGlobal.agent.forceStop = true;
		}
	}

	public void OnDeath()
	{
		playerControllerLocal.animator.enabled = false;

		if (equipmentController.weaponObject != null)
		{
			equipmentController.weaponObject.DropWeaponObject();
		}
		
		playerControllerLocal.ragdollController.EnableRagdoll();

		SaveSystem.DeleteSave(SaveSystem.currentSaveName);
		GameLoader.Instance.LoadLoseScene();
		//GameLoader.Instance.LoadMainMenu();
	}

	private void OnValidate()
	{
		if (!equipmentController) equipmentController = GetComponent<EquipmentController>();
		if (!healthController) healthController = GetComponent<HealthController>();
	}
}
