using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

    public int mapWidth = 200;
    public int mapHeight = 200;
    [Range(10, 500)]
    public int attempts = 200;
    public int maxPathRemove = 200;
    public Tilemap wallsMap;
    public Tilemap groundMap;
    public Tilemap featuresMap;
    public enum TileType { NONE, PATH, ROOM, MERGEDPATH, MERGEDROOM };
    public TileType[,] mergedMap;
    public Tile[] GroundTiles;
    public Tile backgroundTile;
    // Use this for initialization
    public List<Room> roomList;
    public List<Connection> connections;
    public List<Connection> roomToRoomConnections;
    public int roomMinWidth = 4, roomMinHeight = 4, roomMaxHeight = 10, roomMaxWidth = 15;

    public bool DisplayMazeGen = true;
    public bool DisplayFill = true;
    public bool DisplayUncarve = true;

    public GameManager gameMan;
    
    void Start() {

        mergedMap = new TileType[mapWidth + 30, mapHeight + 30];
        connections = new List<Connection>();
        roomToRoomConnections = new List<Connection>();
        StartCoroutine(GenerateMap());


    }


    // Update is called once per frame
    void Update() {

    }

    public IEnumerator GenerateMap()
    {
        //FillBackground();
        yield return GenerateRooms();
        yield return MazeFill();
        yield return ConnectMap();
        yield return Uncarve();
        

        RoomGenerator rGen = GetComponent<RoomGenerator>();
        foreach(Room r in roomList)
        {
            rGen.Generate(r);
        }
        yield return ExpandPath();
        rGen.GeneratePaths(this);
        Room start = roomList[Random.Range(0, roomList.Count)];
        gameMan.SignalReady(start);
        


    }
    public IEnumerator ExpandPath()
    {

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (mergedMap[x, y] == TileType.NONE)
                {
                    
                   if(PathNeighborCount(ref mergedMap,x,y)>=7)
                    {
                        mergedMap[x, y] = TileType.MERGEDPATH;
                        groundMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[1]);
                        continue;
                    }
                   /*
                  
                   if(x-1 >= 0 && mergedMap[x-1,y] == TileType.MERGEDPATH)
                   {
                        if(IsOccupied(wallsMap,x+1,y))
                        {
                            mergedMap[x, y] = TileType.MERGEDPATH;
                            groundMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[1]);
                        }
                   }
                   else if(mergedMap[x+1,y] == TileType.MERGEDPATH)
                    {
                        if(IsOccupied(wallsMap, x-1, y))
                        {
                            mergedMap[x, y] = TileType.MERGEDPATH;
                            groundMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[1]);
                        }
                    }
                   if(y-1 >=0 && mergedMap[x,y-1]== TileType.MERGEDPATH)
                   {
                        if(IsOccupied(wallsMap,x,y-1))
                        {
                            mergedMap[x, y] = TileType.MERGEDPATH;
                            groundMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[1]);
                        }
                   }
                    else if(mergedMap[x, y + 1] == TileType.MERGEDPATH)
                   {
                        if (IsOccupied(wallsMap, x, y + 1))
                        {
                            mergedMap[x, y] = TileType.MERGEDPATH;
                            groundMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[1]);
                        }
                    }
                    */
                }
            }
        }

        yield return null;
    }
    public IEnumerator Uncarve()
    {
        Queue<Vector3Int> tilesToCheck = new Queue<Vector3Int>();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if(mergedMap[x,y] == TileType.MERGEDPATH)
                {
                    tilesToCheck.Enqueue(new Vector3Int(x, y, 0));
                }
            }
        }
        int pathsRemoved = 0;
        int maxIteration = 2000;
        int iterations = 0;
        while(tilesToCheck.Count!= 0 && pathsRemoved < maxPathRemove)
        {
            Vector3Int cur = tilesToCheck.Dequeue();
            if(TileOrthogNeighborCount(groundMap,cur.x,cur.y)==1)
            {
                groundMap.SetTile(cur, null);
                mergedMap[cur.x, cur.y] = TileType.NONE;
                pathsRemoved++;
            }
            if(cur.y+1 <= mapHeight && mergedMap[cur.x,cur.y+1] == TileType.MERGEDPATH){ tilesToCheck.Enqueue(new Vector3Int(cur.x, cur.y + 1, 0));}
            else if (cur.y-1 >= 0 && mergedMap[cur.x, cur.y - 1] == TileType.MERGEDPATH) { tilesToCheck.Enqueue(new Vector3Int(cur.x, cur.y - 1, 0)); }
            else if (cur.x +1 <= mapWidth && mergedMap[cur.x+1, cur.y] == TileType.MERGEDPATH) { tilesToCheck.Enqueue(new Vector3Int(cur.x+1, cur.y, 0)); }
            else if (cur.x-1 >= 0 && mergedMap[cur.x-1, cur.y] == TileType.MERGEDPATH) { tilesToCheck.Enqueue(new Vector3Int(cur.x-1, cur.y, 0)); }

            iterations++;
            if (iterations > maxIteration)
                break;
            if (pathsRemoved % 10 == 0)
                if(DisplayUncarve)
                    yield return null;
        }
        yield return null;
    }
    public IEnumerator ConnectMap()
    {
        Room tempRoom;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (!IsOccupied(groundMap, x, y))
                {
                    if (TileNorthSouthOccupied(groundMap, x, y)) //working with north south connection
                    {
                        if (mergedMap[x, y + 1] == TileType.ROOM && mergedMap[x, y - 1] == TileType.ROOM) //room to room connection, big yay
                        {
                            Vector3Int connectTile = new Vector3Int(x, y, 0);
                            Connection connect = new Connection(connectTile, new Vector3Int(x, y - 1, 0), new Vector3Int(x, y + 1, 0), Connection.Type.RTOR);
                            connections.Add(connect);
                            roomToRoomConnections.Add(connect);
                            Room t = GetRoomAtPos(x, y + 1);
                            connect.startRoom = t;
                            if (t != null)
                                t.connections.Add(connect);
                            t = GetRoomAtPos(x, y - 1);
                            connect.endRoom = t;
                            if (t != null)
                                t.connections.Add(connect);
                        }
                        else if(mergedMap[x, y + 1] == TileType.PATH && mergedMap[x, y - 1] == TileType.PATH)
                        {
                            
                        }
                        else //not a room to room connection
                        {
                            Vector3Int connectTile = new Vector3Int(x, y, 0);
                            Connection connect = new Connection(connectTile, new Vector3Int(x, y - 1, 0), new Vector3Int(x, y + 1, 0), Connection.Type.RTOP);
                            connections.Add(connect);
                            Room t = GetRoomAtPos(x, y + 1);

                            if (t != null)
                            {
                                t.connections.Add(connect);
                                connect.startRoom = t;
                            }
                            t = GetRoomAtPos(x, y - 1);

                            if (t != null)
                            {
                                t.connections.Add(connect);
                                connect.startRoom = t;
                            }
                        }
                    }
                    else if (TileEastWestOccupied(groundMap, x, y)) //working with east west
                    {
                        if (mergedMap[x + 1, y] == TileType.ROOM && mergedMap[x - 1, y] == TileType.ROOM) //room to room connection, big yay
                        {
                            Vector3Int connectTile = new Vector3Int(x, y, 0);
                            Connection connect = new Connection(connectTile, new Vector3Int(x - 1, y, 0), new Vector3Int(x + 1, y, 0), Connection.Type.RTOR);
                            connections.Add(connect);
                            roomToRoomConnections.Add(connect);
                            Room t = GetRoomAtPos(x - 1, y);
                            connect.startRoom = t;
                            if (t != null)
                                t.connections.Add(connect);
                            t = GetRoomAtPos(x + 1, y);
                            connect.endRoom = t;
                            if (t != null)
                                t.connections.Add(connect);
                        }
                        else if (mergedMap[x+1, y] == TileType.PATH && mergedMap[x-1, y] == TileType.PATH)
                        {

                        }
                        else //not a room to room connection
                        {
                            Vector3Int connectTile = new Vector3Int(x, y, 0);
                            Connection connect = new Connection(connectTile, new Vector3Int(x - 1, y, 0), new Vector3Int(x + 1, y, 0), Connection.Type.RTOP);
                            connections.Add(connect);
                            Room t = GetRoomAtPos(x - 1, y);

                            if (t != null)
                            {
                                t.connections.Add(connect);
                                connect.startRoom = t;
                            }
                            t = GetRoomAtPos(x + 1, y);

                            if (t != null)
                            {
                                t.connections.Add(connect);
                                connect.startRoom = t;
                            }
                        }
                    }
                }
            }
        }


        foreach (Connection c in connections)
        {

            groundMap.SetTile(c.connectTile, backgroundTile);
        }

        Room r = roomList[Random.Range(0, roomList.Count)];
        FillRoom(r, GroundTiles[1], TileType.MERGEDROOM);

        Connection initialConnect = r.connections[Random.Range(0, r.connections.Count)];
        int timeOut = 0;
        while (connections.Count != 0 && timeOut < 500)
        {

            if (!initialConnect.IsHalfMerged(ref mergedMap))
            {
                timeOut++;

                Debug.Log("Giving up on connection!");
                initialConnect = connections[Random.Range(0, connections.Count)];
                continue;
            }
            //r.connections.Clear();
            //r.connections.Add(connection);
            if (initialConnect.startRoom != null) initialConnect.startRoom.confirmedConnections.Add(initialConnect);
            if (initialConnect.endRoom != null) initialConnect.endRoom.confirmedConnections.Add(initialConnect);
            yield return FloodFill(initialConnect.connectTile);

            for (int i = connections.Count - 1; i>=0; i--)
            {
                Connection tempC = connections[i];
                if(tempC.Equals(initialConnect))
                {
                    connections.Remove(tempC);
                    
                    continue;
                }
                if(tempC.IsFullyMerged(ref mergedMap))
                {
                    connections.Remove(tempC);
                    r.connections.Remove(tempC);
                    groundMap.SetTile(tempC.connectTile, null);
                }
            }

            //r.connections.Add(initialConnect);

            if(connections.Count>0)
                initialConnect = connections[Random.Range(0, connections.Count)];
            //timeOut++;
        }
        


        yield return null;

    }
    private IEnumerator FloodFill(Vector3Int start)
    {
        //groundMap.SetTile(start, GroundTiles[1]);
        Queue<Vector3Int> searchQueue = new Queue<Vector3Int>();
        //mergedMap[start.x, start.y] = TileType.MERGEDPATH;
        searchQueue.Enqueue(start);
        int iterationCount = 0;
        while(searchQueue.Count!=0)
        {
            Vector3Int cur = searchQueue.Dequeue();
            if (mergedMap[cur.x, cur.y] == TileType.MERGEDPATH || mergedMap[cur.x, cur.y] == TileType.MERGEDROOM)
            {
                //Debug.Log("Tile already merged!");
                continue;
            }
            groundMap.SetTile(cur, GroundTiles[1]);
            if (mergedMap[cur.x, cur.y] == TileType.PATH)
                mergedMap[cur.x, cur.y] = TileType.MERGEDPATH;
            else
                mergedMap[cur.x, cur.y] = TileType.MERGEDROOM;



            if (IsOccupied(groundMap, cur.x,cur.y+1) && (mergedMap[cur.x,cur.y+1] == TileType.PATH || mergedMap[cur.x, cur.y + 1] == TileType.ROOM))
            {
                searchQueue.Enqueue(new Vector3Int(cur.x, cur.y + 1, 0));
            }
            if (IsOccupied(groundMap, cur.x, cur.y - 1) && (mergedMap[cur.x, cur.y - 1] == TileType.PATH || mergedMap[cur.x, cur.y - 1] == TileType.ROOM))
            {
                searchQueue.Enqueue(new Vector3Int(cur.x, cur.y - 1, 0));
            }
            if (IsOccupied(groundMap, cur.x+1, cur.y) && (mergedMap[cur.x+1, cur.y] == TileType.PATH || mergedMap[cur.x+1, cur.y] == TileType.ROOM))
            {
                searchQueue.Enqueue(new Vector3Int(cur.x+1, cur.y, 0));
            }
            if (IsOccupied(groundMap, cur.x-1, cur.y) && (mergedMap[cur.x-1, cur.y] == TileType.PATH || mergedMap[cur.x-1, cur.y] == TileType.ROOM))
            {
                searchQueue.Enqueue(new Vector3Int(cur.x-1, cur.y, 0));
            }

            iterationCount++;
            if (iterationCount >= 8)
            {
                iterationCount = 0;
                if(DisplayFill)
                    yield return null;
            }

        }
        yield return null;
    }
    public IEnumerator MazeFill()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (TileNeighborCount(groundMap, x, y) == 0)
                {
                    yield return StartMaze(x, y);
                }
            }
        }
    }
    public IEnumerator StartMaze(int x, int y)
    {
        Stack<Vector3Int> tileVisitedStack = new Stack<Vector3Int>();
        tileVisitedStack.Push(new Vector3Int(x, y, 0));
        groundMap.SetTile(new Vector3Int(x, y, 0), GroundTiles[0]);

        int currentX = x, currentY = y;
        mergedMap[currentX, currentY] = TileType.PATH;
        int iterationCount = 0;
        while (tileVisitedStack.Count != 0)
        {
            List<int> numList = new List<int>();
            int i = 0;
            while (i < 4)
            {
                numList.Add(i++);
            }
            bool foundTile = false;
            while (numList.Count != 0)
            {
                int index = Random.Range(0, numList.Count);
                int rand = numList[index];
                numList.RemoveAt(index);

                //int neighborCount = TileNeighborCount(groundMap, currentX, currentY);
                int neighborCount = 1;
                //if (groundMap.GetTile(new Vector3Int(currentX, currentY, 0)) != null) neighborCount--; 

                if (rand == 0 && IsInBounds(currentX, currentY + 1) && !IsOccupied(groundMap, currentX, currentY + 1) && TileOrthogNeighborCount(groundMap, currentX, currentY + 1) <= neighborCount)
                {
                    if (IsOccupied(groundMap, currentX - 1, currentY + 2) || IsOccupied(groundMap, currentX + 1, currentY + 2))
                        continue;
                    //currentX = currentX;
                    currentY = currentY + 1;
                    Vector3Int nextPos = new Vector3Int(currentX, currentY, 0);

                    mergedMap[currentX, currentY] = TileType.PATH;
                    groundMap.SetTile(nextPos, GroundTiles[0]);
                    tileVisitedStack.Push(nextPos);
                    foundTile = true;
                    break;
                }
                else if (rand == 1 && IsInBounds(currentX + 1, currentY) && !IsOccupied(groundMap, currentX + 1, currentY) && TileOrthogNeighborCount(groundMap, currentX + 1, currentY) <= neighborCount)
                {
                    if (IsOccupied(groundMap, currentX + 2, currentY - 1) || IsOccupied(groundMap, currentX + 2, currentY + 1))
                        continue;
                    currentX = currentX + 1;
                    //currentY = currentY + 1;
                    Vector3Int nextPos = new Vector3Int(currentX, currentY, 0);
                    mergedMap[currentX, currentY] = TileType.PATH;
                    groundMap.SetTile(nextPos, GroundTiles[0]);
                    tileVisitedStack.Push(nextPos);
                    foundTile = true;
                    break;
                }
                else if (rand == 2 && IsInBounds(currentX - 1, currentY) && !IsOccupied(groundMap, currentX - 1, currentY) && TileOrthogNeighborCount(groundMap, currentX - 1, currentY) <= neighborCount)
                {
                    if (IsOccupied(groundMap, currentX - 2, currentY - 1) || IsOccupied(groundMap, currentX - 2, currentY + 1))
                        continue;
                    currentX = currentX - 1;
                    //currentY = currentY + 1;
                    Vector3Int nextPos = new Vector3Int(currentX, currentY, 0);
                    mergedMap[currentX, currentY] = TileType.PATH;
                    groundMap.SetTile(nextPos, GroundTiles[0]);
                    tileVisitedStack.Push(nextPos);
                    foundTile = true;
                    break;
                }
                else if (rand == 3 && IsInBounds(currentX, currentY - 1) && !IsOccupied(groundMap, currentX, currentY - 1) && TileOrthogNeighborCount(groundMap, currentX, currentY - 1) <= neighborCount)
                {
                    if (IsOccupied(groundMap, currentX - 1, currentY - 2) || IsOccupied(groundMap, currentX + 1, currentY - 2))
                        continue;
                    //currentX = currentX + 1;
                    currentY = currentY - 1;
                    Vector3Int nextPos = new Vector3Int(currentX, currentY, 0);
                    groundMap.SetTile(nextPos, GroundTiles[0]);
                    mergedMap[currentX, currentY] = TileType.PATH;
                    tileVisitedStack.Push(nextPos);
                    foundTile = true;
                    break;
                }
            }


            if (!foundTile)
            {
                currentX = tileVisitedStack.Peek().x;
                currentY = tileVisitedStack.Peek().y;
                tileVisitedStack.Pop();
            }
            iterationCount++;
            if (iterationCount >= 8)
            {
                iterationCount = 0;
                if(DisplayMazeGen)
                    yield return null;
            }
        }
        yield return null;
    }
    public IEnumerator GenerateRooms()
    {
        roomList = new List<Room>();

        int width, height, x, y;
        //Random r = new Random();
        for (int attempt = 0; attempt < attempts; attempt++)
        {
            width = Random.Range(roomMinWidth, roomMaxWidth);
            height = Random.Range(roomMinHeight, roomMaxHeight);
            x = Random.Range(0, mapWidth);
            y = Random.Range(0, mapHeight);
            Rect smallRect = new Rect(x, y, width, height);
            Rect bigRect = new Rect(x - 1, y - 1, width + 2, height + 2);
            Room r = new Room(smallRect, bigRect);
            bool good = true;
            foreach (Room temp in roomList)
            {
                if (temp.innerRect.Overlaps(bigRect))
                {
                    good = false;
                    break;
                }
            }
            if (good)
            {
                roomList.Add(r);
                Debug.Log("Adding room!");
            }

        }

        foreach (Room temp in roomList)
        {
            FillRoom(temp, GroundTiles[0], TileType.ROOM);
        }
        yield return null;
    }
    public void FillRoom(Room room, Tile fillTile, TileType tType)
    {
        Rect r = room.innerRect;
        Debug.Log("Drawing room!");
        for (int x = (int)r.x; x < (int)(r.x + r.width); x++)
        {
            for (int y = (int)r.y; y < (int)(r.y + r.height); y++)
            {
                groundMap.SetTile(new Vector3Int(x, y, 0), fillTile);
                mergedMap[x, y] = tType;
            }
        }
    }
    public void FillBackground()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                groundMap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
            }
        }
    }

    private int TileNeighborCount(Tilemap tmap, int x, int y)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (!(i == 0 && j == 0))
                {
                    if (tmap.GetTile(new Vector3Int(x + i, y + j, 0)) != null)
                    {
                        count++;
                    }
                }

            }
        }
        return count;
    }
    private int PathNeighborCount(ref TileType[,] map, int x, int y)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (!(i == 0 && j == 0))
                {
                    if (x+i >= 0 && y+j >= 0 && map[x+i,y+j] == TileType.PATH)
                    {
                        count++;
                    }
                }

            }
        }
        return count;
    }
    private int TileOrthogNeighborCount(Tilemap tmap, int x, int y)
    {
        int count = 0;

        if (tmap.GetTile(new Vector3Int(x, y + 1, 0)) != null)
            count++;
        if (tmap.GetTile(new Vector3Int(x, y - 1, 0)) != null)
            count++;
        if (tmap.GetTile(new Vector3Int(x + 1, y, 0)) != null)
            count++;
        if (tmap.GetTile(new Vector3Int(x - 1, y, 0)) != null)
            count++;
        return count;
    }
    private bool TileNorthSouthOccupied(Tilemap tmap, int x, int y)
    {
        if (IsOccupied(tmap, x, y + 1) && IsOccupied(tmap, x, y - 1))
        {
            return true;
        }
        else
            return false;
    }
    private bool TileEastWestOccupied(Tilemap tmap, int x, int y)
    {
        if (IsOccupied(tmap, x+1, y) && IsOccupied(tmap, x-1, y))
        {
            return true;
        }
        else
            return false;
    }

    public static bool IsOccupied(Tilemap tmap, int x, int y)
    {
        if (tmap.GetTile(new Vector3Int(x, y, 0)) != null)
            return true;
        else
            return false;
    }
    private bool IsInBounds(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            return true;
        else
            return false;
    }
    private Room GetRoomAtPos(int x, int y)
    {
        foreach(Room r in roomList)
        {
            if (r.innerRect.Contains(new Vector2(x, y)))
                return r;
           
        }
        return null;
    }
    public class Connection
    {
        public Vector3Int startTile;
        public Vector3Int endTile;
        public Vector3Int connectTile;

        public Room startRoom;
        public Room endRoom;

        public enum Type {RTOR, RTOP};
        public Type connectionType;

        public Connection(Vector3Int c, Vector3Int s, Vector3Int e, Type t)
        {
            startTile = s;
            endTile = e;
            connectTile = c;
            connectionType = t;
        }
        public override bool Equals(object obj)
        {
            if (obj is Connection)
            {
                Connection other = (Connection)obj;
                if (startTile.Equals(other.startTile) && endTile.Equals(other.endTile)) return true;
                else if (startTile.Equals(other.endTile) && endTile.Equals(other.startTile)) return true;
                else return false;
            }
            else
                return false;
        }
        public bool IsHalfMerged(ref TileType[,] map)
        {
            if(map[startTile.x, startTile.y] == TileType.MERGEDPATH || map[startTile.x,startTile.y] == TileType.MERGEDROOM)
            {
                if (map[endTile.x, endTile.y] == TileType.PATH || map[endTile.x, endTile.y] == TileType.ROOM)
                {
                    return true;
                }
                else return false;
            }
            else if(map[startTile.x, startTile.y] == TileType.PATH || map[startTile.x, startTile.y] == TileType.ROOM)
            {
                if (map[endTile.x, endTile.y] == TileType.MERGEDPATH || map[endTile.x, endTile.y] == TileType.MERGEDROOM)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }
        public bool IsFullyMerged(ref TileType[,] map)
        {
            if (map[startTile.x, startTile.y] == TileType.MERGEDPATH || map[startTile.x, startTile.y] == TileType.MERGEDROOM)
            {
                if (map[endTile.x, endTile.y] == TileType.MERGEDPATH || map[endTile.x, endTile.y] == TileType.MERGEDROOM)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }
    }
    public class Room
    {
        public Rect innerRect;
        public Rect outerRect;
        public List<Connection> connections;
        public List<Connection> confirmedConnections;
        public Room(Rect i, Rect o)
        {
            innerRect = i;
            outerRect = o;
            connections = new List<Connection>();
            confirmedConnections = new List<Connection>();
        }
    }
}
