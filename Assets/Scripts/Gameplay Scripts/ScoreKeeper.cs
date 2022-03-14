using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }

    private float lastEnemyKilledTime;
    private float streakExpiryTime = 1f;
    private int streakCount;

    private void Awake()
    {
        score = 0;
    }

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {
        if (Time.time > lastEnemyKilledTime + streakExpiryTime)
            streakCount++;
        else
            streakCount = 0;

        lastEnemyKilledTime = Time.time;

        score += 5 + (Random.Range(2, 5) * streakCount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }

} // class
