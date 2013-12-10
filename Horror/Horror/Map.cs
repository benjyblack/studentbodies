using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/* LEGEND:
 * Number | Interpretation
 * 1 | WALL
 * 2 | NARRATIVE PAGE
 * 3 | PLAYER SPAWN
 * 4 | KEY
 * 5 - 9 | DOORS
 * 2 digit numbers | HAZARDS {10: Mouse Trap, 11: Hole, 12: Boulder}
 * 3 digit numbers | ENEMY SPECIFIERS
 * First number of enemy specifier | ENEMY INDEX
 * Second number of enemy specifier | if 0, ENEMY SPAWN ; if ! 0 ENEMY PATROL PATTERN
 * Third number of enemy specifier | ENEMY TYPE {1: Nerd, 2: Jock, 3: Brute, 4: Chemist)
 * */

namespace Horror
{
    public class Map
    {
        public int index, gridSizeX, gridSizeY, gridWidth, gridHeight;
        public const int tileWidth = 32;
        public const int tileHeight = 32;
        public List<string[]> map;
        public Boolean athIsOpen = false;
        List<Hazard> hazards;
        List<Enemy> enemies;
        public Map(int gridSizeX, int gridSizeY, string mapPath)
        {
            gridWidth = gridSizeX;
            gridHeight = gridSizeY;

            map = parseMap(mapPath);
        }

        /// <summary>
        /// Returns true if the given boundingBox collides with a wall
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool isHittingWall(Rectangle boundingBox)
        {
            Vector2 topLeftCoor = pixelsToCoordinates(new Vector2(boundingBox.Left, boundingBox.Top));
            Vector2 topRightCoor = pixelsToCoordinates(new Vector2(boundingBox.Right, boundingBox.Top));
            Vector2 bottomLeftCoor = pixelsToCoordinates(new Vector2(boundingBox.Left, boundingBox.Bottom));
            Vector2 bottomRightCoor = pixelsToCoordinates(new Vector2(boundingBox.Right, boundingBox.Bottom));

            if (isOffMap(boundingBox))
                return true;

            try
            {
                return (map[(int)topLeftCoor.Y][(int)topLeftCoor.X] == "1") ||
                            (map[(int)topRightCoor.Y][(int)topRightCoor.X] == "1") ||
                                (map[(int)bottomLeftCoor.Y][(int)bottomLeftCoor.X] == "1") ||
                                    (map[(int)bottomRightCoor.Y][(int)bottomRightCoor.X] == "1");

            }
            catch (Exception e)
            {
                return true;
            }

        }

        /// <summary>
        /// Returns true if the given coordinate is a wall
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool isHittingWall(Vector2 coordinate)
        {
            return (map[(int)coordinate.Y][(int)coordinate.X] == "1");
        }

        /// <summary>
        /// Returns true if the given coordinate is a hazard
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool isAHazard(Vector2 coordinate)
        {
            return (map[(int)coordinate.Y][(int)coordinate.X] == "10")
                || (map[(int)coordinate.Y][(int)coordinate.X] == "11")
                || (map[(int)coordinate.Y][(int)coordinate.X] == "12");
        }

        /// <summary>
        /// Returns true if the given boundingBox is off the map
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool isOffMap(Rectangle boundingBox)
        {
            return boundingBox.Left <= 0 || boundingBox.Right >= gridWidth * tileWidth || 
                          boundingBox.Top <= 0 || boundingBox.Bottom >= gridHeight * tileHeight;
        }

        /// <summary>
        /// Returns true if the given coordinate is off the map
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool isOffMap(Vector2 coordinate)
        {
            return coordinate.X >= gridWidth || coordinate.X < 0
                          || coordinate.Y >= gridHeight || coordinate.Y < 0;
        }

        /// <summary>
        /// Returns true if the given coordinate is out of bounds
        /// NOT USING FOR NOW, i.e. the -1
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool isOutOfBounds(Vector2 coordinate)
        {
            return map[(int)coordinate.Y][(int)coordinate.X] == "-1";
        }

