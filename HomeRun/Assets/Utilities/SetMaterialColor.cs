using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialColor : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] float colorInfluence = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.color = Color.Lerp(mat.color, color, colorInfluence);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
