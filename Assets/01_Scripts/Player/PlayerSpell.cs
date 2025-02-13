using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    public void OnSpellD()
    {
        if (canUseSpellD)
        {
            RaycastHit hit;
            if (Physics.Raycast(CameraManager.Instance.firstPlayerCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                Vector3 mousePoint = hit.point;
                mousePoint.y = transform.position.y;
                Flash(mousePoint);

                canUseSpellD = false;
                Invoke("SetSpellable", 5.0f);
            }
        }
    }

    private void Flash(Vector3 targetPoint)
    {
        Vector3 targetVector = targetPoint - transform.position;
        if (targetVector.magnitude > flashDistance)
        {
            transform.position += targetVector.normalized * flashDistance;
        }
        else
        {
            transform.position += targetVector;
        }
        transform.rotation = Quaternion.LookRotation(targetVector);
    }

    private void SetSpellable()
    {
        canUseSpellD = true;
    }

    public float flashDistance = 1.5f;
    public bool canUseSpellD = true;
}

public class Spell
{
    public virtual void Execute()
    {

    }
}

public class Flash : Spell
{
    public override void Execute()
    {

    }
}
