using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bladiator.CameraController
{
	public class CamShake : MonoBehaviour
	{
		public static CamShake Instance;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		#if UNITY_EDITOR
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				ShakeCamera(.25f, 0.09f);
			}
		}
		#endif
		
		public void ShakeCamera(float dur, float mag)
		{
			StartCoroutine(Shake(dur, mag));
		}

		//TODO Needs some smoothing
		private IEnumerator Shake(float dur, float mag)
		{
			Vector3 orgPos = transform.position;

			float elapsed = 0f;

			while (elapsed < dur)
			{
				float x = Random.Range(-1f, 1f) * mag;
				float y = Random.Range(-1f, 1f) * mag;

				transform.position = new Vector3(x, y, orgPos.z);

				elapsed += Time.deltaTime;

				yield return null;
			}

			transform.position = orgPos;
		}
	}
}