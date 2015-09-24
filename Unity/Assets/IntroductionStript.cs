using System.Linq;
using UnityEngine;
using CVARC.V2;
using System;

public class IntroductionStript : MonoBehaviour
{
    const string ASSEMBLY_NAME = "RoboMovies";

    static bool serverIsRunned = false;

    void Start()
    {
        if (!serverIsRunned)
        {
            Server();
            serverIsRunned = true;
        }

    }

    void Update()
    {
        Dispatcher.CheckNetworkClient();
    }

    void Server()
    {
        Dispatcher.Start();
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }

    const float
        kMenuWidth = 400.0f, // ширина меню то, куда кнопочки натыканы
        kMenuHeight = 241.0f,
        kMenuHeaderHeight = 26.0f,
        kButtonWidth = 175.0f,
        kButtonHeight = 30.0f;

    public Texture menuBackground, button;
    private Texture background; //то, что будет на заднем фоне
    private int competitionIndex;
    private bool isPressedTests = false;

    private Vector2 scrollViewVector = Vector2.zero;

    public void OnGUI()
    {
        background = new Texture2D(2, 2);
        Color preColor = GUI.color;
        if (Event.current.type == EventType.repaint)
        {
            GUI.color = new Color(preColor.r, preColor.g, preColor.b, 10);
            GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), background);
        }
        GUI.color = new Color(preColor.r, preColor.g, preColor.b, 10);
        Rect menuRect = new Rect(
            (Screen.width - kMenuWidth) * 0.5f,
            (Screen.height - kMenuHeight) * 0.5f,
            kMenuWidth,
            kMenuHeight
        );
       
        GUI.DrawTexture(menuRect, menuBackground);

        var tests = Dispatcher.loader.Levels[ASSEMBLY_NAME]["Test"]().Logic.Tests.Keys.OrderBy(x => x).ToArray();
        LoadingData data = new LoadingData();
        data.AssemblyName = ASSEMBLY_NAME;
        data.Level = "Test";

        GUILayout.BeginArea(menuRect);
        GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            MenuButton(button, "Tests", Color.black, () => { isPressedTests = !isPressedTests; });
            MenuButton(button, "Hardcoded: " + HardcodedTest, GetTestColor(HardcodedTest), () => Dispatcher.RunOneTest(data, HardcodedTest));
               
            GUI.color = preColor;
            GUILayout.EndVertical();

            GUILayout.BeginVertical(new GUILayoutOption[]{GUILayout.MinWidth(kMenuWidth / 2)});
            if (isPressedTests)
            {
                GUILayout.FlexibleSpace();
                MenuButton(button, "Run all tests", Color.black, () => Dispatcher.RunAllTests(data));
                GUILayout.FlexibleSpace();

                scrollViewVector = GUILayout.BeginScrollView(scrollViewVector, false, true);
                foreach (string test in tests)
                {
                    

                    MenuButton(button, test, GetTestColor(test), () => Dispatcher.RunOneTest(data, test));
                }
                GUILayout.EndScrollView(); 
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    const string HardcodedTest = "Movement_Round_Square";
    
    Color GetTestColor(string test)
    {
        Color color;
        if (!Dispatcher.LastTestExecution.ContainsKey(test))
            color = Color.grey;
        else if (Dispatcher.LastTestExecution[test])
            color = Color.green;
        else
            color = Color.red;
        return color;
    }

    void MenuButton(Texture icon, string text, Color color, Action pressAction)  // +Color color, Action pressAction
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect rect = GUILayoutUtility.GetRect(kButtonWidth, kButtonHeight, GUILayout.Width(kButtonWidth), GUILayout.Height(kButtonHeight));

        switch (Event.current.type)
        {
            case EventType.MouseUp:
                if (rect.Contains(Event.current.mousePosition))
                {
                    pressAction();
                }
                break;
            case EventType.Repaint:
                //GUI.DrawTexture(rect, icon);
                var col = GUI.color;
                GUI.color = color;
                GUI.TextField(rect, text);
                GUI.color = col;
                break;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
