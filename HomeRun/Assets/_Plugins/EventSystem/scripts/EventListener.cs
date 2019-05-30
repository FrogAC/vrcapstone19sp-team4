using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventListener: MonoBehaviour {

	public GameEvent Event;
	public UnityEvent Response;

	//register and deregister by enabling
	void OnEnable () {Event.Register (this);}
	void OnDisable () {Event.DeRegister (this);}

	public void OnEventRaised(){Response.Invoke();Debug.Log (this.name);}
		
}
