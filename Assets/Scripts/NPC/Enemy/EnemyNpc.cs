using System;
using HealthSystem;
using System.Collections.Generic;
using InventorySystem;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Utils.StateMachine;
using Random = UnityEngine.Random;
using Interactions;
using InventorySystem.Items;
using NPC;
using Skills;

public class EnemyNpc : MonoBehaviour
{
	public static readonly HashSet<EnemyNpc> aliveEnemies = new();

	[Header("Components")] public HealthController healthController;
	public EquipmentController equipmentController;
	public NavMeshAgent agent;
	[SerializeField] private Animator m_Animator;
	[SerializeField] private RagdollController m_Ragdoll;
	[SerializeField] private Lootable m_Lootable;

	public float baseMoveSpeed;

	public float seekTime = 5;
	[NonSerialized] public float seekTimeRemaining;

	public float shootRangeTolerance = 1f; // dodatkowa losowa warto�� dystansu od kt�rego przeciwnik zmieni attack state na approach
	public float approachMaxRandomModifier = 1f; // maksymalny modyfikator dystansu na kt�ry podchodz� przeciwnicy

	[NonSerialized] public float attackStateTriggerDistance;
	[NonSerialized] public float attackApproachDistance;

	[NonSerialized] public bool playerInDetectionRange;
	[NonSerialized] public bool playerIsSeen;
	[NonSerialized] public bool playerWasSeen;
	[NonSerialized] public bool playerInShootRange;


	[NonSerialized] public float shootTimer;
	[NonSerialized] public float reloadTimer;
	//[NonSerialized] public bool isReloading;

	public bool isTutorialEnemy = false;
	
	private bool m_IsDead;

	[NonSerialized] public Vector3 lastPlayerLocation;

	public EnemyIdleType idleType;

	[Header("Random Wander")] public Vector3 spawnPosition;
	public float patrolRadius;

	public LayerMask sightLayerMask;

	[SerializeField] private ObjectNameTagController m_ObjectNameTagController;

	// state machine
	private StateMachine<EnemyNpc> m_StateMachine;
	public SmartCoroutine stateMachineCoroutine;

	//bodypart debuffs
	public float moveSpeedDecrease;
	public float accuracyDecrease;

	//[SerializeField] private bool RandomInventory;
	[SerializeField] public Weapon m_WeaponItem;
	[SerializeField] private Armor m_TorsoItem;
	[SerializeField] private Armor m_LegsItem;
	[SerializeField] private Armor m_HelmetItem;


	[Header("Outline")]
	[SerializeField] private Renderer[] m_Renderers;
	private bool m_Highlighted;

	// Start is called before the first frame update
	private void Start()
	{
		m_Animator.SetBool(AnimatorVariables.WeaponMode, true);
		aliveEnemies.Add(this);

		string enemyName = NameGenerator.GenerateName();
		if (m_Lootable != null)
		{
			m_Lootable.lootableName = enemyName;
		}

		stateMachineCoroutine.SetMonoBehavior(this);
		spawnPosition = transform.position;

		m_StateMachine.Start(this, EnemyIdleState.Instance);
		CanvasController.Instance.questObjectiveTracker.SetObjectives();
	}

