using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveTest : MonoBehaviour
{
    public enum EMoveType
    {
        Pitch,
        Roll,
        Yaw,
    }

    public EMoveType _moveType = EMoveType.Pitch;
    public float _speed = 3.14f;
    private float _angle = 0.0f;
    
    void Update()
    {
        _angle = Time.deltaTime * _speed;
        Vector3 axis = Vector3.up;
        switch (_moveType)
        {
            case EMoveType.Pitch:
                axis = transform.right;
                break;
            case EMoveType.Roll:
                axis = transform.forward;
                break;
            case EMoveType.Yaw:
                axis = transform.up;
                break;
            default:
                break;
        }
        transform.Rotate(axis,_angle);
    }
}
