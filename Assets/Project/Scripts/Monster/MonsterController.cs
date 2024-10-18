using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public BehaviorGraphAgent behavior;
    private MapGeneratorController mapGenerator;
    public List<GameObject> waypoints;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapGenerator = GameObject.FindWithTag("MapGenerator").GetComponent<MapGeneratorController>();
        waypoints = mapGenerator.waypoints;
        behavior.SetVariableValue("Waypoints", waypoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
