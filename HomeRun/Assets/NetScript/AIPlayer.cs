namespace HomeRun.Net
{
	using UnityEngine;
	using System.Collections;

	// Auto pitcher
	public class AIPlayer : Player {
		void Awake() {
			//gameObject.AddComponent<Pitcherwithdifballs>();
		}

		void OnDestroy() {
			//DestroyImmediate(gameObject.GetComponent<Pitcherwithdifballs>());
		}
	}
}
