using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Copy_Type_Insatnces_Y : MonoBehaviour
{
    [SerializeField]
    RectTransform type_instances_content;

    void LateUpdate()
    {
        transform.localPosition = new Vector2(transform.localPosition.x, type_instances_content.localPosition.y);
    }
}
