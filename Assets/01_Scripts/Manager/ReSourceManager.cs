using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSourceManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Sprite> playerIconImageList;
    public Sprite GetPlayerIcon(int index)
    {
        return playerIconImageList[index];
    }
}
