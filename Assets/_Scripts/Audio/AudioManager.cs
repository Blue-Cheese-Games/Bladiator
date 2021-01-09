using System.Collections.Generic;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Sound
{
	public class AudioManager : MonoBehaviour
	{
		[SerializeField] private Dictionary<string, AudioData> m_Sfx = new Dictionary<string, AudioData>();
		[SerializeField] private AudioData m_Idle, m_Main, m_Ending;
		[SerializeField] private float m_FadeSpeed = 5;

		private AudioData m_Current, m_CrossFade;
		private Queue<AudioData> m_CrossfadeQueue = new Queue<AudioData>();

		private GameState m_LastState = GameState.Ending; // Ending is unused

		// Start is called before the first frame update
		void Start()
		{
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
		}

		private void OnGameStateChange(GameState obj)
		{
			switch (obj)
			{
				case GameState.Idle:
					if (m_LastState == GameState.Fighting)
					{
						m_CrossfadeQueue.Enqueue(m_Ending);
						m_CrossfadeQueue.Enqueue(m_Idle);
					}
					else if (m_LastState == GameState.Pause)
					{
						m_Idle.source.volume = 1;
					}
					else
					{
						m_CrossfadeQueue.Enqueue(m_Idle);
					}

					break;

				case GameState.Animating:
					if (m_LastState == GameState.Idle)
					{
						m_CrossfadeQueue.Enqueue(m_Main);
					}
					break;

				case GameState.Fighting:
					if (m_LastState == GameState.Pause)
					{
						m_Current.source.volume = 1;
					}
					break;

				case GameState.Pause:
					if (m_Current == null) return;

					m_Current.source.volume = 0.25f;
					break;
			}

			m_LastState = obj;
		}

		private void Update()
		{
			if (m_CrossfadeQueue.Count > 0 || m_CrossFade != null)
			{
				if (m_CrossFade == null)
				{
					if (m_Current != null && m_Current.source == m_Ending.source)
					{
						if (m_Current.source.isPlaying && m_Current.source.time < 3) return;
					}
					
					m_CrossFade = m_CrossfadeQueue.Dequeue();

					m_CrossFade.source.volume = 0;

					if (!m_CrossFade.source.isPlaying)
						m_CrossFade.source.Play();
				}

				if (Mathf.Abs(m_CrossFade.source.volume) < 1)
				{
					m_CrossFade.source.volume += Time.deltaTime * m_FadeSpeed;

					if (m_Current != null)
						m_Current.source.volume -= Time.deltaTime * m_FadeSpeed;
				}
				else
				{
					if (m_Current != null)
					{
						if (m_Current.source == m_Main.source)
						{
							m_Current.source.Pause();
						}
						else
						{
							m_Current.source.Stop();
						}
					}

					m_Current = m_CrossFade;
					m_CrossFade = null;
				}
			}
		}
	}
}