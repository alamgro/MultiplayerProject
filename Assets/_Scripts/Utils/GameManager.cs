using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class GameManager : NetworkBehaviour
{
    #region SINGLETON
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    #endregion

    #region PRIVATE VARIABLES
    [SerializeField] private GameObject UI_PauseMenu;
    [SerializeField] private GameObject UI_LevelFinishedMenu;

    private List<Player> playerInstances = new List<Player>();
    private int levelKills = 0;
    private int levelPoints = 0;
    private int totalKills = 0;
    private int totalPoints = 0;
    private int totalDeaths = 0;
    private Dictionary<string, int> leaderBoard = new Dictionary<string, int>();
    #endregion

    //public Player PlayerInstance {get => playerInstance; set => playerInstance = value;}
    public List<Player> PlayerInstances {get => playerInstances; set => playerInstances = value;}
    public int LevelKills { get => levelKills; set => levelKills = value; }
    public int LevelPoints { get => levelPoints; set => levelPoints = value; }
    public int TotalKills { get => totalKills; set => totalKills = value; }
    public int TotalPoints { get => totalPoints; set => totalPoints = value; }
    public int TotalDeaths { get => totalDeaths; set => totalDeaths = value; }
    public Dictionary<string, int> LeaderBoard { get => leaderBoard; set => leaderBoard = value; }

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod]
    static void AutoCreate()
    {
        _instance = new GameObject("GameManager").AddComponent<GameManager>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //FindPlayer();
        ResetLevelStatistics();
    }

    public void ResetLevelStatistics()
    {
        levelKills = 0;
        levelPoints = 0;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

}
