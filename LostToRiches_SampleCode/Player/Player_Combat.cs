using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [Header("Combat Properties")]
    public Transform projectileSpawnPoint;

    [Space(10)]
    public Color projectileColor;
    public float projectileDamage = 5;
    public float projectileSpeed = 10;
    public float projectileSize;
    public float pojectileColliderRadius;

    [Space(10)]

    public float projectileHeightOffset = 0.5f;
    public float offsetThreshhold = 1;

    [Space(10)]

    public float shotCooldown = 1;      // seconds
    float nextShot = 0;

    [Space(10)]

    public int maxAmmo = 10;
    int ammoLeft;
    bool reloading = false;
    public float reloadingTime = 2;     // seconds
    Coroutine reloadingCoroutine = null;

    [Space(10)]

    Ammo_UI ammoUI;


    void Shoot()
    {
        if(projectileSpawnPoint == null)
        {
            return;
        }
        if(!reloading)
        {
            if(ammoLeft > 0)
            {
                GameObject projectile = PoolManager.instance.getBullet();

                Vector3 projectileInitialPosition = projectileSpawnPoint.position;
                projectileInitialPosition.y = transform.position.y + projectileHeightOffset;
                projectile.transform.position = projectileInitialPosition;

                List<ItemSO> items = GetComponent<ItemPlayerManager>().items;

                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.InitializeProjectile(true, projectileDamage, projectileSpeed, projectileSize, projectileColor, false, items);
                projectileScript.IncreaseProjectileRadius(pojectileColliderRadius);

                #region OldAim
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
                //foreach (RaycastHit hit in hits)
                //{
                //    if (hit.transform.tag == "Enemy" || hit.transform.tag == "DestructibleObject" || hit.transform.tag == "Trap")
                //    {
                //        projectile.GetComponent<Projectile>().FireProjectile(hit.transform.GetComponent<Collider>().bounds.center);
                //    }
                //    else if (hit.transform.tag == "Player")
                //    {
                //        continue;
                //    }
                //    else if (hit.transform.tag == "Wall" && hit.transform.GetComponent<SeeThrough_Wall>() != null)
                //    {
                //        continue;
                //    }
                //    else if (hit.transform.tag == "Wall")
                //    {
                //        projectile.GetComponent<Projectile>().FireProjectile(new Vector3(hit.point.x, projectileHeightOffset, hit.point.z));
                //    }
                //    else
                //    {
                //        if (Vector3.Distance(hit.point, transform.position) < offsetThreshhold)
                //        {
                //            projectile.GetComponent<Projectile>().FireProjectile(new Vector3((projectileSpawnPoint.up * 100).x,
                //                projectileHeightOffset, (projectileSpawnPoint.up * 100).z));
                //        }
                //        else
                //        {
                //            projectile.GetComponent<Projectile>().FireProjectile(hit.point + new Vector3(0, projectileHeightOffset, 0));
                //        }
                //    }
                //}

                //// if shoots nothing
                //if (hits.Length == 0)
                //{
                //    projectile.GetComponent<Projectile>().FireProjectile(new Vector3((projectileSpawnPoint.up * 100).x, 0, (projectileSpawnPoint.up * 100).z));
                //}
                #endregion
                   
                //New aim where the y value along the ray is equal to projectileHeightOffset is used to find the x and z values at that position to set the direction of the projectile.
                Camera myCamera = Camera.main;
                Vector3 direction = Input.mousePosition;
                direction = myCamera.ScreenToWorldPoint(new Vector3(direction.x, direction.y, myCamera.nearClipPlane));
                float t = (projectileHeightOffset - myCamera.transform.position.y) / (direction.y - myCamera.transform.position.y); //Used to find the x and z values at the projectileHeightOffset on the ray.
                float xValue = myCamera.transform.position.x + (t * (direction.x - myCamera.transform.position.x)); //The x position at the point on the ray where y is equal to projectileHeightOffset.
                float zValue = myCamera.transform.position.z + (t * (direction.z - myCamera.transform.position.z)); //The z position at the point on the ray where y is equal to projectileHeightOffset.

                projectile.GetComponent<Projectile>().FireProjectile(new Vector3(xValue, 0, zValue));



                ammoLeft--;

                GetComponent<VFX_Handler>().Spawn_MuzzleFlash();
                ammoUI.UpdateAmmo(ammoLeft);

                // audio
                AudioManager.instance.PlaySound("Player_Gunshot", transform.position, true);

                // make the enemies dodge
                foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    if(enemy.GetComponent<Enemy_Combat>().GetTarget() != null)
                    {
                        enemy.GetComponent<Enemy_Combat>().Dodge();
                    }
                }
            }
            else
            {
                AudioManager.instance.PlaySound("Player_NoAmmo", transform.position, false);

                reloadingCoroutine = StartCoroutine(Reload());
            }
        }
        else if(reloading && ammoLeft > 0)
        {
            // if reloading, break out of reloading
            StopCoroutine(reloadingCoroutine);
            ammoUI.FinishReloading();
            reloading = false;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        if (ammoLeft == maxAmmo)
            yield break;

        Debug.Log("Reloading...");
        reloading = true;
        ammoUI.StartReloading(reloadingTime);

        AudioManager.instance.PlaySound("Player_Reload", transform.position, false);

        yield return new WaitForSeconds(reloadingTime);

        Debug.Log("Reloaded");
        ammoLeft = maxAmmo;
        ammoUI.UpdateAmmo(ammoLeft);
        ammoUI.FinishReloading();
        reloading = false;

        AudioManager.instance.PlaySound("Reload_Finish", transform.position, false);
    }
    void Start()
    {
        ammoUI = Player_UI.instance.ammo_UI;

        ammoLeft = maxAmmo;
        ammoUI.InitializaAmmo(maxAmmo);
    }

    void Update()
    {
        if(GameManager.instance.inputAvailable)
        {
            if (GetComponent<Item_BulletRitual>())
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    shotCheck();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                shotCheck();
            }
            
        }

        if(nextShot > 0)
        {
            nextShot -= Time.deltaTime;
        }

        if(!reloading && Input.GetKeyDown(KeyCode.R) && GameManager.instance.inputAvailable)
        {
            reloadingCoroutine = StartCoroutine(Reload());
        }
    }
    void shotCheck()
    {
        if (nextShot <= 0)
        {
            Shoot();
            nextShot = shotCooldown;
        }
    }
}
