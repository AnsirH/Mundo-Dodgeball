using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopManager : ManagerBase<PopManager>
{
    private List<PopBase> openPops = new List<PopBase>();
    
    public void OpenPop(PopBase pop)
    {
        if(!openPops.Contains(pop))
        {
            openPops.Add(pop);
        }
    }
    public void ClosePop(PopBase pop, bool isAllClose)
    {
        if (openPops.Contains(pop))
        {
            openPops.Remove(pop);
        }
        if(isAllClose)
        {
            foreach(PopBase p in openPops)
            {
                p.gameObject.SetActive(false);
            }
            openPops.Clear();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
