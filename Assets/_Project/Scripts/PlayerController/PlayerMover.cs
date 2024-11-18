using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMover : MonoBehaviour
{
    #region Fields
    [Header("Collider Settings:")]
    [Range(0f, 1f)] [SerializeField] private float stepHeightRatio = 0.1f;
    [SerializeField] private float colliderHeight = 2f;
    [SerializeField] private float colliderThickness = 1f;
    [SerializeField] Vector3 colliderOffset = Vector3.zero;

    private Rigidbody rb;
    private Transform tr;
    private CapsuleCollider col;
    private RaycastSensor sensor;
    
    bool isGrounded;
    float baseSensorRange;
    Vector3 currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
    int currentLayer;
    
    [Header("Sensor Settings:")]
    [SerializeField] bool isInDebugMode;
    bool isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions
    #endregion

    private void Awake() {
        Setup();
        RecalculateColliderDimensions();
    }

    private void OnValidate() {
        if (gameObject.activeInHierarchy) {
            RecalculateColliderDimensions();
        }
    }
    
    public void CheckForGround() {
        if (currentLayer != gameObject.layer) {
            RecalculateSensorLayerMask();
        }
            
        currentGroundAdjustmentVelocity = Vector3.zero;
        sensor.castLength = isUsingExtendedSensorRange 
            ? baseSensorRange + colliderHeight * tr.localScale.x * stepHeightRatio
            : baseSensorRange;
        sensor.Cast();
            
        isGrounded = sensor.HasDetectedHit();
        if (!isGrounded) return;
            
        float distance = sensor.GetDistance();
        float upperLimit = colliderHeight * tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
        float middle = upperLimit + colliderHeight * tr.localScale.x * stepHeightRatio;
        float distanceToGo = middle - distance;
            
        currentGroundAdjustmentVelocity = tr.up * (distanceToGo / Time.fixedDeltaTime);
    }
    
    public bool IsGrounded() => isGrounded;
    public Vector3 GetGroundNormal() => sensor.GetNormal();
        
    // NOTE: Older versions of Unity use rb.velocity instead
    public void SetVelocity(Vector3 velocity) => rb.linearVelocity = velocity + currentGroundAdjustmentVelocity;
    public void SetExtendSensorRange(bool isExtended) => isUsingExtendedSensorRange = isExtended;
    
    private void Setup() {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
            
        rb.freezeRotation = true;
        rb.useGravity = false;
    }
    
    private void RecalculateColliderDimensions() {
        if (col == null) {
            Setup();
        }
            
        col.height = colliderHeight * (1f - stepHeightRatio);
        col.radius = colliderThickness / 2f;
        col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);

        if (col.height / 2f < col.radius) {
            col.radius = col.height / 2f;
        }
            
        RecalibrateSensor();
    }
    
    private void RecalibrateSensor() {
        sensor ??= new RaycastSensor(tr);
            
        sensor.SetCastOrigin(col.bounds.center);
        sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
        RecalculateSensorLayerMask();
            
        const float safetyDistanceFactor = 0.001f; // Small factor added to prevent clipping issues when the sensor range is calculated
            
        float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
        baseSensorRange = length * (1f + safetyDistanceFactor) * tr.localScale.x;
        sensor.castLength = length * tr.localScale.x;
    }
    
    void RecalculateSensorLayerMask() {
        int objectLayer = gameObject.layer;
        int layerMask = Physics.AllLayers;

        for (int i = 0; i < 32; i++) {
            if (Physics.GetIgnoreLayerCollision(objectLayer, i)) {
                layerMask &= ~(1 << i);
            }
        }
            
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        layerMask &= ~(1 << ignoreRaycastLayer);
            
        sensor.LayerMask = layerMask;
        currentLayer = objectLayer;
    }
}