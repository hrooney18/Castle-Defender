using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunmanBehavior : MonoBehaviour
{
    private GameObject enemies;
    private GameManager gm;

    private GameObject target;
    private float shotSpeed;
    private float nextShotTime;
    private float shotDamage;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectWithTag("Enemies");
        gm = GameObject.FindObjectOfType<GameManager>();

        nextShotTime = 0;
        shotDamage = 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.gamePaused && !gm.gameOver)
        {
            int enemyCount = enemies.transform.childCount;
            if (!target || (target == enemies))
            {
                if (enemyCount > 0)
                {
                    target = enemies.transform.GetChild(Random.Range(0, enemyCount)).gameObject;
                }
                if (!target)
                {
                    target = enemies;
                }
            }

            if (Time.time >= nextShotTime && target)
            {
                if (target.transform.position.x > -11.2)
                {
                    target.GetComponent<EnemyController>().DamageEnemy(shotDamage, gm);
                    shotSpeed = Random.Range(0.5f, 1.5f);
                    nextShotTime = Time.time + shotSpeed;
                }
            }
        }
    }
}
