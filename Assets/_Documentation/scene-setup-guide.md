# UNITY SCENE SETUP GUIDE FOR 3D PACHINKO
This guide walks through the process of setting up the complete game scene

## PART 1: INITIAL SCENE SETUP

1. Create a new Unity project using Universal Render Pipeline template
2. Create a new scene (File > New Scene)
3. Set up the scene structure following the hierarchy diagram

### Basic scene hierarchy:
```
PachinkoGame
├── Cameras
│   ├── MainCamera
│   └── NoGlassCamera
├── Environment
│   └── MachineCabinet
├── PlayField
├── Lighting
├── Effects
├── UI
└── Managers
```

### Set up scene lighting:
1. Remove default directional light
2. Add new lighting:
   - Key Light (directional): Rotate to illuminate front of cabinet, medium intensity
   - Fill Lights (point lights): Around cabinet for even lighting
   - Accent Lights: Small point lights near special game elements
3. Configure lighting settings:
   - Window > Rendering > Lighting Settings
   - Enable "Baked Global Illumination"
   - Keep "Realtime Global Illumination" off for better performance
   - Environment Lighting: Set ambient color to dark blue-gray

## PART 2: SETTING UP THE CAMERAS

### 1. Position main camera:
1. Select MainCamera
2. Position it in front of the cabinet (around 0, 1.5, -3)
3. Rotation: (0, 0, 0) to look straight ahead
4. Set projection to Perspective
5. Field of View: 35-40 (narrow for more realistic view)
6. Set Clear Flags to "Skybox"
7. Add Post-Processing components:
   - Bloom for lights
   - Ambient Occlusion for depth
   - Vignette for focus

### 2. Set up No-Glass Camera:
1. Duplicate MainCamera
2. Rename to "NoGlassCamera"
3. Position at exact same location
4. Tag one camera "MainCamera" (the glass view)
5. In NoGlassCamera, add a CullingMask to exclude the glass panel layer
6. Disable NoGlassCamera initially
7. Make sure both cameras share the same render settings

## PART 3: CREATING THE CABINET AND PLAYFIELD

### 1. Create the machine cabinet:
1. Create empty GameObject "MachineCabinet"
2. Add child cubes for frame structure:
   - TopFrame: Scale (33, 1, 3), Position (0, 34.5, -1)
   - SideFrameLeft: Scale (1, 70, 3), Position (17, 0, -1)
   - SideFrameRight: Scale (1, 70, 3), Position (-17, 0, -1)
   - BottomFrame: Scale (33, 1, 3), Position (0, -34.5, -1)
3. Create material "CabinetMaterial" (dark wood texture)
4. Apply to all frame objects

### 2. Create the glass panel:
1. Create cube "GlassPanel"
2. Scale (35, 70, 0.05)
3. Position in front of playfield (0, 0, -0.2)
4. Create "GlassMaterial" with:
   - Rendering Mode: Transparent
   - Albedo: White with 10-20% opacity
   - Smoothness: 0.9
   - Metallic: 0.2
5. Create layer "GlassLayer" and assign to this object
6. This layer will be excluded in the NoGlassCamera
7. Create a physics material for the glass panel:
   - Right-click in your Project window
   - Select Create → Physics Material
   - Name it "GlassPanelPhysics"
   - Dynamic Friction: 0.03 (low friction so balls slide easily)
   - Static Friction: 0.05 (slightly higher to prevent micro-movements)
   - Bounciness: 0.7 (high enough for good bounce but not too bouncy)
   - Friction Combine: Minimum (uses the lower friction of the two colliding objects)
   - Bounce Combine: Maximum (uses the higher bounciness value)
8. Apply the physics material to the glass panel

### 3. Set up the playfield:
1. Create cylinder "PlayField"
2. Scale (32, 0.05, 32) and rotate to lie flat
3. Position centered in cabinet (0, 0, 0)
4. Create "PlayfieldMaterial" with dark texture
5. Add a slight concave mesh deformation:
   - Convert to mesh (Component > Mesh > Mesh Filter)
   - Edit in Modeling tool or export/import with a subtle curve
   - This will help guide balls toward the center

## PART 4: GAME ELEMENTS PLACEMENT

### 1. Pin placement:
1. Create Pin prefab as described in prefab guide
2. Create empty "Pins" parent object
3. Place pins in a grid pattern:
   - Staggered rows (triangular pattern)
   - Start with 5-6 rows near the top
   - Place about 6-8 pins per row
   - Pins should be spaced approx. 0.4-0.5 units apart
