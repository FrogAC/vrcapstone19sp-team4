namespace HomeRun.Net
{
	using UnityEngine;
	using System.Collections;
	using HomeRun.Game;

	// This component handles network coordination for moving balls.
	// Synchronizing moving objects that are under the influence of physics
	// and other forces is somewhat of an art and this example only scratches
	// the surface.  Ultimately how you synchronize will depend on the requirements
	// of your application and its tolerance for users seeing slightly different
	// versions of the simulation.
	public class P2PNetworkBall : MonoBehaviour
	{
		int m_id = -1;
		private BallType ballType = BallType.FastBall;

		// cached reference to the GameObject's Rigidbody component
		private Rigidbody rigidBody;

		void Awake()
		{
			rigidBody = gameObject.GetComponent<Rigidbody>();
		}

		public Vector3 velocity
		{
			get { return rigidBody.velocity; }
		}

		public BallType BallType {
			get { return ballType; }
		}

		public int InstanceID {
			get { return m_id; }
		}

		public P2PNetworkBall SetType(BallType t) {
			ballType = t;
			return this;
		}

		public P2PNetworkBall SetInstanceID(int id) {
			m_id = id;
			return this;
		}

		public void ProcessRemoteUpdate(float remoteTime, bool isHeld, Vector3 pos, Vector3 vel)
		{
				transform.position = pos;
		}

		public void ProcessBallThrow(Vector3 pos, Vector3 vel) {

		}

		public void ProcessBallSpawn(Vector3 pos, Vector3 vel) {

		}

		void OnDestroy()
		{
			// called on ThrownBall
			PlatformManager.P2P.RemoveNetworkBall(gameObject);
		}

	}
}
