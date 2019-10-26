//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class State 
//{

//    public virtual void OnEnter() { }
//    public virtual void OnUpdate() { }
//    public virtual void OnExit() { }
//}

//public class StateMachine : State
//{
//    protected List<Transitions> transitions = new List<Transitions>(4);
//    protected State currentState;
//}

//public class JumpState : State
//{
//    protected override void OnUpdate()
//    {
//        base.OnUpdate();

//        //Do Things
//    }
//}

//public class IdleState : State
//{
//    protected override void OnUpdate()
//    {
//        base.OnUpdate();

//        // Don't do things
//    }
//}

//public class HumanMovementStateMachine : StateMachine
//{
//    protected override void OnEnter()
//    {
//        base.OnEnter();

//        // Declare Transtitions
//    }

//    public override void OnUpdate()
//    {
//        base.OnUpdate();

//        currentState.OnUpdate();
//    }
//}