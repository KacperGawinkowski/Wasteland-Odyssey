using System;
using NPC.Friendly;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace World.TownGeneration
{
    public class TownGenerator : MonoBehaviour
    {
        public static TownGenerator Instance;

        [Header("Prefabs")]
        [SerializeField] private GameObject road;
        [SerializeField] private GameObject crossRoad;
        [SerializeField] private FriendlyNpcController m_FriendlyNpcController;
        [SerializeField] public HouseConfig[] houseConfigs;

        [Header("Town Options")]
        [SerializeField] private int roadPoints = 10;
        [SerializeField] public int roadSize = 12;
        [SerializeField] private int roadTilesBetweenCrossroads = 2;
        [SerializeField] private float roadJoinChance = 0.1f;
        [SerializeField] private int additionalTerrainArea;
        private int roadPointsPool;

        [Header("Object containers")]
        [SerializeField] private GameObject roadsContainer;
        [SerializeField] private GameObject buildingsContainer;
        [FormerlySerializedAs("boundaryMeshFilter")][SerializeField] private MeshFilter m_BoundaryMeshFilter;
        [SerializeField] private MeshCollider m_BoundaryMeshCollider;

        [SerializeField] private GameObject m_ImportantNPCsContainer;
        [SerializeField] private GameObject m_FillerNPCsContainer;

        [SerializeField] private Vector3 m_PlayerSpawnPoint = new Vector3(0, 0.25f, 0);

        // calculated map size
        public int Left;
        public int Bottom;
        public int Right;
        public int Top;


        // sus zmienne pomocnicze do generowania
        public Dictionary<HouseData, HouseController> housesDict = new();


        // nav mesh :alien:
        [SerializeField] private NavMeshSurface m_NavMeshSurface;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            VillageData village = null;

            if (PlayerController.Instance != null && PlayerController.Instance.playerControllerGlobal.currentLocation is VillageData vil)
            {
                village = vil;
            }

            if (village == null)
            {
                Debug.LogWarning("there is no village here (quite sus)");
                return;
            }
            
            if (village.layout == null) // generacja nowej wioski
            {
                print("Generating new village");
                GenerateTown(out HashSet<int2> roadsAndCrossroadsLocations, out Dictionary<int2, HouseData> houses);

                // HouseData[] houseDataArray = houses.ToArray();

                // giga chad generacja npc (jaki się tutaj syf zrobił to jest tragiedia xd)
                List<FriendlyNpcData> importantList = new();
                List<FriendlyNpcData> fillerList = new();
                foreach (var house in houses)
                {
                    if (SaveSystem.saveContent.storylineNpcSpawnChance < 0.9)
                    {
                        houseConfigs[house.Value.houseId].houseControllers[house.Value.houseVariant].GenerateNPCs(village, house.Value, importantList, fillerList, SaveSystem.saveContent.storylineNpcSpawnChance/10f);
                    }
                    else
                    {
                        houseConfigs[house.Value.houseId].houseControllers[house.Value.houseVariant].GenerateNPCs(village, house.Value, importantList, fillerList, 1f);
                    }
                }

                village.importantNpc = importantList.ToArray();
                village.fillerNpc = fillerList.ToArray();

                if (village.importantNpc.Length > 0)
                {
                    FriendlyNpcData npcToBecomeQuestGiver = village.importantNpc[Random.Range(0, village.importantNpc.Length)];
                    if (npcToBecomeQuestGiver.npcType != NpcType.storylineNpc)
                    {
                        npcToBecomeQuestGiver.questGiverData = new NPC.QuestGiverData
                        {
                            questsToGive = new()
                        };
                    }
                    
                    if (Random.value > 0.5f)
                    {
                        npcToBecomeQuestGiver = village.importantNpc[Random.Range(0, village.importantNpc.Length)];
                        if (npcToBecomeQuestGiver.npcType != NpcType.storylineNpc)
                        {
                            npcToBecomeQuestGiver.questGiverData = new NPC.QuestGiverData
                            {
                                questsToGive = new()
                            };
                        }
                    }
                }
                

                village.layout = new VillageLayout
                {
                    houses = houses,
                    roads = roadsAndCrossroadsLocations.Select(e => new int2(e.x, e.y)).ToArray(),
                    top = Top,
                    right = Right,
                    left = Left,
                    bottom = Bottom
                };

                if (SaveSystem.saveContent.storylineNpcSpawned == false) SaveSystem.saveContent.storylineNpcSpawnChance += 0.2f;
            }
            else
            {
                Top = village.layout.top;
                Right = village.layout.right;
                Left = village.layout.left;
                Bottom = village.layout.bottom;
            }

            CreateBoundaryMesh();

            // amągus wczytywanie zapisanej gry

            // spawn drog
            foreach (var item in village.layout.roads)
            {
                Vector3 vector = new(item.x, 0, item.y);
                Instantiate(road, vector * roadSize, Quaternion.identity, roadsContainer.transform);
            }

            // spawn budynkow
            foreach (var house in village.layout.houses)
            {
                HouseController houseController = houseConfigs[house.Value.houseId].houseControllers[house.Value.houseVariant];
                if (houseController != null)
                {
                    HouseController hc = Instantiate(houseController, new Vector3(house.Key.x, 0, house.Key.y) * roadSize, Quaternion.Euler(0, house.Value.rotation, 0), buildingsContainer.transform);
                    housesDict.Add(house.Value, hc);
                    
                    // house random interior
                    try
                    {
                        GameObject houseInterior = houseConfigs[house.Value.houseId].interiorVariants[house.Value.interiorVariant];
                        Instantiate(houseInterior, hc.transform);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                else
                {
                    Debug.LogError($"houseConfigIndex error");
                }
            }

            m_NavMeshSurface.BuildNavMesh();

            // spawn npc
            foreach (FriendlyNpcData npcData in village.importantNpc)
            {
                try
                {
                    Transform spawn = housesDict[npcData.houseData].npcSpawnPoints[npcData.spawnPointId].transform;
                    FriendlyNpcController v = Instantiate(m_FriendlyNpcController, spawn.position, spawn.rotation, m_ImportantNPCsContainer.transform);
                    v.SetData(npcData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error: {housesDict[npcData.houseData]}");
                    Debug.LogException(e);
                }
            }

            foreach (FriendlyNpcData npcData in village.fillerNpc)
            {
                try
                {
                    Transform spawn = housesDict[npcData.houseData].npcSpawnPoints[npcData.spawnPointId].transform;
                    var v = Instantiate(m_FriendlyNpcController, spawn.position, spawn.rotation, m_FillerNPCsContainer.transform);
                    v.SetData(npcData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error: {housesDict[npcData.houseData]}");
                    Debug.LogError($"Error: {housesDict[npcData.houseData].npcSpawnPoints != null}");
                    Debug.LogException(e);
                }
            }

            // PlayerController.Instance.playerControllerLocal.transform.position = m_PlayerSpawnPoint;
            PlayerController.Instance.PrepareForFriendlyLocation(m_PlayerSpawnPoint);
        }

        private void GenerateTown(out HashSet<int2> roadsAndCrossroadsLocationsOut, out Dictionary<int2, HouseData> housesOut)
        {
            Stack<(int2, int)> nodeQueue = new();
            nodeQueue.Push((int2.zero, roadPoints));

            HashSet<Road> roads = new();
            HashSet<int2> usedRoadNodes = new();
            HashSet<int2> possibleRoadContinuations = new();
            // generowanie dróg w mieście (na tym etepie generowanie polega na dodawaniu linii z punktu a do b)
            do
            {
                roadPointsPool = 0;
                // główna pętla do rozbudowy drogi
                while (nodeQueue.Count > 0)
                {
                    (int2, int) currentNode = nodeQueue.Pop();

                    if (currentNode.Item2 > 0)
                    {
                        (int, int, int, int) points = DistributePoints(currentNode.Item2);
                        AddNewNode(points.Item1, new int2(0, 1), currentNode.Item1, roads, usedRoadNodes, nodeQueue);
                        AddNewNode(points.Item2, new int2(1, 0), currentNode.Item1, roads, usedRoadNodes, nodeQueue);
                        AddNewNode(points.Item3, new int2(0, -1), currentNode.Item1, roads, usedRoadNodes, nodeQueue);
                        AddNewNode(points.Item4, new int2(-1, 0), currentNode.Item1, roads, usedRoadNodes, nodeQueue);
                    }
                    else
                    {
                        possibleRoadContinuations.Add(currentNode.Item1);
                    }
                }

                // system generowania się zablokował (sus), ale ma jescze trochę dróg do stworzenia
                // więc rozpoczyna generowanie nowe generowanie z punktami startowymi z listy possibleContinuations
                int pointsToGive = roadPointsPool;
                int i = possibleRoadContinuations.Count;
                foreach (int2 possibleContinuation in possibleRoadContinuations)
                {
                    int points = pointsToGive / i;
                    pointsToGive -= points;
                    i--;
                    nodeQueue.Push((possibleContinuation, points));
                }
                possibleRoadContinuations.Clear();
            } while (roadPointsPool > 0);


            HashSet<int2> roadsAndCrossroadsLocations = new(); // potrzebne do spawnowania domków
            HashSet<int2> occupiedSpace = new(); // wszystkie zajęte kafelki
            foreach (Road road in roads)
            {
                roadsAndCrossroadsLocations.Add(road.point1);
                roadsAndCrossroadsLocations.Add(road.point2);
                occupiedSpace.Add(road.point1);
                occupiedSpace.Add(road.point2);

                for (int i = 1; i <= roadTilesBetweenCrossroads; i++)
                {
                    int x = road.point2.x + (int)((float)(road.point1.x - road.point2.x) / (roadTilesBetweenCrossroads + 1) * i);
                    int y = road.point2.y + (int)((float)(road.point1.y - road.point2.y) / (roadTilesBetweenCrossroads + 1) * i);

                    int2 roadVector = new(x, y);
                    //roadsLocations.Add(roadVector);
                    roadsAndCrossroadsLocations.Add(roadVector);
                    occupiedSpace.Add(roadVector);
                }
            }


            // generowanie domków
            Dictionary<int2, HouseData> houses = new(roadsAndCrossroadsLocations.Count * 4);
            foreach (int2 item in roadsAndCrossroadsLocations)
            {
                houses.TryAdd(item + new int2(0, +1), new HouseData {rotation = 0});
                houses.TryAdd(item + new int2(0, -1),new HouseData{rotation = 180});
                houses.TryAdd(item + new int2(-1, 0),new HouseData{rotation = 270});
                houses.TryAdd(item + new int2(+1, 0), new HouseData{rotation = 90});
            }

            // houses.RemoveWhere(x => roadsAndCrossroadsLocations.Contains(x.position));
            // houses = houses.Where(x => !roadsAndCrossroadsLocations.Contains(x.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (int2 position in roadsAndCrossroadsLocations)
            {
                houses.Remove(position);
            }

            List<int2> housesToRemove = new();

            foreach (KeyValuePair<int2, HouseData> house in houses)
            {
                int houseConfigIndex = Random.Range(0, houseConfigs.Length);
                HouseConfig houseConfig = houseConfigs[houseConfigIndex];
                List<int2> houseOccupiedArea = new();
                houseOccupiedArea.Add(house.Key);

                if (houseConfig.Left)
                {
                    Vector2 v2 = Quaternion.Euler(0, 0, -house.Value.rotation) * Vector2.left;
                    int2 v2i = house.Key + new int2(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
                    houseOccupiedArea.Add(v2i);
                }
                if (houseConfig.Right)
                {
                    Vector2 v2 = Quaternion.Euler(0, 0, -house.Value.rotation) * Vector2.right;
                    int2 v2i = house.Key + new int2(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
                    houseOccupiedArea.Add(v2i);
                }
                if (houseConfig.Top)
                {
                    Vector2 v2 = Quaternion.Euler(0, 0, -house.Value.rotation) * Vector2.up;
                    int2 v2i = house.Key + new int2(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
                    houseOccupiedArea.Add(v2i);
                }
                if (houseConfig.TopLeft)
                {
                    Vector2 v2 = Quaternion.Euler(0, 0, -house.Value.rotation) * new Vector2(-1, 1);
                    int2 v2i = house.Key + new int2(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
                    houseOccupiedArea.Add(v2i);
                }
                if (houseConfig.TopRight)
                {
                    Vector2 v2 = Quaternion.Euler(0, 0, -house.Value.rotation) * new Vector2(1, 1);
                    int2 v2i = house.Key + new int2(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
                    houseOccupiedArea.Add(v2i);
                }

                if (!houseOccupiedArea.Any(x => occupiedSpace.Contains(x)))
                {
                    // dodanie domku
                    house.Value.houseId = houseConfigIndex;
                    house.Value.houseVariant = Random.Range(0, houseConfig.houseControllers.Length);
                    house.Value.interiorVariant = Random.Range(0, houseConfig.interiorVariants.Length);
                    occupiedSpace.UnionWith(houseOccupiedArea);
                }
                else
                {
                    // odrzucenie domku
                    housesToRemove.Add(house.Key);
                }
            }

            foreach (int2 position in housesToRemove)
            {
                houses.Remove(position);
            }
            // houses.ExceptWith(housesToRemove);


            //Boundary mesh generation
            Left = occupiedSpace.Min(vec => vec.x) - additionalTerrainArea;
            Bottom = occupiedSpace.Min(vec => vec.y) - additionalTerrainArea;
            Right = occupiedSpace.Max(vec => vec.x) + additionalTerrainArea;
            Top = occupiedSpace.Max(vec => vec.y) + additionalTerrainArea;

            roadsAndCrossroadsLocationsOut = roadsAndCrossroadsLocations;
            housesOut = houses;
        }



        private void AddNewNode(int points, int2 nextNodeOffset, int2 currentNode, HashSet<Road> roads, HashSet<int2> usedRoadNodes, Stack<(int2, int)> nodeQueue)
        {
            if (points > 0)
            {
                int2 nextNodeLocation = currentNode + nextNodeOffset;
                Road newRoad = new(currentNode * (roadTilesBetweenCrossroads + 1), nextNodeLocation * (roadTilesBetweenCrossroads + 1));

                if (!roads.Contains(newRoad) && (!usedRoadNodes.Contains(nextNodeLocation) || Random.value <= roadJoinChance))
                {
                    usedRoadNodes.Add(nextNodeLocation);

                    roads.Add(newRoad);
                    nodeQueue.Push((nextNodeLocation, points - 1));
                }
                else
                {
                    roadPointsPool += points;
                }
            }
        }

        // up, right, down, left
        private (int, int, int, int) DistributePoints(int points)
        {
            int up = 0;
            int right = 0;
            int down = 0;
            int left = 0;
            for (int i = 0; i < points; i++)
            {
                int dir = Random.Range(0, 4);
                switch (dir)
                {
                    case 0:
                        up++;
                        break;
                    case 1:
                        right++;
                        break;
                    case 2:
                        down++;
                        break;
                    case 3:
                        left++;
                        break;
                    default:
                        break;
                }
            }
            return (up, right, down, left);
        }

        #region Boundary Mesh Generation

        private void CreateBoundaryMesh()
        {
            Mesh boundaryMesh = new Mesh();
            //Wartości po przemnożniu o wielkosc drogi powinny dać dokładne koordynaty punktów kątowych trójkątów

            float MaxX = 500;
            float MinX = -500;
            float MaxY = 500;
            float MinY = -500;

            float maxx = Right * roadSize + roadSize / 2;
            float minx = Left * roadSize - roadSize / 2;
            float maxy = Top * roadSize + roadSize / 2;
            float miny = Bottom * roadSize - roadSize / 2;

            const float yOffset = 0.05f;

            Vector3[] newVertices =
            {
            new Vector3(MinX, yOffset, MaxY), // outer left top
            new Vector3(MaxX, yOffset, MaxY), // outer right top
            new Vector3(MinX, yOffset, MinY), // outer left bottom
            new Vector3(MaxX, yOffset, MinY), // outer right bottom
            new Vector3(minx, yOffset, maxy), // inner left top
            new Vector3(maxx, yOffset, maxy), // inner right top
            new Vector3(minx, yOffset, miny), // inner left bottom
            new Vector3(maxx, yOffset, miny)  // inner right bottom
        };
            int[] newTriangles =
            {
            0,1,5,
            0,5,4,
            0,4,2,
            3,7,1,
            3,6,7,
            3,2,6,
            1,7,5,
            2,4,6
        };

            boundaryMesh.vertices = newVertices;
            //boundaryMesh.uv = newUV;
            boundaryMesh.triangles = newTriangles;

            m_BoundaryMeshFilter.mesh = boundaryMesh;

            m_BoundaryMeshCollider.sharedMesh = m_BoundaryMeshFilter.sharedMesh;
        }

        public bool IsInsideTown(Transform transform)
        {
            if (transform.position.x < Left * roadSize || transform.position.x > Right * roadSize || transform.position.z < Bottom * roadSize || transform.position.z > Top * roadSize)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}