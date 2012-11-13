using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour 
{
	public Transform player;	
	public Movement_Controller playerController;
	public int gridSizeX = 10;
	public int gridSizeY = 5;
	public Transform myCamera;
	public AstarPath graph;
	public List<TilePiece> map = new List<TilePiece>();
	private MapGenerator instance;
	
	
	private Vector3 startLocation = new Vector3(0, 0, 0);
	private Vector3 currentGenLocation;
	
	public List<RadiusMapPeice> radiusMapTiles = new List<RadiusMapPeice>();
	public int maxBlockSizeX = 3;
	public int maxBlockSizeY = 3;
	public int minBlockSizeX = 1;
	public int minBlockSizeY = 1;
	
	public int randomRoomCount;
	public int minRooms = 8;
	public int maxRoom = 20;
	
	private float startTime;
	private int roomCounter = 0;
	
	private Vector2 currentTile;
	_PoolingManager objectManagement;
	
	void Start()
	{
		objectManagement = _PoolingManager.Instance;
		instance = this;
		//StartCoroutine(Generate());
	}
	
	void Degenerate()
	{
		for(int i = 0; i < allObjects.Count; i++)
		{	
			objectManagement.DeactivatePooledItem("MapTile", allObjects[i]);
		}
		for(int i = 0; i < tilesPlaced.Count; i++)
		{
			objectManagement.DeactivatePooledItem("4_DoorRoom", tilesPlaced[i]);
		}
		tilesPlaced.Clear();
		mapGenerated = false;
	}
	
	private int neededSpecialRooms = 3;
	private int possibleSpecialRooms = 0;
	private List<GameObject> allObjects = new List<GameObject>();
	
	//Image names for the map pieces.
	private string bossRoomEntered = "bossRoom_Entered";
	private string bossRoomUnEntered = "bossRoom_UnEntered";
	
	private string itemRoomUnlocked = "itemRoom_Entered";
	private string itemRoomEntered_Unlocked = "itemRoom_Entered_Unlocked";
	private string itemRoomUnEntered = "itemRoom_UnEntered";
	
	private string shopRoomEntered = "shopRoom_Entered";
	private string shopRoomUnEntered = "shopRoom_UnEntered";
	
	private string discoveredRoom = "Map_NormalRoom";
	private string undiscoveredRoom = "grimloc_UnexploredRoom";
	private string currentRoom = "Map_CurrentLocation";
	private string emptyRoom = "Empty";
	
	IEnumerator Generate()
	{
		startLocation = this.transform.localPosition;
		itemRoomSet = false;
		bossRoomSet = false;
		shopRoomSet = false;
		
			possibleSpecialRooms = 0;
			if(mapGenerated)
				Degenerate();
		
		mapGenerated = false;
		allObjects = new List<GameObject>();
		randomRoomCount = UnityEngine.Random.Range(minRooms, maxRoom);
		maxBlockSizeX = UnityEngine.Random.Range(3, gridSizeX);
		maxBlockSizeY = UnityEngine.Random.Range(Mathf.Min(1, (gridSizeX-3) ), gridSizeY);
		
		minBlockSizeX = UnityEngine.Random.Range(0, 3);
		minBlockSizeY = UnityEngine.Random.Range(0, 3);
		
		//index location. Will be incremented in the loop.
		int loc = 0;
		int rndObject;
		
		map = new List<TilePiece>();
		for(int i = 0; i < gridSizeX; i++)
		{			
			for(int t = 0; t < gridSizeY; t++)
			{
				map.Add(new TilePiece());	
			}
		}
		
		for(int i = 0; i < gridSizeX; i++)
		{			 
			for(int t = 0; t < gridSizeY; t++)
			{
				//Is the current piece our bordering pieces? Set as Wall/Barrier.
				if(i == 0 || t == 0 || i%(gridSizeX-1) == 0 || t%(gridSizeY-1) == 0)
				{
					//Is a wall
					map[loc].ignore = true;
					map[loc].isRoom = false;
					map[loc].isBlankArea = false;
					map[loc].isSet = true;
				}
				
				else if(map[loc].isSet == false)
				{					
					map[loc].isRoom = false;
					map[loc].isBlankArea = true;
					map[loc].isSet = true;
					//If we are close to the wall in the X or Y direction we need to make sure no to go out of bounds.
					//Using +1 to account for the walls.
					
					
					int blockX = UnityEngine.Random.Range(minBlockSizeX, (int)(i*5421*Time.time)%maxBlockSizeX);
					int blockY = UnityEngine.Random.Range(minBlockSizeY, (int)(t*2313*Time.time)%maxBlockSizeY);
					
					for(int bX = 0; bX < blockX+1; bX++)
					{
						for(int bY = 0; bY < blockY+1; bY++)
						{
							//Assign specified locations as a building.
							if(((i+bX)*gridSizeY + ((t+bY))) < (gridSizeX*gridSizeY))
							{
								if( (bX == blockX || bY == blockY) )
								{
									map[ (((i)+bX)*gridSizeY) + ((t)+bY) ].isRoom = true;
									roomCounter++;
								}
								else
								{
									map[ (((i)+bX)*gridSizeY) + ((t)+bY) ].isBlankArea = true;
								}
								map[ (((i)+bX)*gridSizeY) + ((t)+bY) ].isSet = true;
								
							}							
						}
					}
				}	
				loc++;
			}
		}
		
		loc = 0;
		
		for(int i = 0; i < gridSizeX; i++)
		{			 
			for(int t = 0; t < gridSizeY; t++)
			{
				if(map[loc].ignore)
				{
					map[loc].roomType = RoomType.Empty;
					//map[loc].myObject = (GameObject)Instantiate(wall[0], currentGenLocation, Quaternion.identity);
				}
				else if(map[loc].isBlankArea)
				{
					map[loc].isRoom = false;
					map[loc].roomType = RoomType.Empty;
					//Is a building
					int typeNum = 0;
					int buildType = 0;
					if(map[ ((i+1)*gridSizeY + t) ].isBlankArea)
						buildType+= 1;	//Right
					if(map[ ((i-1)*gridSizeY + t) ].isBlankArea)
						buildType+= 10;	//Left
					if(map[ (i*gridSizeY + (t+1)) ].isBlankArea)
						buildType+= 100;	//Up
					if(map[ (i*gridSizeY + (t-1)) ].isBlankArea)
						buildType+= 1000;//Down
					switch(buildType)
					{
					case 1:
					case 11:
						break;
					}
				}
				else if(map[loc].isRoom)
				{
					int typeNum = 0;
					int roomType = 0;
					DoorPlacement doors = new DoorPlacement();
					map[loc].roomType = RoomType.General;
					if(map[ ((i+1)*gridSizeY + t) ].isRoom)
						roomType+= 1;	//Right 
					if(map[ ((i-1)*gridSizeY + t) ].isRoom)
						roomType+= 10;	//Left
					if(map[ (i*gridSizeY + (t+1)) ].isRoom)
						roomType+= 100;	//Up
					if(map[ (i*gridSizeY + (t-1)) ].isRoom)
						roomType+= 1000;//Down
					
					switch(roomType)
					{
					//Suitable locations for item rooms.
					case 1:
						//Right
						typeNum = 20;
						possibleSpecialRooms++;
						map[loc].roomType = RoomType.CanBeSpecial;
						doors = new DoorPlacement(false, true, false, false);
						break;
					case 10:
						//Left
						typeNum = 20;
						possibleSpecialRooms++;
						map[loc].roomType = RoomType.CanBeSpecial;
						doors = new DoorPlacement(true, false, false, false);
						break;
					case 100:
						//Up
						typeNum = 20;
						map[loc].roomType = RoomType.CanBeSpecial;
						possibleSpecialRooms++;
						doors = new DoorPlacement(false, false, true, false);
						break;
					case 1000: 
						//Down
						typeNum = 20;
						map[loc].roomType = RoomType.CanBeSpecial;
						possibleSpecialRooms++;
						doors = new DoorPlacement(false, false, false, true);
						break;
					case 11:
						//Left Right 1
						typeNum = 10;
						doors = new DoorPlacement(true, true, false, false);
						break;
					case 101:
						//Up Right 2
						typeNum = 10;
						doors = new DoorPlacement(false, true, true, false);
						break;
					case 110:
						//Left Up 3
						typeNum = 10;
						doors = new DoorPlacement(true, false, true, false);
						break;
					case 111:
						//Left Right Up 4
						doors = new DoorPlacement(true, true, true, false);
						typeNum = 10;
						break;
					case 1001:
						//Down Right 5
						doors = new DoorPlacement(false, true, false, true);
						typeNum = 10;
						break;
					case 1010:
						//Down Left 6
						doors = new DoorPlacement(true, false, false, true);
						typeNum = 10;
						break;
					case 1011:
						//Down Left Right 7
						doors = new DoorPlacement(true, true, false, true);
						typeNum = 10;
						break;
					case 1100:
						//Down Up 8
						doors = new DoorPlacement(false, false, true, true);
						typeNum = 10;
						break;
					case 1101:
						//Down Up Right 9 
						doors = new DoorPlacement(false, true, true, true);
						typeNum = 10;
						break;
					case 1110:
						//Down Up Left 10
						doors = new DoorPlacement(true, false, true, true);
						typeNum = 10;
						break;
					case 1111:
						//Down Up Left Right 11
						doors = new DoorPlacement(true, true, true, true);
						typeNum = 10;
						break;
					}
					map[loc].myTypeNumber = typeNum;
					map[loc].doors = doors;
					if(map[loc].roomType != RoomType.CanBeSpecial)
					{
						map[loc].roomType = RoomType.General;
					}
					if(typeNum < 20)
						typeNum = 10;
				}
				loc++;
			}
		}
			
			
			//We want more then 3 rooms that can be our special rooms.
		if(possibleSpecialRooms <= neededSpecialRooms)
		{
			StartCoroutine(Generate());
				yield return null;
		}
		else
		{
			roomCounter = 0;
			GameObject tempObj;
			loc = 0;
			
			itemRoomSet = false;
			bossRoomSet = false;
			shopRoomSet = false;
			
			int itemRoomCount = 0;
			int shopRoomCount = 0;
			int bossRoomCount = 0;
			
			int rndToSet = 0;
			int countingRooms = 0;
			totalGridUsed = 0;
			
			int firstRow = 0;
			int lastRow = 0;
			int firstRoom = 0;
			int lastRoom = 0;
			bool currentTileSet = false;
			currentTile= new Vector2(0, 0);
			int bossTileRow = 0;
			int bossTileCol = 0;
			
			for(int i = 0; i < gridSizeX; i++)
			{			 
				for(int t = 0; t < gridSizeY; t++)
				{
					bool specialRoomWasSet = false;
					if(map[loc].isRoom)
					{
						map[ loc ].x = i;
						map[ loc ].y = t;
						countingRooms++;
					}
					if(countingRooms > randomRoomCount)
					{
						//Set to blank.. Remove all doors.
						map[loc].isRoom = false;
						map[loc].isBlankArea = true;
						map[loc].doors = new DoorPlacement(false, false, false, false);
					}
					else
					{
						if(map[loc].isRoom)
						{
							if(firstRow == 0)
							{
								firstRow = i;
								firstRoom = loc;
							}
							lastRoom = t;
							lastRow = i;
						}
				
						totalGridUsed++;
						if(map[loc].myTypeNumber == 20)
						{
							rndToSet = UnityEngine.Random.Range(0, 3);
			
							if(!itemRoomSet)
							{
								if(rndToSet == 2)
								{
									map[loc].roomType = RoomType.Item;
									map[loc].myTypeNumber = 7;
									specialRoomWasSet = true;
									itemRoomCount++;
									//if(itemRoomCount >= maxItemRooms)
										itemRoomSet = true;
								}
							}
							
							if(!bossRoomSet && !specialRoomWasSet)
							{
								if(rndToSet == 1)
								{
									map[loc].myTypeNumber = 4;
									map[loc].roomType = RoomType.Boss;
									specialRoomWasSet = true;
									bossRoomCount++;
									//if(bossRoomCount >= maxBossRooms)
										bossRoomSet = true;
									bossTileRow = i;
									bossTileCol = t;
								}
							}
							
							if(!shopRoomSet && !specialRoomWasSet)
							{
								if(rndToSet == 0)
								{
									map[loc].myTypeNumber = 9;
									map[loc].roomType = RoomType.Shop;
									specialRoomWasSet = true;
									shopRoomCount++;
									//if(shopRoomCount >= maxShopRooms)
										shopRoomSet = true;
								}
							}
						}
						if(roomCounter >= randomRoomCount)
						{	
							if(t < gridSizeX)
							{
								map[loc].myObject = null;
							}
							else
								yield return null;
						}
						
					}
					loc++;
				}
			}
			
			map[(lastRow)*gridSizeY +lastRoom].doors.downDoor = false;
			map[(lastRow)*gridSizeY +lastRoom].doors.rightDoor = false;
			
			//Handle the 2nd to last Row of Rooms.. So they don't connect to nothing.
			for( int i = lastRow-1; i < lastRow; i++)
			{
				for(int t = lastRoom+1; t < gridSizeY; t++)
				{
					loc = (i)*gridSizeY + (t);
					if(map[loc].isRoom)
					{
						map[loc].doors.rightDoor = false;
					}
				}
			}
			
			//Handle the last row of rooms so they cannot connect to nothing.
			for(int i = lastRow; i < lastRow+1; i++)
			{
				for(int t = 0; t < gridSizeY; t++)
				{
					loc = (i)*gridSizeY + (t);
					if(map[loc].isRoom)
					{
						map[loc].doors.rightDoor = false;
					}
				}
			}
					
			/*
			#region Check for group connections
			bool needsRegeneration = false;
			int groupCounter = 0;
			int lastGroupCount = 0;
			//List<bool> rowConnected = new List<bool>();
			for(int i = firstRow; i <= lastRow; i++)	//Only need to deal with the rows that contain our rooms.
			{
				groupCounter = 0;
				lastGroupCount = 0;
				List<TilePiece> tileGroups = new List<TilePiece>();	//Create groups of rooms that are connected..
				for(int t = 0; t < gridSize; t++)
				{
					//rowConnected.Add(true);
					loc = (i)*gridSize + (t);
					if(map[loc].isRoom)
					{
						bool isARoom = true;
						//Set the X,Y Location; So we can references it later.
						map[ loc ].x = i;
						map[ loc ].y = t;
						tileGroups.Add(map[ loc ]);	//Add the current..
					}
					
					else
					{
						bool isConnected = false;
						if(tileGroups.Count > 0)
						{
							for(int j = 0; j < tileGroups.Count; j++)
							{
								if( map[((tileGroups[j].x+1)*gridSize) + tileGroups[j].y ] != null )
								{	
									if( map[ ((tileGroups[j].x+1)*gridSize) + tileGroups[j].y ].isRoom)	//The Right is a room.
									{
										isConnected = true;
									}
								}
								
								if(i > 0)
									if( map[((tileGroups[j].x-1)*gridSize) + tileGroups[j].y ] != null )
									{	
										if( map[ ((tileGroups[j].x-1)*gridSize) + tileGroups[j].y ].isRoom)	//The Left is a room.
										{
											if(rowConnected[rowConnected.Count-2] && (i-1) > firstRow )
												isConnected = true;
										}
									}
							}
							//None of the tiles connected to the Right.. Or Left.
							if(!isConnected)
							{
								needsRegeneration = true;								
							}
							else
							{
								
							}
							tileGroups = new List<TilePiece>();
							groupCounter++;
						}
					}
					
				}
				#endregion
			*/
			
			loc = 0;
			countingRooms = 0;			
			itemRoomCount = 0;
			shopRoomCount = 0;
			bossRoomCount = 0;
			roomCounter = 0;
			
			shopRoomSet = false;
			bossRoomSet = false;
			itemRoomSet = false;
			for(int i = 0; i < gridSizeX; i++)
			{			 
				for(int t = 0; t < gridSizeY; t++)
				{

					if(map[loc].isRoom)
					{
						countingRooms++;
					}
					if(countingRooms > randomRoomCount)
					{
						// Do Nothing
					}
					else
					{
						tempObj = ReadMap(i, t);
						if(tempObj != null)
						{
							allObjects.Add(tempObj);
							tempObj.name = ""+loc;
							
							//map[loc].myObject = tempObj;
							UISprite myImage = tempObj.GetComponent(typeof(UISprite)) as UISprite;
							map[loc].myMapImage = myImage;
							if(map[loc].roomType == RoomType.Boss && !bossRoomSet)
							{
								
									map[loc].myMapImage.spriteName = bossRoomUnEntered;
									tempObj.name = "0_BossRoom";
								
								bossRoomSet = true;
							}
							else if(map[loc].roomType == RoomType.Shop && !shopRoomSet)
							{
									map[loc].myMapImage.spriteName = shopRoomUnEntered;
									tempObj.name = "0_ShopRoom";
								
								shopRoomSet = true;
							}
							else if(map[loc].roomType == RoomType.Item && !itemRoomSet)
							{
									map[loc].myMapImage.spriteName = itemRoomUnEntered;
									tempObj.name = "0_ItemRoom";
								
								itemRoomSet = true;
							}
							else if(map[loc].roomType == RoomType.General)
							{		
								map[loc].myMapImage.spriteName = undiscoveredRoom;
								rndToSet = UnityEngine.Random.Range(firstRow+1, lastRow);
								if(bossTileCol > 3)
								{
									if(!currentTileSet && i < 3 && t == rndToSet)
									{
										lastUpdatedTile = new Vector2(i, t);
										currentTile = new Vector2(i, t);
										currentTileSet = true;
										map[loc].myMapImage.spriteName = currentRoom;
									}
								}
								else
								{
									if(!currentTileSet &&  i > 3 && t == rndToSet)
									{
										lastUpdatedTile = new Vector2(i, t);
										currentTile = new Vector2(i, t);
										currentTileSet = true;
										map[loc].myMapImage.spriteName = currentRoom;
									}
								}
								
							}
							else if(map[loc].roomType == RoomType.Empty)
							{
								map[loc].myMapImage.spriteName = emptyRoom;
							}
							else
							{
								map[loc].myMapImage.spriteName = undiscoveredRoom;
							}

						}
					}
					loc++;
				}
			}
			mapGenerated = true;	
			loc = 0;

			myTestList = new List<TilePiece>();
			for(int i = 0; i < gridSizeX; i++)
			{			 
				for(int t = 0; t < gridSizeY; t++)
				{
					if(loc == firstRoom)
					{
						if(map[ ((i+1)*gridSizeY + t) ].isRoom)
						{
							if( !myTestList.Contains(map[((i+1)*gridSizeY + t)]) )
							{
								myTestList.Add(map[((i+1)*gridSizeY + t)]);
							}
						}
						if(map[ ((i-1)*gridSizeY + t) ].isRoom)
						{
							if( !myTestList.Contains(map[((i-1)*gridSizeY + t)] ) )
							{
								myTestList.Add(map[((i-1)*gridSizeY + t)]);
							}
						}
						if(map[ (i*gridSizeY + (t+1)) ].isRoom)
						{
							if( !myTestList.Contains(map[(i*gridSizeY + (t+1))] ) )
							{
								myTestList.Add(map[(i*gridSizeY + (t+1))]);
							}
						}
						if(map[ (i*gridSizeY + (t-1)) ].isRoom)
						{
							if( !myTestList.Contains(map[ (i*gridSizeY + (t-1))] ) )
							{
								myTestList.Add(map[ (i*gridSizeY + (t-1))]);
							}
						}
					}
				loc++;
				}
			}
			if(!currentTileSet)
			{
				int rndI = UnityEngine.Random.Range(firstRow, lastRow);
				int rndT = UnityEngine.Random.Range(1, gridSizeY);
				while(!currentTileSet)
				{
					int i = rndI;
					int t = rndT;
					if(i < gridSizeX-1 && !currentTileSet)
					{
						if(map[ ((i+1)*gridSizeY + t) ].roomType == RoomType.General && map[((i+1)*gridSizeY + t)].myMapImage != null)
						{
							lastUpdatedTile = new Vector2( (i+1), t);
							currentTile = new Vector2( (i+1), t);
							currentTileSet = true;
							map[((i+1)*gridSizeY + t)].myMapImage.spriteName = currentRoom;
						}
					}
					else if(i > 0 && !currentTileSet)
					{
						if(map[ ((i-1)*gridSizeY + t) ].roomType == RoomType.General && map[((i-1)*gridSizeY + t)].myMapImage != null)
						{
							lastUpdatedTile = new Vector2((i-1), t);
							currentTile = new Vector2((i-1), t);
							currentTileSet = true;
							map[((i-1)*gridSizeY + t)].myMapImage.spriteName = currentRoom;
						}
					}
					else if(t < gridSizeY && !currentTileSet)
					{
						if(map[ (i*gridSizeY + (t+1)) ].roomType == RoomType.General && map[(i*gridSizeY + (t+1))].myMapImage != null)
						{
							lastUpdatedTile = new Vector2(i, (t+1) );
							currentTile = new Vector2(i, (t+1) );
							currentTileSet = true;
							map[(i*gridSizeY + (t+1))].myMapImage.spriteName = currentRoom;
						}
					}
					else if(t > 0 && !currentTileSet)
					{
						if(map[ (i*gridSizeY + (t-1)) ].roomType == RoomType.General && map[(i*gridSizeY + (t-1))].myMapImage != null)
						{
							lastUpdatedTile = new Vector2(i, (t-1));;
							currentTile = new Vector2(i, (t-1));
							currentTileSet = true;
							map[(i*gridSizeY + (t-1))].myMapImage.spriteName = currentRoom;
						}
					}
					if(rndI > lastRow)
						rndI = 0;
					rndI++;
					
					if(rndT > 6)
						rndT = 0;
					rndT++;
					
					yield return new WaitForSeconds(0.001f);
				}
			}
			totalRoomsTraversed = 0;
			traversedTiles = new List<TilePiece>();
			quitOut = false;
			while(myTestList.Count > 0 && !quitOut)
			{
				int i = myTestList[0].x;
				int t = myTestList[0].y;
				if(map[ ((i+1)*gridSizeY + t) ].isRoom)
				{
					if( !myTestList.Contains(map[((i+1)*gridSizeY + t)]) && !traversedTiles.Contains(map[((i+1)*gridSizeY + t)]) )
					{
						myTestList.Add(map[((i+1)*gridSizeY + t)]);
					}
				}
				if(i > 0)
				{
					if(map[ ((i-1)*gridSizeY + t) ].isRoom)
					{
						if( !myTestList.Contains(map[((i-1)*gridSizeY + t)] ) && !traversedTiles.Contains(map[((i-1)*gridSizeY + t)] ) )
						{
							myTestList.Add(map[((i-1)*gridSizeY + t)]);
						}
					}
				}
				if(map[ (i*gridSizeY + (t+1)) ].isRoom)
				{
					if( !myTestList.Contains(map[(i*gridSizeY + (t+1))] ) && !traversedTiles.Contains(map[(i*gridSizeY + (t+1))] ) )
					{
						myTestList.Add(map[(i*gridSizeY + (t+1))]);
					}
				}
				if(t > 0)
				{
					if(map[ (i*gridSizeY + (t-1)) ].isRoom)
					{
						if( !myTestList.Contains(map[ (i*gridSizeY + (t-1))] ) && !traversedTiles.Contains(map[(i*gridSizeY + (t-1))] ))
						{
							myTestList.Add(map[ (i*gridSizeY + (t-1))]);
						}
					}
				}
				traversedTiles.Add(myTestList[0]);
				myTestList.RemoveAt(0);
				totalRoomsTraversed++;
				if(totalRoomsTraversed > randomRoomCount)
				{
					quitOut = true;
				}
				yield return new WaitForSeconds(0.001f);
			}
			if(totalRoomsTraversed != randomRoomCount || !bossRoomSet || !itemRoomSet || !shopRoomSet)// || !currentTileSet)
			{
				StartCoroutine(Generate());
				yield return null;
			}
			else
			{
				GameObject myobj;
				for(int i = 0; i < gridSizeX-1; i++)
				{			 
					for(int t = 0; t < gridSizeY-1; t++)
					{
						myobj = DisplayPlayTiles(i, t);
						if(myobj != null)
							tilesPlaced.Add(myobj);
						yield return new WaitForSeconds(0.001f);
					}
				}
				lastUpdatedTile = currentTile;
				lastTileUsed = currentTile;
				int curTile = (int)((currentTile.x*gridSizeY)+currentTile.y);
				map[curTile].myObject.transform.FindChild("Room_Graphics").GetChild(0).gameObject.active = true;
				map[curTile].myObject.transform.localPosition =  placedTileLocation.position;
				yield return new WaitForSeconds(0.1f);
				graph.Scan();
				yield return new WaitForSeconds(0.1f);
				if(map[curTile].myObject != null)
					map[curTile].myObject.transform.FindChild("Room_Graphics").GetChild(0).gameObject.active = false;
				//SetRadiusMap();
				StartCoroutine(UpdateCurrent());
			}
		}
		yield return new WaitForSeconds(0.001f);
	}
	
	private List<GameObject> tilesPlaced = new List<GameObject>();
	private bool moveUp = false;
	private bool moveDown = false;
	private bool moveRight = false;
	private bool moveLeft = false;
	
	public void MoveUp()
	{
		moveUp = true;
	}
	public void MoveDown()
	{
		moveDown = true;
	}
	public void MoveLeft()
	{
		moveLeft = true;
	}
	public void MoveRight()
	{
		moveRight = true;
	}
	public void SetRadiusMap()
	{
		//Our current tile..
		int curTile = (int)((currentTile.x*gridSizeY)+currentTile.y);
		UISprite temp;
		UISprite radius;
		for(int i = 0; i < radiusMapTiles.Count; i++)
		{
			switch(radiusMapTiles[i].myName)
			{
			case "Current":
				radius = radiusMapTiles[i].myImage;
				temp = map[curTile].myMapImage;
				radius.spriteName = temp.spriteName;
				break;
			case "Up":
				if(map[ (int)((currentTile.x)*gridSizeY + currentTile.y+1) ].isRoom)
				{
					radiusMapTiles[i].myObject.active = true;
					radius = radiusMapTiles[i].myImage;
					
					temp = map[ (int)((currentTile.x)*gridSizeY + currentTile.y+1)].myMapImage;
					radius.spriteName = temp.spriteName;
				}
				else
				{
					radiusMapTiles[i].myObject.active = false;
				}
				break;
			case "Down":
				if(map[ (int) ((currentTile.x*gridSizeY) + currentTile.y-1) ].isRoom)
				{
					radiusMapTiles[i].myObject.active = true;
					radius = radiusMapTiles[i].myImage;
					
					temp = map[(int) ((currentTile.x*gridSizeY) + currentTile.y-1)].myMapImage;
					radius.spriteName = temp.spriteName;
				}
				else
				{
					radiusMapTiles[i].myObject.active = false;
				}
				break;
			case "Left":
				if(map[ (int)((currentTile.x-1)*gridSizeY + currentTile.y) ].isRoom)
				{
					radiusMapTiles[i].myObject.active = true;
					radius = radiusMapTiles[i].myImage;
					temp = map[(int)((currentTile.x-1)*gridSizeY + currentTile.y)].myMapImage;
					radius.spriteName = temp.spriteName;
				}
				else
				{
					radiusMapTiles[i].myObject.active = false;
				}
				break;
			case "Right":
				if(map[ (int)((currentTile.x+1)*gridSizeY + currentTile.y) ].isRoom)
				{
					radiusMapTiles[i].myObject.active = true;
					radius = radiusMapTiles[i].myImage;
					temp = map[(int)((currentTile.x+1)*gridSizeY + currentTile.y)].myMapImage;
					radius.spriteName = temp.spriteName;
				}
				else
				{
					radiusMapTiles[i].myObject.active = false;
				}
				break;
			}
		}
	}
	
	private bool recentlyMoved = false;
	public IEnumerator UpdateCurrent()
	{
		while(mapGenerated)
		{
			bool up = false;
			bool down = false;
			bool right = false;
			bool left = false;
			if(!recentlyMoved)
			{
				if(moveDown)
				{
					moveDown = false;
					if(map[ (int) ((currentTile.x*gridSizeY) + currentTile.y-1) ].isRoom)
					{
						currentTile.y = currentTile.y-1;
					}
					down = true;
				}
				
				if(moveUp)
				{
					moveUp = false;
					if(map[ (int)((currentTile.x)*gridSizeY + currentTile.y+1) ].isRoom)
					{
						currentTile.y = currentTile.y+1;
					}
					up = true;
				}
				
				if(moveRight)
				{
					moveRight = false;
					if(map[ (int)((currentTile.x+1)*gridSizeY + currentTile.y) ].isRoom)
					{
						currentTile.x = currentTile.x+1;
					}
					right = true;
				}
				
				if(moveLeft)
				{
					moveLeft = false;
					if(map[ (int)((currentTile.x-1)*gridSizeY + currentTile.y) ].isRoom)
					{
						currentTile.x = currentTile.x-1;
					}
					left = true;
				}
				int curTile = (int)((currentTile.x*gridSizeY)+currentTile.y);
				int lastTile = (int)((lastUpdatedTile.x*gridSizeY)+lastUpdatedTile.y);
				
				if(curTile > lastTile || curTile < lastTile)
				{
					recentlyMoved = true;
					lastTileUsed = lastUpdatedTile;
					if(map[curTile].myObject != null)
					{
						//map[lastTile].myObject.transform.localPosition = new Vector3(lastUpdatedTile.x*playTileY, 0, lastUpdatedTile.y*playTileX);
						UISprite temp = map[curTile].myMapImage;
						
						if(temp.spriteName == discoveredRoom || temp.spriteName == undiscoveredRoom || temp.spriteName == currentRoom)
							map[curTile].myMapImage.spriteName = currentRoom;
						
						if(temp.spriteName == bossRoomUnEntered)
						{
							map[curTile].myMapImage.spriteName = bossRoomEntered;
						}
						if(temp.spriteName == shopRoomUnEntered)
						{
							map[curTile].myMapImage.spriteName = shopRoomEntered;
						}
						if(temp.spriteName == itemRoomUnEntered || temp.spriteName == itemRoomUnlocked)
						{
							map[curTile].myMapImage.spriteName = itemRoomEntered_Unlocked;
						}
						
						if(lastTile > 0)
						{
							if(map[lastTile].myMapImage != null)
							{
								UISprite tempt = map[lastTile].myMapImage;
								if(tempt.spriteName == discoveredRoom || tempt.spriteName == undiscoveredRoom || tempt.spriteName == currentRoom)
								{
									map[lastTile].myMapImage.spriteName = discoveredRoom;
								}
								if(tempt.spriteName == bossRoomEntered)
								{
									map[lastTile].myMapImage.spriteName = bossRoomUnEntered;
								}
								if(tempt.spriteName == shopRoomEntered)
								{
									map[lastTile].myMapImage.spriteName = shopRoomUnEntered;
								}
								if(tempt.spriteName == itemRoomEntered_Unlocked)
								{
									map[lastTile].myMapImage.spriteName = itemRoomUnlocked;
								}
							}
						}
						lastUpdatedTile = currentTile;
						map[curTile].myObject.transform.FindChild("Room_Graphics").GetChild(0).gameObject.active = true;
						
						//Move the current tile to Left, Right, Top, Or bottom.. of the current location.
						//Move the Camera as well..
						Vector3 moveLocation;
						DoorLocation doorUsed; //All will the opposites of the door actually used..
						if(left)
						{
							moveLocation = new Vector3(playTileY, 0 , 0);
							doorUsed = DoorLocation.Right;
						}
						else if(right)
						{
							moveLocation = new Vector3(-playTileY, 0, 0);
							doorUsed = DoorLocation.Left;
						}
						else if(up)
						{
							moveLocation = new Vector3(0, 0, -playTileX);
							doorUsed = DoorLocation.Down;
						}
						else
						{
							moveLocation = new Vector3(0, 0, playTileX);
							doorUsed = DoorLocation.Up;
						}
						myCamera.position += moveLocation;
						map[lastTile].myObject.transform.localPosition += moveLocation;
						
						//This is where we actually place the first tile down.
						map[curTile].myObject.transform.localPosition =  placedTileLocation.position;
						yield return new WaitForSeconds(0.1f);
						graph.Scan();
						yield return new WaitForSeconds(0.1f);
						map[curTile].myObject.transform.FindChild("Room_Graphics").GetChild(0).gameObject.active = false;
						StartCoroutine( LerpToActualLocation(lastTileUsed, doorUsed) );
						//SetRadiusMap();
					}
				}
			}
			else
			{
				moveLeft = false;
				moveRight = false;
				moveUp = false;
				moveDown = false;
			}
			
			yield return new WaitForSeconds(0.02f);
		}
		yield return null;
	}

	private float timeToTransition = 0.5f;
	public IEnumerator LerpToActualLocation(Vector3 last, DoorLocation doorUsed)
	{
		playerController.pauseMovement = true;
		float transTime = Time.time;
		int curTile = (int)((currentTile.x*gridSizeY)+currentTile.y);
		int lastTile = (int)((last.x*gridSizeY)+last.y);
		Vector3 target = map[curTile].myObject.transform.FindChild("CameraLocation").position;
		player.localPosition = map[curTile].myObject.transform.FindChild(Enum.GetName(typeof(DoorLocation), doorUsed) ).position; 
			
		while(myCamera.position != target && Time.time-transTime < timeToTransition)
		{
			myCamera.position = Vector3.Lerp(myCamera.position, target, 8*Time.deltaTime);
			yield return new WaitForSeconds(0.01f);
		}
		map[lastTile].myObject.transform.localPosition = new Vector3(lastUpdatedTile.x*playTileY, 0, lastUpdatedTile.y*playTileX);
		yield return new WaitForSeconds(0.1f);
		playerController.pauseMovement = false;
		recentlyMoved = false;
		yield return null;
	}
	
	private Vector3 lastTileUsed;
	private List<TilePiece> myTestList = new List<TilePiece>();
	private List<TilePiece> traversedTiles = new List<TilePiece>();
	private bool quitOut = false;
	private int totalRoomsTraversed = 0;
	
	private int totalGridUsed = 0;
	public int maxShopRooms = 1;
	public int maxItemRooms = 1;
	public int maxBossRooms = 1;
	
	private bool itemRoomSet = false;
	private bool bossRoomSet = false;
	private bool shopRoomSet = false;
	
	private bool moveNegX;
	private bool moveX;
	private bool moveNegY;
	private bool moveY;
	
	
	private Vector2 lastUpdatedTile = new Vector2();
	
	private string holder;
	
	//Holder that keeps track of the players current index location.
	private int currentLocation;
	private int x = 5;
	private int y = 5;
	private string digits;
	
	private bool mapGenerated = false;
	public bool IsMapGenerated()
	{
		return mapGenerated;
	}
	private bool doorsOpen = true;
	public bool DoorsOpen
	{
		get{return doorsOpen;}
		set{doorsOpen = value;}
	}
	
	public int playTileX = 150;
	public int playTileY = 250;
	private Vector3 tileGenLocation = new Vector3(0, 0, 0);
	private Transform doorActivation;
	private Transform doorTrigger;
	private MoveTiles doorTriggerMover;
	private int bossRoomDoorID = 0;
	private int shopRoomDoorID = 0;	//Shop is actually set to 0 currently.
	private int itemRoomDoorID = 0;
	private int generalRoomDoorID = 2;
	private void SetDoors(TilePiece selectedTile, int i, int t, GameObject myObj)
	{
		tk2dSprite temp;
		RoomType room;
		doorActivation = myObj.transform.FindChild("Doors");
		for(int k = 0; k < doorActivation.childCount; k++)
		{
			doorActivation.GetChild(k).gameObject.SetActiveRecursively(false);
		}
		if(selectedTile.doors.leftDoor)
		{
			doorActivation.Find("LeftDoor").gameObject.SetActiveRecursively(true);
			doorTrigger = doorActivation.Find("LeftDoor").transform.GetChild(0);
			doorTriggerMover = doorTrigger.GetComponent(typeof(MoveTiles)) as MoveTiles;
			doorTriggerMover.mapGen = instance;
		
			temp = doorActivation.Find("LeftDoor").GetComponent(typeof(tk2dSprite)) as tk2dSprite;
			room = map[( (i-1)*gridSizeY + (t) )].roomType;
			SetDoorId(room, temp);
		}
		if(selectedTile.doors.rightDoor)
		{
			doorActivation.Find("RightDoor").gameObject.SetActiveRecursively(true);
			
			doorTrigger = doorActivation.Find("RightDoor").transform.GetChild(0);
			doorTriggerMover = doorTrigger.GetComponent(typeof(MoveTiles)) as MoveTiles;
			doorTriggerMover.mapGen = instance;
			
			temp = doorActivation.Find("RightDoor").GetComponent(typeof(tk2dSprite)) as tk2dSprite;
			room = map[( (i+1)*gridSizeY + (t) )].roomType;
			SetDoorId(room, temp);
		}
		if(selectedTile.doors.downDoor)
		{
			doorActivation.Find("UpDoor").gameObject.SetActiveRecursively(true);
			
			doorTrigger = doorActivation.Find("UpDoor").transform.GetChild(0);
			doorTriggerMover = doorTrigger.GetComponent(typeof(MoveTiles)) as MoveTiles;
			doorTriggerMover.mapGen = instance;
			
			temp = doorActivation.Find("UpDoor").GetComponent(typeof(tk2dSprite)) as tk2dSprite;
			room = map[( (i)*gridSizeY + (t+1) )].roomType;
			SetDoorId(room, temp);
		}
		if(selectedTile.doors.upDoor)
		{
			doorActivation.Find("DownDoor").gameObject.SetActiveRecursively(true);
			
			doorTrigger = doorActivation.Find("DownDoor").transform.GetChild(0);
			doorTriggerMover = doorTrigger.GetComponent(typeof(MoveTiles)) as MoveTiles;
			doorTriggerMover.mapGen = instance;
			
			temp = doorActivation.Find("DownDoor").GetComponent(typeof(tk2dSprite)) as tk2dSprite;
			room = map[( (i)*gridSizeY + (t-1) )].roomType;
			SetDoorId(room, temp);
		}
	}
	private void SetDoorId(RoomType room, tk2dSprite sprite)
	{
		sprite.scale = new Vector3(1, 1, 1);
		if(room == RoomType.Boss)
		{
			sprite.spriteId = bossRoomDoorID;
		}
		else if(room == RoomType.Item)
		{
			sprite.spriteId = shopRoomDoorID;
		}
		else if(room == RoomType.Shop)
		{
			sprite.spriteId = itemRoomDoorID;
		}
		else
		{
			sprite.spriteId = generalRoomDoorID;
			sprite.scale = new Vector3(1.24F, 1.24F, 1);
		}
	}
	GameObject DisplayPlayTiles(int i, int t)
	{
		int type = 0;
		TilePiece selectedTile;
		GameObject returnObject = null;
		currentLocation = ( (i)*gridSizeY + (t) );
		selectedTile = map[currentLocation];
		
		
		
		if(selectedTile.roomType == RoomType.Boss)
		{
			//Randomly pick a boss from the current levels pool.
			returnObject = objectManagement.ActivatePooledItem("4_DoorRoom");
			SetDoors(selectedTile, i, t, returnObject);
			
			if(returnObject == null)
				return null;
			returnObject.transform.localScale = new Vector3(returnObject.transform.lossyScale.x, returnObject.transform.lossyScale.y, returnObject.transform.lossyScale.z);

			tileGenLocation = new Vector3(i*playTileY, 0, t*playTileX);
			returnObject.transform.localPosition = tileGenLocation;
			//returnObject.transform.eulerAngles = new Vector3(0, 180, 180);
			
		}
		else if(selectedTile.roomType == RoomType.Item)
		{
			//Set tile to Item room.. Will require to randomly grab an item that the player doesn't already have, as well as is unlocked.
			returnObject = objectManagement.ActivatePooledItem("4_DoorRoom");
			SetDoors(selectedTile, i, t, returnObject);
			
			if(returnObject == null)
				return null;
			returnObject.transform.localScale = new Vector3(returnObject.transform.lossyScale.x, returnObject.transform.lossyScale.y, returnObject.transform.lossyScale.z);

			tileGenLocation = new Vector3(i*playTileY, 0, t*playTileX);
			returnObject.transform.localPosition = tileGenLocation;
			//returnObject.transform.eulerAngles = new Vector3(0, 180, 180);
			
		}
		else if(selectedTile.roomType == RoomType.Shop)
		{
			//Set Tile as Shop.. Will require to pull random items based on the player has unlocked.
			returnObject = objectManagement.ActivatePooledItem("4_DoorRoom");
			SetDoors(selectedTile, i, t, returnObject);
			
			if(returnObject == null)
				return null;
			returnObject.transform.localScale = new Vector3(returnObject.transform.lossyScale.x, returnObject.transform.lossyScale.y, returnObject.transform.lossyScale.z);

			tileGenLocation = new Vector3(i*playTileY, 0, t*playTileX);
			returnObject.transform.localPosition = tileGenLocation;
			//returnObject.transform.eulerAngles = new Vector3(0, 180, 180);
		}
		else if(selectedTile.isRoom)
		{								
			returnObject = objectManagement.ActivatePooledItem("4_DoorRoom");
			SetDoors(selectedTile, i, t, returnObject);
			
			if(returnObject == null)
				return null;
			returnObject.transform.localScale = new Vector3(returnObject.transform.lossyScale.x, returnObject.transform.lossyScale.y, returnObject.transform.lossyScale.z);

			tileGenLocation = new Vector3(i*playTileY, 0, t*playTileX);
			returnObject.transform.localPosition = tileGenLocation;
			//returnObject.transform.eulerAngles = new Vector3(0, 180, 180);
		}
		else if(selectedTile.isBlankArea)
		{				
			returnObject = null;
			if(returnObject == null)
				return null;
			//returnObject.transform.localScale = new Vector3(xSize, zSize, zSize);
			
			tileGenLocation = new Vector3((startLocation.x)+i*playTileY, playTileY, (startLocation.z)+t*playTileX);
			returnObject.transform.localPosition = tileGenLocation;
			//returnObject.transform.eulerAngles = new Vector3(0, 180, 180);
		}		
		
		selectedTile.myObject = returnObject;
		if(i == currentTile.x && t == currentTile.y)
		{
			returnObject.transform.localPosition = placedTileLocation.position;
		}
		return returnObject;
	}
	
	public Transform placedTileLocation;
	public int xSize = 30;
	public int zSize = 17;
	GameObject ReadMap(int i, int t)
	{
		int type = 0;
		TilePiece selectedTile;
		GameObject returnObject = null;
		currentLocation = ( (i)*gridSizeY + (t) );
		selectedTile = map[currentLocation];
		
		if(i == 0 && t == 0)
		{
			Vector3 charPos = new Vector3(0, 2, 0);
			charPos += currentGenLocation;
			//player.transform.position = charPos;
		}
		if(selectedTile.isRoom)
		{					
			returnObject = objectManagement.ActivatePooledItem("MapTile");
			if(returnObject == null)
				return null;
			returnObject.transform.localScale = new Vector3(xSize, zSize, zSize);
			//We have a road
			currentGenLocation = new Vector3((startLocation.x)+i*xSize, 1, (startLocation.z)+t*zSize);
			returnObject.transform.localPosition = currentGenLocation;
		}
		else
		{					
			returnObject = objectManagement.ActivatePooledItem("MapTile");
			if(returnObject == null)
				return null;
			returnObject.transform.localScale = new Vector3(xSize, zSize, zSize);
			//We have a road
			currentGenLocation = new Vector3((startLocation.x)+i*xSize, 1, (startLocation.z)+t*zSize);
			
			returnObject.transform.localPosition = currentGenLocation;
		}
		if(returnObject != null)
			returnObject.transform.parent = this.transform;
		return returnObject;
	}
}

[System.Serializable]
public class RadiusMapPeice
{
	public string myName = "";
	public GameObject myObject;
	public UISprite myImage;
}

[System.Serializable]
public class TilePiece
{
	public int x, y;
	public bool isSet = false;
	public bool isBlankArea = false;
	public bool ignore = false;
	public bool isRoom = false;
	
	public RoomType roomType = RoomType.General;
	public int myTypeNumber = 0;
	public GameObject myObject;
	
	
	public UISprite myMapImage;
	public DoorPlacement doors;
	public bool canBeShown = false;
	
	
	//Need to mark window locations and Door locations for each prefab and put them in here..
	//It will all be managed in here with some simple booleans.
}

[System.Serializable]
public class DoorPlacement
{
	public DoorPlacement()
	{ /**/ }
		
	public DoorPlacement(bool left, bool right, bool down, bool up)
	{
		leftDoor = left;
		rightDoor = right;
		upDoor = up;
		downDoor = down;
	}
	public bool leftDoor;
	public bool rightDoor;
	public bool upDoor;
	public bool downDoor;
}


public enum RoomType
{
	Boss, Shop, Item, General, Empty, CanBeSpecial
}