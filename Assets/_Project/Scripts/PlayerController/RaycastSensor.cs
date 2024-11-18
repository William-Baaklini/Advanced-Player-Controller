using System;
using UnityEditor.PackageManager;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess
public class RaycastSensor
{
    public float castLength = 1f;
    public LayerMask LayerMask = 255;
    
    private Vector3 _origin = Vector3.zero;
    private Transform _transform;
    
    public enum CastDirection { Forward, Right, Up, Backward, Left, Down }
    private CastDirection _castDirection;

    private RaycastHit hitInfo;

    public RaycastSensor(Transform parentTransform)
    {
        _transform = parentTransform;
    }

    public void Cast()
    {
        Vector3 worldOrigin = _transform.TransformPoint(_origin);
        Vector3 worldDirection = GetCastDirection();

        Physics.Raycast(worldOrigin, worldDirection, out hitInfo, castLength, LayerMask, QueryTriggerInteraction.Ignore);
    }

    public bool HasDetectedHit() => hitInfo.collider != null;
    public float GetDistance() => hitInfo.distance;
    public Vector3 GetNormal() => hitInfo.normal;
    public Vector3 GetPosition() => hitInfo.point;
    public Collider GetCollider() => hitInfo.collider;
    public Transform GetTransform() => hitInfo.transform;
    

    public void SetCastDirection(CastDirection direction) => _castDirection = direction;
    public void SetCastOrigin(Vector3 pos) => _origin = _transform.InverseTransformPoint(pos);

    Vector3 GetCastDirection()
    {
        return _castDirection switch
        {
            CastDirection.Forward => _transform.forward,
            CastDirection.Right => _transform.right,
            CastDirection.Up => _transform.up,
            CastDirection.Backward => -_transform.forward,
            CastDirection.Left => -_transform.right,
            CastDirection.Down => -_transform.up,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
