using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int attackDamage;
    private float nextAttackTime;

    public Transform castleWall;

    private EnemyState currentState;
    private float distanceToWall;
    private bool touchingWall;
    private Rigidbody2D rb;
    private GameManager gm;
    enum EnemyState
    {
        Walking,
        Attacking,
        Dead
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        gm = GameObject.FindObjectOfType<GameManager>();
        currentState = EnemyState.Walking;
        touchingWall = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            currentState = EnemyState.Dead;

        switch (currentState)
        {
            case EnemyState.Walking:
                EnemyWalk();
                break;
            case EnemyState.Attacking:
                EnemyAttack();
                break;
            case EnemyState.Dead:
                EnemyDie();
                break;
            default:
                break;
        }
    }

    private void EnemyWalk()
    {
        rb.position = Vector3.MoveTowards(rb.position, new Vector3(castleWall.position.x, transform.position.y, 0), moveSpeed * Time.deltaTime);
        distanceToWall = Vector3.Distance(rb.position, new Vector3(castleWall.position.x, transform.position.y, 0));

        if (distanceToWall <= attackDistance || touchingWall)
        {
            currentState = EnemyState.Attacking;
            nextAttackTime = Time.time;
        }
    }

    private void EnemyAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            gm.DamageCastle(attackDamage);
            nextAttackTime = Time.time + attackSpeed;
        }

    }

    private void EnemyDie()
    {
        if (gameObject.CompareTag("BasicEnemy"))
            gm.AddMoney(100);
        else if (gameObject.CompareTag("Rifleman"))
            gm.AddMoney(300);
        else if (gameObject.CompareTag("Rocketman"))
            gm.AddMoney(700);
        else if (gameObject.CompareTag("Tank"))
            gm.AddMoney(1500);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (!gm.gamePaused && !gm.gameOver)
        {
            if (gm.canShoot)
            {
                if (gm.hasSniper)
                    DamageEnemy(2, gm);
                else DamageEnemy(1, gm);
            }
        }
    }

    public void DamageEnemy(float damage, GameManager gm)
    {
        health -= damage;
        if (health <= 0)
            currentState = EnemyState.Dead;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
            Physics2D.IgnoreCollision(collision.collider, GetComponent<BoxCollider2D>());
        if (collision.gameObject.CompareTag("Wall"))
            touchingWall = true;
    }
}
