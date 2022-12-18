using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    public int dashSpeed;
    public float dashLength;
    public float dashCoolDownLength;

    CharacterController myPlayerController;

    Health myHealth;

    public GameObject stunVFX;
    public GameObject trailPrefab;
    GameObject trailInstance;

    Vector3 playerDirection;
    Vector3 dashDirection;
    Vector3 pushDir;

    float xInput;
    float zInput;
    float dashEnd;
    float dashCoolDownEnd;
    float stunLength;
    float pushForce;
    float pushTime;

    bool dashing;
    bool stunned;
    bool pushed;
    bool invulnerabilityReset;


    Character_AnimHandler animHandler;
    [SerializeField]
    SpriteFlipping spriteFlipping;


    private void Start()
    {
        myPlayerController = gameObject.GetComponent<CharacterController>();
        myHealth = gameObject.GetComponent<Health>();
        animHandler = GetComponent<Character_AnimHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        playerDirection = Vector3.Normalize(new Vector3(xInput, 0, zInput));

        Stun();
        PushBackPlayer();
        MovePlayer();        
        Dash();

        // Animation
        if((spriteFlipping.IsCurrentlyFacingRight() && xInput > 0) || (!spriteFlipping.IsCurrentlyFacingRight() && xInput < 0))
        {
            animHandler.Update_Speed(-myPlayerController.velocity.magnitude);
        }
        else
        {
            animHandler.Update_Speed(myPlayerController.velocity.magnitude);
        }
    }

    void MovePlayer()
    {
        if (!dashing && !stunned && GameManager.instance.inputAvailable)
        {
            myPlayerController.Move(playerSpeed * Time.deltaTime * playerDirection);
        }
    }

    void Dash()
    {
        if (!stunned)
        {
            if (Input.GetAxisRaw("Jump") > 0 && !dashing && Time.time > dashCoolDownEnd && GameManager.instance.inputAvailable)
            {
                dashing = true;
                myHealth.ToggleInvulnerability();
                trailInstance = Instantiate(trailPrefab, gameObject.transform.position, Quaternion.identity);
                trailInstance.transform.SetParent(gameObject.transform);
                dashEnd = dashLength;
                dashDirection = playerDirection;
                dashCoolDownEnd = Time.time + dashCoolDownLength;

                // Audio
                AudioManager.instance.PlaySound("Player_Dash", transform.position, true);

                // Animation
                animHandler.Trigger_Dash();
            }
            else if (0 < dashEnd)
            {
                myPlayerController.Move(dashSpeed * Time.deltaTime * dashDirection);
                dashEnd -= Time.deltaTime;
            }
            else
            {
                if (dashing)
                {
                    dashing = false;
                    //myHealth.ToggleInvulnerability();
                    invulnerabilityReset = false;
                }
                if (trailInstance != null)
                {
                    trailInstance.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            if (dashing && !invulnerabilityReset && dashEnd < (dashLength * 0.5f))
            {
                invulnerabilityReset = true;
                myHealth.ToggleInvulnerability();
            }
        }
    }

    public void StartPushPlayerBack(Vector3 direction, float force, float pushBackTime)
    {
        pushTime = pushBackTime;
        pushed = true;
        pushDir = new Vector3(direction.x, 0, direction.z);
        pushForce = force;
    }

    void PushBackPlayer()
    {
        if (pushed && 0 < pushTime)
        {
            myPlayerController.Move(pushForce * Time.deltaTime * Time.deltaTime * pushDir);
        }
        pushTime -= Time.deltaTime;
    }

    public void SetStun(float stunLength)
    {
        stunned = true;
        stunVFX.SetActive(true);
        this.stunLength = stunLength;
    }

    void Stun()
    {
        if (stunned && 0 > stunLength)
        {
            stunVFX.SetActive(false);
            stunned = false;
            pushed = false;
        }
        stunLength -= Time.deltaTime;
    }
}
