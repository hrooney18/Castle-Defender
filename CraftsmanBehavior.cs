using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftsmanBehavior : MonoBehaviour
{
    private float nextRepairTime;
    private float repairSpeed;
    private GameManager gm;
    private int repairAmount;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        nextRepairTime = 0;
        repairAmount = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.gamePaused && !gm.gameOver)
        {
            if (Time.time >= nextRepairTime && gm.castleHealth < gm.maxCastleHealth)
            {
                gm.RepairCastle(repairAmount);

                repairSpeed = Random.Range(0.5f, 1.5f);
                nextRepairTime = Time.time + repairSpeed;
            }
        }
    }
}
