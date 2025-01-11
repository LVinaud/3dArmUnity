using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

public class MainUIController : MonoBehaviour
{
    public UIDocument uiDocument;
    public GameObject mainCamera;
    public GameObject goal;

    private VisualElement root;
    private Label[] childrenArray;

    private Evolution EvScript;
    private CreateScene SceneScript;
    private PathFinding PathFind;
    private Gridi GridScript;
    private FpsCounter FpsScript;
    private ObjectSelector SelectorScript;

    private MoveObject cameraScript;
    private MoveObject goalScript;

    private int sliderValue;
    private int[] newValuesFromInputs;
    private bool isShowingPath = false;
    private GameObject clickedObject;

    void Start()
    {
        EvScript = GetComponent<Evolution>();
        SceneScript = GetComponent<CreateScene>();
        PathFind = GetComponent<PathFinding>();
        GridScript = GetComponent<Gridi>();
        FpsScript = GetComponent<FpsCounter>();
        SelectorScript = GetComponent<ObjectSelector>();
        cameraScript = mainCamera.GetComponent<MoveObject>();

        // Access the root VisualElement
        root = uiDocument.rootVisualElement;

        /////////////////////////////////////////////////////////////////////////////////////
        // Get all buttons and assign a common click handler
        var buttons = root.Query<Button>().ToList();
        foreach (var button in buttons)
        {
            button.clicked += () => OnButtonClicked(button.name);
        }

        /////////////////////////////////////////////////////////////////////////////////////
        SliderInt volumeSlider = root.Q<SliderInt>("NumObstacles");

        // Get the initial value
        sliderValue = volumeSlider.value;

        // Add a callback to handle value changes
        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            sliderValue = volumeSlider.value;
        });

        //////////////////////////////////////////////////////////////////////////////////////
        Foldout foldout = root.Q<Foldout>("DataFoldOut");
        childrenArray = foldout.Children().OfType<Label>().ToArray();

        //////////////////////////////////////////////////////////////////////////////////////
        var textInputs = root.Query<IntegerField>().ToList();
        newValuesFromInputs = new int[textInputs.Count];
        
        int counter = 0;
        foreach (var input in textInputs)
        {

            // Capture the current value of counter
            int currentIndex = counter;

            input.RegisterValueChangedCallback(evt =>
            {
                newValuesFromInputs[currentIndex] = evt.newValue; // Use captured currentIndex
            });

            counter++;
        }

        //////////////////////////////////////////////////////////////////////////////////////

        var evaluationDropdown = root.Q<DropdownField>("EvaluationTypes");
        // Populate the dropdown with options
        evaluationDropdown.choices = new List<string> {"Option 1", "Option 2", "Option 3"};
        // Set the default value
        evaluationDropdown.value = "Option 1";

        // Register a callback for when the user changes the selection
        evaluationDropdown.RegisterValueChangedCallback(evt =>
        {
            switch(evt.newValue){

                case "Option 1":
                    EvScript.whichFitness = 1;
                    break;

                case "Option 2":
                    EvScript.whichFitness = 2;
                    break;

                case "Option 3":
                    EvScript.whichFitness = 3;
                    break;

                default:
                    break;
            }
        });
    }

    void Update()
    {
        
        float[] uiData = EvScript.getUIData();

        childrenArray[0].text = "Time elapsed(ms): " + uiData[0];
        childrenArray[1].text = "Generations: " + uiData[1];
        childrenArray[2].text = "Number of joints: " + uiData[2];
        childrenArray[3].text = "Population size: " + uiData[3];
        childrenArray[4].text = "MaxStep: " + uiData[4];
        childrenArray[5].text = "Generations per objective: " + uiData[5];
        childrenArray[6].text = "Segment lenght: " + uiData[6];
        childrenArray[7].text = "Minimum obstacle distance: " + uiData[7];
        childrenArray[8].text = "FPS: " + FpsScript.getFPS();

        HandleMovement();

    }
    
    //Mudar nome da variável
    private void HandleMovement(){

        GameObject objectClicked = SelectorScript.getClickedObject();

        if(objectClicked!= null){

            cameraScript.deactivate();

            if(objectClicked != clickedObject && clickedObject != null){

                Destroy(clickedObject.GetComponent<MoveObject>());
            }

            clickedObject = objectClicked;
            if(clickedObject.GetComponent<MoveObject>() == null){

                clickedObject.AddComponent<MoveObject>();
                clickedObject.GetComponent<MoveObject>().cameraTransform = mainCamera.transform;
            }

        } else {

            cameraScript.activate();

            if( clickedObject != null){

                Destroy(clickedObject.GetComponent<MoveObject>());
            }
        }
    }

    // Generic click handler
    private void OnButtonClicked(string buttonName)
    {

        switch (buttonName)
        {
            /*case "MoveCamera":
                
                cameraScript.activate();
                goalScript.deactivate();
                break;

            case "MoveObject":
                
                cameraScript.deactivate();
                goalScript.activate();
                break;*/

            case "ChangeObstacles":

                EvScript.clearObstacles();
                SceneScript.createRandomObstacles(sliderValue);
                PathFind.Awake();
                EvScript.Awake();
                PathFind.activate();
                break;

            case "ChangeGeneration":
                EvScript.maxGenerations = newValuesFromInputs[0];
                PathFind.Awake();
                EvScript.Awake();
                PathFind.activate();
                break;

            case "ChangePopulation":
                EvScript.popSize = newValuesFromInputs[1];
                PathFind.Awake();
                EvScript.Awake();
                PathFind.activate();
                break;

            case "ChangeMaxStep":
                EvScript.maxStep = newValuesFromInputs[2];
                PathFind.Awake();
                EvScript.Awake();
                PathFind.activate();
                break;

            case "ShowPath":
                isShowingPath = !isShowingPath;
                GridScript.setEnabled(isShowingPath);
                break;

            case "ChangeNumSegments":
                SceneScript.destroyRobotArm();
                //EvScript.resetRobotState();
                SceneScript.N = newValuesFromInputs[3];
                SceneScript.createRobotArm();
                PathFind.Awake();
                EvScript.Awake();
                PathFind.activate();
                break;

            case "ResetAStar":
                PathFind.Awake();
                EvScript.Awake();
                PathFind.activate();
                break;

            default:
                Debug.Log("Unknown button clicked");
                break;
        }
    }
}
