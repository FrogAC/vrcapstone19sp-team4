namespace HomeRun.Net
{
	using UnityEngine;
	using UnityEngine.UI;
	using Oculus.Platform.Models;

	public class PlayerArea : MonoBehaviour
	{
		[SerializeField] private PlayerType playerType = PlayerType.Batter;

		// cached gameobject that where the player camera will move to
		private Transform m_playerHolder;
		public Transform PlayerHolder {
			get { return m_playerHolder; }
		}

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
			m_playerHolder = gameObject.transform.Find("Player Holder");
			m_nameText = gameObject.GetComponentInChildren<Text>();
		}

		public T SetupForPlayer<T>(string name) where T : Player
		{
			var oldplayer = m_playerHolder.GetComponent<Player>();
			if (oldplayer) Destroy(oldplayer);

			var player = m_playerHolder.gameObject.AddComponent<T>();
			m_nameText.text = name;

			Debug.Log("Setup PlayerArea for " + ((player is LocalPlayer) ? "LocalPlayer" : "RemotePlayer"));

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
