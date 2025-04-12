using Unity.VisualScripting;
using UnityEngine;

public class RangeDisplayer : MonoBehaviour, IRangeIndicator
{
    [Header("References")]
    public Transform rangeDisplay;

    public bool IsActive => rangeDisplay.gameObject.activeSelf;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void UpdatePosition(Vector3 position, float distance)
    {
        position.y = transform.position.y;
        Vector3 direction = (position - transform.position).normalized;
        rangeDisplay.localPosition = Vector3.forward * distance * 0.5f;
        rangeDisplay.localScale = new Vector3(1.0f, distance, 1.0f);

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
