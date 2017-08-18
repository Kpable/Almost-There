using UnityEngine;

// Enum to specify the direction is heading.
public enum Direction
{
	North, East, South, West,
}



public class Hallway
{
	public int xPos;         // The x coordinate for the start of the corridor.
	public int yPos;         // The y coordinate for the start of the corridor.
	public int width;
	public int height;
	public Direction direction;   // Which direction the corridor is heading from it's room.

    public int Top { get { return yPos + height; } }
    public int Bottom { get { return yPos; } }
    public int Left { get { return xPos; } }
    public int Right { get { return xPos + width; } }

	public void SetupHallway (Room room, IntRange widthRange, IntRange heightRange, IntRange roomWidth, IntRange roomHeight, int columns, int rows, bool firstCorridor)
	{
		// Set a random direction (a random index from 0 to 3, cast to Direction).
		direction = (Direction)Random.Range(0, 4);


		// Find the direction opposite to the one entering the room this corridor is leaving from.
		// Cast the previous corridor's direction to an int between 0 and 3 and add 2 (a number between 2 and 5).
		// Find the remainder when dividing by 4 (if 2 then 2, if 3 then 3, if 4 then 0, if 5 then 1).
		// Cast this number back to a direction.
		// Overall effect is if the direction was South then that is 2, becomes 4, remainder is 0, which is north.
		Direction oppositeDirection = (Direction)(((int)room.enteringHallway + 2) % 4);

		// If this is noth the first corridor and the randomly selected direction is opposite to the previous corridor's direction...
		if (!firstCorridor && direction == oppositeDirection)
		{
			// Rotate the direction 90 degrees clockwise (North becomes East, East becomes South, etc).
			// This is a more broken down version of the opposite direction operation above but instead of adding 2 we're adding 1.
			// This means instead of rotating 180 (the opposite direction) we're rotating 90.
			int directionInt = (int)direction;
			directionInt++;
			directionInt = directionInt % 4;
			direction = (Direction)directionInt;

		}

        room.exitingHallway = direction;

		// Set a random length.
		//corridorLength = length.Random;
		width = widthRange.Random;
		height = heightRange.Random;

		// Create a cap for how long the length can be (this will be changed based on the direction and position).
		int maxWith = widthRange.m_Max;
		int maxHeight = heightRange.m_Max;

		switch (direction)
		{
		// If the choosen direction is North (up)...
		case Direction.North:


			// ... the starting position in the x axis can be random but within the width of the room.
			xPos = Random.Range (room.Left - width + 1, room.Right - 1);

			// The starting position in the y axis must be the top of the room.
			yPos = room.Top;
                // The maximum length the corridor can be is the height of the board (rows) but from the top of the room (y pos + height).
                //maxLength = rows - startYPos - roomHeight.m_Min;

			height = Mathf.Clamp(height, 1, rows - yPos);
			width = Mathf.Clamp(width, 1, columns - xPos);

            xPos = Mathf.Clamp(xPos, 0, columns - width);


			break;
		case Direction.East:
			xPos = room.Right;
			yPos = Random.Range (room.Bottom - height + 1, room.Top - 1);
			//maxLength = columns - startXPos - roomWidth.m_Min;

			height = Mathf.Clamp (height, 1, rows - yPos);
			width = Mathf.Clamp (width, 1, columns - xPos);
			yPos = Mathf.Clamp (yPos, 0, rows - height);

			break;
		case Direction.South:
			xPos = Random.Range (room.Left - width + 1, room.Right - 1);
			yPos = room.Bottom - height;
			//maxLength = startYPos - roomHeight.m_Min;

			height = Mathf.Clamp(height, 1, rows - yPos);
			width = Mathf.Clamp(width, 1, columns - xPos);
            xPos = Mathf.Clamp(xPos, 0, columns - width);


			break;
		case Direction.West:
			xPos = room.Left - width;
			yPos = Random.Range (room.Bottom - height + 1, room.Top - 1);

			height = Mathf.Clamp (height, 1, rows - yPos);
			width = Mathf.Clamp (width, 1, columns - xPos);
			yPos = Mathf.Clamp (yPos, 0, rows - height);
				
			break;
		}

	}
}

