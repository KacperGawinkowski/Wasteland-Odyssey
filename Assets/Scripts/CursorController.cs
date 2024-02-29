using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static CursorController Instance;

    [SerializeField] private Texture2D m_DefaultCursor;
    [SerializeField] private Texture2D m_DialogueCursor;
    [SerializeField] private Texture2D m_ShootCursor;
    [SerializeField] private Texture2D m_LootChestCursor;
    [SerializeField] private Texture2D m_LootBodyCursor;

    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Default:
                Cursor.SetCursor(m_DefaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Dialogue:
                Cursor.SetCursor(m_DialogueCursor, new Vector2(m_DialogueCursor.width / 2f, m_DefaultCursor.height / 2f), CursorMode.Auto);
                break;
            case CursorType.Shoot:
                Cursor.SetCursor(m_ShootCursor, new Vector2(m_ShootCursor.width / 2f, m_ShootCursor.height / 2f), CursorMode.Auto);
                break;
            case CursorType.LootChest:
                Cursor.SetCursor(m_LootChestCursor, new Vector2(m_LootChestCursor.width / 2f, m_LootChestCursor.height / 2f), CursorMode.Auto);
                break;
            case CursorType.LootBody:
                Cursor.SetCursor(m_LootBodyCursor, new Vector2(m_LootBodyCursor.width / 2f, m_LootBodyCursor.height / 2f), CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(m_DefaultCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
}

public enum CursorType
{
    Default,
    Dialogue,
    Shoot,
    LootChest,
    LootBody
}