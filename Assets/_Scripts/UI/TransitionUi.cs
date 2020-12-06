using System;
using System.Collections;
using System.Collections.Generic;
using Bladiator.Managers;
using UnityEngine;

public class TransitionUi : MonoBehaviour
{
	public void OnTransitionBeginDone()
	{
		GameManager.Instance.ResetEvent?.Invoke();
	}
}
