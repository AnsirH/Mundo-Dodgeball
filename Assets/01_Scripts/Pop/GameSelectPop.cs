using System;
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
    [SerializeField] GameObject passWorWindow;
    [SerializeField] TMP_InputField passWordField;
    [SerializeField] TMP_InputField roomNameField;
    [SerializeField] Toggle isVisible;
    [SerializeField] Toggle isSecret;
    [SerializeField] Button createRoomBtn; // �游��� �˾� ����
    [SerializeField] Button roomJoinBtn; // �����
    [SerializeField] Button createBtn; // �游���
    [SerializeField] Button cancelBtn; // �游��� ���
    [SerializeField] TMP_InputField EnterPassWord; // �����, �Է�â

    public event Action<string, string, string> OnCreateRoomRequested;

    public RegularGamePop regularGamePop;
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        ServerManager.Instance.roomManager.joinRoom = null;
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
        OnCreateRoomRequested?.Invoke(roomNameField.text, passWordField.text, "GeneralGameMode");
    }
    public void JoinRoom()
    {
        if(ServerManager.Instance.roomManager.joinRoom == null)
        {
            Debug.Log("no selected room!");
            return;
        }
        OnCreateRoomRequested?.Invoke(roomNameField.text, "", "GeneralGameMode");
    }
    public void PassWordJoinRoom()
    {
        OnCreateRoomRequested?.Invoke(roomNameField.text, EnterPassWord.text, "GeneralGameMode");
        EnterPassWord.text = null;
    }
    public void ButtonSwitch(bool on)
    {
        createRoomBtn.gameObject.SetActive(on);
        roomJoinBtn.gameObject.SetActive(on);
        createBtn.gameObject.SetActive(!on);
        cancelBtn.gameObject.SetActive(!on);
    }
    public void SetPasswordWindow()
    {
        DetailOpen(passWorWindow);
    }
}
