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
<<<<<<< HEAD
		int m_id = -1;
		private BallType m_ballType = BallType.FastBall;
=======
		private BallType ballType = BallType.FastBall;
		
		// the last time this ball locally collided with something
		private float lastCollisionTime;
>>>>>>> parent of cf9859d... Backuo Branch & Clean Up net api

		// cached reference to the GameObject's Rigidbody component
		private Rigidbody m_rigidBody;
		private ThrownBall m_tb;

		void Awake()
		{
			m_rigidBody = gameObject.GetComponent<Rigidbody>();
			m_tb = gameObject.GetComponent<ThrownBall>();
		}

		public ThrownBall ThrowBall {
			get { return m_tb; }
			set { m_tb = value; }
		}

<<<<<<< HEAD
		public BallType BallType {
			get { return m_ballType; }
=======
		public bool IsHeld()
		{
			return !rigidBody.useGravity;
>>>>>>> parent of cf9859d... Backuo Branch & Clean Up net api
		}

		public BallType BallType {
			get { return ballType; }
		}

		public P2PNetworkBall SetType(BallType t) {
			m_ballType = t;
			return this;
		}

<<<<<<< HEAD
		public P2PNetworkBall SetInstanceID(int id) {
			m_id = id;
			return this;
		}

		public void ProcessBallThrow(Vector3 pos, Vector3 vel) {
			m_tb.transform.position = pos;
			m_tb.GrabEnd(vel, Vector3.zero);
		}

		public void ProcessBallHit(Vector3 pos, Vector3 vel) {
			m_tb.transform.position = pos;
			Debug.Log("Ball Hit Velocity:" + vel);
			m_rigidBody.velocity = vel;
			NetEffectController.Instance.PlayBatHitEffect(pos);
=======
		public void ProcessRemoteUpdate(float remoteTime, bool isHeld, Vector3 pos, Vector3 vel)
		{
			if (isHeld)
			{
				transform.position = pos;
			}
			// TODO
			// if we've collided since the update was sent, our state is going to be more accurate so
			// it's better to ignore the update
			else if (lastCollisionTime < remoteTime)
			{
				Debug.Log("SHOULDNT SEE THIS UPDATE!");
				// To correct the position this sample directly moves the ball.
				// Another approach would be to gradually lerp the ball there during
				// FixedUpdate.  However, that approach aggravates any errors that
				// come from estimatePosition and estimateVelocity so the lerp
				// should be done over few timesteps.
				float deltaT = Time.realtimeSinceStartup - remoteTime;
				transform.localPosition = estimatePosition(pos, vel, deltaT);
				rigidBody.velocity = estimateVelocity(vel, deltaT);

				// if the ball is transitioning from held to ballistic, we need to
				// update the RigidBody parameters
				if (IsHeld())
				{
					rigidBody.useGravity = true;
					rigidBody.detectCollisions = true;
				}
			}
		}

		// Estimates the new position assuming simple ballistic motion.
		private Vector3 estimatePosition(Vector3 startPosition, Vector3 startVelocty, float time)
		{
			return startPosition + startVelocty * time + 0.5f * Physics.gravity * time * time;
		}

		// Estimates the new velocity assuming ballistic motion and drag.
		private Vector3 estimateVelocity(Vector3 startVelocity, float time)
		{
			return startVelocity + Physics.gravity * time * Mathf.Clamp01 (1 - rigidBody.drag * time);
		}

		void OnCollisionEnter(Collision collision)
		{
			lastCollisionTime = Time.realtimeSinceStartup;
>>>>>>> parent of cf9859d... Backuo Branch & Clean Up net api
		}

		void OnDestroy()
		{
			PlatformManager.P2P.RemoveNetworkBall(gameObject);
		}

	}
}
