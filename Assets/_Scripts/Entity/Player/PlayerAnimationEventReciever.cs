using Bladiator.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventReciever : MonoBehaviour
{
    [SerializeField] private PlayerController m_Player;

    public void FinishedTurnAround()
    {
        print("finished turnaround");
        m_Player.UnlockMovement();
        m_Player.gameObject.transform.Rotate(0, 180, 0);
        m_Player.GetAnimator().SetBool("changeDirection", false);
    }
}
