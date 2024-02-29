using Pathfinding2D;
using System;
using System.Collections.Generic;
using System.Linq;
using NPC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GlobalMapController : MonoBehaviour
{
    public static GlobalMapController Instance;

    [Header("General settings")]
    [SerializeField] public int m_SizeX;
    [SerializeField] public int m_SizeY;
    [SerializeField] private int m_AmountOfVillages = 5;

    [Header("WFC settings")]
    [FormerlySerializedAs("m_Tilemap")]
    [SerializeField] private Tilemap m_WfcTilemap;
    [SerializeField] private MapGenTile[] m_MapGenTiles;

    [Header("Perlin settings")]
    [SerializeField] private Tilemap m_PerlinTilemap;
    [SerializeField] private Tilemap m_PerlinBackgroundTilemap;
    [SerializeField] private TileBase m_DefaultPerlinTile;
    [SerializeField] private GlobalMapPerlinSettingsEntry[] m_PerlinTiles;
    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;
    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0f;

    [Header("Props")]
    [SerializeField] private Tilemap m_PropsTilemap;
    [SerializeField] private TileBase m_VillageTile;
    [SerializeField] private TileBase m_VillageTileSelected;
    [SerializeField] private TileBase m_QuestTile;
    [SerializeField] private TileBase m_QuestTileSelected;
    
    [SerializeField] private GameObject m_LocationNameText;
    private Dictionary<Location, GameObject> m_LocationNamesMap = new Dictionary<Location, GameObject>();
    
    
    // private GameObject m_PrevLocationNameText;
    private Vector3Int m_SelectedVillageLocation;
    private Vector3Int m_SelectedQuestLocation;

    [Header("Pathfinding")]
    [SerializeField] private PathfindingGrid m_PathfindingGrid;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_PathfindingGrid.CreateGrid(m_SizeX, m_SizeY);

        if (SaveSystem.saveContent.globalMap == null)
        {
            float xOrg = Random.Range(0f, 1000f);
            float yOrg = Random.Range(0f, 1000f);

            SaveSystem.saveContent.perlinY = yOrg;
            SaveSystem.saveContent.perlinX = xOrg;

            SaveSystem.saveContent.globalMap = new GlobalMap();

            GeneratePerlin(xOrg, yOrg);
            GenerateWFC();
            GenerateVillages();
            Debug.Log("Global map created");
        }
        else
        {
            GeneratePerlin(SaveSystem.saveContent.perlinX, SaveSystem.saveContent.perlinY);
            for (int y = 0; y < m_SizeY; y++)
            {
                for (int x = 0; x < m_SizeX; x++)
                {
                    MapGenTile tile = m_MapGenTiles[SaveSystem.saveContent.globalMap.GetTileData(x, y).tileVariantId];
                    if (tile.moveCost > 0)
                    {
                        m_PathfindingGrid.grid[x, y].baseCost = tile.moveCost;
                    }

                    m_WfcTilemap.SetTile(new Vector3Int(x, y), tile.tile);

                    Location location = SaveSystem.saveContent.globalMap.GetLocation(x, y);
                    if (location != null)
                    {
                        ShowLocation(location);
                    }
                }
            }

            Debug.Log("Global map loaded from save");
        }

        m_PathfindingGrid.BuildGrid();
    }

    //private void GeneratePathfindingGrid()
    //{
    //    m_Grid.grid = new PathfindingNode[m_SizeX, m_SizeY];

    //    for (int y = m_SizeY; y < m_SizeY; y++)
    //    {
    //        for (int x = m_SizeX; x < m_SizeX; x++)
    //        {
    //            if (m_Tilemap.GetTile(new Vector3Int(x, y, 0)) == null)
    //            {
    //                PathfindingNode p = new();
    //                p.position = new Vector2Int(x, y);
    //                m_Grid[x - m_MinX, y - m_MinY] = p;
    //            }
    //        }
    //    }
    //}

    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         GenerateWFC();
    //         GeneratePerlin();
    //     }
    // }

    //[ContextMenu("Generate Perlin")]
    private void GeneratePerlin(float xOrg, float yOrg)
    {
        m_PerlinBackgroundTilemap.BoxFill(Vector3Int.zero, m_DefaultPerlinTile, 0, 0, m_SizeX, m_SizeY);
        for (int y = 0; y < m_SizeY; y++)
        {
            for (int x = 0; x < m_SizeX; x++)
            {
                float xCoord = xOrg + x / (float)m_SizeX * scale;
                float yCoord = yOrg + y / (float)m_SizeY * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                //Debug.Log(sample);

                for (int i = m_PerlinTiles.Length - 1; i >= 0; i--)
                {
                    if (sample >= m_PerlinTiles[i].appearFrom)
                    {
                        if (m_PerlinTiles[i].tile != null)
                        {
                            m_PerlinTilemap.SetTile(new Vector3Int(x, y), m_PerlinTiles[i].tile);
                        }

                        m_PathfindingGrid.grid[x, y].baseCost = m_PerlinTiles[i].moveCost;
                        break;
                    }
                }
            }
        }
    }

    [ContextMenu("Generate WFC")]
    private void GenerateWFC()
    {
        m_WfcTilemap.ClearAllTiles();
        GlobalMapGenerator generator = new(m_SizeX, m_SizeY, m_MapGenTiles);
        generator.Generate(m_WfcTilemap);

        int sizeY = generator.m_TileSetArray.GetLength(1);
        int sizeX = generator.m_TileSetArray.GetLength(0);
        // SaveSystem.saveContent.globalMapTiles = new GlobalMapTile[sizeY][];
        for (int y = 0; y < sizeY; y++)
        {
            // SaveSystem.saveContent.globalMapTiles[y] = new GlobalMapTile[sizeX];
            for (int x = 0; x < sizeX; x++)
            {
                GlobalMapTileData g = new();
                MapGenTile tile = generator.m_TileSetArray[x, y].First();
                g.tileVariantId = Array.IndexOf(m_MapGenTiles, tile);
                g.modifier = tile.mapTileModifier;
                SaveSystem.saveContent.globalMap.AddTileData(x, y, g);
                if (tile.moveCost > 0)
                {
                    m_PathfindingGrid.grid[x, y].baseCost = tile.moveCost;
                }
            }
        }
    }

    private void GenerateVillages()
    {
        m_PropsTilemap.ClearAllTiles();

        SaveSystem.saveContent.villageDatas = new VillageData[m_AmountOfVillages];
        for (int i = 0; i < m_AmountOfVillages; i++)
        {
            int x;
            int y;
            do
            {
                x = Random.Range(0, m_SizeX);
                y = Random.Range(0, m_SizeY);
            } while (IsPositionADesert(new Vector2Int(x, y)) == false || IsPositionALocation(new Vector2Int(x, y)) == false);

            VillageData village = new VillageData( /*$"village {i}"*/NameGenerator.GenerateTownName(), null, new Vector2Int(x, y));
            AddLocation(village);
            //SaveSystem.saveContent.globalMap.AddLocation(x, y, village);
            SaveSystem.saveContent.villageDatas[i] = village;
            m_PropsTilemap.SetTile(new Vector3Int(x, y), m_VillageTile);
        }
    }

    public void AddLocation(Location location)
    {
        SaveSystem.saveContent.globalMap.AddLocation(location.positionX, location.positionY, location);
        ShowLocation(location);
    }

    private void ShowLocation(Location location)
    {
        GameObject locationText = null;
        if (!m_LocationNamesMap.ContainsKey(location))
        {
            Debug.Log("Spawning location name");
            locationText = Instantiate(m_LocationNameText, new Vector3(location.positionX + 0.5f, location.positionY + 1f, -0.01f),  Quaternion.identity, gameObject.transform);
        }
        

        switch (location)
        {
            case Quest quest:
                Debug.Log(quest.questLocation);
                Debug.Log(m_PropsTilemap);
                m_PropsTilemap.SetTile(new Vector3Int(quest.positionX, quest.positionY), quest.questLocation.locationTile);
                locationText.GetComponent<TextMeshPro>().text = quest.questLocation.locationName;
                m_LocationNamesMap.Add(quest,locationText); 
                break;
            case VillageData villageData:
                m_PropsTilemap.SetTile(new Vector3Int(location.positionX, location.positionY), m_VillageTile);
                locationText.GetComponent<TextMeshPro>().text = villageData.villageName;
                m_LocationNamesMap.Add(villageData,locationText); 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location));
        }
    }

    // public void AddStorylineLocation(StorylineQuest location)
    // {
    //     SaveSystem.saveContent.globalMap.AddLocation(location.positionX, location.positionY, location);
    //     m_PropsTilemap.SetTile(new Vector3Int(location.positionX, location.positionY), QuestLocation.LastQuestLocation.locationTile);
    // }

    public void RemoveLocation(Location location)
    {
        SaveSystem.saveContent.globalMap.RemoveLocation(location.positionX, location.positionY);
        m_PropsTilemap.SetTile(new Vector3Int(location.positionX, location.positionY), null);
        
        if (m_LocationNamesMap.ContainsKey(location))
        {
            Destroy(m_LocationNamesMap[location].gameObject);
            m_LocationNamesMap.Remove(location); 
        }
    }
    
    
    public void SetLocationAsSelected(Quest questLocation)
    {
        if (questLocation == null)
        {
            return;
        }
        
        foreach (Quest quest in SaveSystem.saveContent.questLog.quests)
        {
            if (quest.questStatus == QuestStatus.InProgress)
            {
                m_PropsTilemap.SetTile(new Vector3Int(quest.positionX, quest.positionY), m_QuestTile);
                m_PropsTilemap.SetTile(new Vector3Int(quest.friendlyNpcData.villageData.positionX, quest.friendlyNpcData.villageData.positionY), m_VillageTile);
            }
            else if (questLocation == null || questLocation.questStatus == QuestStatus.Completed || questLocation.questStatus == QuestStatus.Failed)
            {
                m_PropsTilemap.SetTile(new Vector3Int(quest.positionX, quest.positionY), null);
            }
        }

        if (questLocation == null || questLocation.questStatus == QuestStatus.Failed || questLocation.questStatus == QuestStatus.Completed) return;

        m_SelectedQuestLocation = new Vector3Int(questLocation.positionX, questLocation.positionY);
        m_SelectedVillageLocation = new Vector3Int(questLocation.friendlyNpcData.villageData.positionX, questLocation.friendlyNpcData.villageData.positionY);
        
        m_PropsTilemap.SetTile(m_SelectedQuestLocation, m_QuestTileSelected);
        m_PropsTilemap.SetTile(m_SelectedVillageLocation, m_VillageTileSelected);
    }

    public void DeselectLocations()
    {
        m_PropsTilemap.SetTile(m_SelectedQuestLocation,m_QuestTile);
        m_PropsTilemap.SetTile(m_SelectedVillageLocation, m_VillageTile);
    }
    //
    // public void SetLocationAsSelected(StorylineQuest questLocation)
    // {
    //     if (questLocation == null)
    //     {
    //         return;
    //     }
    //
    //
    //     foreach (Quest quest in SaveSystem.saveContent.questLog.quests)
    //     {
    //         if (quest.questStatus == QuestStatus.InProgress)
    //         {
    //             m_PropsTilemap.SetTile(new Vector3Int(quest.positionX, quest.positionY), quest.questLocation.locationTile);
    //             m_PropsTilemap.SetTile(new Vector3Int(quest.friendlyNpcData.villageData.positionX, quest.friendlyNpcData.villageData.positionY), m_VillageTile);
    //         }
    //         else if (questLocation == null || questLocation.questStatus == QuestStatus.Completed || questLocation.questStatus == QuestStatus.Failed)
    //         {
    //             m_PropsTilemap.SetTile(new Vector3Int(quest.positionX, quest.positionY), null);
    //         }
    //     }
    //
    //     if (questLocation == null || questLocation.questStatus == QuestStatus.Failed || questLocation.questStatus == QuestStatus.Completed || questLocation?.storylineQuestIndex != 2) return;
    //
    //     m_PropsTilemap.SetTile(new Vector3Int(questLocation.positionX, questLocation.positionY), questLocation.questLocation.selectedLocationTile);
    //     m_PropsTilemap.SetTile(new Vector3Int(questLocation.friendlyNpcData.villageData.positionX, questLocation.friendlyNpcData.villageData.positionY), m_VillageTileSelected);
    // }


    // public void SpawnTextAboveLocation(Location location)
    // {
    //     if (m_PrevLocationNameText != null) Destroy(m_PrevLocationNameText.gameObject);
    //
    //     if (location == null) return;
    //
    //     m_PrevLocationNameText = Instantiate(m_LocationNameText, new Vector3(location.positionX + 0.5f, location.positionY + 1f, -0.01f), Quaternion.identity);
    //     if (location is VillageData village) m_PrevLocationNameText.GetComponent<TextMeshPro>().text = village.villageName;
    //     if (location is Quest quest) m_PrevLocationNameText.GetComponent<TextMeshPro>().text = quest.questLocation.locationName;
    // }
    //
    // public void RemoveLocationNameTextXD()
    // {
    //     if (m_PrevLocationNameText != null)
    //     {
    //         Destroy(m_PrevLocationNameText.gameObject);
    //     }
    // }

    public bool IsPositionInsideMap(Vector2Int vector)
    {
        if (vector.x >= m_SizeX || vector.y >= m_SizeY || vector.x < 0 || vector.y < 0) return false;

        return true;
    }

    public bool IsPositionALocation(Vector2Int vector)
    {
        return SaveSystem.saveContent.globalMap.GetLocation(vector) == null;
    }

    public bool IsPositionADesert(Vector2Int vector)
    {
        return SaveSystem.saveContent.globalMap.GetTileData(vector).modifier == GlobalMapTileModifier.None;
    }


    [Serializable]
    private struct GlobalMapPerlinSettingsEntry
    {
        public TileBase tile;
        public float appearFrom;
        public float moveCost;
    }
}