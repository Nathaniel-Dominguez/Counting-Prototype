# 3D Pachinko Machine - Portfolio Project Design Document v2

## 1. Project Overview

### 1.1 Concept
A visually stunning and physically accurate 3D pachinko machine simulation built in Unity. This portfolio piece showcases expertise in Unity's 3D physics system, realistic materials, lighting, and game mechanics while maintaining the authentic feel of a traditional Japanese pachinko machine.

### 1.2 Portfolio Showcase Elements
- **3D Physics Mastery**: Demonstrate proficiency with Unity's 3D physics system
- **Visual Fidelity**: Showcase realistic materials, lighting, and particle effects
- **Audio Design**: Implement spatial audio for an immersive experience
- **Code Architecture**: Display clean, well-organized C# scripts with proper design patterns
- **UI/UX Design**: Create an intuitive user interface that complements the visual style

### 1.3 Target Platform
- Unity 6
- WebGL export for easy portfolio sharing
- Windows/macOS standalone builds for local demonstrations

## 2. Visual Design

### 2.1 Machine Structure
The pachinko machine features a complete cabinet design with:
- Wooden or metal frame with decorative elements
- Glass front panel (using transparency shaders)
- Play tray at the base of the machine next to the ball launcher and below the collection tray
- Mechanical launcher on the left side
- Collection trays at the bottom
- Decorative LED lights and traditional pachinko artwork

### 2.2 Play Field Elements
- **Pins**: Metal or plastic cylindrical pegs arranged in patterns
- **Bumpers**: Larger obstacles with animated responses when hit, providing extra force and visual feedback
- **Spinners**: Rotating elements that send balls in unpredictable directions based on physics interactions
- **Scoring Pockets**: Recessed areas with distinctive visual and audio feedback

### 2.3 Interactive Elements

#### 2.3.1 Bumpers
Bumpers provide dynamic interaction with the following features:
- **Physical Response**: Apply strong repulsion force when hit by balls
- **Visual Feedback**: Flash with emission materials and play animation
- **Light Effects**: Integrated lights that activate on impact
- **Particle Effects**: Small spark or impact particles on collision
- **Sound Effects**: Distinctive "bump" sound with pitch variation
- **Cooldown System**: Brief cooldown period after activation to prevent rapid triggering

#### 2.3.2 Spinners
Spinners create unpredictable ball trajectories with:
- **Physics-Based Rotation**: Rotation influenced by ball impact forces
- **Spin Decay**: Gradual slowing over time with configurable decay rate
- **Physical Influence**: Apply torque to balls that hit the spinner
- **Sound Effects**: Characteristic spinning and impact sounds with pitch variation based on speed
- **Visual Feedback**: Scale pulse effect when hit

#### 2.3.3 Score Pockets
Score pockets reward players when balls settle inside them:
- **Point Values**: Different score values based on pocket type (Low, Medium, High, Jackpot)
- **Ball Settlement**: Physics-based detection for balls that come to rest inside pockets
- **Visual Feedback**: Light flashes, particle effects, and animations when triggered
- **Audio Feedback**: Distinctive sounds based on score value
- **Ball Handling**: Options for direct return to pool or transit to collection tray

#### 2.3.4 Collection Tray
The collection tray manages balls that exit the play field:
- **Ball Collection**: Receives balls from play field exits or score pockets
- **Visual Feedback**: Light flashes and particle effects when balls enter
- **Audio Feedback**: Drain sound effect
- **Ball Management**: Returns balls to the pool for reuse
- **Capacity Management**: Optional ball retention with maximum capacity limit

### 2.4 Visual Effects
- **Ball Trails**: Subtle particle trails following each ball
- **Impact Particles**: Small spark or flash effects when balls hit pins
- **Scoring Celebrations**: Particle bursts and light animations when balls enter scoring zones
- **Ambient Effects**: Subtle dust particles and light reflections to enhance realism
- **Material Emission**: Dynamic emission control for interactive elements

### 2.5 Lighting Considerations
- **Main Lighting**: Overhead lights illuminating the play field
- **Accent Lighting**: Colored lights highlighting special game elements
- **Dynamic Lighting**: Real-time lights for bumpers, spinners, and score pockets
- **Light Flashes**: Temporary brightness increases when elements are triggered
- **Reflection Probes**: For realistic metal and glass materials
- **Global Illumination**: Baked lighting for overall atmosphere

