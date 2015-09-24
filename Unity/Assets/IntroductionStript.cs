using System.Linq;
using UnityEngine;
using CVARC.V2;
using System;
using System.Collections.Generic;

public class IntroductionStript : MonoBehaviour
{
    const string ASSEMBLY_NAME = "RoboMovies";
    private bool openWindowTests = false;
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
        //if (GUI.Button(new Rect(Screen.width - 150, Screen.height - 150, 80, 30), "Tests"))
        //    openWindowTests = !openWindowTests;
        openWindowTests = true;
        if (openWindowTests)
            GUI.Window(
                0,
                new Rect(
                    (Screen.width - kMenuWidth) * 0.5f,
                    (Screen.height - kMenuHeight) * 0.5f,
                    kMenuWidth,
                    kMenuHeight),
                TestsWindow,
                "Tests");
    }

    const string HardcodedTest = "Movement_Round_Square";

    public static Color GetTestColor(string test)
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
    bool folderIsLoad = false;
    Folder folder;

    
    void TestsWindow(int windowID)
    {
        background = new Texture2D(2, 2);
        Color preColor = GUI.color;
        if (Event.current.type == EventType.repaint)
        {
            GUI.color = new Color(preColor.r, preColor.g, preColor.b, 10);
            //GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), background);
        }
        GUI.color = new Color(preColor.r, preColor.g, preColor.b, 10);
        Rect menuRect = new Rect(
            10,
            25,
            kMenuWidth - 20,
            kMenuHeight - 35
        );

        //GUI.DrawTexture(menuRect, menuBackground);

        var tests = Dispatcher.loader.Levels["Demo"]["Test"]().Logic.Tests.Keys;
        LoadingData data = new LoadingData();
        data.AssemblyName = "Demo";
        data.Level = "Test";

        if (!folderIsLoad)
        {
            folder = new Folder(ASSEMBLY_NAME);

            foreach (var test in tests)
            {
                var names = test.Split('_');
                Folder last = folder;
                for (int i = 0; i < names.Length - 1; i++)
                {
                    var f = last.Contains(names[i]);
                    if (f == null)
                    {
                        var newFolder = new Folder(names[i]);
                        last.Files.Add(newFolder);
                        last = newFolder;
                    }
                    else
                        last = (Folder)f;
                }
                if (PlayerPrefs.HasKey(test))
                {
                    Dispatcher.LastTestExecution[test] = PlayerPrefs.GetInt(test) != 1;
                }
                last.Files.Add(test);
            }
            folderIsLoad = true;
        }

        GUILayout.BeginArea(menuRect);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        MenuButton(button, "Tests", Color.white, () => { isPressedTests = !isPressedTests; });
        GUILayout.Space(10);
        MenuButton(button, "Hardcoded: " + HardcodedTest, GetTestColor(HardcodedTest), () => Dispatcher.RunOneTest(data, HardcodedTest));

        GUI.color = preColor;
        GUILayout.EndVertical();

        GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.MinWidth(kMenuWidth / 2) });
        if (isPressedTests)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MenuButton(button, "Run all tests", Color.white, () => Dispatcher.RunAllTests(data));
            GUILayout.Space(20);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            scrollViewVector = GUILayout.BeginScrollView(scrollViewVector, false, true);

            folder.Show(button, data, 0);
            GUILayout.EndScrollView();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public static void MenuButton(Texture icon, string text, Color color, Action pressAction)  // +Color color, Action pressAction
    {
        //GUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();

        Rect rect = GUILayoutUtility.GetRect(kButtonWidth, kButtonHeight, GUILayout.Width(kButtonWidth), GUILayout.Height(kButtonHeight));
        //var rect = GUILayoutUtility.GetRect(160, 30);
        switch (Event.current.type)
        {
            case EventType.MouseUp:
                if (rect.Contains(Event.current.mousePosition))
                {
                    pressAction();
                }
                break;
            case EventType.Repaint:
                //                GUI.DrawTexture(rect, icon);
                var col = GUI.color;
                GUI.color = color;
                GUI.TextField(rect, text);
                GUI.color = col;
                break;
        }

        //GUILayout.FlexibleSpace();
       // GUILayout.EndHorizontal();
    }

    class Folder
    {
        public bool IsOpen;
        public string Name;
        public List<object> Files;

        public Folder(string name)
        {
            Name = name;
            Files = new List<object>();
        }
        public void Show(Texture button, LoadingData data, float shift)
        {
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(shift));
            MenuButton(button,
                       " ‣ " + Name,
                       Color.white,
                       () => IsOpen = !IsOpen);
            GUILayout.EndHorizontal();

            if (IsOpen)
            {

                foreach (var test in Files)
                {
                    if (test is Folder)
                        ((Folder)test).Show(button, data, shift + 10);
                    else
                    {
                        GUILayout.BeginHorizontal();
                            GUILayout.Label("", GUILayout.Width(shift + 10));
                            MenuButton(button, ((string)test).Split('_').Last(), GetTestColor((string)test), () => { 
                                Dispatcher.RunOneTest(data, (string)test);
                                PlayerPrefs.SetInt((string)test, (GetTestColor((string)test) == Color.green) ? 1 : 0);
                            });
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        public object Contains(string name)
        {
            foreach (var e in Files)
            {
                if (name == ((Folder)e).Name)
                    return e;
            }
            return null;
        }
    }
}
