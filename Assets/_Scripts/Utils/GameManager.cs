using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    #endregion

    private Player playerInstance;

    public Player PlayerInstance => playerInstance;
    public int EnemiesKilled { get => enemiesKilled; set => enemiesKilled = value; }

    private int enemiesKilled = 0;

    void Awake()
    {
        _instance = this;

        playerInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    [RuntimeInitializeOnLoadMethod]
    static void AutoCreate()
    {
        _instance = new GameObject("GameManager").AddComponent<GameManager>();
        DontDestroyOnLoad(_instance.gameObject);
    }

}
