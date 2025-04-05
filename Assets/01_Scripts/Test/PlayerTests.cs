//using NUnit.Framework;
//using UnityEngine;

//public class PlayerTests
//{
//    [Test]
//    public void Player_Should_Move_When_Input_Received()
//    {
//        // 1. 테스트 환경 설정
//        var playerObj = new GameObject("Player");
//        var controller = playerObj.AddComponent<PlayerController>();
//        var mockMovement = new MockMovement();

//        // 2. 초기화
//        controller.Initialize(mockMovement);

//        // 3. 테스트 실행
//        controller.HandleMoveInput(Vector3.forward);

//        // 4. 결과 검증
//        Assert.IsTrue(mockMovement.MoveCalled, "이동 입력이 들어왔을 때 Move가 호출되어야 합니다");
//    }

//    [Test]
//    public void Player_Should_Not_Move_When_Dead()
//    {
//        // 1. 테스트 환경 설정
//        var playerObj = new GameObject("Player");
//        var controller = playerObj.AddComponent<PlayerController>();
//        var mockMovement = new MockMovement();

//        // 2. 초기화
//        controller.Initialize(mockMovement);
//        controller.SetDead(true); // 플레이어를 죽은 상태로 설정

//        // 3. 테스트 실행
//        controller.HandleMoveInput(Vector3.forward);

//        // 4. 결과 검증
//        Assert.IsFalse(mockMovement.MoveCalled, "죽은 상태에서는 이동이 불가능해야 합니다");
//    }
//}