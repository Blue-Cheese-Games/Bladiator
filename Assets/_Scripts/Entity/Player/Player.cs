using Bladiator.Entities;
using Bladiator.Entity.Player;
using Bladiator.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : EntityBase
{
    private PlayerController m_PlayerController;

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.AddPlayer(this);
    }

    public override void Move()
    {
        // Movement.
    }
}
