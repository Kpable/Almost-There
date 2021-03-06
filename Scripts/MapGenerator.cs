using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
	
	// The type of tile that will be laid in a specific position.
	public enum TileType
	{
		None, 
		Grass, Wall, Floor, Opening,
		Total
	}
		
	public int columns = 30;                                 // The number of columns on the board (how wide it will be).
	public int rows = 30;                                    // The number of rows on the board (how tall it will be).
	public IntRange numRooms = new IntRange (2, 3);         // The range of the number of rooms there can be.
	public IntRange roomWidth = new IntRange (3, 10);         // The range of widths rooms can have.
	public IntRange roomHeight = new IntRange (3, 10);        // The range of heights rooms can have.
	public IntRange hallwayWidth = new IntRange (1, 3);    // The range of lengths corridors between rooms can have.
	public IntRange hallwayHeight = new IntRange (1, 3);    // The range of lengths corridors between rooms can have.

	public IntRange openningsPerRoom = new IntRange (1, 3);    // The range of lengths corridors between rooms can have.
	//public IntRange openingsPerSide = 
	//	new IntRange (
	//		Mathf.FloorToInt(roomWidth.m_Min / 3), 
	//		Mathf.FloorToInt(roomWidth.m_Max / 3)
	//	);    // The range of lengths corridors between rooms can have.

	public GameObject[] grassTiles;                           // An array of grass tile prefabs.
	public GameObject[] floorTiles;                           // An array of floor tile prefabs.
	public GameObject[] wallTiles;                            // An array of wall tile prefabs.
	public GameObject[] outerWallTiles;                       // An array of outer wall tile prefabs.
    public GameObject[] openingTiles;                       // An array of outer wall tile prefabs.
	public GameObject player;

	private TileType[][] houseTiles;                               // A jagged array of tile types representing the board, like a grid.
	private TileType[][][] houseTilesLayered;
	private Room[] rooms;                                     // All the rooms that are created for this board.
	private Hallway[] hallways;                             // All the corridors that connect the rooms.
	private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.
	private Vector3 playerPos;

	// Use this for initialization
	void Start () {
		//boardHolder = new GameObject("boardholder");
		GenerateHouse ();
    }

    void SetupTilesArray ()
	{
		
		// Set the tiles jagged array to the correct width.
		houseTiles = new TileType[columns][];

		// Go through all the tile arrays...
		for (int i = 0; i < houseTiles.Length; i++)
		{
			// ... and set each tile array is the correct height.
			houseTiles[i] = new TileType[rows];
		}

		houseTilesLayered = new TileType[(int)TileType.Total] [] [];
		for (int i = 0; i < (int)TileType.Total; i++) {
			houseTilesLayered [i] = new TileType[columns][]; 
			for (int j = 0; j < houseTilesLayered[i].Length; j++) {
				houseTilesLayered [i] [j] = new TileType[rows];
			}
		}
	}

	void CreateRoomsAndHallways(){
        rooms = new Room[numRooms.Random];
        //rooms = new Room[1];

        // There should be one less corridor than there is rooms.
        hallways = new Hallway[rooms.Length - 1];
        //hallways = new Hallway[1];

        // Create the first room and corridor.
        rooms[0] = new Room ();
		hallways[0] = new Hallway ();

		// Setup the first room, there is no previous corridor so we do not use one.
		rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

		// Setup the first corridor using the first room.
		hallways[0].SetupHallway(rooms[0], hallwayWidth, hallwayHeight, roomWidth, roomHeight, columns, rows, true);


        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
           rooms[i] = new Room();

            // Setup the room based on the previous corridor.
           rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, hallways[i - 1]);

           // If we haven't reached the end of the corridors array...
            if (i < hallways.Length)
            {
               // ... create a corridor.
                hallways[i] = new Hallway();

                // Setup the corridor based on the room that was just created.
               hallways[i].SetupHallway(rooms[i], hallwayWidth, hallwayHeight, roomWidth, roomHeight, columns, rows, false);
            }

            if (i == rooms.Length * .5f)
            {
                playerPos = new Vector3(rooms[i].xPos, rooms[i].yPos, 0);
            }
        }

    }

	void CreateRoomOpenings()
	{
		// Go through all the rooms...
		for (int i = 0; i < rooms.Length; i++) 
		{
			Room currentRoom = rooms [i];
            currentRoom.SetUpOpenings();

		}

	}

    void SetTileValuesForOpenings()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            for (int j = 0; j < currentRoom.openings.Length; j++)
            {
                int xCoord = currentRoom.openings[j].xPos;
                int yCoord = currentRoom.openings[j].yPos;

                //TODO index out of range error
                houseTilesLayered[(int)TileType.Opening][xCoord][yCoord] = TileType.Opening;
            }

        }
    }

    void SetTilesValuesForGrass(){
		for (int i = 0; i < houseTilesLayered[(int)TileType.Grass].Length; i++) {

			for (int j = 0; j < houseTilesLayered[(int)TileType.Grass][i].Length; j++) {
				houseTilesLayered [(int)TileType.Grass] [i] [j] = TileType.Grass;
			}
		}
	}

    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;

                    // The coordinates in the jagged array are based on the room's position and it's width and height.
					//houseTiles[xCoord][yCoord] = TileType.Floor;
					houseTilesLayered[(int)TileType.Floor][xCoord][yCoord] = TileType.Floor;
                }
            }
        }
    }

	void SetTileValuesForHallways(){
		// Go through every corridor...
		for (int i = 0; i < hallways.Length; i++)
		{
			Hallway currentHallway = hallways[i];

			// ... and for each room go through it's width.
			for (int j = 0; j < currentHallway.width; j++)
			{
				int xCoord = currentHallway.xPos + j;

				// For each horizontal tile, go up vertically through the room's height.
				for (int k = 0; k < currentHallway.height; k++)
				{
					int yCoord = currentHallway.yPos + k;

					//TODO theres an array out of index error here
					// The coordinates in the jagged array are based on the room's position and it's width and height.
					//houseTiles[xCoord][yCoord] = TileType.Floor;
					houseTilesLayered[(int)TileType.Floor][xCoord][yCoord] = TileType.Floor;

				}
			}
		}
	}

	void InstantiateGround(GameObject holder)
    {
        // Go through all the tiles in the jagged array...
        /*for (int i = 0; i < houseTiles.Length; i++)
        {
            for (int j = 0; j < houseTiles[i].Length; j++)
            {
                // ... and instantiate a floor tile for it.
                InstantiateFromArray(grassTiles, i, j, holder);
            }
        }*/

		for (int i = 0; i < houseTilesLayered[(int)TileType.Grass].Length; i++)
		{
			for (int j = 0; j < houseTilesLayered[(int)TileType.Grass][i].Length; j++)
			{
				// ... and instantiate a floor tile for it.
				InstantiateFromArray(grassTiles, i, j, (float)TileType.Grass, holder);
			}
		}
    }

	void InstantiateRooms (GameObject holder){

		// Go through all the rooms...
		for (int i = 0; i < rooms.Length; i++)
		{
			Room currentRoom = rooms[i];
			GameObject roomholder = new GameObject ("Room " + i);
			roomholder.transform.parent = holder.transform;
			// ... and for each room go through it's width.
			for (int j = 0; j < currentRoom.roomWidth; j++)
			{
				int xCoord = currentRoom.xPos + j;

				// For each horizontal tile, go up vertically through the room's height.
				for (int k = 0; k < currentRoom.roomHeight; k++)
				{
					int yCoord = currentRoom.yPos + k;

					//InstantiateFromArray(floorTiles, xCoord, yCoord, roomholder);
					InstantiateFromArray(floorTiles, xCoord, yCoord, (float)TileType.Floor, roomholder);
				}
			}
		}

	}

    void InstatiateOpenings(GameObject openingsHolder)
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];
            GameObject openingholder = new GameObject("Opening " + i);
            openingholder.transform.parent = openingsHolder.transform;

            for (int j = 0; j < currentRoom.openings.Length; j++)
            {
                int xCoord = currentRoom.openings[j].xPos;
                int yCoord = currentRoom.openings[j].yPos;

                InstantiateFromArray(openingTiles, xCoord, yCoord, (float)TileType.Opening, openingholder);
            }
        }
    }

    void InstantiateHallways (GameObject holder){


		// Go through all the rooms...
		for (int i = 0; i < hallways.Length; i++)
		{
			Hallway currentHallway = hallways[i];
			GameObject hallwayholder = new GameObject ("Hallway " + i);
			hallwayholder.transform.parent = holder.transform;

			// ... and for each room go through it's width.
			for (int j = 0; j < currentHallway.width; j++)
			{
				int xCoord = currentHallway.xPos + j;

				// For each horizontal tile, go up vertically through the room's height.
				for (int k = 0; k < currentHallway.height; k++)
				{
					int yCoord = currentHallway.yPos + k;

					//InstantiateFromArray(floorTiles, xCoord, yCoord,  hallwayholder);
					InstantiateFromArray(floorTiles, xCoord, yCoord, (float)TileType.Floor, hallwayholder);

				}
			}
		}

	}

	void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord, GameObject holder)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = holder.transform;
    }

	void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord, float zCoord, GameObject holder)
	{
		// Create a random index for the array.
		int randomIndex = Random.Range(0, prefabs.Length);

		// The position to be instantiated at is based on the coordinates.
		Vector3 position = new Vector3(xCoord, yCoord, 0);

		// Create an instance of the prefab from the random index of the array.
		GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        if (holder.transform.position.z != zCoord * -1)
            holder.transform.position = new Vector3(holder.transform.position.x, holder.transform.position.y, zCoord * -1);
        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = holder.transform;
	}

	public void GenerateHouse(){
		GameObject clr;
		clr = GameObject.Find ("House");
		if (clr != null) Destroy (clr);
		clr = GameObject.Find ("Ground");
		if (clr != null) Destroy (clr);
		clr = GameObject.Find (player.name);
		if (clr != null) Destroy (clr);

		GameObject houseHolder = new GameObject ("House");
		GameObject roomsHolder = new GameObject ("Rooms");
		GameObject openingsHolder = new GameObject ("Openings");
        roomsHolder.transform.parent = houseHolder.transform;
        openingsHolder.transform.parent = houseHolder.transform;
        GameObject hallwaysHolder = new GameObject ("Hallways");
		hallwaysHolder.transform.parent = houseHolder.transform;

		GameObject groundHolder = new GameObject ("Ground");
	
		SetupTilesArray ();
		CreateRoomsAndHallways ();
		CreateRoomOpenings ();

		SetTilesValuesForGrass ();
		SetTilesValuesForRooms();
		SetTileValuesForHallways();
		SetTileValuesForOpenings();

        InstantiateGround(groundHolder);
		InstantiateRooms(roomsHolder);
		InstantiateHallways (hallwaysHolder);
        InstatiateOpenings(openingsHolder);

		//Instantiate(player, playerPos, Quaternion.identity);

	}

    // Update is called once per frame
    void Update () {
	
	}
}
