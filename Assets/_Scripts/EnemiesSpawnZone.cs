using UnityEngine;

public class EnemiesSpawnZone : MonoBehaviour
{
    [SerializeField] private GameObject enemiesToSpawn; //A GameObject which children are the enemies to spawn
    
    void Start()
    {
        if(enemiesToSpawn)
            enemiesToSpawn.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemiesToSpawn)
            enemiesToSpawn.SetActive(true);
    }
}