        /// <summary>
        /// Returns a radian rounded to its nearest 90 degree angle
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static float roundRadian(float radian)
        {
            if (radian <= Math.PI / 4 && radian > - Math.PI / 4)
                return 0.0f;
            else if (radian <= 3 * Math.PI / 4 && radian > Math.PI / 4)
                return (float)Math.PI / 2;
            else if (radian <= 5 * Math.PI / 4 && radian > 3 * Math.PI / 4)
                return (float)Math.PI;
            else
                return 3 * (float)Math.PI / 2;
        }
        /// <summary>
        /// Returns a List of all the doors in the area, their position is in pixels
        /// </summary>
        /// <returns></returns>
        public List<Door> getDoors(String areaName)
        {
            List<Door> doors = new List<Door>();

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                    if (Convert.ToInt16(map[i][j]) >= 5 && Convert.ToInt16(map[i][j]) < 10)
                    {
                        String nextArea = null;
                        switch(areaName)
                        {
                            case Area.TUNNELS:
                                switch(map[i][j])
                                {
                                    case "5": nextArea = Area.DUNTON1; break;
                                    case "6": nextArea = Area.ATHLETICSB; break;
                                    case "7": nextArea = Area.STEACIE1; break;
                                } break;

                            case Area.DUNTON1:
                                switch (map[i][j])
                                {
                                    case "5": nextArea = Area.TUNNELS; break;
                                    case "6": nextArea = Area.DUNTON2; break;
                                } break;

                            case Area.DUNTON2:
                                switch (map[i][j])
                                {
                                    case "5": nextArea = Area.DUNTON3; break;
                                    case "6": nextArea = Area.DUNTON1; break;
                                } break;

                            case Area.DUNTON3:
                                switch (map[i][j])
                                {
                                    case "5": nextArea = Area.DUNTON2; break;
                                    case "6": nextArea = Area.DUNTON4; break;
                                } break;

                            case Area.DUNTON4:
                                switch (map[i][j])
                                {
                                    case "6": nextArea = Area.DUNTON3; break;
                                } break;

                            case Area.ATHLETICSB:
                                switch (map[i][j])
                                {
                                    case "5": nextArea = Area.ATHLETICSA; break;
                                    case "6": nextArea = Area.TUNNELS; break;
                                } break;

                            case Area.ATHLETICSA:
                                switch (map[i][j])
                                {
                                    case "5": nextArea = Area.ATHLETICSB; break;
                                } break;
                            case Area.STEACIE1:
                                switch (map[i][j])
                                {
                                    case "7": nextArea = Area.TUNNELS; break;
                                    case "5": nextArea = Area.STEACIE2; break;
                                } break;

                            case Area.STEACIE2:
                                switch (map[i][j])
                                {
                                    case "5": nextArea = Area.STEACIE1; break;
                                    case "6": nextArea = Area.STEACIE3; break;
                                } break;

                            case Area.STEACIE3:
                                switch (map[i][j])
                                {
                                    case "6": nextArea = Area.STEACIE2; break;
                                } break;
                        }
                        doors.Add(new Door(coordinatesToPixels(new Vector2(j, i)), Convert.ToInt16(map[i][j]), nextArea, areaName));
                    }
            }

            return doors;
        }

