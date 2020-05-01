using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Idx_Handler : MonoBehaviour
{
    public static Idx_Handler Singleton;
    [SerializeField]
    GameObject idx_prefab;

    List<Image> images;

    Color color_default;
    Color color_selected;

    int current_idx = 0;

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

        color_default = images[0].color;
        color_selected = color_default * 0.7f;

        images[0].color = color_selected;
    }

    public void Update_idx(int value)
    {
        for (int x = 0; x < images.Count; x++)
        {
            if (x == value)
                images[x].color = color_selected;
            else
                images[x].color = color_default;
        }
    }
}
