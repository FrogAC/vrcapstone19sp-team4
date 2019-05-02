using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallSelector : MonoBehaviour
{
    [SerializeField] Selection[] selections = new Selection[0];
    int currentIndex = 0;

    [Space]

    [SerializeField] TextMeshProUGUI ballNameText;
    [SerializeField] Transform spawnPoint;
    GameObject spawnedPrefab = null;
    OVRGrabbable spawnedGrabbable;
    [SerializeField] OVRInput.Button selectNext = OVRInput.Button.DpadLeft;
    [SerializeField] OVRInput.Button selectPrev = OVRInput.Button.DpadRight;


    // Start is called before the first frame update
    void Start()
    {
        UpdateSelection();
    }

    private void Update()
    {
        if (OVRInput.GetDown(selectNext))
        {
            NextSelection();
        }
        else if (OVRInput.GetDown(selectPrev))
        {
            PrevSelection();
        }

        if (spawnedGrabbable != null)
        {
            if (spawnedGrabbable.isGrabbed)
            {
                spawnedPrefab.GetComponent<Rigidbody>().isKinematic = false;
                spawnedPrefab.transform.SetParent(null);

                spawnedPrefab = null;
                spawnedGrabbable = null;
                UpdateBallNameText();
            }
        } 
    }

    void UpdateSelection()
    {
        if (spawnedPrefab != null)
        {
            Destroy(spawnedPrefab.gameObject);
        }

        spawnedPrefab = Instantiate(selections[currentIndex].prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        spawnedGrabbable = spawnedPrefab.GetComponent<OVRGrabbable>();
        spawnedPrefab.GetComponent<Rigidbody>().isKinematic = true;

        UpdateBallNameText();
    }

    void UpdateBallNameText()
    {
        if (ballNameText != null)
        {
            if (spawnedPrefab != null)
            {
                ballNameText.text = selections[currentIndex].name;
            } else
            {
                ballNameText.text = "";
            }
            
        }
    }

    public void NextSelection()
    {
        if (spawnedPrefab != null)
        {
            currentIndex = (currentIndex + 1) % selections.Length;
        }
        UpdateSelection();
    }

    public void PrevSelection()
    {
        if (spawnedPrefab != null)
        {
            currentIndex = ((currentIndex - 1) + selections.Length) % selections.Length;
        }
        UpdateSelection();
    }


    [System.Serializable]
    public class Selection
    {
        public string name;
        public GameObject prefab;
    }
}