        /// <summary>
        /// Get the key hidden in the level
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public Key getKey(String areaName)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    if(map[i][j] == "4")
                    {
                        switch(areaName)
                        {
                            case Area.TUNNELS:
                                return new Key(Area.ATHLETICSB, Map.coordinatesToPixels(new Vector2(j,i)));
                            case Area.ATHLETICSA:
                                return new Key(Area.STEACIE1, Map.coordinatesToPixels(new Vector2(j,i))); 
                            case Area.STEACIE2:
                                return new Key(Area.DUNTON1, Map.coordinatesToPixels(new Vector2(j,i))); 
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the narrative page for the level
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public Page getPage(String areaName)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    if(map[i][j] == "2")
                    {
                        switch (areaName)
                        {
                            case Area.TUNNELS:
                                return new Page(@"..\..\..\..\HorrorContent\Narrative\tunnelsPage.txt", 
                                    Map.coordinatesToPixels(new Vector2(j, i)));
                            case Area.ATHLETICSB:
                                return new Page(@"..\..\..\..\HorrorContent\Narrative\athleticsBPage.txt",
                                    Map.coordinatesToPixels(new Vector2(j, i)));
                            case Area.STEACIE1:
                                return new Page(@"..\..\..\..\HorrorContent\Narrative\steacie1Page.txt",
                                    Map.coordinatesToPixels(new Vector2(j, i)));
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a List of all the Enemies in the area
        /// </summary>
        /// <returns></returns>
        public List<Enemy> getEnemies(Player player)
        {
            enemies = new List<Enemy>();

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    int mapCoor = Convert.ToInt16(map[i][j]);
                    if (mapCoor >= 100)
                        if (mapCoor / 10 % 10 == 0 && mapCoor / 10 != 0)
                            switch (mapCoor - (mapCoor / 10) * 10)
                            {
                                case 1: enemies.Add(new Nerd(coordinatesToPixels(new Vector2(j, i)), mapCoor / 10, this, player)); break;
                                case 2: enemies.Add(new Jock(coordinatesToPixels(new Vector2(j, i)), mapCoor / 10, this, player)); break;
                                case 3: enemies.Add(new Brute(coordinatesToPixels(new Vector2(j, i)), mapCoor / 10, this, player)); break;
                                case 4: enemies.Add(new Chemist(coordinatesToPixels(new Vector2(j, i)), mapCoor / 10, this, player)); break;
                            }
                }
            }

            return enemies;
        }

        /// <summary>
        /// Returns a List of all the hazards in the area
        /// </summary>
        /// <returns></returns>
        public List<Hazard> getHazards(Player player, List<Enemy> enemies)
        {
            hazards = new List<Hazard>();

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    switch(map[i][j])
                    {
                        case "10": hazards.Add(new MouseTrap(coordinatesToPixels(new Vector2(j, i)), player, enemies)); break;
                        case "11": hazards.Add(new Hole(coordinatesToPixels(new Vector2(j, i)), player, enemies)); break;
                        case "12": hazards.Add(new Boulder(coordinatesToPixels(new Vector2(j, i)), player, enemies)); break;
                    }
                }
            }

            return hazards;
        }

        /// <summary>
        /// Returns the patrol pattern of a given enemy
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public Vector2[] getPatrolPattern(Enemy enemy)
        {
            int count = 0;
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                    if (Convert.ToInt16(map[i][j])/10 >= enemy.index && Convert.ToInt16(map[i][j])/10 < enemy.index + 10)
                        count++;
            }

            Vector2[] patrolPattern = new Vector2[count];

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                    if (Convert.ToInt16(map[i][j])/10 >= enemy.index && Convert.ToInt16(map[i][j])/10 < enemy.index + 10)
                        patrolPattern[Convert.ToInt16(map[i][j])/10 - enemy.index] = new Vector2(j, i);
            }

            return patrolPattern;
        }

        /// <summary>
        /// Returns the spawn point of the player in the area
        /// </summary>
        /// <returns></returns>
        public Vector2 getPlayerSpawnPoint()
        {
            Vector2 spawnPoint = Vector2.Zero;
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                    if (map[i][j] == "3") spawnPoint = new Vector2(j, i);
            }
            return spawnPoint;
        }

        /// <summary>
        /// Returns the given pixel Vector as a coordinate Vector relative to the current map
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static Vector2 pixelsToCoordinates(Vector2 pixels) 
        {
            return (new Vector2((float)(Math.Floor(pixels.X / tileWidth)), (float)(Math.Floor(pixels.Y / tileHeight))));
        }


        /// <summary>
        /// Returns the given coordinate Vector as a pixel Vector relative to the current map
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static Vector2 coordinatesToPixels(Vector2 coordinates)
        {
            return (new Vector2(tileWidth*coordinates.X, tileHeight*coordinates.Y));
        }


        /// <summary>
        /// Returns the given vector as a point
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point vectorToPoint(Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// Returns the given point as a vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 pointToVector(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        /// <summary>
        /// Takes two pixel Vectors as arguments and returns if they are in the same grid coordinate
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static bool inSameCoordinate(Vector2 pos1, Vector2 pos2)
        {
            return (pixelsToCoordinates(pos1) == pixelsToCoordinates(pos2));
        }

        /// <summary>
        /// Returns an interpreted version of the map text file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string[]> parseMap(string path)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;
                    
                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return parsedData;
        }
        public List<Hazard> getHazards()
        {
            return hazards;
        }
        public List<Enemy> getEnemies()
        {
            return enemies; 
        }
    
    }
}
