using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Slot { Unavailable, Available, Possibly_Available, Occupied }


public class Room
{
    public int xPos;                      // The x coordinate of the lower left tile of the room.
    public int yPos;                      // The y coordinate of the lower left tile of the room.
    public int roomWidth;                     // How many tiles wide the room is.
    public int roomHeight;                    // How many tiles high the room is.

    public Direction enteringHallway;    // The direction of the corridor that is entering this room.
    public Direction exitingHallway;
    public Opening[] openings;

    public bool hasDoor = false;
    public Door door;

    public Window[] windows;

    public int Top { get { return yPos + roomHeight; } }
    public int Bottom { get { return yPos; } }
    public int Left { get { return xPos; } }
    public int Right { get { return xPos + roomWidth; } }

    // This is used for the first room.  It does not have a Corridor parameter since there are no corridors yet.
    public void SetupRoom (IntRange widthRange, IntRange heightRange, int columns, int rows)
	{
		// Set a random width and height.
		roomWidth = widthRange.Random;
		roomHeight = heightRange.Random;

		// Set the x and y coordinates so the room is roughly in the middle of the board.
		xPos = Mathf.RoundToInt(columns / 2f - roomWidth / 2f);
		yPos = Mathf.RoundToInt(rows / 2f - roomHeight / 2f);
	}


