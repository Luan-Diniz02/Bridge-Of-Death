using System;
using TMPro;
using UnityEngine;

public class DistanceView : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TextMeshProUGUI distanceText;
    private float startZPosition;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null || distanceText == null)
        {
            Debug.LogError("DistanceView: Player Transform or Distance Text is not assigned.");
        }

        startZPosition = playerTransform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceTravelled = playerTransform.position.z - startZPosition;
        distanceTravelled = Mathf.Abs(distanceTravelled);
        distanceText.text = $"Distance: {distanceTravelled:F2} m";
    }


}
