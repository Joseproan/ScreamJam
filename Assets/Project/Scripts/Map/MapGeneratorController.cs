using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGeneratorController : MonoBehaviour
{
    [Header("Spawn probability")]
    [SerializeField] private int spawnProbability = 70;

    [Header("Maze configuration")]
    [SerializeField] private int minRoom = 4;
    [SerializeField] private int maxRoomX = 5;
    [SerializeField] private int maxRoomY = 5;
    [SerializeField] private int offset = 6;

    [Header("Room prefab")]
    [SerializeField] private GameObject room;

    [Header("Spawn Player Script")]
    [SerializeField] private GameObject spawnPlayer;    
    [Header("Spawn Monster Script")]
    [SerializeField] private GameObject spawnMonster;


    //Maze generation
    private Vector3[,] roomPos;
    private bool[,] visited;
    private int minRoomCount;

    //Room generation
    private RoomController[,] roomController;

    private int initialColumn;

    // Start is called before the first frame update
    void Start()
    {
        while(minRoomCount < minRoom) GenerateMaze(maxRoomX, maxRoomY);
        //GenerateMaze(maxRoomX, maxRoomY);
        GenerateRooms(maxRoomX, maxRoomY);
        RandomSpawnPlayer(maxRoomX, maxRoomY);
    }

    //Create the maze
    public void GenerateMaze(int rows, int columns)
    {
        //Initialize the arrays
        visited = new bool[rows, columns];
        roomPos = new Vector3[rows, columns];
        minRoomCount = 0;

        //Ensure that the first room is created
        initialColumn = Random.Range(0, 0);
        roomPos[0, initialColumn] = new Vector3(0, 0, 0);
        visited[0, initialColumn] = true;
        minRoomCount++;

        for (int i = 0; i < rows; i++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (Random.Range(0f, 101f) <= spawnProbability) // Chances to spawn a room
                {
                    // Veryfy if the room has an adjacent room
                    bool hasAdjacentRoom = (i > 0 && visited[i - 1, x]) || (i < rows - 1 && visited[i + 1, x]) || (x > 0 && visited[i, x - 1]) || (x < columns - 1 && visited[i, x + 1]);
                    if (hasAdjacentRoom)
                    {
                        //Save the position of the room
                        roomPos[i, x] = new Vector3(x * offset, 0, i * offset);
                        //Mark the room as visited
                        visited[i, x] = true;
                        minRoomCount++;
                    }

                }
            }
        }
    }

    void GenerateRooms(int rows, int columns)
    {
        roomController = new RoomController[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (visited[i, x])
                {
                    //Create the room in the saved position
                    GameObject newRoom = Instantiate(room, roomPos[i, x], Quaternion.identity);

                    //Open the doors of the room that has an adjacent room
                    if (i > 0 && visited[i - 1, x])
                    {
                        roomController[i, x] = newRoom.GetComponent<RoomController>();
                        roomController[i, x].UpdateRoom(2);
                    }
                    if (i < rows - 1 && visited[i + 1, x])
                    {
                        roomController[i, x] = newRoom.GetComponent<RoomController>();
                        roomController[i, x].UpdateRoom(1);
                    }
                    if (x > 0 && visited[i, x - 1])
                    {
                        roomController[i, x] = newRoom.GetComponent<RoomController>();
                        roomController[i, x].UpdateRoom(3);
                    }
                    if (x < columns - 1 && visited[i, x + 1])
                    {
                        roomController[i, x] = newRoom.GetComponent<RoomController>();
                        roomController[i, x].UpdateRoom(0);
                    }
                }                
            }
        }
    }

    void RandomSpawnPlayer(int rows, int columns)
    {
        List<Vector3> availableRoomPositions = new List<Vector3>();

        for (int i = 0; i < rows; i++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (roomPos[i, x] != Vector3.zero) // Verify if the room has been created
                {
                    availableRoomPositions.Add(roomPos[i, x]);
                }
            }
        }
        if (availableRoomPositions.Count == 0)
        {
            //Enseure spawn player at least one room
            Instantiate(spawnPlayer, new Vector3(roomPos[0, initialColumn].x, roomPos[0, initialColumn].y + 2, roomPos[0, initialColumn].z), Quaternion.identity);
            Debug.Log("Here");
            
        }
        else
        {
            int playerIndex = Random.Range(0, availableRoomPositions.Count);
            Vector3 playerRoomPosition = availableRoomPositions[playerIndex];

            // Instanciar el jugador en esa sala
            Instantiate(spawnPlayer, new Vector3(playerRoomPosition.x, playerRoomPosition.y + 2, playerRoomPosition.z), Quaternion.identity);

            // Lista de salas filtrada, excluyendo la sala del jugador
            List<Vector3> remainingRooms = new List<Vector3>(availableRoomPositions);
            remainingRooms.RemoveAt(playerIndex);

            if (remainingRooms.Count > 0) // Asegurarse de que hay al menos otra sala disponible
            {
                // Selecciona una sala diferente para el monstruo
                int monsterIndex = Random.Range(0, remainingRooms.Count);
                Vector3 monsterRoomPosition = remainingRooms[monsterIndex];

                // Instanciar el monstruo en la sala seleccionada
                Instantiate(spawnMonster, new Vector3(monsterRoomPosition.x, monsterRoomPosition.y, monsterRoomPosition.z), Quaternion.identity);
            }
        }

    }
}
