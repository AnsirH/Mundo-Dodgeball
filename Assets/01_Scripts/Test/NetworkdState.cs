using Fusion;
using UnityEngine;
using static Fusion.NetworkBehaviour;

public class NetworkdState : NetworkBehaviour
{
    public GameObject obj;
    [Networked] NetworkBool IsActive { get; set; }
    private ChangeDetector changeDetector;

    private void Awake()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetActive(!gameObject.activeSelf);
        }
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsActive):
                    SetActive(IsActive);
                    break;
            }
        }
    }

    private void SetActive(bool active)
    {
        obj.SetActive(active);
        IsActive = active;
    }
}
