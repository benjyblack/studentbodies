using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Horror
{
    public abstract class Enemy : Character
    {
        public const float ENEMY_DEPTH = 0.80f;
        public const float MAX_PURSUIT_DISTANCE = 20;
        public int counter = 0;
        
        public bool inPursuit, patrolForward;
        public float alertTimer, awarenessRadius, direction;
        public int attackDelay, currentDest, index, type, pursuitPathIndex;
        public double pathFindingStagger, pursuitTimer;
        public Vector2 heading, spawnPoint;
        public Vector2[] patrolPattern;
        public Player player;
        public List<Vector2> pursuitPath;

        public Enemy(Vector2 pos, int i, Map _map, Player plyr)
            : base(pos, _map)
        {
            player = plyr;
            spawnPoint = pos;
            index = i;
            pursuitPathIndex = 0;
            pathFindingStagger = 0;

            depth = ENEMY_DEPTH;

            alertTimer = 0.0f;
            currentDest = 0;

            pursuitTimer = 0;
            inPursuit = false;

            patrolPattern = map.getPatrolPattern(this);

            origin = new Vector2(height - height / 4, width / 2);

            //Just to start
            pursuitPath = new List<Vector2> { position };
        }

        /// <summary>
        /// Updates enemy
        /// </summary>
        /// <param name="light"></param>
        /// <param name="gameTime"></param>
        public virtual void Update(LightSource light, GameTime gameTime)
        {
            Move(light, gameTime);

            if (counter > 50000)
                counter = 0;
            counter++;
            // Check for attack
            if (getBoundingBox().Intersects(player.getBoundingBox()) && attackStagger < (gameTime.TotalGameTime.TotalSeconds))
            {
                Console.WriteLine(this.GetType().ToString() + " attacks Player.");
                Attack(gameTime);
            }
            
            if (counter % 8 == 0)
                Animate();
        }

        /// <summary>
        /// Allows enemies to move
        /// </summary>
        /// <param name="player"></param>
        /// <param name="light"></param>
        public virtual void Move(LightSource light, GameTime gameTime)
        {
            if ((light.enhanced || inBeam(light)) && light.GetType().ToString().Equals("Horror.Flashlight")) light.UseEnhancement(this); 

            else if (pursuitTimer > gameTime.TotalGameTime.TotalSeconds ||
                            inRadius(player.position + player.origin, awarenessRadius))
            {

                //reset pursuitTimer if still in pursuit
                if (pursuitTimer <= gameTime.TotalGameTime.TotalSeconds)
                    pursuitTimer = gameTime.TotalGameTime.TotalSeconds + 8;

                //choose path
                if (pathFindingStagger <= gameTime.TotalGameTime.TotalSeconds) pursuitPath = ChoosePath(gameTime);

                //update next coordinate to move to
                if (pursuitPathIndex < pursuitPath.Count() - 1 &&
                            Map.inSameCoordinate(position, Map.coordinatesToPixels(pursuitPath[pursuitPathIndex]))) pursuitPathIndex++;
                if (pursuitPathIndex == pursuitPath.Count() - 1) pursuitPath = ChoosePath(gameTime);

                //move on that path
                Orient(Map.coordinatesToPixels(pursuitPath[pursuitPathIndex]) + new Vector2(16, 16));
                Step(heading * speed);

                inPursuit = true;
            }

            // If player is not near, patrol
            else
            {
                pursuitPathIndex = 0;
                inPursuit = false;
                if(patrolPattern.Length > 1) Patrol();
            }
        }

        /// <summary>
        /// Choose shortest path
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private List<Vector2> ChoosePath(GameTime gameTime)
        {
            pursuitPathIndex = 0;
            pathFindingStagger = gameTime.TotalGameTime.TotalSeconds + 1;

            List<Vector2> path = FindPath(Map.pixelsToCoordinates(position), Map.pixelsToCoordinates(player.position));

            if (path.Count() == 0)
                return new List<Vector2> { position };

            return path;
        }
        
        /// <summary>
        /// Orients the enemy towards a position
        /// </summary>
        /// <param name="player"></param>
        public virtual void Orient(Vector2 otherPos)
        {
            direction = (float)Math.Atan2(otherPos.Y - this.position.Y, otherPos.X - this.position.X);

            heading = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
        }

        /// <summary>
        /// Orients enemy away from a position
        /// </summary>
        /// <param name="otherPos"></param>
        public virtual void OrientAway(Vector2 otherPos)
        {
            direction = (float)Math.PI + (float)Math.Atan2(otherPos.Y - this.position.Y, otherPos.X - this.position.X);

            heading = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
        }

        /// <summary>
        /// Allows enemies to attack player, it also staggers attacks so they can
        /// not attack again immediately afterwards
        /// </summary>
        /// <param name="character"></param>
        /// <param name="gameTime"></param>
        protected virtual void Attack(GameTime gameTime)
        {
            player.TakeDamage(damage, gameTime);

            attackStagger = (int)gameTime.TotalGameTime.TotalSeconds + attackDelay;
        }

        /// <summary>
        /// Allows enemies to patrol when they are not pursuing player
        /// </summary>
        /// <param name="map"></param>
        protected void Patrol()
        {
            if (Map.pixelsToCoordinates(this.position) == patrolPattern[currentDest])
            {
                if (currentDest == 0) patrolForward = true;
                //if (map.isHittingWall(this.getBoundingBox())) patrolForward = false;
                else if (currentDest == patrolPattern.Length-1) patrolForward = false;
                //else patrolForward = true;

                if (patrolForward) currentDest++;
                else currentDest--;
            }

            direction = (float)Math.Atan2(Map.coordinatesToPixels(patrolPattern[currentDest]).Y + Map.tileHeight / 2 - this.position.Y, 
                Map.coordinatesToPixels(patrolPattern[currentDest]).X + Map.tileWidth/2 - this.position.X);
            heading = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));

            // Patrol speed is half of normal speed
            position += heading * speed/2;
        }

        /*
         * The following path algorithms were adapted from
         * http://danielsaidi.wordpress.com/2010/02/22/xna-find-the-shortest-path-between-two-tiles-in-a-grid/
         * */

        //Placeholder for the calculated path (not thread safe) 
        int[,] pathLengths;

        public List<Vector2> FindPath(Vector2 startTile, Vector2 endTile)
        {
            //Abort if start or end tile is null
            if (startTile == null || endTile == null)
                return new List<Vector2>();

            //Abort if the end tile is non-stoppable
            if (map.isOffMap(endTile) || map.isOutOfBounds(endTile) || map.isHittingWall(endTile))
                return new List<Vector2>();

            //Initialize the path length array
            pathLengths = new int[map.gridWidth, map.gridHeight];
            for (int y = 0; y < pathLengths.GetLength(1); y++)
                for (int x = 0; x < pathLengths.GetLength(0); x++)
                    pathLengths[x, y] = int.MaxValue;

            //Begin at the start tile
            pathLengths[(int)startTile.X, (int)startTile.Y] = 0;
            FindPath_Spread(startTile);

            //Once done, backtrack from the end tile
            List<Vector2> result = FindPath_Trace(endTile);

            //Only return the path if it contains the start tile
            if (result.Contains(startTile))
                return result;
            return new List<Vector2>();
        }

        private void FindPath_Spread(Vector2 tile)
        {
            FindPath_Spread(tile, new Vector2(tile.X, tile.Y - 1));
            FindPath_Spread(tile, new Vector2(tile.X - 1, tile.Y));
            FindPath_Spread(tile, new Vector2(tile.X + 1, tile.Y));
            FindPath_Spread(tile, new Vector2(tile.X, tile.Y + 1));
        }

        private void FindPath_Spread(Vector2 tile, Vector2 target)
        {
            //Abort if any tile is null
            if (tile == null || target == null)
                return;

            //Abort if no movement is allowed
            if (map.isOffMap(target) || map.isOutOfBounds(target) || map.isHittingWall(target))
                return;

            //Get current path lengths
            int tileLength = FindPath_GetPathLength(tile);
            int targetLength = FindPath_GetPathLength(target);

            //Abort if the intended path is more than the maximum allowed pursuit distance,
            //(to improve performance)
            if (tileLength > MAX_PURSUIT_DISTANCE)
                return;

            //Use length if it improves target
            if (tileLength + 1 < targetLength)
            {
                pathLengths[(int)target.X, (int)target.Y] = tileLength + 1;
                FindPath_Spread(target);
            }
        }

        private int FindPath_GetPathLength(Vector2 tile)
        {
            if (tile == null || map.isOffMap(tile))
                return int.MaxValue;
            return pathLengths[(int)tile.X, (int)tile.Y];
        }

        private int RandomHelper(int upToNumber)
        {
            Random rand = new Random();
            return rand.Next(upToNumber);
        }

        private List<Vector2> FindPath_Trace(Vector2 tile)
        {
            //Find the sibling paths
            int tileLength = FindPath_GetPathLength(tile);
            int topLength = FindPath_GetPathLength(new Vector2(tile.X, tile.Y -1));
            int leftLength = FindPath_GetPathLength(new Vector2(tile.X - 1, tile.Y));
            int rightLength = FindPath_GetPathLength(new Vector2(tile.X + 1, tile.Y));
            int bottomLength = FindPath_GetPathLength(new Vector2(tile.X, tile.Y + 1));

            //Don't let the tilelength get too long
            //This fixes that stupid stack overflow error we were getting forever
            if (tileLength > 20)
                return new List<Vector2> { tile };

            //Calculate the lowest path length
            int lowestLength =
               Math.Min(tileLength,
               Math.Min(topLength,
               Math.Min(leftLength,
               Math.Min(rightLength, bottomLength))));

            //Add each possible path
            List<Vector2> possiblePaths = new List<Vector2>();
            if (topLength == lowestLength)
                possiblePaths.Add(new Vector2(tile.X, tile.Y - 1));
            if (leftLength == lowestLength)
                possiblePaths.Add(new Vector2(tile.X - 1, tile.Y));
            if (rightLength == lowestLength)
                possiblePaths.Add(new Vector2(tile.X + 1, tile.Y));
            if (bottomLength == lowestLength)
                possiblePaths.Add(new Vector2(tile.X, tile.Y + 1));

            //Continue through a random possible path
            List<Vector2> result = new List<Vector2>();
            if (possiblePaths.Count() > 0)
                result = FindPath_Trace(possiblePaths[RandomHelper(possiblePaths.Count())]);

            //Add the tile itself, then return
            result.Add(tile);
            return result;
        }
        
        /// <summary>
        /// If enemy is in the radius of the beam (radius currently 60) for more than 
        /// (60 * 0.4f) seconds (roughly .25 seconds),(60 because the game updates at  60 frames per second)
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        protected bool inBeam(LightSource light)
        {
            if (inRadius(light.spotOrigin, 60))
            {
                alertTimer++;
                return (alertTimer > (60 * 0.4f));
            }
            else alertTimer = 0;
            return false;
        }

        /// <summary>
        /// Returns flipped if enemy sprite should be mirrored
        /// </summary>
        /// <returns></returns>
        public virtual SpriteEffects isMirrored()
        {
            if (direction <= Math.PI / 2 && direction > -Math.PI / 2)
                return SpriteEffects.FlipHorizontally;
            return SpriteEffects.None;
        }
    }
}
