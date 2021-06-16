using UnityEngine;
using UnityEditor;


public class CGCCameraCreator : EditorWindow {
    GameObject StickMan;
    Texture2D WindowBanner;
    Rect WindowBannerSection;
    Texture2D BackGround;
    Rect BackGroundSection;
    GUISkin skin;
    float smoothSpeed;
    float Xoffset;
    float Yoffset;


    [MenuItem("Tool/CGC STUDIO/CGCCameraCreator")]
    private static void ShowWindow() {
        var window = GetWindow<CGCCameraCreator>();
        window.titleContent = new GUIContent("CGCCameraCreator");
        window.Show();
        window.minSize = new Vector2 (350,430);
        window.maxSize = new Vector2 (400,460);
               
    }

    private void OnGUI() {
        DrawLayout();
        DrawHader();
        GUILayout.BeginArea(BackGroundSection);
        StickMan = EditorGUILayout.ObjectField("StickManHip",StickMan,typeof(GameObject))as GameObject;
        smoothSpeed = EditorGUILayout.FloatField("smoothSpeed",smoothSpeed);
        GUILayout.Label("Offset",EditorStyles.boldLabel);
        Xoffset = EditorGUILayout.FloatField("X",Xoffset);
        Yoffset = EditorGUILayout.FloatField("Y",Yoffset);
        
        if(StickMan != null)
        {
         if(GUILayout.Button("Create CameraController",GUILayout.Height(50)))
         {
            CreateCamera();
         }
        }else
        {
            EditorGUILayout.HelpBox("Please Assing StickManHip",MessageType.Warning);
        }
           if(GUILayout.Button("Video Tutorial"))
        {
           Application.OpenURL("https://www.youtube.com/watch?v=ZaB-HGlqyRo");
        }
         GUILayout.EndArea();
    }
    void OnEnable() {
        InitTexture();
        skin = Resources.Load<GUISkin>("GUISkin/CGC_RagdollCreatorWindowSkin");
    }
    void InitTexture()
    {  
       BackGround= Resources.Load<Texture2D>("Image/Background/BackGround");
      WindowBanner = Resources.Load<Texture2D>("Image/Banner/CameraCreatorWindowBanner");
    }
    void DrawLayout()
    {
     WindowBannerSection.x = 0;
     WindowBannerSection.y = 0;
     WindowBannerSection.height = Screen.width/5;
     WindowBannerSection.width = Screen.width;

     BackGroundSection.x = 0;
     BackGroundSection.y = Screen.width/5;
     BackGroundSection.height = Screen.height + Screen.height /5;
     BackGroundSection.width = Screen.width;

     GUI.DrawTexture(WindowBannerSection,WindowBanner);
     GUI.DrawTexture(BackGroundSection,BackGround);
    }
     void DrawHader()
     {    GUILayout.BeginArea(WindowBannerSection);
         GUILayout.Label(" CGC Camera Creator ",skin.GetStyle("Header"));
         GUILayout.EndArea();
     }
       void CreateCamera()
    {
     GameObject cameracontroller = new GameObject("CGC_CameraController",typeof(CGCCameraFollow));
     cameracontroller.transform.position = new Vector3(0,0,-10);
     cameracontroller.GetComponent<CGCCameraFollow>().Player = StickMan; 
     cameracontroller.GetComponent<CGCCameraFollow>().smoothSpeed = smoothSpeed;
     cameracontroller.GetComponent<CGCCameraFollow>().Offset.x = Xoffset;
     cameracontroller.GetComponent<CGCCameraFollow>().Offset.y = Yoffset;
    }
}