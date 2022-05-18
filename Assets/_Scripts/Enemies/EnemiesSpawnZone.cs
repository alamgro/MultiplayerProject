using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemiesSpawnZone : NetworkBehaviour
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
                GameObject go = NetworkManager.Instantiate(enemy.prefab, enemy.transform.position, enemy.prefab.transform.rotation);
                //go.transform.position = enemy.transform.position;
                NetworkServer.Spawn(go);
            }
            RCP_DeactivateSpawnZone();
        }
    }

    [ClientRpc]
    private void RCP_DeactivateSpawnZone()
    {
        gameObject.SetActive(false);
    }
}