## 3. Technical Design

### 3.1 3D Physics Implementation
The game leverages Unity's built-in 3D physics system:
- **Rigidbody**: Applied to balls with precise mass and physics properties
- **Colliders**: Mixture of primitive colliders and mesh colliders for complex shapes
- **PhysicMaterial**: Custom materials with appropriate bounce, friction, and damping properties
- **Continuous Collision Detection**: For fast-moving balls to prevent tunneling
- **Physics Layers**: Proper configuration to optimize collision checks
- **Force Application**: Strategic force application for launchers, bumpers, and spinners

### 3.2 Camera System
The game uses a dual-camera system from a fixed front-facing perspective:

#### 3.2.1 Main Camera (Default View)
- **Perspective**: Front-facing view of the pachinko machine
- **Glass Effects**: Includes subtle reflections, glare, and glass panel transparency
- **Shader Effects**: Uses custom shaders for realistic glass surface appearance
- **Lighting**: Full lighting effects including reflections and highlights on the glass

#### 3.2.2 No-Glass Camera
- **Perspective**: Identical position to main camera, but without glass effects
- **Clear View**: Removes glass panel visual obstructions for better gameplay visibility  
- **Enhanced Details**: Makes score values and pin layouts more visible
- **Debugging**: Useful for seeing exact ball paths and physics interactions

#### 3.2.3 Camera Toggling
- Player can switch between views by pressing the 'V' key during gameplay
- System uses smooth transition effects between camera views
- No-glass view automatically activates at game end for clearer final state visualization
- stretch goal, add another camera mode that follows balls or is more zoomed in rnd required

### 3.3 Core Systems & Managers

#### 3.3.1 `BallPoolManager.cs`
Implements an efficient object pooling system for ball management:
- **Singleton Pattern**: Global access point for ball requests
- **Pool Initialization**: Creates initial ball pool on startup
- **Get/Return Methods**: Handles ball activation, deactivation, and reuse
- **Ball Resetting**: Properly resets ball state when returned to pool
- **Pool Expansion**: Optional dynamic expansion if needed
- **Physics Prewarming**: Initializes physics to prevent first-frame hiccups

#### 3.3.2 `GameManager.cs`
Manages overall game state and progression:
- **Singleton Pattern**: Central access point for game state information
- **Score Tracking**: Manages and updates player score
- **UI Management**: Updates score display, balls remaining, and power meter
- **Game State**: Handles game initialization and game over conditions
- **Camera Management**: Controls camera switching between views
- **High Score**: Tracks and saves high scores between sessions
- **Collection Point**: Provides reference to collection tray for ball routing

#### 3.3.3 `Player.cs`
Handles player input and interaction:
- **Input Handling**: Processes keyboard and mouse controls
- **Ball Launching**: Controls ball launcher charging and release
- **Camera Toggle**: Switches between camera views
- **Input Customization**: Configurable input mappings Keyboard or Mouse controls

#### 3.3.4 `BallLauncher.cs`
Controls the ball launching mechanism:
- **Launch Force**: Variable force based on charge time
- **Ball Counting**: Tracks remaining balls
- **Visual Feedback**: Updates power meter UI
- **Animation**: Plays launcher animation when activated
- **Sound Effects**: Plays launch sounds with pitch variation based on power
- **Physics Timing**: Uses coroutines to ensure proper physics application

### 3.4 Ball Component System

#### 3.4.1 `Ball.cs`
Manages individual ball behavior and state:
- **Pocket Tracking**: Tracks which score pockets the ball is currently inside
- **Pool Integration**: Handles reset when returned to object pool
- **Physics Reset**: Zeroes velocities and restores defaults
- **Visual Reset**: Restores material properties and opacity
- **Pocket Notification**: Notifies pockets when ball is returned to pool

### 3.5 Interactive Element Scripts

#### 3.5.1 `Bumper.cs`
Controls bumper behavior and effects:
- **Bounce Physics**: Applies force to balls on collision
- **Visual Feedback**: Controls animator, lights, and particle effects
- **Material Emission**: Dynamic emission color control
- **Sound Effects**: Plays impact sounds with force-based variation
- **Cooldown System**: Prevents rapid retriggering with configurable cooldown

