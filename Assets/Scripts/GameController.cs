using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    GameObject enemies;

    int currentWave = 1;
    int totalEnemiesToSpawn = 0;
    int currentEnemiesSpawned = 0;
    int maxConcurrentEnemies = 0;
    int currentConcurrentEnemies = 0;
    float lastEnemySpawnTime = 0f;

    // Params
    public TextMeshProUGUI waveLabel;
    public TextMeshProUGUI remainingEnemiesLabel;
    public GameObject enemyPrefab;
    public float delayBetweenEnemySpawn = 1f; 

    void Start()
    {
        // Parent pour les ennemis
        enemies = new GameObject("Enemies");
        enemies.transform.position = Vector3.zero;
        enemies.transform.rotation = Quaternion.identity;
        enemies.transform.localScale = Vector3.one;

        // Initialisation de la vague
        totalEnemiesToSpawn = Mathf.RoundToInt(10 * Mathf.Pow(1.2f, currentWave - 1));
        maxConcurrentEnemies = Mathf.RoundToInt(5 * Mathf.Pow(1.15f, currentWave - 1));
    }

    void Update()
    {
        GameObject? spawnedEnemy = handleEnemySpawn();
        refreshCurrentConcurrentEnemies();
        initializeNextWave();

        waveLabel.text = "WAVE " + currentWave;
        
        // Calculer les ennemis restants (vivants + restants à spawn)
        int enemiesRemainingToSpawn = totalEnemiesToSpawn - currentEnemiesSpawned;
        int totalRemainingEnemies = currentConcurrentEnemies + enemiesRemainingToSpawn;
        
        remainingEnemiesLabel.text = "Remaining enemies: " + totalRemainingEnemies + " / " + totalEnemiesToSpawn;
        
        Debug.Log("Current wave: " + currentWave + " - Remaining enemies to spawn: " + enemiesRemainingToSpawn + " - Current concurrent enemies: " + currentConcurrentEnemies + "/" + maxConcurrentEnemies);
    }

    /**
     * Gère le spawn des ennemis
     */
    GameObject? handleEnemySpawn()
    {
        // On choisit un spawner d'ennemis aléatoire
        EnemySpawner enemySpawner = enemySpawners[Random.Range(0, enemySpawners.Count)];

        // On spawn l'ennemi si délai écoulé et si on peut spawner plus d'ennemis
        if (Time.time - lastEnemySpawnTime > delayBetweenEnemySpawn && currentConcurrentEnemies < maxConcurrentEnemies && currentEnemiesSpawned < totalEnemiesToSpawn)
        {
            GameObject spawnedEnemy = enemySpawner.spawnEnemy(enemies);
            if (spawnedEnemy != null)
            {
                currentEnemiesSpawned++;
                lastEnemySpawnTime = Time.time;
                return spawnedEnemy;
            }
        }
        return null;
    }

    /**
     * Rafraîchit le nombre d'ennemis en vie
     */
    void refreshCurrentConcurrentEnemies()
    {
        currentConcurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    /**
     * Initialise la vague suivante
     */
    void initializeNextWave()
    {
        if (totalEnemiesToSpawn - currentEnemiesSpawned == 0 && currentConcurrentEnemies == 0)
        {
            currentWave++;
            currentEnemiesSpawned = 0;
            lastEnemySpawnTime = 0f;

            totalEnemiesToSpawn = Mathf.RoundToInt(10 * Mathf.Pow(1.2f, currentWave - 1));
            maxConcurrentEnemies = Mathf.RoundToInt(5 * Mathf.Pow(1.15f, currentWave - 1));
        }
    }

    /**
     * Enregistre un spawner d'ennemis
     * @param enemySpawner Le spawner d'ennemis à enregistrer
     */
    public void registerEnemySpawner(EnemySpawner enemySpawner)
    {
        enemySpawners.Add(enemySpawner);
    }
}