4. Create pin groups for easier organization

### 2. Bumper placement:
1. Create bumper prefabs with variation colors
2. Place strategically on the playfield:
   - In spaces between pins
   - Create natural paths and obstacles
   - Emphasize flow toward scoring zones
3. Adjust bounce forces to create balanced gameplay

### 3. Spinner placement:
1. Create spinner prefabs
2. Place 2-3 spinners on the playfield
3. Position in areas where they'll redirect balls toward interesting paths
4. Vary rotation speeds and directions

### 4. Scoring zone placement:
1. Create scoring zone prefabs
2. Place at bottom of playfield
3. Typical layout:
   - High value (1000 pts) in center, difficult to reach
   - Medium value (300-500 pts) on sides
   - Low value (100 pts) at outer edges, easier to reach
4. Make sure each zone has correct Counter.cs settings

## PART 5: BALL LAUNCHER SETUP

### 1. Create the ball launcher:
1. Create empty "BallLauncher" object
2. Position on right side of playfield (0.9, 0, 0)
3. Create visual elements:
   - LauncherBase (cube for mounting)
   - Spring (cylinder)
   - Plunger (cylinder with rounded cap)
4. Add BallLauncher.cs component
5. Create "LaunchPoint" transform child
6. Set up launcher properties in inspector:
   - Min/Max Launch Force
   - Max Balls

### 2. Set up ball reservoir:
1. Create "BallReservoir" object above launcher
2. Create visual representation (open-top box)
3. Optional: Add ball visualizations inside
4. This is cosmetic, actual balls are handled by BallPoolManager

### 3. Create ball collection tray:
1. Create "CollectionTray" object at bottom
2. Model as a long trough across bottom
3. Create transform "CollectionPoint" as target for scored balls
4. Assign in GameManager inspector

## PART 6: MANAGERS AND UI SETUP

### 1. Set up GameManager:
1. Create empty "Managers" GameObject
2. Create child "GameManager" and add GameManager.cs
3. Configure references in inspector:
   - BallLauncher
   - ScoreText
   - BallsRemainingText
   - PowerMeterSlider
   - GameOverPanel
   - Collection Tray
   - Cameras

### 2. Set up BallPoolManager:
1. Create "BallPoolManager" object
2. Add BallPoolManager.cs component
3. Create ball prefab and assign
4. Set initial pool size (20-30 is good)
5. Enable "Expand If Needed"

### 3. Set up AudioManager:
1. Create "AudioManager" object
2. Add AudioController.cs component
3. Create Audio Mixer (Window > Audio > Audio Mixer)
4. Add three groups: Master, Music, SFX
5. Assign sound effects in inspector
6. Set up music if desired

### 4. Create UI Canvas:
1. Create UI > Canvas
2. Add required elements:
   - ScoreText (TopLeft)
   - BallsRemainingText (Below ScoreText)
   - PowerMeter (Right side)
   - GameOverPanel (centered, inactive initially)
3. Configure RectTransforms for proper layout
4. Style UI elements to match visual theme

## PART 7: FINAL ADJUSTMENTS AND TESTING

### 1. Physics adjustments:
1. Edit > Project Settings > Physics
2. Set Gravity to -9.81 on Y axis only
3. Configure layers for collision matrix:
   - Create layers for Pins, Bumpers, Balls
   - Ensure correct collision relationships

### 2. Lighting bake:
1. Set static flags on non-moving objects
2. Configure light sources as Mixed or Baked
3. Click "Generate Lighting" in Lighting panel

### 3. Performance testing:
1. Launch multiple balls to test physics
2. Check for framerate drops
3. Optimize collision detection
4. Make sure object pooling works properly

### 4. Game balance:
1. Test gameplay feel
2. Adjust pin layout for good flow
3. Balance scoring difficulty
4. Tweak launcher force
5. Ensure camera views work as expected

### 5. Building for portfolio:
1. Create build for WebGL platform
2. Configure compression for optimal loading
3. Create documentation screenshots
4. Record demo video if needed

### Final checklist before sharing:
1. All scripts are error-free
2. Game runs at consistent framerate
3. Scoring and game over logic work properly
4. Audio is properly balanced
5. Visual effects enhance gameplay
6. Both camera views function correctly
7. Instructions are clear for players
