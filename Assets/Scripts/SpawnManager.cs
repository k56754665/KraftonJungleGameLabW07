using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject emergencyKitPrefab;
    public GameObject redZonePrefab;
    public GameObject redZoneBoundsPrefab;
    public GameObject patientPrefab;

    [Header("References")]
    private GameObject _Player;
    [Header("Spawn Settings")]
    public float spawnRadius = 6f;
    [Header("Map Settings")]
    public float mapSizeX = 20f; 
    public float mapSizeY = 10f;

    GameManager gm;
    GameObject player; 

    void Start()
    {
        _Player = Resources.Load<GameObject>("Prefabs/Player");
        gm = GameObject.FindFirstObjectByType<GameManager>();

        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnEmergencyKits());
        StartCoroutine(SpawnDeadZones());
        StartCoroutine(SpawnPatients());

        player = GameObject.FindFirstObjectByType<Player>().gameObject;
    }

    // Enemy Spawning: 20 units away from Player
    IEnumerator SpawnEnemies()
    {
        while (!gm.isgameover)
        {
            yield return new WaitForSeconds(3f);
            if (gm.isgameover) break;
            if (_Player != null)
            {
                Vector3 spawnPos = GetRandomPositionFarFromPlayer(20f);
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
        }
        yield return 0;
    }

    // Emergency Kit Spawning: Within 20 units of Player
    IEnumerator SpawnEmergencyKits()
    {
        while (!gm.isgameover)
        {
            yield return new WaitForSeconds(5f);
            if (_Player != null)
            {
                Vector3 spawnPos = GetRandomPositionNearPlayer(20f);
                Instantiate(emergencyKitPrefab, spawnPos, Quaternion.identity);
            }
        }
        yield return 0;
    }

    // Dead Zone Spawning: Within 20 units of Player
    IEnumerator SpawnDeadZones()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (_Player != null)
            {
                Vector3 spawnPos = GetRandomPositionNearPlayer(15f);
                GameObject redZoneBound = Instantiate(redZoneBoundsPrefab, spawnPos, Quaternion.identity);
                GameObject redZone = Instantiate(redZonePrefab, spawnPos, Quaternion.identity);
                redZone.GetComponent<RedZoneFire>().redZoneBound = redZoneBound;
            }
        }
    }

    // Patient Spawning: After 1.5 seconds, then every 3 seconds
    IEnumerator SpawnPatients()
    {
        yield return new WaitForSeconds(1.5f);
        while (!gm.isgameover)
        {
            yield return new WaitForSeconds(4f);
            Vector3 spawnPos = GetRandomScreenPosition();
            Instantiate(patientPrefab, spawnPos, Quaternion.identity);
        }
        yield return 0;
    }

    // Get a random position at least 'minDistance' away from the player
    Vector3 GetRandomPositionFarFromPlayer(float minDistance)
    {
        Vector3 spawnPos;
        do
        {
            spawnPos = new Vector3(
                Random.Range(-mapSizeX, mapSizeX),
                Random.Range(-mapSizeY, mapSizeY),
                0);
        } while (Vector3.Distance(spawnPos, _Player.transform.position) < minDistance);

        return spawnPos;
    }

    // Get a random position within 'maxDistance' of the player
    Vector3 GetRandomPositionNearPlayer(float maxDistance)
    {
        Vector2 randomCircle = Random.insideUnitCircle * maxDistance;
        if (player)
        {

            float x = player.transform.position.x + randomCircle.x;
            float y = player.transform.position.y + randomCircle.y;
            x = Mathf.Clamp(x, -mapSizeX, mapSizeX);
            y = Mathf.Clamp(y, -mapSizeY, mapSizeY);
            return new Vector3(x, y, 0);
        }
        else 
        {
            return new Vector3(randomCircle.x, randomCircle.y, 0);
        } 
    }

    // Get a random position within the visible screen area
    Vector3 GetRandomScreenPosition()
    {
        return new Vector3(
            Random.Range(-mapSizeX, mapSizeX),
            Random.Range(-mapSizeY, mapSizeY),
            0);
    }
}
