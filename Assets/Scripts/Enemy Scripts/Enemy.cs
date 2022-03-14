using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking }
    private State _currentState;

    public ParticleSystem deathEffect;
    private ParticleSystem.MainModule deathEffectMain;
    public static event System.Action OnDeathStatic;

    public NavMeshAgent _pathFinder;

    private Transform _target;
    private LivingEntity _targetEntity;
    private Material _skinMaterial;
    private Color _originalColor;

    public float refreshAfter = 0.5f;
    public float attackDistanceTreshold = 0.5f;

    private float _timeBetweenAttacks = 1f;
    private float _nextAttackTime = 0f;
    private float _myCollisionRadius;
    private float _targetCollisionRadius;

    private bool _hasTarget;

    public float damage = 1f;

    public AudioClip[] hurtClips;
    public AudioClip[] deathClips;
    public AudioClip enemyAttackClip;

    private void Awake()
    {
        _target = GameObject.FindWithTag(TagManager.PLAYER_TAG).transform;

        if (_target)
        {
            _hasTarget = true;
            _targetEntity = _target.GetComponent<LivingEntity>();

            _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;
        }

    }

    protected override void Start()
    {
        base.Start();

        if (_hasTarget)
        {
            _currentState = State.Chasing;
            _targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
    }

    private void Update()
    {
        if (_hasTarget)
        {
            if (Time.time > _nextAttackTime)
            {
                float sqDstToTarget = (_target.position - transform.position).sqrMagnitude;

                if (sqDstToTarget < Mathf.Pow(attackDistanceTreshold +
                    _myCollisionRadius + _targetCollisionRadius, 2))
                {
                    _nextAttackTime = Time.time + _timeBetweenAttacks;
                    AudioManager.instance.PlaySound(enemyAttackClip, transform.position);
                    StartCoroutine(Attack());
                }

            }
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer,
        float enemyHealth, Color skinColor)
    {
        _pathFinder.speed = moveSpeed;

        if (_hasTarget)
        {
            damage = Mathf.Ceil(_targetEntity.initialHealth / hitsToKillPlayer);
        }

        initialHealth = enemyHealth;

        deathEffectMain = deathEffect.main;
        deathEffectMain.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1f);

        _skinMaterial = GetComponent<Renderer>().material;
        _skinMaterial.color = skinColor;
        _originalColor = _skinMaterial.color;
    }

    IEnumerator UpdatePath()
    {
        while (_hasTarget)
        {
            if (_currentState == State.Chasing)
            {
                Vector3 dirToTarget = (_target.position - transform.position).normalized;

                Vector3 targetPosition = _target.position - dirToTarget *
                    (_myCollisionRadius + _targetCollisionRadius + attackDistanceTreshold / 2f);

                if (!dead)
                {
                    _pathFinder.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(refreshAfter);
        }
    }

    void OnTargetDeath()
    { 
        _hasTarget = false;
        _currentState = State.Idle;
    }

    IEnumerator Attack()
    {
        _currentState = State.Attacking;

        _pathFinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (_target.position - transform.position).normalized;
        Vector3 attackPosition = _target.position - dirToTarget * _myCollisionRadius;

        float percent = 0;
        float attackSpeed = 3f;

        _skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                _targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4f;

            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        _skinMaterial.color = _originalColor;
        _currentState = State.Chasing;
        _pathFinder.enabled = true;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDir)
    {
        AudioManager.instance.PlaySound(hurtClips[Random.Range(0, hurtClips.Length)],
            transform.position);

        if (damage >= health)
        {
            if (OnDeathStatic != null)
                OnDeathStatic();

            AudioManager.instance.PlaySound(deathClips[Random.Range(0, deathClips.Length)],
            transform.position);

            GameObject deathParticles = Instantiate(deathEffect.gameObject,
                hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDir));

            Destroy(deathParticles, deathEffectMain.startLifetimeMultiplier);
        }

        base.TakeDamage(damage);
    }

} // class
