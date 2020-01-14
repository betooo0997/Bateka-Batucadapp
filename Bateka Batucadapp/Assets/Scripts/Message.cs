﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    Image image;
    Text text;

    static string content;

    float alpha = 1;
    float alpha_rate = 1f;

    float timer = 0;
    float timer_limit = 2;

    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        text.text = content;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timer_limit)
        {
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, color.a - alpha_rate * Time.deltaTime);

            if (image.color.a <= 0)
            {
                image.color = new Color(color.r, color.g, color.b, 0);
                SceneManager.UnloadSceneAsync("Message");
            }
        }
    }

    public static void ShowMessage(string message)
    {
        content = message;
        Load_Scene.Load_Scene_ST("Message");
    }
}