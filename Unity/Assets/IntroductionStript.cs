using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using CVARC.V2;
using System;

public class IntroductionStript : MonoBehaviour
{

    public static Func<IWorld> worldInitializer;

    public static CVARC.V2.Loader loader;
    bool guiIsRunned = false;
    static bool serverIsRunned = false;

    void Start()
    {
        loader = new CVARC.V2.Loader();
        loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());
        // loader.AddLevel("RepairTheStarship", "Level1", () => new RepairTheStarship.Level1());
        //copy here other levels of demo

        //надо запустить тред Server

        if (!serverIsRunned)
        {
            Server();
            serverIsRunned = true;
        }

    }

    void Update()
    {
        Dispatcher.CheckNetworkClient(); // нарм?
    }

    void Server()
    {
        Dispatcher.Start();
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
        //Dispatcher.KillThreads();
    }

    const float
        kMenuWidth = 200.0f, // ширина меню то, куда кнопочки натыканы
        kMenuHeight = 241.0f,
        kMenuHeaderHeight = 26.0f,
        kButtonWidth = 175.0f,
        kButtonHeight = 30.0f;

    public Texture menuBackground, button;
    private Texture background; //то, что будет на заднем фоне
    private int competitionIndex;

    public void OnGUI()
    {
        if (!guiIsRunned)
        {
            EditorGUILayoutEnumPopup.Init();
            guiIsRunned = true;
        }
//        background = new Texture2D(2, 2);
//        Color preColor = GUI.color;
//        if (Event.current.type == EventType.repaint)
//        {
//            GUI.color = new Color(preColor.r, preColor.g, preColor.b, 10);
//            GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), background);
//        }
//        GUI.color = new Color(preColor.r, preColor.g, preColor.b, 10);
//        Rect menuRect = new Rect(
//            (Screen.width - kMenuWidth) * 0.5f,
//            (Screen.height - kMenuHeight) * 0.5f,
//            kMenuWidth,
//            kMenuHeight
//        );
//        GUI.DrawTexture(menuRect, menuBackground);
//
//        var competitions = IntroductionStript.loader.Levels.Keys.ToArray();
//        var competitionsGUI = competitions.Select(x => x.ToString()).ToArray();
//        var levels = IntroductionStript.loader.Levels[competitions[0]].Keys.ToArray();
//        var levelsGUI = levels.Select(x => x.ToString()).ToArray();
//        var comp = IntroductionStript.loader.Levels[competitions[0]][levels[0]]();
//        var modeNames = new[] { "Test", "Tutorial", "BotDemo" };
//        var modesGUI = modeNames.Select(z => z).ToArray();
//        var bots = comp.Logic.Bots.Keys.ToArray();
//        var botsGUI = bots.Select(x => x.ToString()).ToArray();
//
//        GUILayout.BeginArea(menuRect);
////        GUILayout.Space(kMenuHeaderHeight);
//        GUILayout.FlexibleSpace();
//        foreach (string competition in modeNames)
//        {
//            if (MenuButton(button, competition))
//            {
//                Debugger.Log(DebuggerMessageType.Unity,competition);
//                run();
//                //            StartCoroutine(DoRestart());
//            }
//            GUILayout.FlexibleSpace();
//        }
//        GUILayout.EndArea();
//        GUI.color = preColor;
    }

//        GUI.color = Color.red; GUILayout.Box("I'm red");
//        GUI.color = Color.yellow; GUILayout.Box("I'm yellow");
//        GUI.color = new Color(1, 1, 1, 0.5f); GUILayout.Box("I'm translucent");
//        GUI.color = Color.white; GUILayout.Box("I'm normal");

    void run()
    {
        var modeNames = new[] { "Test", "Tutorial", "BotDemo" };
        var runMode = modeNames[1];
        var competitions = IntroductionStript.loader.Levels.Keys.ToArray();
        var levels = IntroductionStript.loader.Levels[competitions[0]].Keys.ToArray();
        LoadingData data = new LoadingData();
        data.AssemblyName = competitions[competitionIndex];
        data.Level = levels[0];
        var factory = IntroductionStript.loader.CreateControllerFactory(runMode);
        SettingsProposal proposal = new SettingsProposal();
//            proposal.Controllers = new System.Collections.Generic.List<ControllerSettings>
//                {
//                    new ControllerSettings {ControllerId = "Left", Type = ControllerType.Bot, Name = bots[leftBot]},
//                    new ControllerSettings {ControllerId = "Right", Type = ControllerType.Bot, Name = bots[rightBot]}
//                };
        Dispatcher.WorldPrepared(() => IntroductionStript.loader.CreateSimpleMode(data, proposal, factory));
        
    }

    bool MenuButton(Texture icon, string text)
    {
        bool wasPressed = false;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect rect = GUILayoutUtility.GetRect(kButtonWidth, kButtonHeight, GUILayout.Width(kButtonWidth), GUILayout.Height(kButtonHeight));

        switch (Event.current.type)
        {
            case EventType.MouseUp:
                if (rect.Contains(Event.current.mousePosition))
                {
                    wasPressed = true;
                }
                break;
            case EventType.Repaint:
//                GUI.DrawTexture(rect, icon);
                GUI.TextField(rect, text);
                break;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        return wasPressed;
    }
}
internal class EditorGUILayoutEnumPopup : EditorWindow
{
    public static void Init()
    {
        var window = GetWindow(typeof(EditorGUILayoutEnumPopup));
        window.Show();
    }

