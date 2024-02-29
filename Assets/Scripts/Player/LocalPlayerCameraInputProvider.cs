using System;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

public class LocalPlayerCameraInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
    [NonSerialized] public Vector2 input;

    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0: return input.x;
            case 1: return input.y;
            default: return 0;
        }
    }
}