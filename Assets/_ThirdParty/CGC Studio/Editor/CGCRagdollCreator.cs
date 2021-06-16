
using UnityEngine;
using UnityEditor;

public class CGCRagdollCreator : EditorWindow {

    
    GameObject StickManContainer;
    GameObject head;
    GameObject spine;
    GameObject hip;
    GameObject LeftUpperHand;
    GameObject LeftLowerHand;
    GameObject RightLowerHand;
    GameObject RightUpperHand;
    GameObject LeftUpperLeg;
    GameObject LeftLowerLeg;
    GameObject RightUpperLeg;
    GameObject RightLowerLeg;
    GameObject LeftArm;
    GameObject RightArm;
    GameObject LeftLeg;
    GameObject RightLeg;

    Texture2D WindowBanner;
    Rect WindowBannerSection;
    Texture2D BackGround;
    Rect BackGroundSection;

    GUISkin skin;

  
    
    [MenuItem("Tool/CGC STUDIO/CGCRagdollCreator")]
    private static void ShowWindow() {
        var window = GetWindow<CGCRagdollCreator>();
        window.titleContent = new GUIContent("CGC RagdollCreator");
        window.minSize = new Vector2 (350,430);
        window.maxSize = new Vector2 (400,460);
       
        window.Show();
    }
     void DrawHader()
     {    GUILayout.BeginArea(WindowBannerSection);
         GUILayout.Label("CGC Ragdoll Creator",skin.GetStyle("Header"));
         GUILayout.EndArea();
     }
    private void OnGUI() 
    {    DrawHader();
        DrawLayout();
        GUILayout.Label("CGC Ragdoll Creator",skin.GetStyle("Header"));
       GUILayout.BeginArea(BackGroundSection);
        
        StickManContainer = EditorGUILayout.ObjectField("StickManContainer",StickManContainer,typeof(GameObject))as GameObject;
        head = EditorGUILayout.ObjectField("Head",head,typeof(GameObject))as GameObject;
        spine = EditorGUILayout.ObjectField("Spine",spine,typeof(GameObject))as GameObject;
        hip = EditorGUILayout.ObjectField("Hip",hip,typeof(GameObject))as GameObject;
        LeftLeg = EditorGUILayout.ObjectField("LeftLeg",LeftLeg,typeof(GameObject))as GameObject;
        LeftUpperLeg = EditorGUILayout.ObjectField("LeftUpperLeg",LeftUpperLeg,typeof(GameObject))as GameObject;
        LeftLowerLeg = EditorGUILayout.ObjectField("LeftLowerLeg",LeftLowerLeg,typeof(GameObject))as GameObject;
        RightLeg = EditorGUILayout.ObjectField("RightLeg",RightLeg,typeof(GameObject))as GameObject;
        RightUpperLeg = EditorGUILayout.ObjectField("RightUpperLeg",RightUpperLeg,typeof(GameObject))as GameObject;
        RightLowerLeg = EditorGUILayout.ObjectField("RightLowerLeg",RightLowerLeg,typeof(GameObject))as GameObject;
        LeftArm = EditorGUILayout.ObjectField("LeftArm",LeftArm,typeof(GameObject))as GameObject;
        LeftUpperHand = EditorGUILayout.ObjectField("LeftUpperHand",LeftUpperHand,typeof(GameObject))as GameObject;
        LeftLowerHand = EditorGUILayout.ObjectField("LeftLowerHand",LeftLowerHand,typeof(GameObject))as GameObject;
        RightArm = EditorGUILayout.ObjectField("RightArm",RightArm,typeof(GameObject))as GameObject;
        RightUpperHand = EditorGUILayout.ObjectField("RightUpperHand",RightUpperHand,typeof(GameObject))as GameObject;
        RightLowerHand = EditorGUILayout.ObjectField("RightLowerHand",RightLowerHand,typeof(GameObject))as GameObject;
        

        
        //CreatButton
        if(StickManContainer != null)
        {
          if(head  != null)
          {
              if(spine  != null)
              {
                  if(hip  != null)
                  {
                     if(LeftUpperLeg  != null)
                     {
                        if(LeftLowerLeg  != null)
                        {
                          if( RightUpperLeg  != null)
                          {
                              if(RightLowerLeg  != null)
                              {
                                if(LeftUpperHand != null)
                                {
                                    if(LeftLowerHand != null)
                                    {
                                        if(RightUpperHand != null)
                                        {
                                            if(RightLowerHand != null)
                                            {
                                                  if(GUILayout.Button("Create Stick Man"))
                                                  {
                                                    CreateRagdoll();
                                                  }
                                            }
                                        }
                                    }
                                }
                              }
                          }       
                          
                        }
                     }
                  }
              }
          }
        
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
      WindowBanner = Resources.Load<Texture2D>("Image/Banner/RagdollCreatorWindowBanner");
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

     


    public void CreateRagdoll()
    {
    //AddMainControllerToPlayer
     StickManContainer.AddComponent<StickManController>();
     StickManContainer.AddComponent<PlayerDead>();
     StickManContainer.AddComponent<IgnoreCollision>();
     Rigidbody2D rb2dLeft = LeftLeg.GetComponent<Rigidbody2D>();
     Rigidbody2D rb2dRight = RightLeg.GetComponent<Rigidbody2D>();
     StickManContainer.GetComponent<StickManController>().rbRIGHTLEG =rb2dLeft; 
     StickManContainer.GetComponent<StickManController>().rbLEFTLEG = rb2dRight; 

     //CreateJump&FootStep
     hip.AddComponent<Jump>();
     GameObject footstepleft = new GameObject("DustController",typeof(OnGrounded));
     GameObject footstepright = new GameObject("DustController",typeof(OnGrounded));
     footstepleft.GetComponent<OnGrounded>().PlayerHip = hip; 
     footstepleft.transform.position = new Vector3(0,-2.4f,0);
     footstepright.transform.position = new Vector3(0,-2.4f,0);
     footstepright.GetComponent<OnGrounded>().PlayerHip = hip; 
     footstepright.AddComponent<CircleCollider2D>().radius =0.07f; 
     footstepleft.AddComponent<CircleCollider2D>().radius =0.07f; 
     footstepleft.transform.SetParent(LeftLowerLeg.transform);
     footstepright.transform.SetParent(RightLowerLeg.transform);

    //CreateRagdollCollider
    head.AddComponent<CircleCollider2D>();
    spine.AddComponent<BoxCollider2D>();
    hip.AddComponent<BoxCollider2D>();
    LeftUpperLeg.AddComponent<BoxCollider2D>();
    LeftLowerLeg.AddComponent<BoxCollider2D>();
    RightUpperLeg.AddComponent<BoxCollider2D>();
    RightLowerLeg.AddComponent<BoxCollider2D>();
    LeftUpperHand.AddComponent<BoxCollider2D>();
    LeftLowerHand.AddComponent<BoxCollider2D>();
    RightUpperHand.AddComponent<BoxCollider2D>();
    RightLowerHand.AddComponent<BoxCollider2D>();

    //CreateRagdollBodyPart
    head.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    spine.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    hip.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    LeftUpperLeg.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    LeftLowerLeg.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    RightUpperLeg.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    RightLowerLeg.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    LeftUpperHand.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    LeftLowerHand.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    RightUpperHand.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    RightLowerHand.AddComponent<BodyPart>().Player = StickManContainer.GetComponent<PlayerDead>();
    
    
     
    //CreateRagdollRigidbody
    head.AddComponent<Rigidbody2D>();
    spine.AddComponent<Rigidbody2D>();
    hip.AddComponent<Rigidbody2D>();
    LeftLeg.AddComponent<Rigidbody2D>();
    LeftLowerLeg.AddComponent<Rigidbody2D>();
    RightLeg.AddComponent<Rigidbody2D>();
    RightLowerLeg.AddComponent<Rigidbody2D>();
    LeftArm.AddComponent<Rigidbody2D>();
    LeftLowerHand.AddComponent<Rigidbody2D>();
    RightArm.AddComponent<Rigidbody2D>();
    RightLowerHand.AddComponent<Rigidbody2D>();
    
    //CreateRagdollJoint
    head.AddComponent<HingeJoint2D>();
    spine.AddComponent<HingeJoint2D>();
    hip.AddComponent<HingeJoint2D>();
    LeftLeg.AddComponent<HingeJoint2D>();
    LeftLowerLeg.AddComponent<HingeJoint2D>();
    RightLeg.AddComponent<HingeJoint2D>();
    RightLowerLeg.AddComponent<HingeJoint2D>();
    LeftArm.AddComponent<HingeJoint2D>();
    LeftLowerHand.AddComponent<HingeJoint2D>();
    RightArm.AddComponent<HingeJoint2D>();
    RightLowerHand.AddComponent<HingeJoint2D>();

    //CreateRagdollJointConnect
    head.GetComponent<HingeJoint2D>().connectedBody = spine.GetComponent<Rigidbody2D>();
    spine.GetComponent<HingeJoint2D>().connectedBody = hip.GetComponent<Rigidbody2D>();
    hip.GetComponent<HingeJoint2D>().connectedBody = spine.GetComponent<Rigidbody2D>();
    LeftLeg.GetComponent<HingeJoint2D>().connectedBody = hip.GetComponent<Rigidbody2D>();
    RightLeg.GetComponent<HingeJoint2D>().connectedBody = hip.GetComponent<Rigidbody2D>();
    LeftArm.GetComponent<HingeJoint2D>().connectedBody = spine.GetComponent<Rigidbody2D>(); 
    RightArm.GetComponent<HingeJoint2D>().connectedBody = spine.GetComponent<Rigidbody2D>();
    LeftLowerLeg.GetComponent<HingeJoint2D>().connectedBody = LeftLeg.GetComponent<Rigidbody2D>(); 
    RightLowerLeg.GetComponent<HingeJoint2D>().connectedBody = RightLeg.GetComponent<Rigidbody2D>(); 
    LeftLowerHand.GetComponent<HingeJoint2D>().connectedBody = LeftArm.GetComponent<Rigidbody2D>(); 
    RightLowerHand.GetComponent<HingeJoint2D>().connectedBody = RightArm.GetComponent<Rigidbody2D>(); 
    } 

}
