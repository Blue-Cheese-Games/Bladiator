using UnityEngine;

namespace Bladiator.UI
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