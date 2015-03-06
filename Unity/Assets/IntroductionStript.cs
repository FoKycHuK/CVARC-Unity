using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using CVARC.V2;
using System;

public class IntroductionStript : MonoBehaviour {

    public static Func<IWorld> worldInitializer;

    public static CVARC.V2.Loader loader;
    bool guiIsRunned = false;
    static bool serverIsRunned = false;

    void Start()
    {
        loader = new CVARC.V2.Loader ();
        loader.AddLevel ("Demo", "Test", () => new DemoCompetitions.Level1());
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

    public void OnGUI()
    {
        if (!guiIsRunned)
        {
            EditorGUILayoutEnumPopup.Init();
            guiIsRunned = true;
        }
//        GUILayout.BeginArea (new Rect (0, 0, Screen.height, Screen.width), GUI.skin.textArea);
//        {
//            GUIStyle style = new GUIStyle (GUI.skin.button);
//            style.margin = new RectOffset (50, 50, 50, 50);
//            style.font = new Font ();
//            bool result=GUILayout.Button(new GUIContent("Create"), style,GUILayout.MinHeight(60));
//            if (result)
//            {
//                IRunMode mode = RunModeFactory.Create(RunModes.BotDemo); // <-- выбор из списка Tutorial/BotDemo
//                LoadingData data = new LoadingData();
//
//                data.AssemblyName="Demo"; // <-- выбор из списка loader.Levels.Keys
//                data.Level="Test"; // <-- loader.Levels[AsseblyName].Keys;
//
//                SettingsProposal proposal=new SettingsProposal();
//                proposal.Controllers = new System.Collections.Generic.List<ControllerSettings> // <-- только BotDemo
//                {
//                    new ControllerSettings { ControllerId="Left", Type= ControllerType.Bot, Name="Random"},
//                    new ControllerSettings { ControllerId="Right", Type= ControllerType.Bot, Name="Square"}
//                };
//                worldInitializer=()=>loader.LoadNonLogFile(mode, data, proposal);
//                Application.LoadLevel("Round");
//                Debug.Log("Ok");
//            }
//        }
//        GUILayout.EndArea ();
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


    public void OnGUI()
    {
        var competitions = IntroductionStript.loader.Levels.Keys.ToArray();
        var competitionsGUI = competitions.Select(x => new GUIContent(x.ToString())).ToArray();
        competitionIndex = EditorGUILayout.Popup(new GUIContent("Choose competition:"), competitionIndex, competitionsGUI);

        var levels = IntroductionStript.loader.Levels[competitions[competitionIndex]].Keys.ToArray();
        var levelsGUI = levels.Select(x => new GUIContent(x.ToString())).ToArray();
        levelIndex = EditorGUILayout.Popup(new GUIContent("Choose level:"), levelIndex, levelsGUI);

        var comp = IntroductionStript.loader.Levels[competitions[competitionIndex]][levels[levelIndex]]();

		var modeNames = new[] { Test, Tutorial, BotDemo};
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

			if (runMode != Test)
			{
				Debug.Log("Non test starting");
				var factory = IntroductionStript.loader.CreateControllerFactory(runMode);
				

				SettingsProposal proposal = new SettingsProposal();
				if (runMode == "BotDemo")
					proposal.Controllers = new System.Collections.Generic.List<ControllerSettings>
                {
                    new ControllerSettings {ControllerId = "Left", Type = ControllerType.Bot, Name = bots[leftBot]},
                    new ControllerSettings {ControllerId = "Right", Type = ControllerType.Bot, Name = bots[rightBot]}
                };

				//            Debug.Log("Ok");
				this.Close();
				Dispatcher.WorldPrepared(() => IntroductionStript.loader.CreateSimpleMode(data, proposal, factory));

			}
			else // запуск одного теста
			{
				Debug.Log("Tests starting");
				//
				Dispatcher.RunAllTests(data);
				//Action runner = () => Dispatcher.RunAllTests(data);
				//Dispatcher.RunThread(runner, "test runner");

				if (false)
				{
					//var competitionsInstance = IntroductionStript.loader.GetCompetitions(data);
					//var testName = "Forward";// competitionsInstance.Logic.Tests.First().Key; //Насте - нужен PopUp
					//var test = IntroductionStript.loader.GetTest(data, testName);
					//var asserter = new UnityAsserter();
					//Dispatcher.WaitingNetworkServer.LoadingData = data;

					//Action action = () => test.Run(Dispatcher.WaitingNetworkServer, asserter);
					//var testInfo = new BullShi();
					//Action action = () => testInfo.Run(Dispatcher.WaitingNetworkServer, asserter);
					//action.BeginInvoke(null, null);
					//Dispatcher.RunThread(action, "test thread");
				}
				Debug.Log("tests started");
			}

		}
    }

}
