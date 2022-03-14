using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tag.player))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        print($"Enemies Killed: {GameManager.Instance.EnemiesKilled}");
        PlayfabManager.Instance.UpdateKills(GameManager.Instance.EnemiesKilled);
    }

}
