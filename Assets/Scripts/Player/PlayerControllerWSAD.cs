using System.Collections;
using System.Collections.Generic;
using Interactions;
using InventorySystem.Items;
using InventorySystem.UI;
using NPC.Friendly;
using Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerControllerWSAD : MonoBehaviour, PlayerWsadInput.IPlayerLocalActions
    {
        [Header("References")]
        public CharacterController characterController;
        [SerializeField] private GameObject m_CameraGameObject;
        [FormerlySerializedAs("m_Camera")] public Camera localCamera;
        [SerializeField] private LocalPlayerCameraInputProvider m_LocalPlayerCameraInputProvider;
        [SerializeField] public Animator animator;
        [SerializeField] public RagdollController ragdollController;

        [SerializeField] private float m_MoveSpeed;
        private Vector2 m_MoveVector;

        private EnemyNpc m_ShootTarget;


        private CanvasAmmoController canvasAmmoController;

        public Vector3 playerVelocity;

        //Interactions with objects
        [Header("Interactions")]
        [SerializeField] private LayerMask m_InteractionLayerMask;


        [HideInInspector] public SmartCoroutine interactionCoroutine;
        private bool m_InteractionInterruptedByWalking;

        // shooting
        private float m_ShootCooldown;
        public LayerMask sightLayerMask;
        private bool m_Reloading;

        //body part debuffs
        private float m_MoveSpeedDecrease;
        private float m_AccuracyDecrease;

        // skills
        [Header("Skills")]
        public SmartCoroutine qSkillCoroutine;
        public SmartCoroutine eSkillCoroutine;
        public float moveSpeedBoost;
        public float fireRateBoost;

        [SerializeField] private LayerMask m_AreaSkillLayerMask;
        private GameObject m_AreaSelector;
        private GameObject m_AreaEffectToSpawn;

        public LocationBoundary locationBoundary;

        private Interactable m_CurrentHoveredInterractable;
        private bool m_CanInteract;

        // Start is called before the first frame update
        private void Start()
        {
            InputManager.input.PlayerLocal.AddCallbacks(this);

            canvasAmmoController = CanvasController.Instance.ammoInterface;
            canvasAmmoController.SetAmmoCanvas(PlayerController.Instance.equipmentController.currentlyHeldWeapon);

            // Debug.Log($"player start: {Time.realtimeSinceStartup}");
        }

        private void Update()
        {
            InteractableDetection();
            // skill area selector
            if (m_AreaSelector)
            {
                Vector2 pos = Mouse.current.position.ReadValue();
                Ray ray = localCamera.ScreenPointToRay(pos);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, m_AreaSkillLayerMask))
                {
                    if (hit.collider != null)
                    {
                        m_AreaSelector.SetActive(true);
                        m_AreaSelector.transform.position = hit.point;
                    }
                    else
                    {
                        m_AreaSelector.SetActive(false);
                    }
                }
            }


            bool groundedPlayer = characterController.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            // move direction
            float cameraAngle = m_CameraGameObject.transform.rotation.eulerAngles.y;
            float playerAngle = transform.rotation.eulerAngles.y;
            Vector3 rotatedMoveVector = Quaternion.AngleAxis(cameraAngle, Vector3.up) * new Vector3(m_MoveVector.x, 0, m_MoveVector.y);

            characterController.Move((m_MoveSpeed + moveSpeedBoost) * (1 - m_MoveSpeedDecrease) * Time.deltaTime * rotatedMoveVector);

            if (m_ShootTarget != null)
            {
                Vector3 position = m_ShootTarget.transform.position;
                transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
            }
            else
            {
                Vector2 positionOnScreen = localCamera.WorldToScreenPoint(transform.position);
                Vector2 mousePos = Mouse.current.position.ReadValue();
                float angle = Mathf.Atan2(positionOnScreen.y - mousePos.y, positionOnScreen.x - mousePos.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 270f - angle + cameraAngle, 0f));
            }

            if (m_MoveVector != Vector2.zero)
            {
                if (m_InteractionInterruptedByWalking)
                {
                    StopInteractionCoroutine();

                    if (CanvasController.Instance.inventoryPanel.activeSelf)
                    {
                        CanvasController.Instance.ToggleInventory();
                    }
                }

                CanvasController.Instance.dialogueCanvasController.gameObject.SetActive(false);
                CanvasController.Instance.storylineDialogueCanvasController.gameObject.SetActive(false);
                CanvasController.Instance.inventoryPanel.SetActive(false);
            }

            playerVelocity += Physics.gravity * (10f * Time.deltaTime);
            characterController.Move(playerVelocity * Time.deltaTime);

            Vector3 animatorVector = Quaternion.AngleAxis(cameraAngle - playerAngle, Vector3.up) * new Vector3(m_MoveVector.x, 0, m_MoveVector.y);
            const float dampTime = 0.1f;
            animator.SetFloat(AnimatorVariables.WalkX, animatorVector.x, dampTime, Time.deltaTime);
            animator.SetFloat(AnimatorVariables.WalkY, animatorVector.z, dampTime, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Shooting();

            // cooldowny skilli
            foreach (WeaponInstance weapon in PlayerController.Instance.equipmentController.equippedWeapons)
            {
                if (weapon != null)
                {
                    for (int index = 0; index < weapon.skillSlots.Length; index++)
                    {
                        SkillSlot skillSlot = weapon.skillSlots[index];
                        if (skillSlot?.cooldown > 0 && skillSlot.skill != null)
                        {
                            skillSlot.cooldown -= Time.deltaTime;
                            CanvasController.Instance.skillsCanvasController.SetCooldown(index, skillSlot.cooldown / skillSlot.skill.cooldown);
                            if (skillSlot.skill is TemporaryBoostSkill activeSkill && skillSlot.isActive && skillSlot.cooldown < activeSkill.cooldown)
                            {
                                skillSlot.isActive = false;
                                activeSkill.DeactivateSkill(this);
                            }
                        }
                    }
                }
            }
        }

        private void InteractableDetection()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = localCamera.ScreenPointToRay(Mouse.current.position.ReadValue()); // sus lepiej to odczytać z input systemu
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, m_InteractionLayerMask))
                {
                    // EnemyNpc enemy = hit.collider.GetComponentInParent<EnemyNpc>();
                    Interactable interactable = hit.collider.GetComponentInParent<Interactable>();

                    // if (enemy != null)
                    // {
                    //     CursorController.Instance.SetCursor(CursorType.SHOOT);
                    // }
                    if (interactable != null) //&& CanInteract(interactable, hit.collider.gameObject)
                    {
                        if (interactable.GetComponent<FriendlyNpcController>()?.friendlyNpcData?.npcType == NpcType.wanderer)
                        {

                        }
                        else
                        {
                            CursorController.Instance.SetCursor(interactable.cursorType);
                            if (m_CurrentHoveredInterractable != null)
                            {
                                m_CurrentHoveredInterractable.HideOutline();
                            }
                        }

                        m_CanInteract = CanInteract(interactable, hit.collider.gameObject);
                        interactable.ShowOutline(m_CanInteract);
                        m_CurrentHoveredInterractable = interactable;

                        return;
                    }
                }

                DefaultCursor();
            }
            else //if (m_ShootTarget == null) 
            {
                CursorController.Instance.SetCursor(CursorType.Default);
            }


            if (m_CurrentHoveredInterractable != null)
            {
                m_CanInteract = false;
                m_CurrentHoveredInterractable.HideOutline();
                m_CurrentHoveredInterractable = null;
            }
        }

        public void DefaultCursor()
        {
            if (PlayerController.Instance.equipmentController.currentlyHeldWeapon != null && PlayerController.Instance.playerControllerGlobal.currentLocation is not VillageData)
            {
                CursorController.Instance.SetCursor(CursorType.Shoot);
            }
            else
            {
                CursorController.Instance.SetCursor(CursorType.Default);
            }
        }

        private void Shooting()
        {
            if (m_Reloading) return;

            if (m_ShootCooldown > 0)
            {
                m_ShootCooldown -= Time.deltaTime;
            }
            else
            {
                WeaponInstance weapon = PlayerController.Instance.equipmentController.currentlyHeldWeapon;
                if (weapon != null && weapon.currentAmmo > 0 && m_ShootTarget != null)
                {
                    if (IsEnemySeen(m_ShootTarget))
                    {
                        float attackSpeed = weapon.GetWeapon().attackSpeed;
                        while (m_ShootCooldown <= 0)
                        {
                            m_ShootCooldown += (attackSpeed / (fireRateBoost + 1));

                            float weaponAccuracy = weapon.accuracy * (1 - m_AccuracyDecrease);
                            float distance = Vector3.Distance(transform.position, m_ShootTarget.transform.position);
                            float randomNumber = Random.value;
                            if (distance <= weapon.GetWeapon().effectiveRange)
                            {
                                WeaponShoot(randomNumber, weapon, weaponAccuracy, 0);
                            }
                            else
                            {
                                WeaponShoot(randomNumber, weapon, weaponAccuracy, 0.1f);
                            }

                            weapon.currentAmmo--;
                            canvasAmmoController.DecreaseAmmoCanvas(weapon);
                            if (PlayerController.Instance.equipmentController.weaponObject != null)
                            {
                                PlayerController.Instance.equipmentController.weaponObject.PlayEffect();
                            }

                            if (weapon.currentAmmo <= 0)
                            {
                                Debug.Log("Auto reload");
                                StartInteractionCoroutine(ReloadCoroutine(weapon), false, weapon.GetWeapon().reloadSpeed, "Reloading");
                            }
                        }
                    }
                }
            }
        }

        private void WeaponShoot(float randomNumber, WeaponInstance weapon, float weaponAccuracy, float accuracyDebuff)
        {
            if (weapon.GetWeapon().ammunitionType == AmmunitionType.SHOTGUN)
            {
                for (int i = 0; i < 4; i++)
                {
                    randomNumber = Random.value;
                    if (randomNumber <= weaponAccuracy - accuracyDebuff)
                    {
                        m_ShootTarget.healthController.ApplyDamage(weapon.damage);
                    }
                    else
                    {
                        m_ShootTarget.DisplayTextStatus("Missed", Color.yellow, 1);
                    }
                }
            }
            else
            {
                if (randomNumber <= weaponAccuracy - accuracyDebuff)
                {
                    m_ShootTarget.healthController.ApplyDamage(weapon.damage);
                }
                else
                {
                    m_ShootTarget.DisplayTextStatus("Missed", Color.yellow, 1);
                }
            }
        }

        private IEnumerator ReloadCoroutine(WeaponInstance weapon)
        {
            //smartCoroutine.Stop();
            //CancelInteraction();
            m_Reloading = true;
            float reloadTime = weapon.GetWeapon().reloadSpeed;
            // CanvasController.Instance.loadingInterface.StartCircle(reloadTime, "Reloadnig");
            yield return new WaitForSeconds(reloadTime);
            PlayerController.Instance.equipmentController.LoadWeapon();
            canvasAmmoController.ReloadAmmoCanvas(PlayerController.Instance.equipmentController.currentlyHeldWeapon);
            m_Reloading = false;
        }

        public void ApplyBodyDebuffsEffects()
        {
            m_MoveSpeedDecrease = 0;
            m_AccuracyDecrease = 0;
            foreach (PassiveSkill debuff in PlayerController.Instance.healthController.debuffs)
            {
                m_MoveSpeedDecrease += debuff.movement;
                m_AccuracyDecrease += debuff.accuracy;
            }
        }

        #region interactions

        private bool CanInteract(Interactable interactable, GameObject hitObject)
        {
            if (Vector3.Distance(interactable.transform.position, transform.position) <= interactable.interactionDistance)
            {
                if (m_MoveVector == Vector2.zero)
                {
                    return IsObjectSeen(interactable, hitObject);
                }
            }

            return false;
        }

        private IEnumerator InteractionCoroutine(Interactable interactable)
        {
            // CanvasController.Instance.loadingInterface.StartCircle(interactable.interactionTime, interactable.interactionText);
            yield return new WaitForSeconds(interactable.interactionTime);
            //interactable.OnInteract.Invoke();
            interactable.Interact();
            // print("Interaction successful");
        }

        public void StartInteractionCoroutine(IEnumerator coroutine, bool interruptedByWalking, float time, string text)
        {
            interactionCoroutine.Start(coroutine);
            m_InteractionInterruptedByWalking = interruptedByWalking;
            CanvasController.Instance.loadingInterface.StartCircle(time, text);
        }

        public void StopInteractionCoroutine()
        {
            interactionCoroutine.Stop();
            CanvasController.Instance.loadingInterface.HideCircle();
        }

        #endregion

        #region input

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                //Disallow player to move if he is overweight
                if (PlayerController.Instance.equipmentController.currentWeight > PlayerController.Instance.equipmentController.maxWeight)
                {
                    return;
                }

                m_MoveVector = context.ReadValue<Vector2>();
            }
        }

        public void OnRotateCamera(InputAction.CallbackContext context)
        {
            if (m_LocalPlayerCameraInputProvider == null) return;

            if (context.performed)
            {
                Vector2 input = context.ReadValue<Vector2>();
                m_LocalPlayerCameraInputProvider.input = input;
            }
            else if (context.canceled)
            {
                m_LocalPlayerCameraInputProvider.input = Vector2.zero;
                ;
            }
            else if (context.started)
            {
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                WeaponInstance currentlyHeldWeapon = PlayerController.Instance.equipmentController.currentlyHeldWeapon;
                if (currentlyHeldWeapon != null)
                {
                    if (currentlyHeldWeapon.currentAmmo == currentlyHeldWeapon.GetWeapon().magazineCapacity) return;

                    StartInteractionCoroutine(ReloadCoroutine(currentlyHeldWeapon), false, currentlyHeldWeapon.GetWeapon().reloadSpeed, "Reloading");
                    //StartReload(playerEquipmentController.currentlyHeldWeapon);
                }
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (PlayerController.Instance.equipmentController.currentlyHeldWeapon == null) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (context.performed)
            {
                Vector2 screenPoint = Mouse.current.position.ReadValue();
                m_ShootTarget = GetClosestEnemyTarget(screenPoint);
                if (m_ShootTarget == null) return;
                m_ShootTarget.Highlight(true);
                print($"shoot target: {m_ShootTarget.gameObject.name}");
            }
            else if (context.canceled && m_ShootTarget != null && !m_ShootTarget.healthController.Alive)
            {
                m_ShootTarget.Highlight(false);
                m_ShootTarget = null;
            }
        }

        public void OnSelectPrimaryWeapon(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // animator.SetBool(AnimatorVariables.WeaponMode, true);
                interactionCoroutine.Stop();
                CanvasController.Instance.loadingInterface.HideCircle();
                PlayerController.Instance.equipmentController.SetWeaponInHand(0);
            }
        }

        public void OnSelectSecondaryWeapon(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // animator.SetBool(AnimatorVariables.WeaponMode, true);
                interactionCoroutine.Stop();
                CanvasController.Instance.loadingInterface.HideCircle();
                PlayerController.Instance.equipmentController.SetWeaponInHand(1);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Interactable interactable = m_CurrentHoveredInterractable;
                if (interactable != null && m_CanInteract)
                {
                    if (interactable.interactionTime > 0)
                    {
                        StartInteractionCoroutine(InteractionCoroutine(interactable), true, interactable.interactionTime, interactable.interactionText);
                    }
                    else
                    {
                        interactable.Interact();
                    }
                }
            }
        }

        public void OnGigaLag(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Application.targetFrameRate = 1;
            }
            else if (context.canceled)
            {
                Application.targetFrameRate = 120;
            }
        }

        // private void OnShowInventory()
        // {
        //     CanvasController.Instance.ToggleInventory();
        //     CanvasController.Instance.inventoryInterface.CreatePlayerInventoryPanel();
        // }

        // private void OnShowHealthInterface()
        // {
        //     CanvasController.Instance.ToggleHealthPanel();
        // }

        // private void OnShowQuestInterface()
        // {
        //     CanvasController.Instance.ToggleQuestPanel();
        // }

        public void OnHideWeapon(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PlayerController.Instance.equipmentController.HideWeapon();
                // animator.SetBool(AnimatorVariables.WeaponMode, false);   
            }
        }

        public void OnPlaceSkill(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (m_AreaSelector != null)
                {
                    Instantiate(m_AreaEffectToSpawn, m_AreaSelector.transform.position, m_AreaSelector.transform.rotation);
                    Destroy(m_AreaSelector);
                }
            }
        }

        public void OnCancelSkill(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (m_AreaSelector != null)
                {
                    Destroy(m_AreaSelector);
                }
            }
        }

        public void OnSkillQ(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UseSkill(0);
            }
        }

        public void OnSkillE(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UseSkill(1);
            }
        }

        #endregion

        private void UseSkill(int skillId)
        {
            SkillSlot skillSlot = PlayerController.Instance.equipmentController.currentlyHeldWeapon?.skillSlots[skillId];
            if (skillSlot != null)
            {
                switch (skillSlot.skill)
                {
                    case TemporaryBoostSkill activeSkill:
                        if (skillSlot.cooldown <= 0)
                        {
                            skillSlot.isActive = true;
                            activeSkill.ActivateSkill(this);
                            skillSlot.cooldown = activeSkill.cooldown + activeSkill.duration;
                        }

                        break;
                    case AreaSkill areaSkill:
                        if (m_AreaSelector == null && skillSlot.cooldown <= 0)
                        {
                            m_AreaEffectToSpawn = areaSkill.effectToSpawn;
                            m_AreaSelector = Instantiate(areaSkill.areaSelector);
                            skillSlot.cooldown = areaSkill.cooldown;
                        }

                        break;
                    case null:
                        break;
                    default:
                        Debug.LogError("Suspicious Skill type");
                        break;
                }
            }
        }

        private EnemyNpc GetClosestEnemyTarget(Vector2 screenPoint)
        {
            HashSet<EnemyNpc> enemies = EnemyNpc.aliveEnemies;
            if (enemies.Count == 0) return null;

            float distance = Mathf.Infinity;
            EnemyNpc closestEnemy = null;

            foreach (EnemyNpc enemy in enemies)
            {
                Vector2 enemyPoint = localCamera.WorldToScreenPoint(enemy.transform.position);
                float temp = Vector2.Distance(enemyPoint, screenPoint);
                if (temp < distance && IsEnemySeen(enemy))
                {
                    distance = temp;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        private bool IsEnemySeen(EnemyNpc enemy)
        {
            Vector3 playerPosition = transform.position;
            Vector3 playerHeadPosition = new(playerPosition.x, playerPosition.y + 1.6f, playerPosition.z);
            Vector3 enemyPosition = enemy.transform.position;
            Vector3 enemyHeadPosition = new(enemyPosition.x, enemyPosition.y + 1.6f, enemyPosition.z);
            bool raycast = Physics.Raycast(playerHeadPosition, (enemyHeadPosition - playerHeadPosition), out RaycastHit hit, 100f, sightLayerMask);

            return raycast && hit.collider.gameObject == enemy.gameObject && IsInCameraBoundaries(enemy.gameObject);
        }

        private bool IsObjectSeen(Interactable interactable, GameObject hitObject)
        {
            Vector3 playerPosition = transform.position;
            Vector3 playerHeadPosition = new(playerPosition.x, playerPosition.y + 1.6f, playerPosition.z);
            Vector3 objPosition = hitObject.transform.position;
            bool raycast = Physics.Raycast(playerHeadPosition, (objPosition + interactable.interactableOffset - playerHeadPosition), out RaycastHit hit, 500f, sightLayerMask);

            return raycast && /*(hit.collider.gameObject == hitObject)*/ hit.collider.gameObject.layer == hitObject.layer; // && IsInCameraBoundaries(obj.gameObject);
        }

        private bool IsInCameraBoundaries(GameObject otherGameObject)
        {
            Vector3 viewPos = localCamera.WorldToViewportPoint(otherGameObject.transform.position);
            if (viewPos.x >= -0.1f && viewPos.x <= 1.1f && viewPos.y >= -0.1f && viewPos.y <= 1.1f && viewPos.z > 0)
            {
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            InputManager.input.PlayerLocal.Enable();
        }

        private void OnDisable()
        {
            InputManager.input.PlayerLocal.Disable();
        }

        private void OnDestroy()
        {
            InputManager.input.PlayerLocal.RemoveCallbacks(this);
        }

        private void OnValidate()
        {
            if (!characterController) characterController = GetComponent<CharacterController>();
            if (!animator) animator = GetComponentInChildren<Animator>();
        }
    }
}