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
    [SerializeField] Button createRoomBtn; // �游��� �˾� ����
    [SerializeField] Button roomJoinBtn; // �����
    [SerializeField] Button createBtn; // �游���
    [SerializeField] Button cancelBtn; // �游��� ���
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