	// This is an overload of the SetupRoom function and has a corridor parameter that represents the corridor entering the room.
	public void SetupRoom (IntRange widthRange, IntRange heightRange, int columns, int rows, Corridor corridor)
	{
		// Set the entering corridor direction.
		enteringHallway = corridor.direction;

		// Set random values for width and height.
		roomWidth = widthRange.Random;
		roomHeight = heightRange.Random;

		switch (corridor.direction)
		{
		// If the corridor entering this room is going north...
		case Direction.North:
			// ... the height of the room mustn't go beyond the board so it must be clamped based
			// on the height of the board (rows) and the end of corridor that leads to the room.
			roomHeight = Mathf.Clamp(roomHeight, 1, rows - corridor.EndPositionY);

			// The y coordinate of the room must be at the end of the corridor (since the corridor leads to the bottom of the room).
			yPos = corridor.EndPositionY;

			// The x coordinate can be random but the left-most possibility is no further than the width
			// and the right-most possibility is that the end of the corridor is at the position of the room.
			xPos = Random.Range (corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);

			// This must be clamped to ensure that the room doesn't go off the board.
			xPos = Mathf.Clamp (xPos, 0, columns - roomWidth);
			break;
		case Direction.East:
			roomWidth = Mathf.Clamp(roomWidth, 1, columns - corridor.EndPositionX);
			xPos = corridor.EndPositionX;

			yPos = Random.Range (corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
			yPos = Mathf.Clamp (yPos, 0, rows - roomHeight);
			break;
		case Direction.South:
			roomHeight = Mathf.Clamp (roomHeight, 1, corridor.EndPositionY);
			yPos = corridor.EndPositionY - roomHeight + 1;

			xPos = Random.Range (corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
			xPos = Mathf.Clamp (xPos, 0, columns - roomWidth);
			break;
		case Direction.West:
			roomWidth = Mathf.Clamp (roomWidth, 1, corridor.EndPositionX);
			xPos = corridor.EndPositionX - roomWidth + 1;

			yPos = Random.Range (corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
			yPos = Mathf.Clamp (yPos, 0, rows - roomHeight);
			break;
		}
	}

    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows, Hallway hallway)
    {
        // Set the entering corridor direction.
        enteringHallway = hallway.direction;

        // Set random values for width and height.
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        switch (hallway.direction)
        {
            // If the corridor entering this room is going north...
            case Direction.North:
                // ... the height of the room mustn't go beyond the board so it must be clamped based
                // on the height of the board (rows) and the end of corridor that leads to the room.
				roomHeight = Mathf.Clamp(roomHeight, 1, rows - (hallway.Top));

                // The y coordinate of the room must be at the end of the corridor (since the corridor leads to the bottom of the room).
                yPos = hallway.Top;

                // The x coordinate can be random but the left-most possibility is no further than the width
                // and the right-most possibility is that the end of the corridor is at the position of the room.
				xPos = Random.Range(hallway.Left - roomWidth + 1, hallway.Right - 1);

                // This must be clamped to ensure that the room doesn't go off the board.
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.East:
				roomWidth = Mathf.Clamp(roomWidth, 1, columns - (hallway.Right));
				xPos = hallway.Right;

				yPos = Random.Range((hallway.Top) - roomHeight + 1, (hallway.Top) - 1);
                yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;
            case Direction.South:
				roomHeight = Mathf.Clamp(roomHeight, 1, hallway.Bottom);
                yPos = hallway.Bottom - roomHeight;

				xPos = Random.Range(hallway.Left - roomWidth + 1, hallway.Right -1);
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.West:
				roomWidth = Mathf.Clamp(roomWidth, 1, hallway.Left);
                xPos = hallway.Left - roomWidth;

				yPos = Random.Range(hallway.Bottom - roomHeight + 1, hallway.Top -1);
                yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;
        }
    }

	public void SetUpOpenings(){

		openings = new Opening[Random.Range (1, 3)];

		if (enteringHallway == null) {
			
		}

		for (int i = 0; i < openings.Length; i++) {
			Direction openingDirection  = (Direction)Random.Range (0, 4);
            if (openingDirection == enteringHallway) openingDirection++;

			openings [i] = new Opening ();
			switch (openingDirection) {
			case Direction.North:
				openings [i].xPos = Random.Range (xPos, xPos + roomWidth - 1);
				openings [i].yPos = yPos + roomHeight;
				break;
			case Direction.East:
				openings [i].xPos = xPos + roomWidth;
				openings [i].yPos = Random.Range(yPos, yPos + roomHeight - 1);
				break;
			case Direction.South:
				openings [i].xPos = Random.Range (xPos, xPos + roomWidth - 1);
				openings [i].yPos = yPos + roomHeight;
				break;
			case Direction.West:
				openings [i].xPos = xPos;
				openings [i].yPos = Random.Range(yPos, yPos + roomHeight - 1);
				break;
			default:
				break;
			}
		}


	}

    public void PlaceDoor()
    {
        if (hasDoor)
        {
            door = new Door();
            //Door cannot be on the either side of the room where theres a hallway. 
            while (door.dir != enteringHallway && door.dir != exitingHallway)
            {
                door.dir = (Direction)Random.Range(0, 4);
            }

            switch (door.dir)
            {
                case Direction.North:
                    door.xPos = Random.Range(Left, Right - 1);
                    door.yPos = Top;
                    break;
                case Direction.East:
                    door.xPos = Right;
                    door.yPos = Random.Range(Bottom, Top - 1);
                    break;
                case Direction.South:
                    door.xPos = Random.Range(Left, Right - 1);
                    door.yPos = Bottom;
                    break;
                case Direction.West:
                    door.xPos = Left;
                    door.yPos = Random.Range(Bottom, Top - 1);
                    break;
                default:
                    break;
            }

        }
        else
        {
            Debug.LogWarning("Attempted to place a door in a room that does not have one");
        }
    }
    
    // Window Rules:
    // - Cannot be on corners
    // - Cannot be on room sides with hallways
    // - Cannot be adjacent to one another or doors
    // - Can be varying lengths
    public void PlaceWindows()
    {
        Direction windowDirection = Direction.East;
        bool sideWithDoor = false;
        // pick a direction that does not have a hallway attached. 
        while (windowDirection != enteringHallway && windowDirection != exitingHallway)
        {
            windowDirection = (Direction)Random.Range(0, 4);
        }

        // grab the length of that side
        int sideLength = 0;

        switch (windowDirection)
        {
            case Direction.North:
            case Direction.South:
                sideLength = roomWidth;
                break;
            case Direction.East:
            case Direction.West:
                sideLength = roomHeight;
                break;
            default:
                break;
        }

        int roomToWorkWith = sideLength - 2; // Dont want windows on the corners so sidelength - 2

        //Now that we know the side length lets check that the side is long enough to support a window. 
        if (hasDoor)
        {
            if (door.dir == windowDirection) // this is the side with the door
            {
                sideWithDoor = true;
                //Anything under 5 fails the above rules. 
                if (sideLength < 5)         // if this side is less than 5 units long
                {
                    // Choose the side without door/hallways
                    while (windowDirection != enteringHallway &&
                        windowDirection != exitingHallway &&
                        windowDirection != door.dir)
                    {
                        windowDirection = (Direction)Random.Range(0, 4);
                    }

                    // We have changed to the last side no door
                }
                else    // This side is long enough that we care about the door
                {

                    if (door.xPos == Left || door.xPos == Right ||
                        door.yPos == Bottom || door.yPos == Top)
                    {
                        roomToWorkWith -= 1; // Door is on corner so we just need to leave one unit space. 
                    }
                    else if (door.xPos == Left + 1 || door.xPos == Right - 1 ||
                        door.yPos == Bottom + 1 || door.yPos == Top - 1)
                    {
                        roomToWorkWith -= 2; // leave Two spaces since door is adjacent to corner. 
                    }
                    else
                    {
                        roomToWorkWith -= 3; // Leave a space on either side of the door. 
                    }

                }
            }
        }

        if (roomToWorkWith < 1) Debug.LogWarning("This should never have happend. We dont have enough room to place a window here.");

        // Set the number of windows, max is sideLength / 2 rounded down
        // Windows must be at least 1 unit apart
        int numWindows = Random.Range(1, Mathf.CeilToInt(roomToWorkWith / 2));

        windows = new Window[numWindows];

        //Slot out the side of the room. 

        SlotPosition[] sideSlots = new SlotPosition[sideLength];

        CreateSideSlots(windowDirection, sideWithDoor, sideSlots);
      
        SlotPosition[] windowLocations = RandomWindowOrder(numWindows, sideSlots);

        for (int i = 0; i < windows.Length; i++)
        {
            //The windows will all be on one side and in the same direction.
            windows[i].direction = windowDirection;

            switch (windowDirection)
            {
                case Direction.North:
                    windows[i].yPos = Top;
                    break;
                case Direction.South:
                    windows[i].yPos = Bottom;
                    break;
                case Direction.East:
                    windows[i].xPos = Right;
                    break;
                case Direction.West:
                    windows[i].xPos = Left;
                    break;
                default:
                    break;
            }


        }

        if (sideWithDoor)
        {

        }
    }

    private void CreateSideSlots(Direction windowDirection, bool sideWithDoor, SlotPosition[] sideSlots)
    {
        int sidePos = 0;

        switch (windowDirection)
        {
            // The North and South sides go along the xPos.
            case Direction.North:
            case Direction.South:
                sidePos = xPos;
                break;
            // The East and West sides go along the yPos.
            case Direction.East:
            case Direction.West:
                sidePos = yPos;
                break;
            default:
                break;
        }

        for (int j = 0; j < sideSlots.Length; j++)
        {
            sideSlots[j].availability = Slot.Available;
            sideSlots[j].pos = sidePos + j;
        }

        // Eliminate the slots we cant use based on the rules. 

        // No Corners
        sideSlots[0].availability = Slot.Unavailable;
        sideSlots[sideSlots.Length - 1].availability = Slot.Unavailable;
        // No Adjacency to the Door if present.
        if (sideWithDoor)
        {
            if (sidePos == xPos)     // If using the x position, compare the x position. 
            {
                for (int i = 0; i < sideSlots.Length; i++)
                {
                    if (sideSlots[i].pos == door.xPos)
                    {
                        sideSlots[i].availability = Slot.Unavailable;
                        //Get the left and right of the door as well, if they are valid. 
                        if (i > 0)
                            sideSlots[i - 1].availability = Slot.Unavailable;
                        if (i < sideSlots.Length - 1)
                            sideSlots[i + 1].availability = Slot.Unavailable;
                    }

                }
            }
            else if (sidePos == yPos)    // else compare the y position
            {
                for (int i = 0; i < sideSlots.Length; i++)
                {
                    if (sideSlots[i].pos == door.yPos)
                    {
                        sideSlots[i].availability = Slot.Unavailable;
                        //Get the left and right of the door as well, if they are valid. 
                        if (i > 0)
                            sideSlots[i - 1].availability = Slot.Unavailable;
                        if (i < sideSlots.Length - 1)
                            sideSlots[i + 1].availability = Slot.Unavailable;
                    }

                }
            }
        }
    }

    private SlotPosition[] RandomWindowOrder(int numWindows, SlotPosition[] side)
    {
        List<SlotPosition[]> orders = new List<SlotPosition[]>();
        SlotPosition[] placement = side;
        while (!orders.Contains(placement))
        {
            placement.First(slot => slot.availability == Slot.Available);
        }



        return placement;
    }

    public class Door
    {
        public int xPos;
        public int yPos;
        public Direction dir;
    }

    public class Window
    {
        public int xPos;
        public int yPos;
        public int length;
        public Direction direction;
    }
    public class Opening
    {
	    public int xPos;                      // The x coordinate of the lower left tile of the room.
	    public int yPos;                      // The y coordinate of the lower left tile of the room.
    }
}

public class SlotPosition
{
    public Slot availability;
    public int pos;
}
