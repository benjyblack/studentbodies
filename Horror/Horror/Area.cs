using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Horror
{
    class Area
    {
        public const String TUNNELS = "Tunnels";
        public const String ATHLETICSA = "Soccer Field";
        public const String ATHLETICSB = "Locker Room";
        public const String DUNTON1 = "Dunton Tower - Level 1";
        public const String DUNTON2 = "Dunton Tower - Level 2";
        public const String DUNTON3 = "Dunton Tower - Level 3";
        public const String DUNTON4 = "Dunton Tower - Top Level";
        public const String STEACIE1 = "Steacie Laboratory";
        public const String STEACIE2 = "Steacie - Hallway 1";
        public const String STEACIE3 = "Steacie - Hallway 2";

        public string areaName;

        public Map map;
        public List<Enemy> enemies;
        public List<Hazard> hazards;
        public List<Door> doors;
        public Player player;
        public Texture2D mapTexture;
        public Key key;
        public Page page;
        public Vector2 playerSpawnPoint = Vector2.Zero;

        private SoundEffectInstance rewardNoise;

        public bool locked;

        public Area(Texture2D maptexture, Player plyr, Map Map, string name, SoundEffectInstance _rewardNoise)
        {
            map = Map;
            mapTexture = maptexture;
            areaName = name;
            player = plyr;

            rewardNoise = _rewardNoise;

            doors = map.getDoors(areaName);
            playerSpawnPoint = map.getPlayerSpawnPoint();
            enemies = map.getEnemies(player);
            hazards = map.getHazards(player, enemies);

            key = map.getKey(areaName);
            page = map.getPage(areaName);

            //Decide which areas should be locked
            if (areaName == Area.ATHLETICSB || areaName == Area.DUNTON1 || areaName == Area.STEACIE1)
                locked = true;
            else locked = false;
        }

        /// <summary>
        /// Update enemies & objects in the area
        /// </summary>
        /// <param name="lightSource"></param>
        /// <param name="gametime"></param>
        public void Update(LightSource lightSource, GameTime gametime)
        {
            /* Enemies */
            Enemy deadEnemy = null;

            foreach (Enemy e in enemies)
            {
                e.Update(lightSource, gametime);

                if (e.GetType().ToString() == "Horror.Chemist")
                {
                    if(gametime.TotalGameTime.Milliseconds % 1000 == 0) hazards.Add(new Goo(e.position, player, enemies, gametime));
                }

                if (e.health <= 0) deadEnemy = e;
            }

            if (deadEnemy != null)
            {
                Console.WriteLine("Removing " + deadEnemy.GetType().ToString() + "...");
                enemies.Remove(deadEnemy);
            }
            
            /* Hazards */
            Hazard compromisedHazard = null;

            foreach (Hazard h in hazards)
                if (h.beingAnimated || !h.disabled)
                {
                    h.Update(map, gametime);

                    if (h.GetType().BaseType.ToString().Equals("Horror.MoveableHazard"))
                    {
                       if(map.isOffMap(h.getBoundingBox()) || map.isHittingWall(h.getBoundingBox()))
                            compromisedHazard = h;
                    }
                    if (h.GetType().BaseType.ToString().Equals("Horror.TimedHazard"))
                    {
                        if (gametime.TotalGameTime.Seconds > (((TimedHazard)h).creationTime + 6))
                            compromisedHazard = h;
                    }
                }

            if (compromisedHazard != null)
            {
                Console.WriteLine("Removing " + compromisedHazard.GetType().ToString() + "...");
                hazards.Remove(compromisedHazard);
            }

            if (key != null && !key.taken && key.getBoundingBox().Intersects(player.getBoundingBox()))
            {
                rewardNoise.Play();
                key.taken = true;
                player.keys.Add(key);
            }
        }
    }
}
