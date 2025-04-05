//using NUnit.Framework;
//using UnityEngine;

//public class PlayerTests
//{
//    [Test]
//    public void Player_Should_Move_When_Input_Received()
//    {
//        // 1. �׽�Ʈ ȯ�� ����
//        var playerObj = new GameObject("Player");
//        var controller = playerObj.AddComponent<PlayerController>();
//        var mockMovement = new MockMovement();

//        // 2. �ʱ�ȭ
//        controller.Initialize(mockMovement);

//        // 3. �׽�Ʈ ����
//        controller.HandleMoveInput(Vector3.forward);

//        // 4. ��� ����
//        Assert.IsTrue(mockMovement.MoveCalled, "�̵� �Է��� ������ �� Move�� ȣ��Ǿ�� �մϴ�");
//    }

//    [Test]
//    public void Player_Should_Not_Move_When_Dead()
//    {
//        // 1. �׽�Ʈ ȯ�� ����
//        var playerObj = new GameObject("Player");
//        var controller = playerObj.AddComponent<PlayerController>();
//        var mockMovement = new MockMovement();

//        // 2. �ʱ�ȭ
//        controller.Initialize(mockMovement);
//        controller.SetDead(true); // �÷��̾ ���� ���·� ����

//        // 3. �׽�Ʈ ����
//        controller.HandleMoveInput(Vector3.forward);

//        // 4. ��� ����
//        Assert.IsFalse(mockMovement.MoveCalled, "���� ���¿����� �̵��� �Ұ����ؾ� �մϴ�");
//    }
//}