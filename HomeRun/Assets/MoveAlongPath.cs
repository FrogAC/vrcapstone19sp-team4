using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
    private Vector3 startPosition;
    [Space]
    public float animLength = 1;
    public float movementScale = 1;
    [Space]
    public AnimationCurve xAnim = AnimationCurve.EaseInOut(0,0,1,1);
    public AnimationCurve yAnim = AnimationCurve.Constant(0,1,0);
    public AnimationCurve zAnim = AnimationCurve.Linear(0,0,1,1);

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        float t = Time.time / animLength;
        float x = xAnim.Evaluate(t);
        float y = yAnim.Evaluate(t);
        float z = zAnim.Evaluate(t);
        transform.localPosition = startPosition + new Vector3(x,y,z) * movementScale;
    }
}
