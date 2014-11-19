//using System.Linq;
//using CVARC.V2;
//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Collections;

//namespace Assets
//{
//    public class FirstMenu : MonoBehaviour
//    {
//        private bool flag = true;

//        public void OnGUI()
//        {
//            if (flag)
//            {
//                EditorGUILayoutEnumPopup.Init();
//                flag = false;
//            }

//        }
//    }

//    internal class EditorGUILayoutEnumPopup : EditorWindow
//    {
//        public static void Init()
//        {
//            var window = GetWindow(typeof(EditorGUILayoutEnumPopup));
//            window.Show();
//        }

//        public static Rect position = new Rect(100, 100, 100, 100);
//        private GUIContent[] op = { new GUIContent("eee"), new GUIContent("ttt") };
//        public int competitionIndex = 0;
//        public int levelIndex = 0;
//        public int botIndex = 0;
//        public int controllerIndex = 0;
//        public Font buttonFont;
//        public float buttonMinHeight = 60f;
////            var aaa = new GUIContent[] { new GUIContent("aaa"), new GUIContent("ddd"), new GUIContent("sssss") };


//        public void OnGUI()
//        {
//            Loader loader = new Loader();
//            loader.AddLevel("Demo", "Level1", () => new DemoCompetitions.Level1());
//            loader.AddLevel("RepairTheStarship", "Level1", () => new RepairTheStarship.Level1());

//            var competition = loader.Levels.Keys.Select(x => new GUIContent(x.ToString())).ToArray();
//            var levels = loader.Levels["Demo"].Keys.Select(x => new GUIContent(x.ToString())).ToArray();
//            //var competitions = new Competitions();
//            //var bots = Competitions.Logic.Bots.Keys.Select(x => new GUIContent(x.ToString())).ToArray();
//            //var controllers = competitions.Logic.ControllersId.Select(x => new GUIContent(x.ToString())).ToArray();
//            competitionIndex = EditorGUILayout.Popup(new GUIContent("Choose competition:"), competitionIndex, competition);
//            levelIndex = EditorGUILayout.Popup(new GUIContent("Choose level:"), levelIndex, levels);
//            //botIndex = EditorGUILayout.Popup(new GUIContent("Choose bot:"), botIndex, bots);
//            //controllerIndex = EditorGUILayout.Popup(new GUIContent("Choose controller:"), controllerIndex, controllers);
            
//            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
//            buttonStyle.font = buttonFont;
//            buttonStyle.margin = new RectOffset(20, 20, 3, 3);
//            if (GUILayout.Button("Start", buttonStyle, GUILayout.MinHeight(buttonMinHeight)))
//            {
//                // загрузка сцены с именем Level
//                Debug.Log(competitionIndex);
//                this.Close();
//                Application.LoadLevel("scene");
//            }
//        }

//    }
//}

//////для Насти:
////loader.Levels.Keys.ToArray(); // список всех соревнований
////loader.Levels["Demo"].Keys.ToArray(); //список всех уровней соревнования
////var competitions = loader.Levels["Demo"]["Level1"]();
////competitions.Logic.Bots.Keys.ToArray(); //список всех доступных ботов
////competitions.Logic.ControllersId.ToArray(); //список всех контроллеров (Left/Right в наших соревнованиях)
////// надо создать
////LoadingData data = null;
////SettingsProposal proposal = null;
////// и из этого мы потом сделаем соревнования
///// 
///// 
///// return loader.Load(LoadProposal);