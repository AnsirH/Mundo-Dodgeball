using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopManager : ManagerBase<PopManager>
{
    public List<PopBase> pops = new List<PopBase>();
    #region Pop Active
    private List<PopBase> openPops = new List<PopBase>();
    private List<GameObject> DetailOpenPops = new List<GameObject>();
    public void OpenPop(PopBase pop)
    {
        if(!openPops.Contains(pop))
        {
            openPops.Add(pop);
        }
    }
    public void ClosePop(PopBase pop)
    {
        if (openPops.Contains(pop))
        {
            openPops.Remove(pop);
        }
        if(openPops.Count > 0)
        {
            foreach (PopBase p in openPops)
            {
                p.gameObject.SetActive(false);
            }
            openPops.Clear();
        }
        if(DetailOpenPops.Count > 0)
        {
            foreach (GameObject p in DetailOpenPops)
            {
                p.gameObject.SetActive(false);
            }
            DetailOpenPops.Clear();
        }
    }
    public void DetailOpenPop(GameObject pop)
    {
        if (!DetailOpenPops.Contains(pop))
        {
            DetailOpenPops.Add(pop);
        }
    }
    public void DetailClosePop(GameObject pop)
    {
        if (DetailOpenPops.Contains(pop))
        {
            DetailOpenPops.Remove(pop);
        }
    }
    #endregion
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
