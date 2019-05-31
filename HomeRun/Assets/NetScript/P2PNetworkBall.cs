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
		private BallType m_ballType = BallType.FastBall;

		// cached reference to the GameObject's Rigidbody component
		private Rigidbody m_rigidBody;
		private ThrownBall m_throwball;

		void Awake()
		{
			m_rigidBody = gameObject.GetComponent<Rigidbody>();
			m_throwball = gameObject.GetComponent<ThrownBall>();
		}

		public ThrownBall ThrowBall {
			get { return m_throwball; }
			set { m_throwball = value; }
		}

		public BallType BallType {
			get { return m_ballType; }
		}

		public int InstanceID {
			get { return m_id; }
		}

		public P2PNetworkBall SetType(BallType t) {
			m_ballType = t;
			return this;
		}

		public P2PNetworkBall SetInstanceID(int id) {
			m_id = id;
			return this;
		}

		public void ProcessBallThrow(Vector3 pos, Vector3 vel, Vector3 strikePos) {
			transform.SetParent(null, true);  // no distort
			m_rigidBody.isKinematic = false;
            m_rigidBody.useGravity = false;
			m_throwball.transform.position = pos;
			NetStrikeZone.strikezone.transform.position = strikePos;

			m_throwball.GrabEnd(vel, Vector3.zero);
		}

		public void ProcessBallHit(Vector3 pos, Vector3 vel) {
			Debug.Log("Hit!" + vel);
			// stop ball animation
            m_throwball.StopThrow();
			m_rigidBody.isKinematic = false;
            m_rigidBody.useGravity = true;

			//m_rigidBody.isKinematic = true;
			m_rigidBody.angularVelocity = Vector3.zero;
			m_throwball.transform.position = pos;
			m_rigidBody.velocity = vel;
			m_rigidBody.constraints = RigidbodyConstraints.None;

			NetEffectController.Instance.PlayBatHitEffect(pos, m_ballType);
			
            StartCoroutine(EnableSelect(3));
			Destroy(this, 3.0f);
		}

		IEnumerator EnableSelect(int s) {
			yield return new WaitForSeconds(s);
			GlobalSettings.Selectable = true;
		}

		void OnDestroy()
		{
			// called on ThrownBall
			PlatformManager.P2P.RemoveNetworkBall(gameObject);
		}

	}
}
