using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    public static GameLoader Instance;

    //public static event Action OnLocationSwitchBegin;
    //public static event Action OnLocationSwitchEnd;

    [SerializeField] private GameObject m_LoadingCanvas;
    [SerializeField] private Image m_Loading;

#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset m_PlayerScene;
    [SerializeField] private UnityEditor.SceneAsset m_MenuScene;
    [SerializeField] private UnityEditor.SceneAsset m_VillageScene;
    [SerializeField] private UnityEditor.SceneAsset m_QuestScene;
    [SerializeField] private UnityEditor.SceneAsset m_RandomEventScene;
    [SerializeField] private UnityEditor.SceneAsset m_StartingQuestScene;
    [SerializeField] private UnityEditor.SceneAsset m_WinScene;
    [SerializeField] private UnityEditor.SceneAsset m_LoseScene;
    private void OnValidate()
    {
        if (m_PlayerScene != null) m_PlayerSceneName = m_PlayerScene.name;
        if (m_MenuScene != null) m_MenuSceneName = m_MenuScene.name;
        if (m_VillageScene != null) m_VillageSceneName = m_VillageScene.name;
        if (m_QuestScene != null) m_QuestSceneName = m_QuestScene.name;
        if (m_RandomEventScene != null) m_RandomEventSceneName = m_RandomEventScene.name;
        if (m_StartingQuestScene != null) m_StartingQuestSceneName = m_StartingQuestScene.name;
        if (m_WinScene != null) m_WinSceneName = m_WinScene.name;
        if (m_LoseScene != null) m_LoseSceneName = m_LoseScene.name;
    }
#endif

    [Header("Required Scenes")]
    [SerializeField, HideInInspector] private string m_PlayerSceneName;
    [SerializeField, HideInInspector] private string m_MenuSceneName;
    [SerializeField, HideInInspector] private string m_VillageSceneName;
    [SerializeField, HideInInspector] private string m_QuestSceneName;
    [SerializeField, HideInInspector] private string m_RandomEventSceneName;
    [SerializeField, HideInInspector] private string m_StartingQuestSceneName;
    [SerializeField, HideInInspector] private string m_WinSceneName;
    [SerializeField, HideInInspector] private string m_LoseSceneName;

    private readonly List<AsyncOperation> m_AsyncOperations = new();


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ItemIndex.Initialize();

        // SaveSystem.Load("save");

        //QualitySettings.SetQualityLevel(0, true);
        Instance = this;
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMainMenuCoroutine());
    }

    private IEnumerator LoadMainMenuCoroutine()
    {
        m_AsyncOperations.Clear();
        m_Loading.fillAmount = 0;
        m_LoadingCanvas.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(m_MenuSceneName, LoadSceneMode.Single);
        asyncOperation.completed += _ => Debug.Log("loaded main menu");

        while (!asyncOperation.isDone)
        {
            m_Loading.fillAmount = asyncOperation.progress;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);
        m_LoadingCanvas.SetActive(false);
    }

    public void LoadRandomEvent()
    {
        StartCoroutine(LoadPlayerScene(m_RandomEventSceneName, true));
    }

    public void UnloadScene()
    {
        StartCoroutine(LoadPlayerScene(null, false));
    }

    public void LoadVillage()
    {
        StartCoroutine(LoadPlayerScene(m_VillageSceneName, true));
    }

    public void LoadQuest(Quest quest)
    {
        StartCoroutine(LoadPlayerScene(m_QuestSceneName, true));
    }

    public void LoadStartingQuest()
    {
        StartCoroutine(LoadPlayerScene(m_StartingQuestSceneName, true));
    }

    private IEnumerator LoadPlayerScene(string sceneName, bool loadingScreen)
    {
        m_AsyncOperations.Clear();
        m_Loading.fillAmount = 0;
        m_LoadingCanvas.SetActive(loadingScreen);

        Scene playerScene = SceneManager.GetSceneByName(m_PlayerSceneName);
        bool playerSceneValid = playerScene.IsValid();
        if (playerSceneValid)
        {
            for (int i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.buildIndex != playerScene.buildIndex)
                {
                    string unloadedSceneName = scene.name;
                    AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene);
                    asyncOperation.completed += _ => Debug.Log($"unloaded scene: {unloadedSceneName}");
                    m_AsyncOperations.Add(asyncOperation);
                }
            }
        }
        else
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(m_PlayerSceneName, LoadSceneMode.Single);
            asyncOperation.completed += _ => Debug.Log("loaded player scene");
            m_AsyncOperations.Add(asyncOperation);
        }

        if (sceneName != null && sceneName != m_PlayerSceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            asyncOperation.completed += _ => SceneManager.SetActiveScene(scene);
            asyncOperation.completed += _ => Debug.Log($"loaded scene: {sceneName}");
            m_AsyncOperations.Add(asyncOperation);
        }

        if (loadingScreen)
        {
            bool pending = true;
            while (pending)
            {
                float progress = 0;
                pending = false;
                foreach (AsyncOperation item in m_AsyncOperations)
                {
                    if (!item.isDone)
                    {
                        pending = true;
                    }

                    progress += item.progress;
                }

                m_Loading.fillAmount = progress / m_AsyncOperations.Count;

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.3f);
            m_LoadingCanvas.SetActive(false);
        }
    }

    public void LoadLoseScene()
    {
        SceneManager.LoadScene(m_LoseSceneName);
    }
    
    public void LoadWinScene()
    {
        SceneManager.LoadScene(m_WinSceneName);
    }
}