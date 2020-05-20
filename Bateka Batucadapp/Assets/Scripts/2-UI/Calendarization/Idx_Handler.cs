#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Idx_Handler : MonoBehaviour
{
    public static Idx_Handler Singleton;

    [SerializeField]
    GameObject idx_prefab = null;

    [SerializeField]
    Sprite idx_selected, idx_unselected = null;

    List<Image> images;

    private void Awake()
    {
        Singleton = this;
        images = new List<Image>();
    }

    public void Initialize(int idxs)
    {
        Image[] present_images = GetComponentsInChildren<Image>();

        for (int x = present_images.Length - 1; x >= 0; x--)
        {
            images.Remove(present_images[x]);
            Destroy(present_images[x].gameObject);
        }            

        for(int x = 0; x < idxs; x++)
            images.Add(Instantiate(idx_prefab, transform).GetComponent<Image>());

        images[0].sprite = idx_selected;
    }

    public void Update_idx(int value)
    {
        for (int x = 0; x < images.Count; x++)
        {
            if (x == value)
                images[x].sprite = idx_selected;
            else
                images[x].sprite = idx_unselected;
        }
    }
}
