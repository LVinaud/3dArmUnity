﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScene : MonoBehaviour
{
    public GameObject robotBase;
    private Evolution EvScript; //so that it can call the evolution script after everyithing is set up
    private PathFinding PathFind; //so that it can call the pathfinding algorithm after everything is set up
    private Gridi Gridi;
    public Vector3 worldSize;
    public GameObject obstaclePrefab; 
    public GameObject armPart;
    public GameObject joint;
    public int N;
    // List to store references to each arm segment
    public List<GameObject> armParts = new List<GameObject>();
    public GameObject armEnd;
    public float distJoints = 0.4f;
    public int numObstacles = 5;

    void Start(){
        EvScript = GetComponent<Evolution>();//gets the scripts
        PathFind = GetComponent<PathFinding>();
        Gridi = GetComponent<Gridi>();
        createRobotArm();
        createRandomObstacles(numObstacles);//creates random obstacles, this will eventually be substituted by reading gameobjects from a save file like the 2d version
        Gridi.createGrid();
        PathFind.activate();//activates the pathfind that will eventually activate the evolution script
    }

    void createRobotArm(){
        GameObject lastArm = robotBase;

        for (int i = 0; i < N; i++)
        {
            // Instantiate the arm part
            GameObject nextArm = Instantiate(armPart, Vector3.zero, Quaternion.identity);
            GameObject currentJoint = Instantiate(joint, Vector3.zero, Quaternion.identity);
            
            // Parent it to the last arm
            nextArm.transform.parent = lastArm.transform;
            currentJoint.transform.parent = lastArm.transform;

            // Set local position to extend along Y-axis
            if(i != 0) {
                nextArm.transform.localPosition = new Vector3(0, distJoints, 0);
                nextArm.transform.localRotation = Quaternion.identity;
                currentJoint.transform.localPosition = new Vector3(0, distJoints, 0);
                currentJoint.transform.localRotation = Quaternion.Euler(i%3!=2?0:90, i%3!=1?0:90, i%3!=0?0:90);
            } else {
                nextArm.transform.localPosition = new Vector3(0, 0, 0);
                nextArm.transform.localRotation = Quaternion.identity;
                currentJoint.transform.localPosition = new Vector3(0, 0, 0);
                currentJoint.transform.localRotation = Quaternion.Euler(i%3!=2?0:90, i%3!=1?0:90, i%3!=0?0:90);
            }
            // Add to the list of arm parts
            armParts.Add(nextArm);

            // Update lastArm to the current arm part
            lastArm = nextArm;
        }

        // Instantiate the arm end and parent it to the last arm segment
        armEnd = Instantiate(armEnd, Vector3.zero, Quaternion.identity);
        armEnd.transform.parent = lastArm.transform;
        armEnd.transform.localPosition = new Vector3(0, distJoints, 0);
        armEnd.transform.localRotation = Quaternion.identity;
    }

    void createRandomObstacles(int n) {
        //this will create random obstacles inside the grid sizes and add them to the evolution osbtacle list
        //for this i need to get a random x, y, z inside the real grid
        float sizeX = worldSize.x/2;
        float sizeY = worldSize.y/2;
        float sizeZ = worldSize.z/2;
    
        for(int i = 0; i < n; i++) {
            GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(Random.Range(-sizeX, sizeX), Random.Range(0, sizeY), Random.Range(-sizeZ, sizeZ)), Quaternion.identity);
            newObstacle.layer = 3;
            EvScript.addObstacle(newObstacle);
        }
    }
}