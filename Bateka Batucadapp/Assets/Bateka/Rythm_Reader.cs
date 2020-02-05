using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rythm_Reader : MonoBehaviour
{
    void Update()
    {
        transform.localPosition += new Vector3(Time.deltaTime * 15, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Sound");
        collision.gameObject.GetComponent<Sound_Collider>().Sound.source.Play();
    }
}
