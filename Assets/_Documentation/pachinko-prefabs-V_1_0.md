# 3D Pachinko Machine - Core Prefab Elements

## Playfield Components

### 1. Pin Prefab
- **Description**: Basic obstacle pin that balls bounce off of
- **Components**:
  - Cylinder mesh with appropriate scale
  - CapsuleCollider (aligned vertically)
  - Custom PhysicMaterial (bounciness: 0.7-0.8, friction: 0.2)
  - Metallic material with high reflectivity
  - Optional light reflection point at top
- **Hierarchy**:
  ```
  Pin
  └── PinVisual (mesh)
  ```

### 2. Bumper Prefabs
- **Description**: Larger obstacles that provide strong repulsion when hit
- **Variations**: Red, Blue, Yellow, Green (with different force values)
- **Components**:
  - Cylinder or sphere mesh with appropriate scale
  - SphereCollider for physics interaction
  - Bumper.cs script for force application
  - Emission-enabled material for flash effect
  - Animation components for impact reaction
  - AudioSource for impact sounds
  - Particle system for impact effects
- **Hierarchy**:
  ```
  Bumper
  ├── BumperVisual (mesh with emission map)
  ├── BumperCollider
  └── BumperEffects
      ├── ParticleSystem
      └── AudioSource
  ```

### 3. Spinner Prefabs
- **Description**: Rotating elements that change ball trajectory
- **Components**:
  - Center pivot with custom mesh (triangular or arrow-shaped)
  - Spinner.cs script for rotation behavior
  - Cylindrical or spherical collider
  - Custom physics for applying torque to balls
  - AudioSource for spinning sound
  - Optional particle trail effect
- **Hierarchy**:
  ```
  Spinner
  ├── PivotPoint
  │   └── SpinnerVisual (rotating mesh)
  ├── SpinnerCollider
  └── SpinnerEffects
      ├── RotationTrail
      └── AudioSource
  ```

### 4. Gates
- **Description**: Narrow passages that direct balls to specific areas
- **Components**:
  - Two parallel box colliders creating a channel
  - Custom material with low friction
  - Optional animation for opening/closing
  - AudioSource for ball passage sounds
- **Hierarchy**:
  ```
  Gate
  ├── LeftGateWall
  ├── RightGateWall
  └── GateTrigger (for detecting ball entry/exit)
  ```

### 5. Scoring Zones
- **Description**: Recessed pockets that award points when balls enter
- **Variations**: Jackpot (center, 1000pts), Purple (bottom, 100-500pts), Teal (sides, 300pts)
- **Components**:
  - Recessed mesh for visual appearance
  - Trigger collider for ball detection
  - Counter.cs script for score calculation
  - Particle system for celebration effects
  - Light component for illumination when scored
  - AudioSource for scoring sounds
- **Hierarchy**:
  ```
  ScoringZone
  ├── ZoneVisual (recessed mesh)
  ├── ScoreTrigger
  ├── ScoreText (3D text showing point value)
  └── ScoringEffects
      ├── CelebrationParticles
      ├── ScoreLight
      └── AudioSource
  ```

## Cabinet Structure

### 6. Glass Panel
- **Description**: Transparent front surface of the machine
- **Components**:
  - Thin box mesh with transparent material
  - Custom shader for reflections and glare
  - Box collider (can be toggled with camera switch)
  - Optional dirt/smudge texture overlay
- **Hierarchy**:
  ```
  GlassPanel
  ├── GlassMesh
  ├── GlassCollider
  └── GlareEffect
  ```

### 7. Cabinet Frame
- **Description**: Outer structure that holds the machine together
- **Components**:
  - Wooden/metal frame meshes
  - Decorative corner elements (card symbols from SVG)
  - LED/neon trim lights with emission
  - Machine branding/artwork textures
- **Hierarchy**:
  ```
  CabinetFrame
  ├── TopFrame
  ├── SideFrames
  ├── BottomFrame
  ├── DecorativeCorners
  │   ├── TopLeftCorner (Q)
  │   ├── TopRightCorner (K)
  │   ├── BottomLeftCorner (J)
  │   └── BottomRightCorner (A)
  └── TrimLights
  ```

