using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UserData
{
    public string name;
    public int level;
    public int exp;
    public int gold;
    public int iconInDex;

    public UserData()
    {   
        name = SteamManager.GetSteamName();
        level = 1;
        exp = 0;
        gold = 0;
        iconInDex = 0;
    }
}

public class UserDataManager : ManagerBase<UserDataManager>
{
    UserData userData;

    // Start is called before the first frame update
    void Start()
    {
        if(userData ==  null)
        {
            userData = new UserData();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
