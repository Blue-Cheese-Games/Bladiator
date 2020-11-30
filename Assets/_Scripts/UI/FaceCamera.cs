using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.UI
{
	public class FaceCamera : MonoBehaviour
	{
		void Update()
		{
			if (Camera.main == null) return;

			gameObject.transform.forward = -Camera.main.transform.forward;
		}
	}
}