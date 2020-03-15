using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterAnimation : MonoBehaviour
{

    [SerializeField]
    private MyCharacterController controller;
    
    private Animator anim;
    private SpriteRenderer sprite;

    private bool idleFlag = true;
    private bool jumpFlag = true;
    
    //private Vector3 tempRotation;
    private Vector3 upDirection;

    private int tilesLayer;
    private int movableLayer;
    private int standable;

    private bool deadState = false;

    [SerializeField]
    private GameObject scarf;


    [SerializeField]
    private GameObject deathEffects;

    void Start()
    {
        tilesLayer = LayerMask.GetMask("Tiles");
        movableLayer = LayerMask.GetMask("Movable");

        standable = tilesLayer | movableLayer;

        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        upDirection = new Vector3(0, 1, 0);
        //tempRotation = new Vector3(0, 0, 0);
        
    }

    private void RotateOnSurface()
    {
        /*
        if (controller.rb.velocity.y < -0.4f) {
            if (controller.rb.velocity.x > 0f) {
                tempRotation.z = -45f;
            } else if (controller.rb.velocity.x < 0.4f) {
                tempRotation.z = 45f;
            }

        } else if (controller.rb.velocity.y > 0.5f) {
            if (controller.rb.velocity.x > 0f) {
                tempRotation.z = 45f;
            } else if (controller.rb.velocity.x < 0f) {
                tempRotation.z = -45f;
            }

        } else {
            tempRotation.z = 0f;
        }
        */
        

        if (controller.IsGrounded()) {

            RaycastHit xx;

            if (Physics.Raycast(transform.position, -Vector3.up, out xx, 0.3f, standable, QueryTriggerInteraction.Ignore) ||
                Physics.Raycast(transform.position - Vector3.right * 0.15f, -Vector3.up, out xx, 0.3f, standable, QueryTriggerInteraction.Ignore) ||
                Physics.Raycast(transform.position + Vector3.right * 0.15f, -Vector3.up, out xx, 0.3f, standable, QueryTriggerInteraction.Ignore)) {
                
                transform.up = xx.normal;

            }

        }

        //transform.localEulerAngles = tempRotation;

    }

    private void RunningAnimation() {
        
        anim.speed = (0.33f * (Mathf.Abs(controller.rb.velocity.x)/6)) + 0.05f;

        if (Input.GetKey(KeyCode.A) && controller.rb.velocity.x < 0f) {

            if (idleFlag) {
                idleFlag = false;
                
                anim.Play("new_run", 0, 0f);

                // if (!Mathf.Approximately(controller.rb.velocity.y, 0f)) 
                
            }

        } else if (Input.GetKey(KeyCode.D) && controller.rb.velocity.x > 0f) {

            if (idleFlag) {
                idleFlag = false;

                anim.Play("new_run", 0, 0f);
                
            }

        } else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {

            idleFlag = true;

        }
    }

    private void MidAirAnimation()
    {
        
        if (jumpFlag) {
            
            jumpFlag = false;
            idleFlag = true;

            anim.speed = 0.1f;
            anim.Play("new_jump", 0, 0f);
        }

        transform.up = upDirection;
        //tempRotation.z = 0f;
        //transform.localEulerAngles = tempRotation;

    }

    private void FlipSide()
    {
        
        if (Input.GetKey(KeyCode.A)) {
            sprite.flipX = true;
        } else if (Input.GetKey(KeyCode.D)) {
            sprite.flipX = false;
        }
        
    }

    private void IdleAnimation()
    {
        // 0 velocity means standing still
        if (controller.rb.velocity.sqrMagnitude < 0.1f) {
            idleFlag = true;
            //anim.Play("idletest", 0, 0f);
            anim.Play("new_idle", 0, 0f);

            //tempRotation.z = 0f;
            //transform.localEulerAngles = tempRotation;

        } else {
            if (idleFlag) {

                if (controller.rb.velocity.x < 0) {
                    sprite.flipX = true;
                } else if (controller.rb.velocity.x > 0) {
                    sprite.flipX = false;
                }
                
                //anim.Play("slidetest", 0, 0f);
                anim.Play("new_slide", 0, 0f);
            }
        }

    }

    private void WallSlideAnimation() {

        //   0 = mid air
        //  -1 = left wall
        //   1 = right wall

        if (controller.IsNearWall(0.3f) == -1) {

            //tempRotation.z = 0f;
            //transform.localEulerAngles = tempRotation;
            transform.up = upDirection;

            if (Input.GetKey(KeyCode.A)) {
                anim.Play("new_wall_slide", 0, 0f);
            } else {
                anim.Play("new_jump", 0, 1f);
            }

        } else if (controller.IsNearWall(0.3f) == 1) {

            //tempRotation.z = 0f;
            //transform.localEulerAngles = tempRotation;
            transform.up = upDirection;

            if (Input.GetKey(KeyCode.D)) {
                anim.Play("new_wall_slide", 0, 0f);
            } else {
                anim.Play("new_jump", 0, 1f);
            }

        }

    }

    void Update()
    {
        if (deadState) return;

        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null) {
            // don't place or delete tiles when interacting with ui
            return;
        }

        FlipSide();

        if (controller.IsGrounded()) {

            IdleAnimation();
            RotateOnSurface();
            RunningAnimation();

            jumpFlag = true;

        } else if (controller.IsNearWall(0.4f) != 0) {

            WallSlideAnimation();

            jumpFlag = true;

        } else {

            MidAirAnimation();

        }

    }

    public void Revive()
    {
        deadState = false;
        sprite.enabled = true;
        scarf.SetActive(true);
    }

    public void Die() {
        deadState = true;
        sprite.enabled = false;
        scarf.SetActive(false);
        
        Instantiate(deathEffects, transform.position, transform.rotation);
        // show some effects
    }
}
