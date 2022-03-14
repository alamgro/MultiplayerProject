using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject UI_StatisticsMenu;
    [SerializeField] private GameObject UI_LeaderBoardMenu;
    [SerializeField] private TextMeshProUGUI UI_Points;
    [SerializeField] private TextMeshProUGUI UI_Kills;
    [SerializeField] private TextMeshProUGUI UI_Deaths;
    [SerializeField] private TextMeshProUGUI UI_LeaderBoard;


    void Start()
    {
        DataSetup();
        
        UI_StatisticsMenu.SetActive(false);
    }


    public void Play()
    {
        SceneManager.LoadScene(GameConstants.Scene.game);
    }

    public void OpenStatisticsMenu()
    {
        UI_StatisticsMenu.SetActive(true);
        UI_Points.SetText($"Score: {GameManager.Instance.TotalPoints}");
        UI_Kills.SetText($"Kills: {GameManager.Instance.TotalKills}");
        UI_Deaths.SetText($"Deaths: {GameManager.Instance.TotalDeaths}");
    }

    public void OpenLeaderBoardMenu()
    {
        UI_LeaderBoardMenu.SetActive(true);
        UI_LeaderBoard.text = string.Empty;

        foreach (var leader in GameManager.Instance.LeaderBoard)
        {
            UI_LeaderBoard.text += ($"{leader.Key}: {leader.Value} points.\n");
        }

    }

    public void Quit()
    {
        Application.Quit();
    }

    private void DataSetup()
    {
        PlayfabManager.Instance.GetStatisticsPoints();
        PlayfabManager.Instance.GetKills();
        PlayfabManager.Instance.GetDeaths();
        PlayfabManager.Instance.FetchLeaderBoard();

    }

    


}
