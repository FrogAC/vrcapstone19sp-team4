namespace HomeRun.Net
{
	using Oculus.Platform.Models;

	public class RemotePlayer : Player
	{
		private User m_user;

		public User User
		{
			set { m_user = value; }
		}

		public ulong ID
		{
			get { return m_user.ID; }
		}

		void Start() {
			base.BallSelector = PlatformManager.Instance.RemoteBallSelector;
		}

	}
}
