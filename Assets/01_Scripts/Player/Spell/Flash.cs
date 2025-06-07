//using UnityEngine;

//public class Flash : Spell
//{
//    public Flash(IPlayerContext context) : base(context)
//    {
//        this.context = context;
//    }

//    public override void Execute()
//    {
//        Vector3 targetPoint = context.MousePositionGetter.ClickPoint.Value;
//        Vector3 direction = (context.MousePositionGetter.ClickPoint.Value - context.Trf.position).normalized;

//        context.Trf.position = targetPoint;
//        context.Trf.rotation = Quaternion.LookRotation(direction);


//        currentCoolTime = maxCoolTime;
//    }

//    float distance = 3.0f;
//}