### 8. Ball Launcher
- **Description**: Mechanism for launching balls into the playfield
- **Components**:
  - Spring-loaded launcher model with animation
  - BallLauncher.cs script for force calculation
  - Launch point transform for ball positioning
  - Power meter visual indicator
  - AudioSource for launch sounds
- **Hierarchy**:
  ```
  BallLauncher
  ├── LauncherMechanism
  │   ├── Spring (animated)
  │   └── Plunger
  ├── LaunchPoint (transform)
  ├── PowerMeter
  └── LauncherAudio
  ```

### 9. Ball Reservoir
- **Description**: Storage area for balls waiting to be launched
- **Components**:
  - Visual container mesh
  - Optional visible ball objects or particle system
  - Connection to BallPoolManager.cs
- **Hierarchy**:
  ```
  BallReservoir
  ├── ReservoirMesh
  ├── BallVisuals (or particle system)
  └── BallSpawnPoint
  ```

### 10. Collection Trays
- **Description**: Bottom receptacles where balls end up after play
- **Components**:
  - Tray mesh with appropriate colliders
  - Trigger zone for ball recycling to pool
  - AudioSource for ball collection sounds
- **Hierarchy**:
  ```
  CollectionTray
  ├── TrayMesh
  ├── BallReturnTrigger
  └── CollectionAudio
  ```

## Effect Systems

### 11. Score Lights
- **Description**: Lights that activate when scoring points
- **Components**:
  - Point light or spot light components
  - Animation controller for flashing/pulsing
  - Script for connecting to scoring events
- **Hierarchy**:
  ```
  ScoreLight
  ├── LightComponent
  └── LightGlow (mesh with emission)
  ```

### 12. Effect Systems
- **Description**: Various particle and visual effects
- **Variations**: Impact effects, scoring celebrations, ambient effects
- **Components**:
  - Particle systems with appropriate settings
  - Light components for illumination
  - AudioSources for effect sounds
- **Hierarchy**:
  ```
  EffectSystems
  ├── ImpactEffects
  │   ├── PinImpact
  │   ├── BumperImpact
  │   └── WallImpact
  ├── ScoringEffects
  │   ├── JackpotCelebration
  │   └── StandardScoring
  └── AmbientEffects
      └── BackgroundParticles
  ```

### 13. Ball Prefab
- **Description**: The pachinko balls that players launch
- **Components**:
  - Sphere mesh with metallic material
  - Sphere collider with appropriate physics properties
  - Rigidbody with proper mass settings
  - Custom PhysicMaterial (bounciness: 0.8, friction: 0.1)
  - Trail renderer for motion trails
  - AudioSource for rolling/collision sounds
- **Hierarchy**:
  ```
  Ball
  ├── BallMesh
  ├── BallCollider
  ├── TrailEffect
  └── BallAudio
  ```

## Additional Elements

### 14. Ball Guides
- **Description**: Rails or paths that guide balls in specific directions
- **Components**:
  - Curved mesh for visual representation
  - Curved collider for physics interaction
  - PhysicMaterial with low friction
- **Hierarchy**:
  ```
  BallGuide
  ├── GuideVisual
  └── GuideCollider
  ```

### 15. Playfield Base
- **Description**: The main circular play area
- **Components**:
  - Cylinder mesh (flattened)
  - Custom material with playfield design
  - Slight concave mesh deformation for ball centering
  - Box collider ring around edge to keep balls in
- **Hierarchy**:
  ```
  PlayfieldBase
  ├── PlayfieldMesh
  └── EdgeBarrier
  ```

### 16. Ball Exit Chutes
- **Description**: Paths leading to collection trays
- **Components**:
  - Chute mesh with appropriate colliders
  - Triggers for special effects or scoring
- **Hierarchy**:
  ```
  ExitChute
  ├── ChuteMesh
  ├── ChuteColliders
  └── ExitTrigger
  ```