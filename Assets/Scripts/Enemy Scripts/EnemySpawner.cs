using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public float minSpeed;
        public float maxSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;

        public Color skinColor;
    }

    public event System.Action<int> OnNewWave;

    public Wave[] waves;
    public Enemy enemy;

    private LivingEntity playerEntity;
    private GameObject playerT;

    private Wave _currentWave;
    private int _currentWaveNumber;
    private int _enemiesRemainingToSpawn;
    private int _enemiesRemainingAlive;
    private float _nextSpawnTime;

    public Transform[] spawnPoints;
    public GameObject spawnIndicator;

    public float timeBetweenCampingChecks = 2f;
    public float campTresholdDistance = 1.5f;
    private float nextCampChecktime;
    private Vector3 campPositionOld;
    private bool isCamping;

    private bool isDisabled;

    public AudioClip levelCompleted;

    private void Start()
    {
        playerEntity = GameObject.FindWithTag(TagManager.PLAYER_TAG).GetComponent<Player>();
        playerT = playerEntity.gameObject;

        nextCampChecktime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.transform.position;
        playerEntity.OnDeath += OnPlayerDeath;

        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampChecktime)
            {
                nextCampChecktime = Time.time + timeBetweenCampingChecks;

                isCamping = Vector3.Distance(playerT.transform.position, campPositionOld)
                    < campTresholdDistance;

                campPositionOld = playerT.transform.position;
            }

            if ((_enemiesRemainingToSpawn > 0 || _currentWave.infinite) && Time.time > _nextSpawnTime)
            {
                _nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

                _enemiesRemainingToSpawn--;

                StartCoroutine("SpawnEnemy");
            }

        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;
        float spawnTimer = 0;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (isCamping)
            spawnPoint = playerT.transform;

        Vector3 temp = spawnPoint.position;
        temp.y = spawnIndicator.transform.position.y;
        spawnIndicator.transform.position = temp;

        while (spawnTimer < spawnDelay)
        {
            spawnIndicator.SetActive(!spawnIndicator.activeInHierarchy);

            spawnTimer += Time.deltaTime;

            yield return null;
        }

        spawnIndicator.SetActive(false);

        Enemy spawnedEnemy =
            Instantiate(enemy, spawnPoint.position + Vector3.up, Quaternion.identity) as Enemy;

        spawnedEnemy.OnDeath += OnEnemyDeath;

        spawnedEnemy.SetCharacteristics(_currentWave.moveSpeed,
            _currentWave.hitsToKillPlayer, _currentWave.enemyHealth, _currentWave.skinColor);
    }

    void OnPlayerDeath()
    {
        StopCoroutine("SpawnEnemy");
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        _enemiesRemainingAlive--;

        if (_enemiesRemainingAlive == 0)
            NextWave();
    }

    void NextWave()
    {
        if (_currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound(levelCompleted, transform.position);
        }

        _currentWaveNumber++;

        if (_currentWaveNumber - 1 < waves.Length)
        {
            _currentWave = waves[_currentWaveNumber - 1];

            _enemiesRemainingToSpawn = _currentWave.enemyCount;
            _enemiesRemainingAlive = _enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(_currentWaveNumber);
            }
        }
    }

} // class
