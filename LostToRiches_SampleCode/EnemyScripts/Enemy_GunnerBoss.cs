using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_GunnerBoss : Enemy_RangedCombat
{
    [Space(10)]

    [Header("Charging Properties")]
    public GameObject trailPrefab;
    GameObject trailInstance;

    EnemyWindUp_UI chargeWindUpUI;

    Vector3 chargeDirection;

    public float meleeDamage;
    public float chargeDamage;
    public float meleeAttackRange;
    public int chargeSpeed;
    public float chargeLength;
    public float chargeCoolDownLength;
    public float windUpLength;
    public float meleeAttackRate;

    float chargeTime;
    float windUpTime;
    float currentCoolDownTime;
    float nextMeleeAttack;

    bool charging;

    [Space(10)]

    public float timeBeforeCombat = 2.5f;   // seconds

    private void Start()
    {
        chargeWindUpUI = GetComponentInChildren<EnemyWindUp_UI>();

        StartCoroutine(StartCombat());
    }

    public override void Update()
    {
        base.Update();

        MeleeAttackAndChargeAI();
    }

    void MeleeAttackAndChargeAI()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= meleeAttackRange)
            {
                if (nextMeleeAttack <= 0)
                {
                    if (charging)
                    {
                        ChargeAttack();
                        Vector3 pushDir = (new Vector3(target.position.x, 0, target.position.z) - new Vector3(gameObject.transform.position.x, 0 , gameObject.transform.position.z)).normalized;
                        targetController.StartPushPlayerBack(pushDir, pushBackAmount, 0.2f);
                        targetController.SetStun(1f);
                        charging = false;
                        charging = false;
                        enemyNavMesh.isStopped = false;
                        Destroy(trailInstance);
                    }
                    else
                    {
                        MeleeAttack();
                    }
                    nextMeleeAttack = meleeAttackRate;
                }

            }
            if (nextMeleeAttack > 0)
            {
                nextMeleeAttack -= Time.deltaTime;
            }
            Charge();
        }
    }

    void Charge()
    {
        if (!charging && windUpTime > windUpLength)
        {
            charging = true;
            trailInstance = Instantiate(trailPrefab, gameObject.transform.position, Quaternion.identity);
            trailInstance.transform.SetParent(gameObject.transform);
            chargeDirection = (target.position - gameObject.transform.position).normalized;
            currentCoolDownTime = 0;
            windUpTime = 0;
            chargeTime = 0;
        }
        else if (charging && chargeTime < chargeLength)
        {
            enemyNavMesh.Move(chargeSpeed * Time.deltaTime * chargeDirection);
            chargeTime += Time.deltaTime;
        }
        else
        {
            if (charging)
            {
                charging = false;
                enemyNavMesh.isStopped = false;
                trailInstance.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }

            if (currentCoolDownTime > chargeCoolDownLength)
            {
                enemyNavMesh.isStopped = true;
                if (chargeWindUpUI.winding)
                {
                    windUpTime += Time.deltaTime;
                }
                else if (chargeWindUpUI.winding == false)
                {
                    chargeWindUpUI.SetWinding(windUpLength);
                }
            }
            currentCoolDownTime += Time.deltaTime;
        }
    }

    void MeleeAttack()
    {
        Debug.Log("Melee Attack!!!");
        GetTarget().GetComponent<Health>().AddDamage(meleeDamage);
        //GetTarget().GetComponentInChildren<ToggleUIVisibility>().ShowUI();
    }

    void ChargeAttack()
    {
        Debug.Log("Melee Attack!!!");
        GetTarget().GetComponent<Health>().AddDamage(chargeDamage);
        //GetTarget().GetComponentInChildren<ToggleUIVisibility>().ShowUI();
    }


    public IEnumerator StartCombat()
    {
        yield return new WaitForSeconds(timeBeforeCombat);
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform, null);
    }
}
