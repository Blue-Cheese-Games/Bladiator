using UnityEngine;

namespace Bladiator.Managers
{
	public class IdleUi : MonoBehaviour
	{
		public void ShowFight()
		{
			UiManager.Instance.ShowFight();
		}

		public void AnimationDone()
		{
			UiManager.Instance.AnimationDone();
		}
	}
}