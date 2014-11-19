using UnityEngine;
using System.Collections;
using CVARC.V2;
using System;

public class IntroductionStript : MonoBehaviour {

	public static Func<IWorld> worldInitializer;

	CVARC.V2.Loader loader;

	void Start()
	{
		loader = new CVARC.V2.Loader ();
		loader.AddLevel ("Demo", "Test", () => new DemoCompetitions.Level1());
		//copy here other levels of demo

        //надо запустить тред Server
	}

    void Server()
    {
        // var server=new PercistentServer(); - открывает TcpListener, делает AcceptClient, останавливается и выходит из Run. 
        // server.Run()
        // server.ReadObject(typeof()); //из открытого клиента читает строку и десериализует ее. Если фейл, то снова сделать AcceptClient. Если и это фейл - пересоздать листенерю Можно сносить вообще все.
        // var mode = new DebugRunMode(server); // PercistentServer должен реализовывать интерфейс IMessagingClient
        // var configProposal = mode.GetConfigurationProposal();
        //worldInitializer = () => loader.LoadNonLogFile(mode, data, proposal);
        //Application.LoadLevel("Round");
        //Debug.Log("Ok");
    }

	public void OnGUI()
	{
		GUILayout.BeginArea (new Rect (0, 0, Screen.height, Screen.width), GUI.skin.textArea);
		{
			GUIStyle style = new GUIStyle (GUI.skin.button);
			style.margin = new RectOffset (50, 50, 50, 50);
			style.font = new Font ();
			bool result=GUILayout.Button("Create",style,GUILayout.MinHeight(60));
			if (result)
			{
				IRunMode mode = RunModeFactory.Create(RunModes.BotDemo); // <-- выбор из списка Tutorial/BotDemo
				LoadingData data = new LoadingData();

				data.AssemblyName="Demo"; // <-- выбор из списка loader.Levels.Keys
				data.Level="Test"; // <-- loader.Levels[AsseblyName].Keys;

				SettingsProposal proposal=new SettingsProposal();
                proposal.Controllers = new System.Collections.Generic.List<ControllerSettings> // <-- только BotDemo
                {
                    new ControllerSettings { ControllerId="Left", Type= ControllerType.Bot, Name="Random"},
                    new ControllerSettings { ControllerId="Right", Type= ControllerType.Bot, Name="Square"}
                };
                worldInitializer=()=>loader.LoadNonLogFile(mode, data, proposal);
                Application.LoadLevel("Round");
                Debug.Log("Ok");
			}
		}
		GUILayout.EndArea ();


	}
}
