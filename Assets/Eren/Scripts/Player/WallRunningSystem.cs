using UnityEngine;

public class WallRunningSystem : MonoBehaviour
{
    [Header("Ayarlar")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunSpeed = 8f;
    public float wallClimbSpeed = 0f;
    public float wallJumpUpForce = 12f;
    public float wallJumpSideForce = 15f;
    public float wallJumpBackForce = 10f;
    public float wallJumpCoyoteTime = 0.25f;

    [Header("Momentum Ayarý")]
    public float momentumKillTime = 0.3f;

    [Header("Detection")]
    public float wallCheckDistance = 0.8f;
    public float minJumpHeight = 1.0f;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit frontWallHit;

    private bool wallLeft;
    private bool wallRight;
    private bool wallFront;

    private float lastWallTime;
    private Vector3 lastWallNormal;

    // Zýpladýðýmýzda W'ye bassak bile tekrar yapýþmayý engeller
    private bool exitingWall;
    private float exitWallTimer;
    private float exitWallTime = 0.2f;

    [Header("Referanslar")]
    public Transform orientation;
    private AniCharNEw mainScript;
    private Rigidbody rb;

    public bool isWallRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainScript = GetComponent<AniCharNEw>();
        if (orientation == null) orientation = transform;
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        // --- YENÝ EKLENEN: ANÝMASYON BAÐLANTISI ---
        UpdateAnimationLink();
    }

    private void FixedUpdate()
    {
        if (isWallRunning)
            WallRunningMovement();
    }

    // --- YENÝ EKLENEN FONKSÝYON ---
    private void UpdateAnimationLink()
    {
        // Duvar tarafýný belirle (1: Sað, -1: Sol)
        float side = 0f;
        if (wallRight) side = 1f;
        else if (wallLeft) side = -1f;

        // Ana scripte durumu bildir
        mainScript.SetWallRunAnimation(isWallRunning, side);
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
        wallFront = Physics.Raycast(transform.position, orientation.forward, out frontWallHit, wallCheckDistance, whatIsWall);

        if (wallRight || wallLeft || wallFront)
        {
            lastWallTime = Time.time;
            if (wallRight) lastWallNormal = rightWallHit.normal;
            else if (wallLeft) lastWallNormal = leftWallHit.normal;
            else if (wallFront) lastWallNormal = frontWallHit.normal;
        }
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    public bool IsWallJumpPossible()
    {
        bool wallNearby = isWallRunning || (Time.time < lastWallTime + wallJumpCoyoteTime);
        return (wallNearby || wallFront) && AboveGround();
    }

    private void StateMachine()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");

        // --- WALL RUN BAÞLATMA ---
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !wallFront && !exitingWall)
        {
            if (!isWallRunning)
                StartWallRun();
        }
        else if (exitingWall)
        {
            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        else
        {
            if (isWallRunning)
                StopWallRun();
        }

        // --- ZIPLAMA KONTROLÜ ---
        if (Input.GetKeyDown(KeyCode.Space) && IsWallJumpPossible())
        {
            WallJump();
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        rb.useGravity = false;
        mainScript.ResetJumpCount();
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        mainScript.CancelInvoke("StopPhysicsMomentum");
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        Vector3 targetVelocity = wallForward * wallRunSpeed;
        targetVelocity.y = wallClimbSpeed;
        rb.linearVelocity = targetVelocity;

        if (!(wallLeft && Input.GetAxisRaw("Horizontal") > 0) && !(wallRight && Input.GetAxisRaw("Horizontal") < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    private void WallJump()
    {
        isWallRunning = false;
        rb.useGravity = true;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceNormal = lastWallNormal;
        if (wallRight) forceNormal = rightWallHit.normal;
        else if (wallLeft) forceNormal = leftWallHit.normal;
        else if (wallFront) forceNormal = frontWallHit.normal;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 jumpDirection;

        if (wallFront && !wallLeft && !wallRight)
        {
            jumpDirection = transform.up * wallJumpUpForce + forceNormal * wallJumpBackForce;
        }
        else
        {
            jumpDirection = transform.up * wallJumpUpForce + forceNormal * wallJumpSideForce;
            jumpDirection += orientation.forward * (wallJumpSideForce / 2f);
        }

        rb.AddForce(jumpDirection, ForceMode.Impulse);

        // --- YENÝ: ANÝMASYON TETÝKLE ---
        mainScript.TriggerWallJumpAnimation();

        mainScript.EnablePhysicsMovement();
        mainScript.LockJumpInput(0.15f);

        mainScript.CancelInvoke("StopPhysicsMomentum");
        mainScript.Invoke("StopPhysicsMomentum", momentumKillTime);

        lastWallTime = 0f;
    }
}