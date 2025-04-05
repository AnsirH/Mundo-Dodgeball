using UnityEngine;

public class MockMovement : IMovement
{
    public bool MoveCalled { get; private set; }

    public void Move(Vector3 direction)
    {
        MoveCalled = true;
    }
}

public interface IMovement
{
    void Move(Vector3 direction);
}