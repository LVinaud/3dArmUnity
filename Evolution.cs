﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject robot;
    public GameObject goal;
    public GameObject testCube;

    private GameObject armEnd;
    private List<float> robotState = new List<float>();
    private List<List<float>> popStates = new List<List<float>>();
    private bool robotCreated = false;

    // Evolution parameters
    private int N = 0;
    public int popSize;
    public float maxStep;

    // List of arm parts from CreateScene
    private List<GameObject> armParts;

    void Awake(){
        N = GetComponent<CreateScene>().N;
        armParts = GetComponent<CreateScene>().armParts;
    }

    void Start(){
        for (int i = 0; i < N; i++){
            robotState.Add(0);
        }
        
        for(int i=0; i<popSize; i++){
            popStates.Add(new List<float>());
            for (int j= 0; j < N; j++){
                float angle = Random.Range(-maxStep + robotState[j], maxStep + robotState[j]);
                popStates[i].Add(angle);
            }
        }
    }

    void Update(){
        if(!robotCreated && robot.transform.childCount != 0){
            robotCreated = true;
            Debug.Log("Robot created");

            // Access the arm end from the CreateScene script
            armEnd = GetComponent<CreateScene>().armEndgo;
        }

        if(robotCreated){
            int bestInd = -1;
            float bestFitness = Mathf.Infinity;

            for (int i = 0; i < popSize; i++){
                // Test individual arm position
                Vector3 endPosition = simulatedArm(i);
                float fitness = Vector3.Distance(endPosition, goal.transform.position);
                if(fitness <= bestFitness){
                    bestInd = i;
                    bestFitness = fitness;
                }
            }

            newPopulation(bestInd);
        }
    }

    Vector3 simulatedArm(int who) {
        GameObject baseRobot = robot.transform.GetChild(0).gameObject;
        Vector3 position = baseRobot.transform.position; // Starting from the base position
        Quaternion rotation = Quaternion.identity; // Initialize rotation as identity

        float segmentLength = 0.4f; // Should match distJoints

        for (int j = 0; j < N; j++) {
            // Determine the axis of rotation
            Vector3 axis = Vector3.zero;
            if (j % 3 == 0) {
                axis = Vector3.right; // X axis
            } else if (j % 3 == 1) {
                axis = Vector3.up; // Y axis
            } else {
                axis = Vector3.forward; // Z axis
            }

            // Create rotation for this joint
            Quaternion jointRotation = Quaternion.AngleAxis(popStates[who][j], axis);

            // Accumulate rotation
            rotation *= jointRotation;

            // Update position
            position += rotation * Vector3.up * segmentLength; // Move along local Y axis
        }

        // Return the calculated end effector position
        return position;
    }

    void newPopulation(int bestInd){
        // Apply the best individual's joint angles to the robot
        for(int j = 0; j < N; j++){
            Vector3 axis = Vector3.zero;
            if (j % 3 == 0) {
                axis = Vector3.right; // X axis
            } else if (j % 3 == 1) {
                axis = Vector3.up; // Y axis
            } else {
                axis = Vector3.forward; // Z axis
            }

            Quaternion jointRotation = Quaternion.AngleAxis(popStates[bestInd][j], axis);

            // Apply the rotation to the corresponding arm part
            armParts[j].transform.localRotation = jointRotation;
        }

        // Update the robot's current state
        robotState = new List<float>(popStates[bestInd]);

        // Copy the best individual into the first index of the population
        for (int i = 0; i < N; i++){
            popStates[0][i] = robotState[i];
        }

        // Generate new population
        for (int i = 1; i < popSize; i++){
            for (int j = 0; j < N; j++){
                float angle = Random.Range(-maxStep + robotState[j], maxStep + robotState[j]);
                popStates[i][j] = angle;
            }
        }
    }
}