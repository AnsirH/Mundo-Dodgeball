using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectPop : PopBase
{
    [SerializeField] GameObject roomCreatePopObj;
    [SerializeField] GameObject passWordFieldObj;
    [SerializeField] TMP_InputField passWordField;
    [SerializeField] Toggle isVisible;
    [SerializeField] Toggle isSecret;
    [SerializeField] Button createRoomBtn; // 방만들기 팝업 열기
    [SerializeField] Button roomJoinBtn; // 방들어가기
    [SerializeField] Button createBtn; // 방만들기
    [SerializeField] Button cancelBtn; // 방만들기 취소
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }
    public override void DetailOpen(GameObject g)
    {
        base.DetailOpen(g);
    }
    public override void DetailClose(GameObject g) 
    {
        base.DetailClose(g);
    }
    public void ActivePassWord()
    {
        bool on = isSecret.isOn;
        passWordFieldObj.SetActive(on);
    }
    public void CreateRoom()
    {

    }
    public void ButtonSwitch(bool on)
    {
        createRoomBtn.gameObject.SetActive(on);
        roomJoinBtn.gameObject.SetActive(on);
        createBtn.gameObject.SetActive(!on);
        cancelBtn.gameObject.SetActive(!on);
    }
}
