using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSourceManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Sprite> playerIconImageList;
    [SerializeField] List<Sprite> roundImageList;
    [SerializeField] List<Sprite> winAndLoseImageList;
    public Sprite GetPlayerIcon(int index) => playerIconImageList[index];

    public Sprite GetRoundImage(int index) => roundImageList[index];
    public Sprite GetResultImage(int index) => winAndLoseImageList[index];
}