#### 3.5.2 `Spinner.cs`
Manages spinner physics and rotation:
- **Rotation Physics**: Calculates rotation based on impact forces
- **Spin Decay**: Gradually reduces rotation speed over time
- **Ball Interaction**: Applies forces and torque to balls on contact
- **Visual Feedback**: Scale pulsing effect when hit
- **Sound Effects**: Dynamic audio based on rotation speed
- **External Control**: Methods for manual rotation and reset

#### 3.5.3 `ScorePocket.cs`
Handles scoring zones and ball settlement detection:
- **Score Types**: Low, Medium, High, and Jackpot configurations
- **Ball Settlement**: Physics-based detection for balls that stop within the pocket
- **Visual Effects**: Controls lights, particles, and animations
- **Sound Effects**: Plays appropriate sound based on score value
- **Ball Handling**: Options for direct return to pool or transit to collection point
- **Ball Tracking**: Maintains registry of balls currently in the pocket

#### 3.5.4 `CollectionTray.cs`
Manages the ball collection area at the bottom of the playfield:
- **Ball Collection**: Detects when balls enter the tray
- **Ball Processing**: Handles ball deactivation and return to pool
- **Visual Effects**: Provides feedback when balls enter
- **Audio Effects**: Plays drain sounds
- **Ball Retention**: Optional capacity-limited ball storage
- **Ball Fade**: Controls smooth visual transition before return to pool

#### 3.5.5 `PlayTray.cs`
Manages balls in the ball launcher pool:
- **To Be Built**

### 3.6 Audio System

#### 3.6.1 `SFXManager.cs`
Comprehensive sound effect management:
- **Singleton Pattern**: Global access point for sound requests
- **Sound Effects**: Organized library of game sounds
- **3D Audio**: Spatial audio for positioned sound sources
- **Audio Pooling**: Efficient reuse of audio sources
- **Volume Control**: Master and SFX volume management
- **Settings Persistence**: Saves audio preferences between sessions
- **Sound Priorities**: Smart management of simultaneous sounds
- **Dynamic Audio**: Pitch and volume variation based on gameplay factors

#### 3.6.2 `Jukebox.cs`
Music management system:
- **Singleton Pattern**: Global access for music control
- **Music Library**: Maintains collection of available songs
- **Intensity Levels**: Supports multiple intensity variations per song
- **Seamless Transitions**: Smooth blending between songs and intensities
- **Volume Control**: Master and music-specific volume settings
- **Settings Persistence**: Saves audio preferences between sessions
- **Loop Management**: Handles music looping with configurable reverb tails

#### 3.6.3 `Song.cs`
Data structure for song information:
- **Song Data**: Contains song name, audio clips, and properties
- **Intensity Clips**: Multiple intensity variations of the same composition
- **Reverb Tail**: Configuration for seamless looping

### 3.7 Custom Editor Extensions

#### 3.7.1 `BumperEditor.cs`
Custom Unity editor for bumper configuration:
- **Quick Presets**: One-click setup for different bumper types
- **Organized Interface**: Categorized properties with explanations
- **Visual Debugging**: Scene view visualization of bounce forces
- **Validation**: Checks for required components and configurations

#### 3.7.2 `ScorePocketEditor.cs`
Custom Unity editor for score pocket configuration:
- **Quick Presets**: Easy setup for different score pocket types
- **Visual Categorization**: Organized property groups
- **Scene Visualization**: Visual indicators for score values and ball paths
- **Component Validation**: Automatic detection and fixing of configuration issues

#### 3.7.3 `SpinnerEditor.cs`
Custom Unity editor for spinner configuration:
- **Spinner Presets**: Quick setup for different spinner behaviors
- **Visualization**: Scene view indicators for rotation direction and effects
- **Property Organization**: Categorized fields with tooltips
- **Documentation**: Inline help text for complicated properties

### 3.8 Materials and Shaders

#### 3.8.1 PBR Materials
The project uses Unity's Physically Based Rendering (PBR) materials for realistic appearance:
- **Metal Materials**: Brushed stainless steel for pins with proper metallic and smoothness values
- **Plastic Materials**: Colorful plastic materials for bumpers and other elements
- **Glass Material**: Transparent material with refraction for the front panel
- **Ball Material**: Shiny metallic material with high smoothness for the pachinko balls

#### 3.8.2 Dynamic Material Properties
- **Emission Control**: Runtime control of emission colors and intensity
- **Opacity Transitions**: Smooth fading for ball return effects
- **Material Animation**: Pulsing and flashing effects for interactive elements

