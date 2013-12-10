using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Horror
{
    public enum GameScene { SCENE_MENU, SCENE_GAME, SCENE_PAUSED, SCENE_CONTROLS }

    public class MainGame : Microsoft.Xna.Framework.Game
    {
        AudioListener listener;
        AudioEmitter emitter;

        Area tunnelsA, athleticsA, athleticsB, dunton1, dunton2, dunton3, dunton4, steacieHallway1, steacieHallway2, steacieLab, currentArea;
        Camera camera;
        Cabinet cabinet;
        Effect mist;
        public GameScene scene;
        MenuChoice menuChoice;
        GameObject target;
        GamePadState gamePadState;
        GraphicsDeviceManager graphics;
        KeyboardState keyboardState;
        LightSource flashlight, candle, laserPointer, currentLightSource;
        Map tunnelMap, athleticsAMap, athleticsBMap, dunton1Map, dunton2Map, dunton3Map, dunton4Map, steacieHallway1Map, steacieHallway2Map, steacieLabMap;
        Player player;
        Rectangle damageScreen;
        Song tunnels_bgsong, intro_song, athletics_bgsong, staecie_bgsong;
        SoundEffect soundMonsterGrowl, soundHeartbeat, mouseTrapSound, holeSound, doorSound, boulderSound, playerGasp, pageTurn, gong, feedback;
        SoundEffectInstance soundMonsterGrowlInstance, soundHeartbeatInstance, pageTurnInstance, gongInstance,
                                mouseTrapSoundInstance, holeSoundInstance, doorSoundInstance, boulderSoundInstance, playerGaspInstance, feedbackInstance;
        SpriteBatch spriteBatch;
        SpriteFont areaText, gameOverText, narrativeText;
        Texture2D playerTexture, nerdTexture, jockTexture, candleBody, candleCutout, keyImage, pageImage, cabinetImage,
                    candleEffect, flashBody, flashEffect, flashCutout, laserBody, laserEffect, laserCutout,
                        mist1, mist2, tunnelsImage, athletics1AImage, athletics1BImage,
                            dunton1Image, dunton2Image, dunton3Image, dunton4Image, steacieHallway1Image, steacieHallway2Image, steacieLabImage,
                                currentLightCutoutTexture, currentLightEffectTexture, currentLightBodyTexture, mouseTrap, hole, boulder, bruteTexture, gooText, chemist,
                                    titleMenuStart, titleMenuExit, titleMenuControls, pauseScreen;

        Texture2D[] infoScreens;

        List<Area> areas;
        List<Effect> effects;
        List<SoundEffectInstance> soundEffectList;

        LightSource[] lightSources;

        bool alertPlayed, muted, songStart, changeMenuChoice, bButtonPressed,
             backButtonPressed, xButtonPressed, gameStarted;
        int screenHeight, screenWidth, areaTextTimer, textTimer, infoScreenIndex;
        float mistScale;

        float pageOpacity = 0.0f;

        String textToDisplay = "";

        int muteStagger, travelStagger;

        enum Lights { Candle, Flashlight, Laserpointer };
        enum MenuChoice { Start, Controls, Exit };
        //DEV
        List<Rectangle> gridRects;
        Texture2D testTexture;
                
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //this.graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content . Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            alertPlayed = false;
            muted = false;
            songStart = false;
            changeMenuChoice = false;
            bButtonPressed = false;
            xButtonPressed = false;
            backButtonPressed = false;
            gameStarted = false;

            mistScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/800;

            muteStagger = 0;
            areaTextTimer = 5;
            travelStagger = 0;
            infoScreenIndex = 0;
            
            screenHeight = GraphicsDevice.Viewport.Height;
            screenWidth = GraphicsDevice.Viewport.Width;
            
            tunnelMap = new Map(75, 75,
               @"..\..\..\..\HorrorContent\Maps\Tunnels.txt");
            athleticsAMap = new Map(82, 61,
                @"..\..\..\..\HorrorContent\Maps\Athletics1A.txt");
            athleticsBMap = new Map(81, 42,
                @"..\..\..\..\HorrorContent\Maps\Athletics1B.txt");
            dunton1Map = new Map(32, 32,
                @"..\..\..\..\HorrorContent\Maps\DuntonLevel1.csv");
            dunton2Map = new Map(32, 32,
                @"..\..\..\..\HorrorContent\Maps\DuntonLevel2.csv");
            dunton3Map = new Map(32, 32,
                @"..\..\..\..\HorrorContent\Maps\DuntonLevel3.csv");
            dunton4Map = new Map(32, 32,
                @"..\..\..\..\HorrorContent\Maps\DuntonTopLevel.csv");
            steacieHallway1Map = new Map(25, 25,
                @"..\..\..\..\HorrorContent\Maps\SteacieHallway1.txt");
            steacieHallway2Map = new Map(23, 21,
                @"..\..\..\..\HorrorContent\Maps\SteacieHallway2.txt");
            steacieLabMap = new Map(32, 32,
                @"..\..\..\..\HorrorContent\Maps\SteacieLab.csv");

            Console.WriteLine(Directory.GetCurrentDirectory());

            // DEV
            testTexture = new Texture2D(GraphicsDevice, 1, 1);
            testTexture.SetData(new Color[] { Color.White });

            scene = GameScene.SCENE_MENU;
            menuChoice = MenuChoice.Start;

            base.Initialize();
        }

        /// <summary>
        /// Creates a grid to identify entities visually for debug purposes 
        /// </summary>
        private void CreateDebugGrid()
        {
            gridRects = new List<Rectangle>();
            for (int i = 0; i < currentArea.map.gridWidth; i++)
            {
                for (int j = 0; j < currentArea.map.gridHeight; j++)
                {
                    gridRects.Add(new Rectangle(32 * i, 32 * j, 32, 32));
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load game fonts
            areaText = Content.Load<SpriteFont>(@"Fonts/SpriteFont1");
            gameOverText = Content.Load<SpriteFont>(@"Fonts/SpriteFont2");
            narrativeText = Content.Load<SpriteFont>(@"Fonts/SpriteFont3");

            // Load game images
            bruteTexture = Content.Load<Texture2D>(@"Images/Brute");
            playerTexture = Content.Load<Texture2D>(@"Images/charTemplate3");
            nerdTexture = Content.Load<Texture2D>(@"Images/SpriteSheet_Nerd");
            jockTexture = Content.Load<Texture2D>(@"Images/Spritesheet_Footy");

            keyImage = Content.Load<Texture2D>(@"Images/key");
            pageImage = Content.Load<Texture2D>(@"Images/page");
            cabinetImage = Content.Load<Texture2D>(@"Images/cabinet");

            candleBody = Content.Load<Texture2D>(@"Images/candle");
            candleCutout = Content.Load<Texture2D>(@"Images/candleEffect");
            candleEffect = Content.Load<Texture2D>(@"Images/candleLight2");
            flashBody = Content.Load<Texture2D>(@"Images/flashBody");
            flashEffect = Content.Load<Texture2D>(@"Images/flashLightGradient5");
            flashCutout = Content.Load<Texture2D>(@"Images/flashEffect6");
            laserBody = Content.Load<Texture2D>(@"Images/laserBody");
            laserEffect = Content.Load<Texture2D>(@"Images/laserPointerEffect2");
            laserCutout = Content.Load<Texture2D>(@"Images/laserCutout");

            mist1 = Content.Load<Texture2D>(@"Images/mist1");
            mist2 = Content.Load<Texture2D>(@"Images/mist2");

            infoScreens = new Texture2D[3];

            // Load game images
            titleMenuStart = Content.Load<Texture2D>(@"Images/TitleMenu_Start_800x480");
            titleMenuControls = Content.Load<Texture2D>(@"Images/TitleMenu_Controls_800x480");
            titleMenuExit = Content.Load<Texture2D>(@"Images/TitleMenu_Exit_800x480");
            infoScreens[0] = Content.Load<Texture2D>(@"Images/ControlsScreen_800x480");
            infoScreens[1] = Content.Load<Texture2D>(@"Images/LightTypesScreen_800x480");
            infoScreens[2] = Content.Load<Texture2D>(@"Images/EnemyTypesScreen_800x480");
            pauseScreen = Content.Load<Texture2D>(@"Images/PauseScreen_800x480");
            tunnelsImage = Content.Load<Texture2D>(@"Images/Tunnels");
            athletics1AImage = Content.Load<Texture2D>(@"Images/Athletics1A");
            athletics1BImage = Content.Load<Texture2D>(@"Images/Athletics1B");
            dunton1Image = Content.Load<Texture2D>(@"Images/DuntonLevel1");
            dunton2Image = Content.Load<Texture2D>(@"Images/DuntonLevel2");
            dunton3Image = Content.Load<Texture2D>(@"Images/DuntonLevel3");
            dunton4Image = Content.Load<Texture2D>(@"Images/DuntonTopLevel");
            steacieHallway1Image = Content.Load<Texture2D>(@"Images/SteacieHallway1");
            steacieHallway2Image = Content.Load<Texture2D>(@"Images/SteacieHallway2");
            steacieLabImage = Content.Load<Texture2D>(@"Images/SteacieLab");

            mouseTrap = Content.Load<Texture2D>(@"Images/mouseTrapSheet");
            hole = Content.Load<Texture2D>(@"Images/hole");
            boulder = Content.Load<Texture2D>(@"Images/boulderResized");
            gooText = Content.Load<Texture2D>(@"Images/goo2");
            chemist = Content.Load<Texture2D>(@"Images/SpriteSheet_Chemist");

            // Load game sounds
            listener = new AudioListener();
            emitter = new AudioEmitter();

            tunnels_bgsong = Content.Load<Song>(@"Sound/Horror_TunnelsAmbientBG");
            athletics_bgsong = Content.Load<Song>(@"Sound/Audio_SportsFacilityBG");
            staecie_bgsong = Content.Load<Song>(@"Sound/Horror_FeedbackFlutter");
            intro_song = Content.Load<Song>(@"Sound/StudentBodiesIntro");
            feedback = Content.Load<SoundEffect>(@"Sound/Screech");
            soundMonsterGrowl = Content.Load<SoundEffect>(@"Sound/monsterGrowl");
            soundHeartbeat = Content.Load<SoundEffect>(@"Sound/heartbeat");
            mouseTrapSound = Content.Load<SoundEffect>(@"Sound/mouseTrap");
            holeSound = Content.Load<SoundEffect>(@"Sound/wilhelm");
            boulderSound = Content.Load<SoundEffect>(@"Sound/boulder");
            doorSound = Content.Load<SoundEffect>(@"Sound/doorSound");
            playerGasp = Content.Load<SoundEffect>(@"Sound/gasp");
            pageTurn = Content.Load<SoundEffect>(@"Sound/pageTurn");
            gong = Content.Load<SoundEffect>(@"Sound/gong");

            // Sounds
            soundHeartbeatInstance = soundHeartbeat.CreateInstance();

            doorSoundInstance = doorSound.CreateInstance();
            gongInstance = gong.CreateInstance();

            feedbackInstance = feedback.CreateInstance();

            playerGaspInstance = playerGasp.CreateInstance();
            pageTurnInstance = pageTurn.CreateInstance();

            soundMonsterGrowlInstance = soundMonsterGrowl.CreateInstance();
            soundMonsterGrowlInstance.Apply3D(listener, emitter);

            mouseTrapSoundInstance = mouseTrapSound.CreateInstance();
            mouseTrapSoundInstance.Apply3D(listener, emitter);

            holeSoundInstance = holeSound.CreateInstance();
            holeSoundInstance.Apply3D(listener, emitter);

            boulderSoundInstance = boulderSound.CreateInstance();
            boulderSoundInstance.Apply3D(listener, emitter);

            soundEffectList = new List<SoundEffectInstance> { soundMonsterGrowlInstance, soundHeartbeatInstance, mouseTrapSoundInstance,
                                                                   holeSoundInstance, boulderSoundInstance, feedbackInstance };

            // Player
            player = new Player(Map.coordinatesToPixels(tunnelMap.getPlayerSpawnPoint()), tunnelMap, playerGaspInstance);
            
            // Camera
            camera = new Camera(screenWidth, screenHeight, player, graphics);

            //Cabinet
            cabinet = new Cabinet();

            // Areas
            tunnelsA = new Area(tunnelsImage, player, tunnelMap, Area.TUNNELS, gongInstance);
            athleticsA = new Area(athletics1AImage, player, athleticsAMap, Area.ATHLETICSA, gongInstance);
            athleticsB = new Area(athletics1BImage, player, athleticsBMap, Area.ATHLETICSB, gongInstance);
            dunton1 = new Area(dunton1Image, player, dunton1Map, Area.DUNTON1, gongInstance);
            dunton2 = new Area(dunton2Image, player, dunton2Map, Area.DUNTON2, gongInstance);
            dunton3 = new Area(dunton3Image, player, dunton3Map, Area.DUNTON3, gongInstance);
            dunton4 = new Area(dunton4Image, player, dunton4Map, Area.DUNTON4, gongInstance);
            steacieLab = new Area(steacieLabImage, player, steacieLabMap, Area.STEACIE1, gongInstance);
            steacieHallway1 = new Area(steacieHallway1Image, player, steacieHallway1Map, Area.STEACIE2, gongInstance);
            steacieHallway2 = new Area(steacieHallway2Image, player, steacieHallway2Map, Area.STEACIE3, gongInstance);
            currentArea = tunnelsA;

            areas = new List<Area> { tunnelsA, athleticsA, athleticsB, dunton1, dunton2, dunton3, dunton4, steacieLab, steacieHallway1, steacieHallway2};

            // Lights
            candle = new PointLight(player);    
            flashlight = new Flashlight(player);
            laserPointer = new LaserPointer(player);
            lightSources = new LightSource[4] { candle, flashlight, laserPointer, null};

            currentLightSource = candle;

            // Effects
            effects = new List<Effect>();
            mist = new Mist(camera, player, mist1, mist2, this.GraphicsDevice);
            effects.Add(mist);

            CreateDebugGrid();

            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gamePadState = GamePad.GetState(PlayerIndex.One);
            keyboardState = Keyboard.GetState(PlayerIndex.One);

            switch (scene)
            {
                case GameScene.SCENE_MENU:
                    if (!songStart)
                    {
                        MediaPlayer.Play(intro_song);
                        songStart = true;
                    }
                    UpdateMenu();
                    break;

                case GameScene.SCENE_CONTROLS:
                    UpdateControlsScreen();
                    break;

                case GameScene.SCENE_GAME:
                    UpdateInput(gameTime);

                    // Badly implemented game-winning case
                    if (currentArea.areaName == Area.DUNTON4 && player.getBoundingBox().Intersects(cabinet.getBoundingBox()))
                    {
                        Console.WriteLine("YOU WIN!");
                        this.Exit();
                    }

                    if (!GameOver())
                    {
                        camera.Update();
                        currentLightSource.Update(gamePadState, gameTime);
                        player.Update(gamePadState, keyboardState, gameTime);

                        currentArea.Update(currentLightSource, gameTime);

                        foreach (Effect e in effects) e.Update();
                    }

                    // Pauses the game
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && backButtonPressed == false) ||
                        Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        scene = GameScene.SCENE_PAUSED;
                        backButtonPressed = true;
                    }
                    break;

                case GameScene.SCENE_PAUSED:
                    UpdatePauseScreen();
                    break;

                default:
                    break;
            }

            base.Update(gameTime);
        }

        public void UpdateMenu()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                switch (menuChoice)
                {
                    case MenuChoice.Exit:
                        this.Exit();
                        break;

                    case MenuChoice.Start:
                        scene = GameScene.SCENE_GAME;
                        MediaPlayer.Stop();
                        songStart = false;
                        gameStarted = true;
                        feedbackInstance.Play();
                        break;

                    case MenuChoice.Controls:
                        scene = GameScene.SCENE_CONTROLS;
                        feedbackInstance.Play();
                        break;
                }

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X == 0)
                changeMenuChoice = false;

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0 && !changeMenuChoice)
                if (menuChoice < MenuChoice.Exit)
                {
                    menuChoice = menuChoice + 1;
                    changeMenuChoice = true;
                }
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0 && !changeMenuChoice)
                if (menuChoice > MenuChoice.Start)
                {
                    menuChoice = menuChoice - 1;
                    changeMenuChoice = true;
                }
        }

        private void UpdateControlsScreen()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && bButtonPressed == false)
            {
                bButtonPressed = true;
                if (infoScreenIndex == 0)
                {
                    if (!gameStarted)
                        scene = GameScene.SCENE_MENU;
                    else
                        scene = GameScene.SCENE_PAUSED;
                }
                else
                    infoScreenIndex--;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed && xButtonPressed == false)
            {
                xButtonPressed = true;
                if (infoScreenIndex < 2)
                    infoScreenIndex++;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released)
                bButtonPressed = false;
            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Released)
                xButtonPressed = false;
        }

        private void UpdatePauseScreen()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && bButtonPressed == false)
            {
                bButtonPressed = true;
                Exit();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed && xButtonPressed == false)
            {
                xButtonPressed = true;
                scene = GameScene.SCENE_CONTROLS;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && backButtonPressed == false)
            {
                scene = GameScene.SCENE_GAME;
                backButtonPressed = true;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released)
                backButtonPressed = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released)
                bButtonPressed = false;
            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Released)
                xButtonPressed = false;
        }

        /// <summary>
        /// Update controller input
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateInput(GameTime gameTime)
        {

            if(GameOver() && GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Enter)) ManageRespawn(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released)
                backButtonPressed = false;
            
            ManageLightSources(gameTime);

            ManageSound(gameTime);
                
            ManageTravel(gameTime);

            ManageNarrative();
        }

        /// <summary>
        /// Allows the player to respawn and resets all enemies in all areas into their spawn positions
        /// </summary>
        /// <param name="gameTime"></param>
        public void ManageRespawn(GameTime gameTime)
        {
            if (currentArea.playerSpawnPoint == Vector2.Zero) 
                Console.WriteLine("ERROR: No defined spawn point in current area or spawn is at coordinate (0,0).");
            else player.position = Map.coordinatesToPixels(currentArea.playerSpawnPoint);

            foreach (Area a in areas)
                foreach (Enemy e in a.enemies) e.position = e.spawnPoint;

            ((LaserPointer)lightSources[(int)Lights.Laserpointer]).ResetTags();

            player.health = Player.PLAYER_HEALTH;
            areaTextTimer = (int)gameTime.TotalGameTime.TotalSeconds + 5;
        }

        /// <summary>
        /// Manages sound
        /// </summary>
        /// <param name="gameTime"></param>
        public void ManageSound(GameTime gameTime)
        {
            /* Toggle Mute */
            if (gamePadState.Buttons.Start == ButtonState.Pressed && muteStagger < gameTime.TotalGameTime.TotalSeconds)
            {
                if (!muted) Mute(gameTime);
                else UnMute(gameTime);
                muteStagger = (int)gameTime.TotalGameTime.TotalSeconds + 1;
            }

            /* BG Loop */
            if (!songStart)
                switch (currentArea.areaName)
                {
                    case Area.TUNNELS:
                        MediaPlayer.Play(tunnels_bgsong);
                        songStart = true;
                        break;
                    case Area.ATHLETICSA: case Area.ATHLETICSB:
                        MediaPlayer.Play(athletics_bgsong);
                        songStart = true;
                        break;
                    case Area.STEACIE1: case Area.STEACIE2: case Area.STEACIE3:
                        MediaPlayer.Play(staecie_bgsong);
                        songStart = true;
                        break;
                    default:
                        MediaPlayer.Play(athletics_bgsong);
                        songStart = true;
                        break;
                }

            /* Reset position of player in sound manager */
            listener.Position = new Vector3(Map.pixelsToCoordinates(player.position), 0);
            /* Enemy Sound */
            if (currentArea.enemies.Count() == 0 || GameOver()) GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            else
            {
                foreach (Enemy e in currentArea.enemies)
                {
                    if (e.inPursuit)
                    {
                        GamePad.SetVibration(PlayerIndex.One, 0.1f, 0.0f);
                        if (!alertPlayed)
                        {
                            emitter.Position = new Vector3(Map.pixelsToCoordinates(e.position), 0);
                            switch(e.GetType().ToString())
                            {
                                case "Horror.Nerd": 
                                    soundMonsterGrowlInstance.Apply3D(listener, emitter);
                                    soundMonsterGrowlInstance.Play(); break;
                                case "Horror.Jock": 
                                    soundMonsterGrowlInstance.Apply3D(listener, emitter);
                                    soundMonsterGrowlInstance.Play(); break;
                            }
                        }
                        alertPlayed = true;

                        if (!soundHeartbeatInstance.State.Equals(SoundState.Playing)) soundHeartbeatInstance.Play();
                    }
                    else
                    {
                        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                        alertPlayed = false;
                    }
                }
            }

            /* Hazard Sound */
            bool boulderExists = false;

            foreach (Hazard h in currentArea.hazards)
                if (h.beingAnimated)
                {
                    emitter.Position = new Vector3(Map.pixelsToCoordinates(h.position), 0);
                    switch (h.GetType().ToString())
                    {
                        case "Horror.MouseTrap":
                            mouseTrapSoundInstance.Apply3D(listener, emitter);
                            mouseTrapSoundInstance.Play(); break;
                        case "Horror.Hole":
                            holeSoundInstance.Apply3D(listener, emitter);
                            holeSoundInstance.Play(); break;
                        case "Horror.Boulder":
                            boulderSoundInstance.Apply3D(listener, emitter);
                            boulderSoundInstance.Play();
                            boulderExists = true; break;
                    }
                }

            if (boulderSoundInstance.State == SoundState.Playing && !boulderExists) boulderSoundInstance.Stop();
        }

        /// <summary>
        /// Allows the player to change their light source and light source attributes
        /// </summary>
        /// <param name="gameTime"></param>
        public void ManageLightSources(GameTime gameTime)
        {
            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                currentLightSource = lightSources[(int)Lights.Candle];
                currentLightSource.RemoveEnhancement();
            }
            else if (lightSources[(int)Lights.Flashlight] != null && gamePadState.Buttons.B == ButtonState.Pressed)
            {
                currentLightSource = lightSources[(int)Lights.Flashlight];
                currentLightSource.RemoveEnhancement();
            }
            else if (lightSources[(int)Lights.Laserpointer] != null && gamePadState.Buttons.X == ButtonState.Pressed)
            {
                currentLightSource = lightSources[(int)Lights.Laserpointer];
                currentLightSource.RemoveEnhancement();
            }

            /* Changes the color of the laser pointer based on what it's pointed to
             * Couldn't think of a way of doing this better, I'm aware it's pretty darn inefficient... */
            if (currentLightSource == lightSources[(int)Lights.Laserpointer])
            {
                target = null;
                foreach (Enemy e in currentArea.enemies)
                    if (e.getProperBoundingBox().Contains(Map.vectorToPoint(currentLightSource.spotOrigin)))
                    {
                        target = e;
                        currentLightSource.color = Color.Purple;
                    }

                foreach (Hazard h in currentArea.hazards)
                    if (h.getBoundingBox().Contains(Map.vectorToPoint(currentLightSource.spotOrigin)))
                    {
                        target = h;
                        currentLightSource.color = Color.Yellow;
                    }

                foreach (Door d in currentArea.doors)
                    if (Map.inSameCoordinate(d.position, currentLightSource.spotOrigin))
                    {
                        target = d;
                        currentLightSource.color = Color.Green;
                    }
                if (target == null) currentLightSource.color = Color.Red;
            }

            if (gamePadState.Buttons.RightStick == ButtonState.Pressed)
                switch (currentLightSource.GetType().ToString())
                {
                    case "Horror.PointLight": currentLightSource.AddEnhancement(10, gameTime); break;
                    case "Horror.Flashlight": currentLightSource.AddEnhancement(5, gameTime); break;
                    case "Horror.LaserPointer": 
                        if(target != null) currentLightSource.AddEnhancement(target, gameTime); break;
                }
        }

        /// <summary>
        /// Allows the player to travel to different areas of the game
        /// </summary>
        /// <param name="gameTime"></param>
        public void ManageTravel(GameTime gameTime)
        {
            if (travelStagger < gameTime.TotalGameTime.TotalSeconds)
            {
                foreach (Door d in currentArea.doors)
                {
                    if (player.getBoundingBox().Intersects(d.getBoundingBox()))
                    {
                        doorSoundInstance.Play();
                        foreach (Area a in areas)
                            if (a.areaName == d.doorTo)
                            {
                                bool hasAccess = false;
                                foreach (Key k in player.keys)
                                    if (k.toWhere == a.areaName) hasAccess = true;

                                if (hasAccess || !a.locked)
                                {
                                    // Only stop bg music if moving into or out of tunnels
                                    switch (currentArea.areaName)
                                    {
                                        case Area.TUNNELS:
                                            MediaPlayer.Stop();
                                            songStart = false;
                                            break;
                                        case Area.STEACIE1: case Area.ATHLETICSB: case Area.DUNTON1:
                                            if (d.doorTo == Area.TUNNELS)
                                            {
                                                MediaPlayer.Stop();
                                                songStart = false;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    currentArea = a;
                                    
                                    player.map = currentArea.map;
                                    foreach (Door D in currentArea.doors)
                                        if (D.doorTo == d.doorFrom)
                                        {
                                            Vector2 offset = new Vector2(player.width / 2, -(player.width / 2));
                                            player.position = D.position + offset;
                                        }
                                    areaTextTimer = (int)gameTime.TotalGameTime.TotalSeconds + 5;
                                    travelStagger = (int)gameTime.TotalGameTime.TotalSeconds + 2;

                                }
                                else
                                {
                                    textToDisplay = "You don't have the key!";
                                    textTimer = (int)gameTime.TotalGameTime.TotalSeconds + 4;
                                }
                            }
                        
                        CreateDebugGrid();
                    }
                }
            }
        }

        /// <summary>
        /// Manage page reading
        /// </summary>
        public void ManageNarrative()
        {
            if (currentArea.page != null && player.getBoundingBox().Intersects(currentArea.page.getBoundingBox()))
            {
                if (pageOpacity != 1.0f)
                    pageTurnInstance.Play();
                pageOpacity = 1.0f;
            }
            else pageOpacity = 0.0f;
        }

        /// <summary>
        /// Mute all sound
        /// </summary>
        public void Mute(GameTime gameTime)
        {
            //Console.WriteLine("Muted");
            foreach (SoundEffectInstance s in soundEffectList)
                s.Volume = 0.0f;
            MediaPlayer.Volume = 0.0f;
            muted = true;
        }

        /// <summary>
        /// UnMute all sound
        /// </summary>
        /// <param name="gameTime"></param>
        public void UnMute(GameTime gameTime)
        {
            //Console.WriteLine("Unmuted");
            foreach (SoundEffectInstance s in soundEffectList)
                s.Volume = 1.0f;
            MediaPlayer.Volume = 1.0f;
            muted = false;
        }

        /// <summary>
        /// For debugging
        /// </summary>
        /// <param name="aSpriteBatch"></param>
        /// <param name="rect"></param>
        /// <param name="aColor"></param>
        public void DrawRectangle(SpriteBatch aSpriteBatch, Rectangle rect, Color aColor)
        {
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Left, rect.Top, rect.Width, 1), aColor);
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Left, rect.Bottom, rect.Width, 1), aColor);
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Left, rect.Top, 1, rect.Height), aColor);
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Right, rect.Top, 1, rect.Height), aColor);
        }

        /// <summary>
        /// For debugging
        /// </summary>
        /// <param name="aSpriteBatch"></param>
        /// <param name="rect"></param>
        /// <param name="aColor"></param>
        /// <param name="depth"></param>
        public void DrawRectangle(SpriteBatch aSpriteBatch, Rectangle rect, Color aColor, float depth)
        {
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Left, rect.Top, rect.Width, 1), null, aColor, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Left, rect.Bottom, rect.Width, 1), null, aColor, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Left, rect.Top, 1, rect.Height), null, aColor, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            aSpriteBatch.Draw(testTexture, new Rectangle(rect.Right, rect.Top, 1, rect.Height), null, aColor, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        /// <summary>
        /// Checks if the game is over (i.e. player is out of health)
        /// </summary>
        /// <returns></returns>
        public bool GameOver()
        {
            return (player.health <= 0);
        }

        /// <summary>
        /// Manage draw depths for when player is in front/behind enemy
        /// </summary>
        public void manageDepths(Enemy e)
        {
            if (player.position.Y > e.position.Y) e.depth = player.depth + 0.01f;
            else e.depth = Enemy.ENEMY_DEPTH;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, DepthStencilState.Default, null, null, camera.TransformMatrix);

            switch (scene)
            {
                case GameScene.SCENE_MENU:
                    DrawMenu();
                    spriteBatch.End();
                    break;
                case GameScene.SCENE_GAME:
                    DrawGame(gameTime);
                    spriteBatch.End();
                    break;
                case GameScene.SCENE_PAUSED:
                    spriteBatch.End();
                    spriteBatch.Begin();
                    spriteBatch.Draw(pauseScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    spriteBatch.End();
                    break;
                case GameScene.SCENE_CONTROLS:
                    spriteBatch.End();
                    spriteBatch.Begin();
                    spriteBatch.Draw(infoScreens[infoScreenIndex], new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    spriteBatch.End();
                    break;
            }

            //spriteBatch.End();

            base.Draw(gameTime);
        }


        private void DrawMenu()
        {
            switch (menuChoice)
            {
                case MenuChoice.Start:
                    spriteBatch.Draw(titleMenuStart, new Rectangle(0, 0, screenWidth, screenHeight), null, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
                    break;
                case MenuChoice.Controls:
                    spriteBatch.Draw(titleMenuControls, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                case MenuChoice.Exit:
                    spriteBatch.Draw(titleMenuExit, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                default:
                    break;
            }
        }

        private void DrawGame(GameTime gameTime)
        {
            if(currentArea.areaName == Area.DUNTON4)
                   spriteBatch.Draw(cabinetImage, cabinet.position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.92f);
            //doors (only for laser pointer tagging)
            foreach (Door d in currentArea.doors)
            {
                if (d.tagged)
                    DrawRectangle(spriteBatch, d.getBoundingBox(), Color.Green);
            }

            //enemies
            foreach (Enemy e in currentArea.enemies)
            {
                manageDepths(e);
                switch (e.GetType().ToString())
                {
                    case "Horror.Nerd": spriteBatch.Draw(nerdTexture, e.position, e.sourceFrame, e.color, 0.0f, e.origin, 1.0f, e.isMirrored(), e.depth); break;
                    case "Horror.Jock": spriteBatch.Draw(jockTexture, e.position, e.sourceFrame, e.color, 0.0f, e.origin, 1.0f, e.isMirrored(), e.depth); break;
                    case "Horror.Brute": spriteBatch.Draw(bruteTexture, e.position, e.sourceFrame, e.color, 0.0f, e.origin, 1.0f, e.isMirrored(), e.depth); break;
                    case "Horror.Chemist": spriteBatch.Draw(chemist, e.position, e.sourceFrame, e.color, 0.0f, e.origin, 1.0f, e.isMirrored(), e.depth); break;
                }
                if (e.tagged)
                    DrawRectangle(spriteBatch, e.getProperBoundingBox(), Color.Purple);
            }

            //player
            spriteBatch.Draw(playerTexture, player.position, player.sourceFrame, Color.DarkGray, 0.0f, player.origin, 1.0f, SpriteEffects.None, player.depth);

            //hazards
            foreach (Hazard h in currentArea.hazards)
            {
                switch (h.GetType().ToString())
                {
                    case "Horror.MouseTrap": spriteBatch.Draw(mouseTrap, h.position, h.sourceFrame, h.color, h.rotation, Vector2.Zero, 1.0f, SpriteEffects.None, h.depth); break;
                    case "Horror.Hole": spriteBatch.Draw(hole, h.position, h.sourceFrame, h.color, h.rotation, Vector2.Zero, 1.0f, SpriteEffects.None, h.depth); break;
                    case "Horror.Boulder": spriteBatch.Draw(boulder, h.position, h.sourceFrame, h.color, h.rotation, new Vector2(boulder.Width / 2, boulder.Height / 2), 1.0f, SpriteEffects.None, h.depth); break;
                    case "Horror.Goo": spriteBatch.Draw(gooText, h.position, h.sourceFrame, h.color, h.rotation, Vector2.Zero, 1.0f, SpriteEffects.None, h.depth); break;
                }
                if (h.tagged)
                    DrawRectangle(spriteBatch, h.getBoundingBox(), Color.Yellow);
            }

            //Key
            if (currentArea.key != null && !currentArea.key.taken)
                spriteBatch.Draw(keyImage, currentArea.key.position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.92f);
            //Page
            if (currentArea.page != null)
                spriteBatch.Draw(pageImage, currentArea.page.position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.92f);

            switch (currentLightSource.GetType().ToString())
            {
                case "Horror.PointLight":
                    currentLightBodyTexture = candleBody;
                    currentLightEffectTexture = candleEffect;
                    currentLightCutoutTexture = candleCutout; break;
                case "Horror.Flashlight":
                    currentLightBodyTexture = flashBody;
                    currentLightEffectTexture = flashEffect;
                    currentLightCutoutTexture = flashCutout; break;
                case "Horror.LaserPointer":
                    currentLightBodyTexture = laserBody;
                    currentLightEffectTexture = laserEffect;
                    currentLightCutoutTexture = laserCutout; break;
            }

            //background
            spriteBatch.Draw(currentArea.mapTexture, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            //mist
            //foreach (Mist m in effects)
            //{
            //    spriteBatch.Draw(mist1, m.mist1Position, null, Color.White * 0.6f, 0.0f, Vector2.Zero, mistScale,
            //    SpriteEffects.None, 0.95f);
            //    spriteBatch.Draw(mist2, m.mist2Position, null, Color.White * 0.6f, 0.0f, Vector2.Zero, mistScale,
            //    SpriteEffects.None, 0.95f);
            //} 

            //lightsource body
            //if(!currentLightSource.GetType().ToString().Equals("Horror.LaserPointer")) currentLightSource.depth = player.depth - 0.01f;
            spriteBatch.Draw(currentLightBodyTexture, currentLightSource.position, null, Color.White,
                currentLightSource.lightRotation, currentLightSource.origin, 1.0f, SpriteEffects.None, currentLightSource.depth);

            //lightsource cutout
            spriteBatch.Draw(currentLightCutoutTexture, currentLightSource.position, null, Color.White,
                currentLightSource.lightRotation, currentLightSource.lightCutoutOrigin,
                    currentLightSource.lightScale, SpriteEffects.None, 0.75f);

            //lightsource effect
            spriteBatch.Draw(currentLightEffectTexture, currentLightSource.position, null,
                currentLightSource.color, currentLightSource.lightRotation, currentLightSource.lightEffectOrigin,
                    currentLightSource.lightScale, SpriteEffects.None, currentLightSource.depth - 0.01f);

            //damage screen
            damageScreen = new Rectangle((int)(player.position.X + player.origin.X - screenWidth / 2),
                                    (int)(player.position.Y - screenHeight / 2), screenWidth, screenHeight);
            spriteBatch.Draw(testTexture, damageScreen, null, Color.Black * ((10.0f - player.health) / 10), 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);

            //text
            if (!GameOver())
                spriteBatch.DrawString(areaText, currentArea.areaName, new Vector2(player.position.X, player.position.Y - screenHeight / 4),
                    Color.White * ((areaTextTimer * 1000 - (gameTime.TotalGameTime.Milliseconds + (int)gameTime.TotalGameTime.TotalSeconds * 1000))
                        / 5000.0f), 0, areaText.MeasureString(currentArea.areaName) / 2, 1.0f, SpriteEffects.None, 0.0f);
            else
                spriteBatch.DrawString(gameOverText, "        Game Over\nPress A to respawn.", new Vector2(player.position.X, player.position.Y - screenHeight / 4),
                    Color.White, 0, gameOverText.MeasureString("         Game Over\nPress A to respawn.") / 2, 1.0f, SpriteEffects.None, 0.0f);

            //Draw miscellaneous text (i.e. Don't have the key!)
            spriteBatch.DrawString(areaText, textToDisplay, new Vector2(player.position.X, player.position.Y - screenHeight / 4),
                Color.White * ((textTimer * 1000 - (gameTime.TotalGameTime.Milliseconds + (int)gameTime.TotalGameTime.TotalSeconds * 1000))
                        / 5000.0f), 0, areaText.MeasureString(textToDisplay) / 2, 1.0f, SpriteEffects.None, 0.0f);

            //Draw narrative page
            if(currentArea.page != null)
            spriteBatch.DrawString(narrativeText, currentArea.page.text, new Vector2(player.position.X, player.position.Y - screenHeight / 18),
                Color.White * pageOpacity, 0, narrativeText.MeasureString(currentArea.page.text) / 2, 1.0f, SpriteEffects.None, 0.0f);


            DrawTestShapes();
        }

        /// <summary>
        /// Draw shapes on-screen for debugging
        /// </summary>
        public void DrawTestShapes()
        {
            if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Tab))
            {
                DrawRectangle(spriteBatch, player.getBoundingBox(), Color.Purple);

                foreach (Enemy e in currentArea.enemies)
                {
                    DrawRectangle(spriteBatch, e.getBoundingBox(), Color.Purple);
                    //foreach(Hazard h in e.chemGoo)
                        //spriteBatch.Draw(gooText, h.position, h.sourceFrame, h.color, h.rotation, Vector2.Zero, 1.0f, SpriteEffects.None, h.depth);
                }

                foreach (Rectangle r in gridRects)
                {
                    Vector2 gridCoor = Map.pixelsToCoordinates(new Vector2(r.Left, r.Top));
                    switch (currentArea.map.map[(int)gridCoor.Y][(int)gridCoor.X])
                    {
                        case "0": DrawRectangle(spriteBatch, r, Color.Black, 0.10f); break;
                        case "1": DrawRectangle(spriteBatch, r, Color.White, 0.09f); break;
                        case "3": DrawRectangle(spriteBatch, r, Color.Blue, 0.08f); break;
                        case "5": DrawRectangle(spriteBatch, r, Color.Green, 0.07f); break;
                        case "6": DrawRectangle(spriteBatch, r, Color.Green, 0.07f); break;
                        case "7": DrawRectangle(spriteBatch, r, Color.Green, 0.07f); break;
                        case "8": DrawRectangle(spriteBatch, r, Color.Green, 0.07f); break;
                        case "9": DrawRectangle(spriteBatch, r, Color.Green, 0.07f); break;
                        case "10": DrawRectangle(spriteBatch, r, Color.Red, 0.06f); break;
                        case "11": DrawRectangle(spriteBatch, r, Color.Red, 0.06f); break;
                        case "12": DrawRectangle(spriteBatch, r, Color.Red, 0.06f); break;
                    }
                }
            }
        }
     }
}
