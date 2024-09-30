using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject robot;
    public GameObject goal;
    public GameObject testCube;
    //private GameObject testing;
    private GameObject armEnd;
    private List<float> robotState = new List<float>();
    private List<List<float> > popStates = new List<List<float> >();
    private bool robotCreated = false;

    //---------- Evolution parameters ----------// 
    private int N = 0;
    public int popSize;
    public float maxStep;
    public bool colliding=false;

    //---------- Forward Kinematics ----------//
    private float distGround = 0.2f;
    private float distJoints = 0.4f;

    void Awake(){
        N = GetComponent<CreateScene>().N;
    }

    void Start(){
        for (int i = 0; i < N; i++){
            robotState.Add(0);
        }
        
        for(int i=0; i<popSize; i++){
            popStates.Add(new List<float>());
            for (int j= 0; j < N; j++){
                float angle = Random.Range(-maxStep + robotState[i], maxStep + robotState[i]);//the random range was only limited by the maxstep and i wwanted it to not be limited at all
                popStates[i].Add(angle);
            }
        }
    }

    // Update is called once per frame
    void Update(){
        if(!robotCreated && robot.transform.childCount != 0){
            robotCreated=true;
            Debug.Log("created");

            GameObject lastPiece = robot.transform.GetChild(1).gameObject;
            for(int i=0; i<N-1; i++){
                lastPiece = lastPiece.transform.GetChild(1).gameObject;
            }
            armEnd = lastPiece.transform.GetChild(1).gameObject;
        }

        if(robotCreated){
            int bestInd = -1;
            float bestFitness = 1000;// inf

            for (int i = 0; i < popSize; i++){
                 GameObject lastPiece = robot.transform.GetChild(1).gameObject;

                // Test individual arm position
                for(int j=0; j<N; j++){
                    lastPiece.transform.localRotation = Quaternion.Euler(
                         j%3!=2?0:popStates[i][j],
                         j%3!=0?0:-popStates[i][j],
                         j%3!=1?0:-popStates[i][j]
                        );
                    lastPiece = lastPiece.transform.GetChild(1).gameObject;
                }

                if(distToGoal(i)<=bestFitness){
                    bestInd = i;
                    bestFitness = distToGoal(i);
                }
            }

            newPopulation(bestInd);
        }
    }

    void newPopulation(int bestInd){

       GameObject lastPiece = robot.transform.GetChild(1).gameObject;
        // Test individual arm position
        for(int j=0; j<N; j++){
            lastPiece.transform.localRotation = Quaternion.Euler(
                    j%3!=2?0:popStates[bestInd][j],
                    j%3!=0?0:-popStates[bestInd][j],
                    j%3!=1?0:-popStates[bestInd][j]
                );
            lastPiece = lastPiece.transform.GetChild(1).gameObject;
        }

        robotState = popStates[bestInd];//gets the robot ot act like the best one
        
        for(int i=0;i<N;i++){
            popStates[0][i] = robotState[i];//copies the best one into the first index of the population
        }
        
        for(int i=1; i<popSize; i++){
            for (int j = 0; j < N; j++){
                float angle = Random.Range(-maxStep + robotState[j], maxStep + robotState[j]);
                popStates[i][j] = angle;
            }
        }
    }

    float distToGoal(int i){
        Vector3 robotP = armEnd.transform.position;
        Vector3 goalP = goal.transform.position;
        return Vector3.Distance(robotP, goalP);
    }

}
