using UnityEngine;
using UnityEngine.EventSystems;

public class MyCharacterController : MonoBehaviour {

    [SerializeField]
    private float playerSpeed = 2.0f;

    [SerializeField]
    private float playerJumpPower = 4.5f;
    
    private GameObject playerAvatar;
    
    private CharacterAnimation chAnim;

    //public Rigidbody avatarRb;

    private float wallJumpDistance, wallSlideDistance;
    private bool grounded;
    private bool jump;
    //private bool sliding = false;

    [HideInInspector]
    public Rigidbody rb;
    private Vector3 moveVel;

    [SerializeField]
    private float maxAngularVelocity;
    
    private ParticleSystem jumpPartSyst;

    private int tilesLayer;
    private int movableLayer;
    private int standable;

    private int instantDeath = 13;

    private GameObject gameplay;
    private GameplayLogic gameplayLogic;

    private bool deadState = false;

    private void Awake()
    {
        playerAvatar = GameObject.Find("Player's Avatar");
        gameplay = GameObject.Find("Gameplay Manager");
        chAnim = GameObject.Find("char_anim_0").GetComponent<CharacterAnimation>();
        jumpPartSyst = GameObject.Find("Jump Effects").GetComponent<ParticleSystem>();

        if (gameplay) {
            gameplayLogic = gameplay.GetComponent<GameplayLogic>();
        }

    }

    void Start ()
    {
        tilesLayer = LayerMask.GetMask("Tiles");
        movableLayer = LayerMask.GetMask("Movable");

        standable = tilesLayer | movableLayer;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;

        moveVel = new Vector3(0, 0, 0);
        
    }
	
	public bool IsGrounded() {
        
		if (Physics.Raycast(transform.position, -Vector3.up, 0.3f, standable, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(transform.position-Vector3.up*0.27f, -Vector3.right, 0.18f, standable, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(transform.position-Vector3.up*0.27f,  Vector3.right, 0.18f, standable, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(transform.position - Vector3.right * 0.15f, -Vector3.up, 0.3f, standable, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(transform.position + Vector3.right * 0.15f, -Vector3.up, 0.3f, standable, QueryTriggerInteraction.Ignore)) {
            
            return true;
		}
		
		return false;
	}
	
	public float IsNearWall(float dist) {
		
		if (Physics.Raycast(transform.position - Vector3.up * 0.1f, Vector3.left, dist, standable, QueryTriggerInteraction.Ignore)){
			return -1f;
		}
		
		if (Physics.Raycast(transform.position - Vector3.up * 0.1f, Vector3.right, dist, standable, QueryTriggerInteraction.Ignore)){
			return 1f;
		}
		
		return 0f;
	}

    void DebugRays() {

        // close to wall?
        Debug.DrawLine(transform.position - Vector3.up * 0.1f, transform.position - Vector3.up * 0.1f - Vector3.left * 0.4f, Color.green);
        Debug.DrawLine(transform.position - Vector3.up * 0.1f, transform.position - Vector3.up * 0.1f - Vector3.right * 0.4f, Color.green);

        // grounded?
        Debug.DrawLine(transform.position, transform.position - Vector3.up * 0.3f, Color.red);
        Debug.DrawLine(transform.position - Vector3.up * 0.27f, transform.position - Vector3.up * 0.27f - Vector3.right * 0.18f, Color.red);
        Debug.DrawLine(transform.position - Vector3.up * 0.27f, transform.position - Vector3.up * 0.27f + Vector3.right * 0.18f, Color.red);

        Debug.DrawLine(transform.position - Vector3.right * 0.15f, transform.position - Vector3.right * 0.15f - Vector3.up * 0.3f, Color.red);
        Debug.DrawLine(transform.position + Vector3.right * 0.15f, transform.position + Vector3.right * 0.15f - Vector3.up * 0.3f, Color.red);
        
        }

	void FixedUpdate() {

        if (deadState) return;

        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null) {
            // don't place or delete tiles when interacting with ui
            return;
        }

        DebugRays();

        Running();

        Jump();

        SlideDown();

    }

    private void Jump() {
        
        JumpyFeel();

        if (jump) {
            jump = false;

            wallJumpDistance = IsNearWall(0.4f);

            if (IsGrounded()) {

                EmitJumpParticles();

                //rb.velocity += Vector3.up * playerJumpPower;
                Vector3 v = rb.velocity;
                v.y = (v.y/2f) + playerJumpPower;

                rb.velocity = v;

            } else if (wallJumpDistance != 0) {

                EmitJumpParticles();

                if (rb.velocity.y > -0.4f) {
                    rb.velocity += Vector3.up * playerJumpPower * 1.1f + Vector3.left * wallJumpDistance * playerJumpPower / 1.5f;
                } else {
                    rb.velocity = Vector3.up * playerJumpPower / 1.5f + Vector3.left * wallJumpDistance * playerJumpPower / 1.5f;
                }

            }
        }
    }

    private void EmitJumpParticles()
    {
        jumpPartSyst.Emit(10);
        //jumpPartSyst.Play();
    }
    
    private void SlideDown() {

        wallSlideDistance = IsNearWall(0.25f);

        if (wallSlideDistance == -1 && !IsGrounded() && Input.GetAxis("Horizontal") < 0) {
            if (rb.velocity.y < 0) {
                //sliding = true;
                rb.velocity = rb.velocity * 0.8f;
            }
        } else if (wallSlideDistance == 1 && !IsGrounded() && Input.GetAxis("Horizontal") > 0) {
            if (rb.velocity.y < 0) {
                //sliding = true;
                rb.velocity = rb.velocity * 0.8f;
            }
        } //else sliding = false;

    }

    private void JumpyFeel() {

        if (Input.GetKey(KeyCode.W) && rb.velocity.y > -0.6f) {
            //rb.velocity += Vector3.up * 0.12f;
            rb.AddForce(Vector3.up * 1.2f);
        }

    }

    private void Running() {

        moveVel.x = Input.GetAxis("Horizontal");

        if (IsGrounded()) {
            if (Mathf.Abs(moveVel.x) < 0.9f) {
                rb.AddTorque(transform.forward * playerSpeed * -Input.GetAxis("Horizontal") * 100f * Time.deltaTime);
            } else {
                rb.AddTorque(transform.forward * playerSpeed * -Input.GetAxis("Horizontal") * 500f * Time.deltaTime);
            }

        } else {
            if (rb.velocity.y > -0.6f) {
                rb.AddForce(moveVel * playerSpeed);
            } else {
                rb.AddForce(moveVel * playerSpeed / 2f);
            }
        }
        
    }

	void Update () {

        if (deadState) return;

        playerAvatar.transform.position = transform.position;

        /*
        if (!sliding){
			playerAvatar.transform.position = transform.position;

		} else {
			playerAvatar.transform.position = transform.position;
			//playerAvatar.transform.position = transform.position + wallSlideDistance * Vector3.right * 0.1f;
		}
        */

        if (EventSystem.current.IsPointerOverGameObject()) {
            // don't place or delete tiles when interacting with ui
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.W)){
			jump = true;
            //Jump();
		}

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == instantDeath) {
            
            if (gameplay) {
                gameplayLogic.GameOver();
            }

            Death();
            
        }
    }

    public void Death()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        deadState = true;
        chAnim.Die();
    }

    public void Respawn()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        deadState = false;
        chAnim.Revive();
    }
}
