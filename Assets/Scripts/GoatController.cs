using UnityEngine;
using UnityEngine.InputSystem; 

public class GoatController : MonoBehaviour
{
    private Rigidbody2D rb; 
    private Collider2D goatCollider;
    private InputSystem_Actions inputActions;
    private bool isGrounded = true; 
    
    [Header("1. Managers & References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StoneManager stoneManager;
    [SerializeField] private float speedBufferFactor = 1.05f;

    [Header("2. Physics & Jump Settings")]
    [SerializeField] private float jumpForce = 450f;
    
    [Header("3. Power Jump Settings")]
    [SerializeField] private float doubleTapForceMultiplier = 1.3f;
    
    [Header("4. Fall & Ground Check")]
    [SerializeField] private float fallThresholdY = -10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f; 
    
    // Before Start, initialize references
    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        goatCollider = GetComponent<Collider2D>();
        
        inputActions = new InputSystem_Actions();
        
        inputActions.Player.Jump.performed += OnJump;
        
        inputActions.Player.PowerJump.performed += OnDoubleTapJump;

        if (rb == null || goatCollider == null)
        {
            Debug.LogError("Rigidbody2D oder Collider2D fehlt!");
        }
    }

    void Update()
    {
        // If Goat falls below threshold, trigger game over
        if (transform.position.y < fallThresholdY && gameManager != null)
        {
            gameManager.OnPlayerCollided(); 
            return;
        }

        CheckIfGrounded();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        PerformJump(jumpForce);
    }
    
    private void OnDoubleTapJump(InputAction.CallbackContext context)
    {
        float strongJumpForce = jumpForce * doubleTapForceMultiplier;
        PerformJump(strongJumpForce);
    }
    
    private void PerformJump(float force)
    {
        if (rb != null && isGrounded && stoneManager != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
            
            float verticalComponent = force;
            // Increase horizontal speed slightly according to stone speed
            float targetXVelocity = stoneManager.CurrentMoveSpeed * speedBufferFactor; 
            
            rb.linearVelocity = new Vector2(targetXVelocity, verticalComponent);

            isGrounded = false;
        }
    }

    // Check if the goat is currently grounded using raycasts
    void CheckIfGrounded() 
    {
        if (goatCollider == null) return; 

        Vector3 raycastOrigin = new Vector3(
            goatCollider.bounds.center.x, 
            goatCollider.bounds.min.y, 
            transform.position.z
        );
        raycastOrigin += Vector3.down * 0.05f; 

        Vector2[] directions = { 
            Vector2.down, 
            new Vector2(-1f, -1f).normalized, 
            new Vector2(1f, -1f).normalized 
        };
        
        bool hitGround = false;
        // If any raycast hits the ground, consider the goat grounded
        foreach (Vector2 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, direction, groundCheckDistance, groundLayer);
            Debug.DrawRay(raycastOrigin, direction * groundCheckDistance, hit.collider != null ? Color.green : Color.red);

            if (hit.collider != null)
            {
                hitGround = true;
                break; 
            }
        }
        
        isGrounded = hitGround;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.PowerJump.performed -= OnDoubleTapJump; 
        inputActions.Disable();
    }
}