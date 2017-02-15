using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class WindowToggler : MonoBehaviour
{

    public UnityUIQuestLogWindow questLogWindow; // Assign in inspector
    private bool questLogIsShowing;

    // Use this for initialization
    void Start()
    {
        questLogIsShowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            questLogIsShowing = !questLogIsShowing;
            if (questLogIsShowing)
            {
                questLogWindow.Open();
            }
            else
            {
                questLogWindow.Close();
            }
        }

    }
}