	private void Update()
	{
		if (m_IsDead) return;

		if (!isTutorialEnemy)
		{
			const float dampTime = 0.1f;
			float speed = agent.velocity.magnitude;
			m_Animator.SetFloat(AnimatorVariables.WalkY, speed / 4f, dampTime, Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (m_IsDead) return;

		if ((playerWasSeen || playerInDetectionRange) && !isTutorialEnemy)
		{
			PlayerControllerWSAD player = PlayerController.Instance.playerControllerLocal;
			if (player != null)
			{
				Vector3 playerPosition = player.transform.position;
				Vector3 playerHeadPosition = new(playerPosition.x, playerPosition.y + 1.6f, playerPosition.z);
				Vector3 enemyPosition = transform.position;
				Vector3 enemyHeadPosition = new(enemyPosition.x, enemyPosition.y + 1.6f, enemyPosition.z);
				Debug.DrawLine(playerHeadPosition, enemyHeadPosition);
				bool raycast = Physics.Raycast(enemyHeadPosition, (playerHeadPosition - enemyHeadPosition), out RaycastHit hit, 100f, sightLayerMask);
				if (raycast && hit.collider.gameObject == player.gameObject)
				{
					playerIsSeen = true;
					playerWasSeen = true;
				}
				else
				{
					playerIsSeen = false;
				}
			}
		}

		if (reloadTimer > 0)
		{
			reloadTimer -= Time.deltaTime;
			if (reloadTimer <= 0)
			{
				equipmentController.currentlyHeldWeapon.currentAmmo = equipmentController.currentlyHeldWeapon.GetWeaponMagazineCapacity();
			}
		}

		m_StateMachine.FixedUpdate();
	}

	public void TriggerEnemy()
	{
		playerWasSeen = true;
	}

	private void OnDestroy()
	{
		aliveEnemies.Remove(this);
	}

	public void OnDeath()
	{
		m_IsDead = true;
		aliveEnemies.Remove(this);
		m_Animator.enabled = false;
		agent.enabled = false;

		m_StateMachine.Stop();
		
		equipmentController.weaponObject.DropWeaponObject();
		m_Ragdoll.EnableRagdoll();

		if (PlayerController.Instance.playerControllerGlobal.currentLocation is Quest questLocation)
		{
			questLocation.CheckIfQuestCompleted();
		}
		CanvasController.Instance.questObjectiveTracker.SetObjectives();
		
		Highlight(false);
	}

	public void Shoot()
	{
		if (equipmentController.currentlyHeldWeapon.currentAmmo > 0)
		{
			WeaponInstance weapon = equipmentController.currentlyHeldWeapon;
			float attackSpeed = weapon.GetWeapon().attackSpeed;

			shootTimer += Time.deltaTime;
			while (shootTimer >= attackSpeed)
			{
				shootTimer -= attackSpeed;

				PlayerControllerWSAD player = PlayerController.Instance.playerControllerLocal;

				float distance = Vector3.Distance(player.transform.position, transform.position);
				float randomNumber = Random.value;

				float weaponAccuracy = weapon.accuracy * (1 - accuracyDecrease);

				if (distance <= weapon.GetWeapon().effectiveRange)
				{
					WeaponShoot(randomNumber, weapon, weaponAccuracy, 0, player);
				}
				else
				{
					WeaponShoot(randomNumber, weapon, weaponAccuracy, 0.1f, player);
				}

				equipmentController.currentlyHeldWeapon.currentAmmo--;
				equipmentController.weaponObject.PlayEffect();
			}
		}
		else
		{
			if (reloadTimer <= 0)
			{
				reloadTimer = equipmentController.currentlyHeldWeapon.GetWeapon().reloadSpeed;
				DisplayTextStatus("Reloading!", Color.blue, 1);
			}
		}
	}

	private void WeaponShoot(float randomNumber, WeaponInstance weapon, float weaponAccuracy, float accuracyDebuff, PlayerControllerWSAD player)
	{
		if (weapon.GetWeapon().ammunitionType == AmmunitionType.SHOTGUN)
		{
			for (int i = 0; i < 4; i++)
			{
				randomNumber = Random.value;
				if (randomNumber <= weaponAccuracy - accuracyDebuff)
				{

					PlayerController.Instance.healthController.ApplyDamage(weapon.damage);
				}
			}
		}
		else
		{
			if (randomNumber <= weaponAccuracy - accuracyDebuff)
			{
				PlayerController.Instance.healthController.ApplyDamage(weapon.damage);
			}
		}
	}

	public void DisplayTextStatus(string text, Color color, int time)
	{
		ObjectNameTagController objectNameTagController = Instantiate(m_ObjectNameTagController, transform);
		objectNameTagController.SetHitForDuration(text, color, time);
	}

	public void DisplayReceivedDamage(HealthController hc)
	{
		DisplayTextStatus(hc.lastReceivedDamage.ToString(), Color.red, 1);
	}

	public void ApplyBodyDebuffsEffects()
	{
		moveSpeedDecrease = 0;
		accuracyDecrease = 0;
		foreach (PassiveSkill debuff in healthController.debuffs)
		{
			moveSpeedDecrease += debuff.movement;
			accuracyDecrease += debuff.accuracy;
		}

		agent.speed = baseMoveSpeed * (1 - moveSpeedDecrease);
	}



	public void FitEnemy(int value)
	{
		if (m_WeaponItem == null)
		{
			LootPreset weapons = Resources.Load<LootPreset>("LootPresets/Lp_Weapon");
			int randWeapon = Random.Range(0, weapons.items.Length);
			m_WeaponItem = (Weapon)weapons.items[randWeapon];
		}
		FitUniqueItem(m_WeaponItem, (int)(value * 0.5f));

		if (m_LegsItem == null)
		{
			List<Armor> legs = ItemIndex.GetArmorBySlot(ArmorSlot.LEGS);
			int randLegs = Random.Range(0, legs.Count);
			m_LegsItem = legs[randLegs];
		}
		FitUniqueItem(m_LegsItem, (int)(value * 0.5f));

		if (m_TorsoItem == null)
		{
			List<Armor> torso = ItemIndex.GetArmorBySlot(ArmorSlot.TORSO);
			int randTorso = Random.Range(0, torso.Count);
			m_TorsoItem = torso[randTorso];
		}
		FitUniqueItem(m_TorsoItem, (int)(value * 0.5f));

		if (m_HelmetItem == null)
		{
			List<Armor> head = ItemIndex.GetArmorBySlot(ArmorSlot.HEAD);
			int randHead = Random.Range(0, head.Count);
			m_HelmetItem = head[randHead];
		}
		FitUniqueItem(m_HelmetItem, (int)(value * 0.5f));

		equipmentController.currentlyHeldWeapon.currentAmmo = m_WeaponItem.magazineCapacity;
		equipmentController.AddItem(m_WeaponItem.ammunition, Random.Range(m_WeaponItem.magazineCapacity / 2, m_WeaponItem.magazineCapacity * 2));
	}

	private void FitUniqueItem(UniqueItem item, int itemValue)
	{
		UniqueItemInstance itemInstance = SetItemLevel(item, itemValue);
		equipmentController.AddItem(itemInstance, 1);
		equipmentController.EquipItem(itemInstance);
	}

	private UniqueItemInstance SetItemLevel(UniqueItem item, int itemValue)
	{
		int tempXaxILVL = 111;
		//int itemLvlLowerBound = tempXaxILVL - 10;
		UniqueItemInstance itemInstance = item.CreateInstance(tempXaxILVL);
		while (tempXaxILVL >= 2 && itemInstance.SellPrice > itemValue * 1.1f)
		{
			tempXaxILVL--;

			int itemLvlLowerBound = tempXaxILVL - 20;
			if (itemLvlLowerBound <= 0)
			{
				itemLvlLowerBound = 1;
			}

			itemInstance.SetItemLvl(Random.Range(itemLvlLowerBound, tempXaxILVL));
		}

		return itemInstance;
	}

	public void Highlight(bool b)
	{
		if (m_Highlighted && !b)
		{
			MaterialPropertyBlock m_Block = new();
			m_Block.SetInt("_Visible", 0);
			foreach (Renderer r in m_Renderers)
			{
				r.SetPropertyBlock(m_Block);
			}

			m_Highlighted = false;
		}
		else if (!m_Highlighted && b)
		{
			MaterialPropertyBlock m_Block = new();
			m_Block.SetInt("_Visible", 1);
			m_Block.SetColor("_OutlineColor", Color.red);
			foreach (Renderer r in m_Renderers)
			{
				r.SetPropertyBlock(m_Block);
			}

			m_Highlighted = true;
		}
	}


	private void OnValidate()
	{
		if (!agent) agent = GetComponent<NavMeshAgent>();
		if (!healthController) healthController = GetComponent<HealthController>();
		if (!equipmentController) equipmentController = GetComponent<EquipmentController>();
		if (!m_Animator) m_Animator = GetComponentInChildren<Animator>();
		if (!m_Ragdoll) m_Ragdoll = GetComponentInChildren<RagdollController>();
		if (!m_Lootable) m_Lootable = GetComponentInChildren<Lootable>();
	}
}