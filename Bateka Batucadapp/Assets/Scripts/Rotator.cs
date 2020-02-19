using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField]
    float speed = 360;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.Rotate(new Vector3(0, 0, -Time.deltaTime * speed));
    }
}