    int competitionIndex = 0;
    int levelIndex = 0;
    int botIndex = 0;
    int controllerIndex = 0;
    Font buttonFont;
    float buttonMinHeight = 60f;

    int leftBot = 0;
    int rightBot = 0;

    const string Tutorial = "Tutorial";
    const string BotDemo = "BotDemo";
    const string Test = "Test";
    private const string AllTests = "AllTests";


    public void OnGUI()
    {
        var competitions = IntroductionStript.loader.Levels.Keys.ToArray();
        var competitionsGUI = competitions.Select(x => new GUIContent(x.ToString())).ToArray();
        competitionIndex = EditorGUILayout.Popup(new GUIContent("Choose competition:"), competitionIndex, competitionsGUI);

        var levels = IntroductionStript.loader.Levels[competitions[competitionIndex]].Keys.ToArray();
        var levelsGUI = levels.Select(x => new GUIContent(x.ToString())).ToArray();
        levelIndex = EditorGUILayout.Popup(new GUIContent("Choose level:"), levelIndex, levelsGUI);

        var comp = IntroductionStript.loader.Levels[competitions[competitionIndex]][levels[levelIndex]]();

        var modeNames = new[] { AllTests, Test, Tutorial, BotDemo };
        var modesGui = modeNames.Select(z => new GUIContent(z)).ToArray();
        controllerIndex = EditorGUILayout.Popup(new GUIContent("Choose mode:"), controllerIndex, modesGui);
        var runMode = modeNames[controllerIndex];

        var bots = comp.Logic.Bots.Keys.ToArray();
        var botsGUI = bots.Select(x => new GUIContent(x.ToString())).ToArray();
        if (runMode == BotDemo)
        {
            leftBot = EditorGUILayout.Popup(new GUIContent("Choose left controller:"), leftBot, botsGUI);
            rightBot = EditorGUILayout.Popup(new GUIContent("Choose right controller:"), rightBot, botsGUI);
        }
        else
            botIndex = EditorGUILayout.Popup(new GUIContent("Choose bot:"), botIndex, botsGUI);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = buttonFont;
        buttonStyle.margin = new RectOffset(20, 20, 3, 3);
        if (GUILayout.Button("Start", buttonStyle, GUILayout.MinHeight(buttonMinHeight)))
        {
            LoadingData data = new LoadingData();

            data.AssemblyName = competitions[competitionIndex];
            data.Level = levels[levelIndex];

            if (runMode == Tutorial && runMode == BotDemo)
            {
                Debugger.Log(DebuggerMessageType.Unity,"Non test starting");
                var factory = IntroductionStript.loader.CreateControllerFactory(runMode);


                SettingsProposal proposal = new SettingsProposal();
                if (runMode == "BotDemo")
                    proposal.Controllers = new System.Collections.Generic.List<ControllerSettings>
                {
                    new ControllerSettings {ControllerId = "Left", Type = ControllerType.Bot, Name = bots[leftBot]},
                    new ControllerSettings {ControllerId = "Right", Type = ControllerType.Bot, Name = bots[rightBot]}
                };

                //            Debugger.Log(DebuggerMessageType.Unity,"Ok");
                this.Close();
                Dispatcher.WorldPrepared(() => IntroductionStript.loader.CreateSimpleMode(data, proposal, factory));

            }
            else if (runMode == AllTests) // запуск всех тестов
            {
                Debugger.Log(DebuggerMessageType.Unity, "Tests starting");
                //
                Dispatcher.RunAllTests(data);
            }
            else
            {
                var testName = "Movement_Round_Forward";
                Dispatcher.RunOneTest(data,testName);
                   
            }

        }
    }

}
