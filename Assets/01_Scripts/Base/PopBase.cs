using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopBase : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
        PopManager.instance.OpenPop(this);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
        PopManager.instance.ClosePop(this);
    }
    public virtual void DetailOpen(GameObject g)
    {
        g.SetActive(true);
        PopManager.instance.DetailOpenPop(g);
    }
    public virtual void DetailClose(GameObject g)
    {
        g.SetActive(false);
        PopManager.instance.DetailClosePop(g);
    }
}
