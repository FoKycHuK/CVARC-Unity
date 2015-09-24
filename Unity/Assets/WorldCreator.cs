using CVARC.V2;
using UnityEngine;


public partial class RoundScript : MonoBehaviour
{
    IWorld CreateWorld(params string[] args)
    {
        var loader = new Loader();
        loader.AddLevel("Demo", "Level1", () => new DemoCompetitions.Level1());
        loader.AddLevel("RoboMovies", "Level1", () => new RMCompetitions.Level1());
        return loader.Load(args);
    }

    IWorld CreateDemoWorld()
    {
        return CreateWorld("Demo", "Level1", "BotDemo", "-TimeLimit", "10", "-EnableLog", "-Controller.Left", "Bot.Square", "-Controller.Right", "Bot.Square");
    }

    IWorld CreateRTSWorld()
    {
        return CreateWorld("RepairTheStarship", "Level1", "BotDemo", "-Controller.Left", "Bot.Azura", "-Controller.Right", "Bot.Sanguine");
    }

    IWorld CreateTutorialWorld()
    {
        return CreateWorld("RepairTheStarship", "Level1", "Tutorial");
    }

    IWorld CreateRMWorld()
    {
        return CreateWorld("RoboMovies", "Level1", "Tutorial");
    }

    IWorld CreateCameraDemo()
    {
        return CreateWorld("");
    }

    void CameraCreator()
    {
        myCamera = new GameObject("myCamera");
        myCamera.AddComponent<Camera>();
        myCamera.AddComponent<GUILayer>();
        myCamera.AddComponent<AudioListener>();
        myCamera.transform.position = new Vector3(0, 200, -200);
        myCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    void ScoresFieldsCreator()
    {
        scoresTextLeft = new GameObject("LeftScoreText").AddComponent<GUIText>() as GUIText;
        scoresTextLeft.pixelOffset = new Vector2(1, 1);
        scoresTextLeft.text = "Left Scores: 0";
        scoresTextLeft.transform.position = new Vector3(0, 1, 0);
        scoresTextRight = new GameObject("RightScoreText").AddComponent<GUIText>() as GUIText;
        scoresTextRight.pixelOffset = new Vector2(2, 2);
        scoresTextRight.text = "Right Scores: 0";
        scoresTextRight.transform.position = new Vector3(0.88f, 1, 0);
    }
    void UpdateScores()
    {
        foreach (var player in world.Scores.GetAllScores())
        {
            if (player.Item1 == "Left")
                scoresTextLeft.text = "Left Scores: " + player.Item2;
            if (player.Item1 == "Right")
                scoresTextRight.text = "Right Scores: " + player.Item2;
        }
    }
}
