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
    [SerializeField] TMP_InputField roomNameField;
    [SerializeField] Toggle isVisible;
    [SerializeField] Toggle isSecret;
    [SerializeField] Button createRoomBtn; // 방만들기 팝업 열기
    [SerializeField] Button roomJoinBtn; // 방들어가기
    [SerializeField] Button createBtn; // 방만들기
    [SerializeField] Button cancelBtn; // 방만들기 취소
    public RegularGamePop regularGamePop;
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        ServerManager.Instance.roomManager.joinRoomId = null;
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
        ServerManager.Instance.roomManager.CreateRoom(roomNameField.text, isVisible.isOn, passWordField.text);
    }
    public void JoinRoom()
    {
        if(ServerManager.Instance.roomManager.joinRoomId == null)
        {
            Debug.Log("no selected room!");
            return;
        }
        ServerManager.Instance.roomManager.JoinRoom(ServerManager.Instance.roomManager.joinRoomId);
    }
    public void ButtonSwitch(bool on)
    {
        createRoomBtn.gameObject.SetActive(on);
        roomJoinBtn.gameObject.SetActive(on);
        createBtn.gameObject.SetActive(!on);
        cancelBtn.gameObject.SetActive(!on);
    }
}