### 3.9 Scene Structure

```
- Jukebox (Music System)
- Cameras
  |- Main Camera (with glass effects)
  |- No-Glass Camera (clear view, same position)
  
- Environment
  |- Machine Cabinet
    |- Frame
    |- Glass Front Panel
    |- Decorative Elements
    |- Ball Release Mechanism
    |- Ball Reservoir
    |- Collection Trays
  
- Play Field
  |- Pins (Multiple with varied arrangements)
  |- Bumpers (with Bumper.cs)
  |- Gates
  |- Spinners (with Spinner.cs)
  |- Scoring Zones (with ScorePocket.cs)
  |- Collection Tray (with CollectionTray.cs)
  
- Lighting
  |- Key Light
  |- Fill Lights
  |- Accent Lights
  |- Score Zone Lights
  |- Bumper Lights
  |- Spinner Lights
  
- Effects
  |- Impact Particle Systems
  |- Ambient Particle System
  |- Score Celebration Effects
  |- Bumper Particles
  |- Spinner Particles
  
- UI Canvas
  |- Score Text
  |- Balls Remaining Text
  |- Power Meter
  |- View Toggle Button (or press 'V')
  |- Game Over Panel (initially inactive)
    |- Final Score
    |- High Score
    |- Restart Button
  
- Managers
  |- GameManager
  |- BallPoolManager
  |- SFXManager
  |- Player (Input Management)
```

## 4. Audio Design

### 4.1 Sound Effects
Realistic pachinko machine sounds with proper 3D spatial audio:
- **Ball Launch**: Mechanical spring sound with pitch variation based on launch power
- **Pin Hits**: Various metallic ping sounds of different pitches based on impact force
- **Bumper Hits**: Distinctive bouncy sound with electrical feedback
- **Spinner Effects**: Dynamic sounds based on rotation speed
- **Scoring Effects**: Tiered celebration sounds based on score value (Low, Medium, High, Jackpot)
- **Drain Sound**: Distinctive sound when balls enter the collection tray
- **UI Sounds**: Button clicks and interface feedback
- **Game Over**: Special sound effect for end of game

### 4.2 Sound Priority System
The SFXManager implements a smart priority system:
- **Sound Importance**: Assigns importance values to different sound types
- **Simultaneous Limit**: Caps the maximum number of sounds playing at once
- **Smart Culling**: Replaces less important sounds when reaching the limit
- **Critical Sounds**: Ensures high-priority sounds (like jackpots) always play

### 4.3 Music System
Adaptive music system with intensity variations:
- **Base Songs**: Background music tracks for gameplay
- **Intensity Levels**: Multiple versions of each song for dynamic progression
- **Seamless Transitions**: Smooth blending between songs and intensities
- **Volume Control**: Independent control of music volume
- **Persistence**: Saves player audio preferences between sessions

## 5. Portfolio Showcase Elements

### 5.1 Technical Showcases
The following elements highlight technical skills:

#### 5.1.1 Advanced Physics Implementation
- **Multi-Body Interactions**: Showcase how multiple balls interact realistically with pins, bumpers, and spinners
- **Custom Physics Parameters**: Demonstrate understanding of Unity's physics system through custom materials and force calculations
- **Stable Simulation**: Maintain consistent performance even with many objects through optimized code and object pooling
- **Interactive Elements**: Bumpers provide responsive force feedback, while spinners create unpredictable redirections
- **Ball Settlement**: Physics-based detection system for scoring conditions

#### 5.1.2 Visual Effects & Materials
- **Dynamic Emission**: Runtime control of material emission for interactive feedback
- **Particle Integration**: Responsive particle systems triggered by game events
- **Light Animation**: Programmatic control of light intensity and color
- **Material Transitions**: Smooth opacity and color changes
- **Glass Effects**: Realistic glass shader with proper reflections and transparency

#### 5.1.3 Systems Design
- **Clean Code Architecture**: Well-organized, commented code following best practices
- **Design Patterns**: Implementation of singleton, object pooling, and component-based architecture
- **Custom Editors**: Extended Unity editor functionality for improved workflow
- **Performance Optimization**: Efficient code with proper physics layers and optimized memory management
- **Memory Management**: Demonstration of object pooling for audio sources and game objects

