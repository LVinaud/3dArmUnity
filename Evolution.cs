﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
public class Evolution : MonoBehaviour
{   
    //stopwatch to record the time elapsed during the simulation
    private Stopwatch sw;
    //the filename where the experiment results will be stored in the case of a limited amount of generations
    public string fileName = "experimentLog";
    //the streamWriter to save the log
    private StreamWriter logFile;
    //robot is the whole sctructure created by createScene beggining by the robot's base
    public GameObject robot;
    //the gameobject working as the objective, its 3d position is changed and used for the fitness calculations
    public GameObject goal;
    //this is a list containing the robot's current angles
    private List<float> robotState = new List<float>();
    //this is the population, all the possible solutions for the problem
    private List<List<float>> popStates = new List<List<float>>();

    // Evolution parameters

    //N is the number of joints
    private int N = 0;
    //how many individuals in the same population, to be determined by each experiment configuration
    public int popSize;
    //the maximum possible change in a joint's angulation, the lower the more of a robotic like movement
    public float maxStep;
    //a variable to mark what the current generation is
    private int generation = 1;
    //not used at the moment, its future use will be the speed to change between each point of the path found as the goal
    private int generationsPerObjective = 200;
    //how many generations will be calculated for the experiment, -1 means undefined or neverending simulation
    public int maxGenerations = -1;
    // List of arm parts from CreateScene
    private List<GameObject> armParts;
    //this segmentLength should match distJoint in createScene, as it does in the awake method
    private float segmentLength = 0.4f;
    //just a variable to determine if an individual is better or worse than the previous best one
    private float previousBestfitness;

    void Awake(){
        //gets the relevant infos from the createscene script
        N = GetComponent<CreateScene>().N;
        armParts = GetComponent<CreateScene>().armParts;
        segmentLength = GetComponent<CreateScene>().distJoints;
        //start to write into the file
        if(maxGenerations != -1) {
            sw = new Stopwatch();   
            logFile = new StreamWriter(fileName, false);
        }
    }
    //starts the robot with a zero angulation in all joints and a random population
    public void Start(){
        //starts the time count
        sw.Start();
        //initialize the lists with a 0 angulation
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
    //the never ending loop of evaluating a population, displaying the best configuration and generating a new population based on the previous best one
    void Update(){
        
        fitnessEvaluation();
        showArm();
        newPopulation();
        //prints the generation and best fitness, i intend to make this to be written into a text file for another program to read and plot
        
        if(maxGenerations != -1) {
            //makes the update method stop when a certain number of generations is reached
            logFile.WriteLine(generation + ", " + previousBestfitness);
            if(generation >= maxGenerations) {
                //writes data to the file and closes it
                sw.Stop();
                logFile.WriteLine("Time elapsed(ms): " + sw.ElapsedMilliseconds);
                logFile.WriteLine("Generations: " + maxGenerations);
                logFile.WriteLine("Number of joints: " + N);
                logFile.WriteLine("Population size: " + popSize);
                logFile.WriteLine("MaxStep: " + maxStep);
                logFile.WriteLine("Generations per objective: " + generationsPerObjective);
                logFile.WriteLine("Segment lenght: " + segmentLength);
                logFile.Close();
                logFile.Dispose();
                this.enabled = false;
            }
        }
        generation++;
    }

    void fitnessEvaluation() {
        //the bestInd saves what is the best one in the popStates
        int bestInd = -1;
        //this calculation is needed in the case of the goal moving, so that the bestFitness is recalculated, if it were a gloval variable and this wasn't done, it would save a fitness from a different goal or, in other words, another problem
        float bestFitness = Vector3.Distance(simulatedArm(robotState), goal.transform.position);
        for (int i = 0; i < popSize; i++){
            // Test individual arm end position
            Vector3 endPosition = simulatedArm(popStates[i]);
            //calculated the fitness, or distance to the goal
            float fitness = Vector3.Distance(endPosition, goal.transform.position);
            //checks if its better and saves its configuration
            if(fitness <= bestFitness){
                bestInd = i;
                bestFitness = fitness;
                robotState = new List<float>(popStates[bestInd]);
            }
        }
        previousBestfitness = bestFitness;
    }

    void showArm() {
        //for each angle stored in robotState, it uses the rotation stored in a specific axis only, as a more realistic and factual one would
        for(int j = 0; j < N; j++){
            Vector3 axis = Vector3.zero;
            if (j % 3 == 0) {
                axis = Vector3.right; // X axis
            } else if (j % 3 == 1) {
                axis = Vector3.up; // Y axis
            } else {
                axis = Vector3.forward; // Z axis
            }
            //gets a robotState[j] angle applies in the axis
            Quaternion jointRotation = Quaternion.AngleAxis(robotState[j], axis);
            // Apply the rotation to the corresponding arm part
            armParts[j].transform.localRotation = jointRotation;
        }
    }

    //does the math to calculate the end position given a set of angles
    Vector3 simulatedArm(List<float> evaluated) {
        //the position starts at the base position
        GameObject baseRobot = robot.transform.GetChild(0).gameObject;
        Vector3 position = baseRobot.transform.position;
        //the cumulative rotation starts with the identity
        Quaternion rotation = Quaternion.identity;

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
            Quaternion jointRotation = Quaternion.AngleAxis(evaluated[j], axis);

            // Accumulate rotation
            rotation *= jointRotation;

            // Update position
            position += rotation * Vector3.up * segmentLength; // Move along local Y axis
        }

        // Return the calculated end effector position
        return position;
    }

    void newPopulation(){
        // Generate new population
        for (int i = 0; i < popSize; i++){
            for (int j = 0; j < N; j++){
                float angle = Random.Range(-maxStep + robotState[j], maxStep + robotState[j]);
                popStates[i][j] = angle;
            }
        }
    }
}