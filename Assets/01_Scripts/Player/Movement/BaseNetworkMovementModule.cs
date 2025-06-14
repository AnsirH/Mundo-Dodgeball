using Fusion;
using UnityEngine;

public class BaseNetworkMovementModule
{
    public BaseNetworkMovementModule(NetworkCharacterController cc, float moveSpeed = 5.0f, float rotateSpeed = 10.0f)
    {
        _cc = cc;
        MoveSpeed = moveSpeed;
        RotateSpeed = rotateSpeed;
    }

    public void MoveForDeltaTime(Vector3 normalizedDirection, float runnerDeltaTime)
    {
        _cc.Move(normalizedDirection * MoveSpeed * runnerDeltaTime);
    }

    public void Teleport(Vector3 targetPosition)
    {
        _cc.Teleport(targetPosition);
    }

    public void RotateForDeltaTime(Quaternion currentRotation, Vector3 direction, float runnerDeltaTime)
    {
        Quaternion targetRotation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(direction), RotateSpeed * runnerDeltaTime);
        _cc.transform.rotation = targetRotation;
    }

    protected NetworkCharacterController _cc;
    public float MoveSpeed;
    public float RotateSpeed;
}
