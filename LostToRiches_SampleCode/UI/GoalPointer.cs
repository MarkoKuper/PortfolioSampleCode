using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPointer : MonoBehaviour
{
    [SerializeField]
    GameObject pointerImage;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    float hideDistance = 15f;

    GameObject[] enemies;

    Transform playerPostion;
    Transform currentGoal;

    Vector3 dir;
    Vector3 screenCenter;
    Vector3 screenBound;

    float angle;
    float shortestDistToPlayer;
    float screenDiagonal;
    float angleBC;
    float angleAC;

    bool point;

    private void Start()
    {
        float width = canvas.GetComponent<CanvasScaler>().referenceResolution.x;
        float height = canvas.GetComponent<CanvasScaler>().referenceResolution.y;
        screenCenter = new Vector3(width, height) * 0.5f;

        screenBound = screenCenter * 0.8f;

        screenDiagonal = Mathf.Sqrt((width * width) + (height * height));

        angleBC = Mathf.Acos(((width * width) + (screenDiagonal * screenDiagonal) - (height * height)) / (2 * width * screenDiagonal)) * Mathf.Rad2Deg;

        angleAC = 180 - 90 - angleBC;
    }

    // Update is called once per frame
    void Update()
    {
        if (point)
        {
            FindClosestEnemyToPlayer();
            HideArrowWhenClose();
            PointToGoal();
            MoveArrowToEdgeOfScreen();
        }
    }

    void PointToGoal()
    {
        if (currentGoal != null && playerPostion != null)
        {
            dir = playerPostion.position - currentGoal.position;
            angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            pointerImage.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void MoveArrowToEdgeOfScreen()
    {
        Vector3 screenPosition = new();
        float m;
        if (angle < angleBC && -angleBC < angle)
        {
            m = Mathf.Tan(angle * Mathf.Deg2Rad);
            screenPosition = new Vector3(-screenBound.x, -screenBound.x * m, 0);
        }
        else if (angle > 180 - angleBC && angle <= 180 || angle < -180 + angleBC && angle >= -180)
        {
            if(angle > 0)
            {
                m = Mathf.Tan((angle - 180) * Mathf.Deg2Rad);
            }
            else
            {
                m = Mathf.Tan((angle + 180) * Mathf.Deg2Rad);
            }
            screenPosition = new Vector3(screenBound.x, screenBound.x * m, 0);
        }
        else if (90 - angleAC < angle && angle < 90 + angleAC)
        {
            m = Mathf.Tan((-90 + (180 - angle)) * Mathf.Deg2Rad);
            screenPosition = new Vector3(-screenBound.y * m, -screenBound.y, 0);
        }
        else if (-90 - angleAC < angle && angle < -90 + angleAC)
        {
            m = Mathf.Tan((90 + (-180 - angle)) * Mathf.Deg2Rad);
            screenPosition = new Vector3(screenBound.y * m, screenBound.y, 0);
        }

        pointerImage.transform.localPosition = screenPosition;
    }

    void FindClosestEnemyToPlayer()
    {
        if (enemies.Length > 0)
        {
            shortestDistToPlayer = 0;
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && playerPostion != null)
                {
                    float enemyDist = Vector3.Distance(playerPostion.position, enemy.transform.position);
                    if (enemyDist < shortestDistToPlayer || shortestDistToPlayer == 0)
                    {
                        shortestDistToPlayer = enemyDist;
                        currentGoal = enemy.transform;
                    }
                }
            }
        }
    }

    void HideArrowWhenClose()
    {
        if (currentGoal != null && playerPostion != null)
        {
            //if (enemies.Length > 0)
            //{
            //    Renderer goalRenderer = currentGoal.gameObject.GetComponentInChildren<Renderer>();

            //    pointerImage.SetActive(!goalRenderer.isVisible);
            //}
            //else
            //{
                float distanceToGoal = Vector3.Distance(playerPostion.position, currentGoal.position);

                if (pointerImage.activeInHierarchy == true && distanceToGoal < hideDistance)
                {
                    pointerImage.SetActive(false);
                }
                else if (pointerImage.activeInHierarchy == false && distanceToGoal >= hideDistance)
                {
                    pointerImage.SetActive(true);
                }
            //}
        }
    }


    public void UpdateEnemyList(GameObject[] enemyList)
    {
        enemies = enemyList;
    }

    public void ToggleGoalPointer()
    {
        point = !point;
        playerPostion = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetGoal(Transform newGoal)
    {
        currentGoal = newGoal;
    }
}
