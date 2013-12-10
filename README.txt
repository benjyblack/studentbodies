StudentBodies v1.1

Controls:
Move character with left thumbstick.
Rotate flashlight/laser pointer with right thumbstick.
Right trigger increases your speed.
Face buttons switch light sources.
Left bumper turns on dev mode.
Click right thumbstick to activate light source ability.
Press Start to mute.

------------------------------------------------------------------------------------
*Known errors
-Trap noises don't stop playing when you're killed by them
------------------------------------------------------------------------------------

Changes (v1.2)
-Reorganized classes, see Class Diagram
-Added new LightSource, LaserPointer, a "tag system" for it and its sprites
------------------------------------------------------------------------------------

Changes (v1.1)
-Added 3D Sound
-Added Hazard functionality
-Added MouseTrap & Hole classes, each with their own animations and sounds
-Changed collision detection from point-checking to bounding-boxes
-Reorganized a lot of code, made it cleaner and more efficient
------------------------------------------------------------------------------------

Changes (v1.0)
-Changed the fundamentals of a bunch of things, made the code more efficient and slightly more sensible
-You can now add different enemy types to the same area
-Enemies remain visible for a little after encountering them so you can manoeuvre yourself away
------------------------------------------------------------------------------------

Changes (v0.9)
-Re-added mist
-Made "Enemy" class abstract and added two varieties, each with their own
specific types of movement and attack strengths: Jock and Nerd
------------------------------------------------------------------------------------

Changes (v0.8)
-Added new "Area" class to keep track of each world area and its constituents
-Added functionality to travel from one world area to another
-Added fading text sprite to indicate which area you are currently in
-Added "attack" ability for enemies
-Added "regain health" ability for player
-Can now mute game by pressing the Start button
------------------------------------------------------------------------------------

Changes (v0.7)
-Converted to full-screen
-Fixed problem where you couldn't move properly against walls
-Improved map importer
-Added ability for enemies to patrol
-Added rumble support
------------------------------------------------------------------------------------

Changes (v0.6)
-Added Map class and ability to load in CSV files as maps
-Added collision detection with walls
-Added Camera class that keeps the player centered on the screen, the class also has functionality to rotate and zoom if we need it
------------------------------------------------------------------------------------

Changes (v0.5)
-Added sound effects
-Added special abilities for each light source
	*Flashlight draws enemies towards its projected beam
	*Candle triples in luminosity
-Changed control mappings
-Added collision detection so the enemy won't step on the player
------------------------------------------------------------------------------------

Changes (v0.4)
-Removed ability to change flashlight color
-Added ability to change light source (i.e. candle)
-Added a neverending mist overlay for some atmosphere
-Added an early version of our main character for the sprite
-Added animation to main character
-Added functionality so that enemy will chase you if within a certain radius
-Added functionality so that enemy will chase you if he lies within the beam of your flashlight for too long (0.5 seconds)
-Completely reorganized code into classes, makes much more sense now
------------------------------------------------------------------------------------

Changes (v0.3):
-Ability to change flashlight color
-Added a gradient to flashlight texture
-Added background music (just to test, not actually thinking of using that song)
-Organized code and added some notations
------------------------------------------------------------------------------------

Changes (v0.2):
-Simple AI