#### 5.1.4 Audio Implementation
- **3D Spatial Audio**: Proper positioning and attenuation of sound sources
- **Audio Pooling**: Efficient reuse of audio sources
- **Dynamic Audio**: Pitch and volume modulation based on gameplay factors
- **Adaptive Music**: Multi-layered music system with intensity transitions
- **Priority System**: Smart management of simultaneous sounds

### 5.2 Documentation
- **Design Document**: Comprehensive design documentation (this document)
- **Code Comments**: Well-documented code with clear explanations
- **Custom Editors**: Intuitive editor extensions with helpful tooltips
- **Development Blog**: Create accompanying posts documenting the development process
- **Video Demonstration**: Create a 1-2 minute overview video highlighting features

### 5.3 Extended Features (Time Permitting)
- **Ball Customization**: Allow different ball types with unique physics properties
- **Multiple Machines**: Create several machine designs with different layouts
- **Progressive Difficulty**: Implement increasing challenge as player scores
- **Gates**: Narrow passages that direct balls to different scoring areas
- **Camera**: Add a new camera angle for tracking balls

## 6. Implementation Plan

### 6.1 Development Phases

#### Phase 1: Core Structure and Physics (1-2 weeks)
- Set up 3D scene with cabinet model
- Implement basic physics for balls and pins
- Create Ball.cs functionality
- Implement BallLauncher.cs
- Set up BallPoolManager.cs for object pooling

#### Phase 2: Interactive Elements (1 week)
- Implement Bumper.cs with physics and visual feedback
- Create Spinner.cs with rotation mechanics
- Add ScorePocket.cs for scoring zones
- Implement CollectionTray.cs for ball collection
- Create custom editor extensions

#### Phase 3: Visual Polish (1-2 weeks)
- Add PBR materials to all objects
- Implement lighting system
- Create particle effects
- Design and implement UI
- Add glass shader effects

#### Phase 4: Audio Implementation (1 week)
- Integrate SFXManager.cs
- Implement Jukebox.cs and Song.cs for music
- Create and assign sound effects
- Configure 3D spatial audio
- Implement volume controls and settings persistence

#### Phase 5: Camera System (3 days)
- Set up dual camera views (glass/no-glass)
- Implement camera switching with visual transitions
- Configure camera post-processing effects

#### Phase 6: Refinement and Portfolio Elements (1 week)
- Optimize performance
- Create documentation
- Record demonstration video
- Add final polish

### 6.2 Testing Focus Areas
- **Physics Stability**: Ensure consistent behavior across different hardware
- **Visual Consistency**: Test on different quality settings and displays
- **Performance**: Maintain 60+ FPS on target hardware
- **User Experience**: Gather feedback on gameplay feel and response
- **Audio Balance**: Ensure proper mixing of sound effects and music

## 7. Technical Requirements

### 7.1 Development Environment
- Unity 2022.3 LTS or newer
- Visual Studio 2022 or JetBrains Rider
- Version control with Git

### 7.2 Unity Packages
- Universal Render Pipeline (URP) for enhanced visuals
- Post Processing Stack for visual effects
- TextMeshPro for high-quality text
- ProBuilder for quick prototype modeling

### 7.3 Asset Creation Tools
- Blender for 3D models
- Substance Painter/Designer for materials (optional)
- Audacity for audio editing
- Photoshop/GIMP for textures

### 7.4 Performance Targets
- 60+ FPS on mid-range hardware
- Optimized collision detection using physics layers
- Efficient use of lighting with mixed lighting approach
- Object and audio source pooling for memory efficiency
- Smart sound management to prevent audio overload

## 8. Marketing for Portfolio

### 8.1 Presentation Elements
- **GitHub Repository**: With well-documented code and README
- **Demo Video**: 1-2 minute showcase of features and gameplay
- **Technical Blog Post**: Explaining development challenges
- **WebGL Build**: For easy access to playable demo

### 8.2 Key Selling Points
- **Physics Simulation**: "Implemented realistic 3D physics using Unity's physics engine"
- **Visual Fidelity**: "Created authentic-looking pachinko machine with PBR materials"
- **Code Architecture**: "Designed clean, maintainable code following best practices"
- **Audio Design**: "Developed comprehensive audio system with spatial effects and adaptive music"
- **Complete Project**: "Fully functional game with polished UI, sound, and gameplay"