using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameController gameController;

    // Params
    public float spawnRadius = 1.0f;

    void Start()
    {
        gameController = GameObject.FindFirstObjectByType<GameController>();
        gameController.registerEnemySpawner(this);
    }

    /**
     * Spawn un ennemi dans le rayon de spawn
     * @param enemies Le game object des ennemis
     * @return Le game object de l'ennemi spawné
     */
    public GameObject spawnEnemy(GameObject enemies)
    {
        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), transform.position.y, Random.Range(-spawnRadius, spawnRadius));
        GameObject newEnemy = Instantiate(gameController.enemyPrefab, randomPosition, transform.rotation);
        newEnemy.transform.parent = enemies.transform;
        return newEnemy;
    }

    /** 
     * ==========================================
     * Debug
     * ==========================================
    */

    /**
     * Dessine les gizmos dans l'éditeur
     */
    private void OnDrawGizmos()
    {
        DrawSpawnRadius();
    }

    /**
     * Dessine un cercle pour voir le rayon de spawn (uniquement dans l'éditeur)
     */
    private void DrawSpawnRadius()
    {
        Gizmos.color = Color.blue;
        
        // Dessiner un cercle en utilisant des segments de ligne
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 center = transform.position;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * spawnRadius, 0, Mathf.Sin(angle1) * spawnRadius);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * spawnRadius, 0, Mathf.Sin(angle2) * spawnRadius);
            
            Gizmos.DrawLine(point1, point2);
        }
    }
}
