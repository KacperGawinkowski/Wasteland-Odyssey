using Pathfinding2D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using static PlayerWsadInput;

public class PlayerControllerGlobal : MonoBehaviour, IPlayerGlobalActions
{
    [SerializeField] private GameObject m_PlayerObject;

    [SerializeField] private Tilemap m_Tilemap;

    [SerializeField] private Camera m_Camera;
    private bool m_CenterCamera;

    public Vector2Int currentMapPosition;
    public Vector2Int targetMapPosition;
    [NonSerialized] public Location currentLocation;

    [SerializeField] private float m_ScrollSensitivity = 0.03f;
    [SerializeField] private float m_CameraSensitivity = 0.25f;
    private Vector2 m_MoveCameraVector;

    [FormerlySerializedAs("m_Agent")] public PathfindingAgent2D agent;

    private void OnEnable()
    {
        // if (currentLocation != null)
        // {
        //     CanvasController.Instance.hubertEffects.SetActive(true);
        // }
        InputManager.input.PlayerGlobal.Enable();
    }

    private void Start()
    {
        InputManager.input.PlayerGlobal.SetCallbacks(this);
    }

    private void OnDisable()
    {
        // if (CanvasController.Instance)
        // {
        //     CanvasController.Instance.hubertEffects.SetActive(false);
        // }
        InputManager.input.PlayerGlobal.Disable();
    }

    private void OnDestroy()
    {
        InputManager.input.PlayerGlobal.RemoveCallbacks(this);
    }

    private void LateUpdate()
    {
        MoveCamera();
    }



    #region Inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!EventSystem.current.IsPointerOverGameObject() || CanvasController.Instance.enterLeaveInterfaceController.gameObject.activeSelf)
            {
                Vector3 clickPoint = m_Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3Int tilemapPoint = m_Tilemap.WorldToCell(clickPoint);

                if (!GlobalMapController.Instance.IsPositionInsideMap(new Vector2Int(tilemapPoint.x, tilemapPoint.y))) return;

                targetMapPosition = new Vector2Int(tilemapPoint.x, tilemapPoint.y);

                agent.FindPath(new Vector2Int(tilemapPoint.x, tilemapPoint.y));
                CanvasController.Instance.enterLeaveInterfaceController.Clear();
            }
        }
    }

    public void SetMapPosition(Vector2Int position)
    {
        currentMapPosition = position;
        currentLocation = SaveSystem.saveContent?.globalMap?.GetLocation(currentMapPosition);

        if (currentLocation != null)
        {
            CanvasController.Instance.enterLeaveInterfaceController.ShowEnterLocationInterface(currentLocation);
        }
        else
        {
            CanvasController.Instance.enterLeaveInterfaceController.Clear();
        }
    }

    private void MoveCamera()
    {
        if (m_CenterCamera)
        {
            Vector3 finalPosition = new Vector3(m_PlayerObject.transform.position.x, m_PlayerObject.transform.position.y, m_Camera.transform.position.z);
            finalPosition.x = Mathf.Clamp(finalPosition.x, 0, 100);
            finalPosition.y = Mathf.Clamp(finalPosition.y, 0, 50);
            m_Camera.transform.position = finalPosition;
        }
        else
        {
            Vector3 camPosition = m_Camera.transform.position;
            Vector3 finalPosition;
            if (m_MoveCameraVector != Vector2.zero)
            {
                finalPosition = new Vector3(camPosition.x + m_MoveCameraVector.x * m_CameraSensitivity * Time.deltaTime, camPosition.y + m_MoveCameraVector.y * m_CameraSensitivity * Time.deltaTime, camPosition.z);
                finalPosition.x = Mathf.Clamp(finalPosition.x, 0, 100);
                finalPosition.y = Mathf.Clamp(finalPosition.y, 0, 50);
                m_Camera.transform.position = finalPosition;
            }
        }
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!CanvasController.Instance.inventoryPanel.activeSelf)
            {
                //float value = Mouse.current.scroll.ReadValue().y;
                float value = context.ReadValue<float>();
                float ortSize = m_Camera.orthographicSize - (value * m_ScrollSensitivity);
                ortSize = Mathf.Clamp(ortSize, 5, 30);
                m_Camera.orthographicSize = ortSize;
            }
        }
    }

    public void OnMoveMap(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            m_MoveCameraVector = context.ReadValue<Vector2>();
        }
    }

    public void OnCenterOnPlayer(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            m_CenterCamera = context.ReadValueAsButton();
        }
    }

    public void OnClickOnTile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 clickPoint = m_Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());  // sus lepiej to odczytać z input systemu
                Vector3Int tilemapPoint = m_Tilemap.WorldToCell(clickPoint);

                if (!GlobalMapController.Instance.IsPositionInsideMap(new Vector2Int(tilemapPoint.x, tilemapPoint.y))) return;

                Location clickedLocation = SaveSystem.saveContent.globalMap.GetLocation(tilemapPoint.x, tilemapPoint.y);
                if (clickedLocation != null)
                {
                    //GlobalMapController.Instance.SpawnTextAboveLocation(clickedLocation);
                    if (clickedLocation is Quest quest)
                    {
                        CanvasController.Instance.questCanvasController.gameObject.SetActive(true);
                        foreach (Transform transform in CanvasController.Instance.questCanvasController.questList.transform)
                        {
                            QuestButton questButton = transform.GetComponent<QuestButton>();
                            if (questButton.quest == quest)
                            {
                                if (quest is StorylineQuest)
                                {
                                    CanvasController.Instance.questCanvasController.SetQuestInfo(quest, questButton, "Main Quest");
                                }
                                else
                                {
                                    CanvasController.Instance.questCanvasController.SetQuestInfo(quest, questButton, "Side Quest");
                                }
                            }
                        }
                    }
                    else if (clickedLocation is VillageData village)
                    {
                        print("Village Name: " + village.villageName);
                    }
                }
            }
        }
    }

    #endregion Inputs
}
