using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyController;

public class EventTrigger : MonoBehaviour
{
    public EnemyController controller;
    public Events eventToExecute;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (eventToExecute)
            {
                case Events.WalkInRoof:
                    controller.PlayAnimWalkInRoof();
                    break;
                case Events.JumpScareImage:
                    controller.PlayJumpScareImage();
                    break;
            }
        }
    }
}