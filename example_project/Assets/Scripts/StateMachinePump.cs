using EnemyHideout.StateMachine;
using UnityEngine;

namespace DefaultNamespace
{
  public class StateMachinePump : MonoBehaviour
  {
    void Update()
    {
      StateMachineRunner.Instance.Update();
    }
  }
}