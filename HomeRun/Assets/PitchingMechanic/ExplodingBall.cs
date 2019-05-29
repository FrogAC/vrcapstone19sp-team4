using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBall : MonoBehaviour
{
    private bool bombArmed = false;

    public float armingDelay = 0.5f;
    public float explosionRadius = 3;

    public ParticleSystem explosionFX;
    [Space]
    public LayerMask bombArmingLayers;
    public LayerMask bombExplodingLayers;
    public LayerMask explodableLayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ArmTime());
        if (bombArmed || (LayerMask.GetMask(LayerMask.LayerToName(collision.collider.gameObject.layer)) & bombExplodingLayers) != 0)
        {
            Destroy(this.gameObject);
            //Instantiate(explosionFX, transform.position, transform.rotation);

            Debug.Log("BOOM @ " + transform.position);

            Collider[] explosionTargets = Physics.OverlapSphere(transform.position, explosionRadius, explodableLayers);
            foreach (Collider hitCollider in explosionTargets)
            {
                Debug.Log("hit " + hitCollider.name + " with explosion");
                Destroy(hitCollider.gameObject);
            }
        } else if (!bombArmed && (LayerMask.GetMask(LayerMask.LayerToName(collision.collider.gameObject.layer)) & bombArmingLayers) != 0 )
        {
            Debug.Log("hit by "+collision.collider.name);
            StartCoroutine(ArmTime());
        } 
    }

    IEnumerator ArmTime()
    {
        yield return new WaitForSeconds(armingDelay);
        bombArmed = true;
    }
}
