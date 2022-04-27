using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemiesSpawnZone : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawn
    {
        public Transform transform;
        public GameObject prefab;
    }

    [SerializeField] private EnemySpawn[] enemiesToSpawn; 

    /*
    void Start()
    {
        if(enemiesToSpawn)
            enemiesToSpawn.SetActive(false);
    }
    */

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tag.player))
        {
            foreach (EnemySpawn enemy in enemiesToSpawn)
            {
                GameObject go = Instantiate(enemy.prefab, transform);
                go.transform.position = enemy.transform.position;
            }
            this.enabled = false;
        }

    }
}
