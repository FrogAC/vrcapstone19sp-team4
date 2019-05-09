namespace HomeRun.Net
{
	using UnityEngine;
	using UnityEngine.UI;
	using Oculus.Platform.Models;

	public class PlayerArea : MonoBehaviour
	{
		// cached gameobject that where the player camera will move to
		private GameObject m_playerHolder;

		// cached Text component where we'll render the player's name
		private Text m_nameText;

		public Player Player
		{
			get { return m_playerHolder.GetComponent<Player>(); }
		}

		public Text NameText
		{
			get { return m_nameText; }
		}

		void Awake()
		{
			m_playerHolder = gameObject.transform.Find("Player Holder").gameObject;
			m_nameText = gameObject.GetComponentsInChildren<Text>()[1];
		}

		public T SetupForPlayer<T>(string name) where T : Player
		{
			var oldplayer = m_playerHolder.GetComponent<Player>();
			if (oldplayer) Destroy(oldplayer);

			var player = m_playerHolder.AddComponent<T>();
			m_nameText.text = name;

			if (player is RemotePlayer)
			{
			}
			else if (player is LocalPlayer)
			{
			}
			else
			{
			}

			return player;
		}
	}
}
