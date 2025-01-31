using UnityEngine;

public class RangeDisplayer : MonoBehaviour
{
    [Header("References")]
    public Transform rangeDisplay;

    public void UpdateRange(Vector3 direction, float distance)
    {
        direction.y = 0.0f;

        rangeDisplay.localPosition = Vector3.forward * distance * 0.5f;
        rangeDisplay.localScale = new Vector3(1.0f, distance, 1.0f);

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
