using System;
using ImprovedTimers;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] InputReader input;
    
    Transform tr;
    PlayerMover mover;
    // CeilingDetector ceilingDetector;
    
    bool jumpKeyIsPressed;    // Tracks whether the jump key is currently being held down by the player
    bool jumpKeyWasPressed;   // Indicates if the jump key was pressed since the last reset, used to detect jump initiation
    bool jumpKeyWasLetGo;     // Indicates if the jump key was released since it was last pressed, used to detect when to stop jumping
    bool jumpInputIsLocked;   // Prevents jump initiation when true, used to ensure only one jump action per press
    
    public float movementSpeed = 7f;
    public float airControlRate = 2f;
    public float jumpSpeed = 10f;
    public float jumpDuration = 0.2f;
    public float airFriction = 0.5f;
    public float groundFriction = 100f;
    public float gravity = 30f;
    public float slideGravity = 5f;
    public float slopeLimit = 30f;
    public bool useLocalMomentum;
    
    StateMachine stateMachine;
    CountdownTimer jumpTimer;
        
    [SerializeField] Transform cameraTransform;
        
    Vector3 momentum, savedVelocity, savedMovementVelocity;
        
    public event Action<Vector3> OnJump = delegate { };
    public event Action<Vector3> OnLand = delegate { };
    #endregion
    
    void Awake() {
        tr = transform;
        mover = GetComponent<PlayerMover>();
        // ceilingDetector = GetComponent<CeilingDetector>();
            
        jumpTimer = new CountdownTimer(jumpDuration);
        // SetupStateMachine();
    }
}