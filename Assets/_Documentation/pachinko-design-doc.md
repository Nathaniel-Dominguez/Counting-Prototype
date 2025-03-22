# 3D Pachinko Machine - Portfolio Project Design Document

## 1. Project Overview

### 1.1 Concept
A visually stunning and physically accurate 3D pachinko machine simulation built in Unity. This portfolio piece will showcase expertise in Unity's 3D physics system, realistic materials, lighting, and game mechanics while maintaining the authentic feel of a traditional Japanese pachinko machine.

### 1.2 Portfolio Showcase Elements
- **3D Physics Mastery**: Demonstrate proficiency with Unity's 3D physics system
- **Visual Fidelity**: Showcase realistic materials, lighting, and particle effects
- **Audio Design**: Implement spatial audio for an immersive experience
- **Code Architecture**: Display clean, well-organized C# scripts with proper design patterns
- **UI/UX Design**: Create an intuitive user interface that complements the visual style

### 1.3 Target Platform
- Unity 2022.3 LTS or newer
- WebGL export for easy portfolio sharing
- Windows/macOS standalone builds for local demonstrations

## 2. Visual Design

### 2.1 Machine Structure
The pachinko machine will feature a complete cabinet design with:
- Wooden or metal frame with decorative elements
- Glass front panel (using transparency shaders)
- Ball reservoir at the top
- Mechanical launcher on the right side
- Collection trays at the bottom
- Decorative LED lights and traditional pachinko artwork

### 2.2 Play Field Elements
- **Pins**: Metal or plastic cylindrical pegs arranged in patterns
- **Bumpers**: Larger obstacles with animated responses when hit, providing extra force and visual feedback
- **Gates**: Narrow passages that direct balls to different scoring areas
- **Spinners**: Rotating elements that send balls in unpredictable directions based on physics interactions
- **Scoring Pockets**: Recessed areas with distinctive visual and audio feedback

### 2.3 Interactive Elements

#### 2.3.1 Bumpers
Bumpers will provide dynamic interaction with the following features:
- **Physical Response**: Apply strong repulsion force when hit by balls
- **Visual Feedback**: Flash with emission materials and play animation
- **Light Effects**: Integrated lights that activate on impact
- **Particle Effects**: Small spark or impact particles on collision
- **Sound Effects**: Distinctive "bump" sound with pitch variation

#### 2.3.2 Spinners
Spinners will create unpredictable ball trajectories with:
- **Continuous Rotation**: Constant movement changes the play field dynamics
- **Direction Changes**: Optional random rotation direction changes
- **Physical Influence**: Apply torque to balls that hit the spinner
- **Sound Effects**: Characteristic spinning and impact sounds
- **Visual Design**: Clear indication of rotation direction

### 2.4 Visual Effects
- **Ball Trails**: Subtle particle trails following each ball
- **Impact Particles**: Small spark or flash effects when balls hit pins
- **Scoring Celebrations**: Particle bursts and light animations when balls enter scoring zones
- **Ambient Effects**: Subtle dust particles and light reflections to enhance realism

### 2.4 Lighting Considerations
- **Main Lighting**: Overhead lights illuminating the play field
- **Accent Lighting**: Colored lights highlighting special game elements
- **Dynamic Shadows**: Real-time shadows for enhanced depth perception
- **Reflection Probes**: For realistic metal and glass materials
- **Global Illumination**: Baked lighting for overall atmosphere

## 3. Technical Design

### 3.1 3D Physics Implementation
The game will leverage Unity's built-in 3D physics system:
- **Rigidbody**: Applied to balls with precise mass and physics properties
- **Colliders**: Mixture of primitive colliders and mesh colliders for complex shapes
- **PhysicMaterial**: Custom materials with appropriate bounce, friction, and damping properties
- **Continuous Collision Detection**: For fast-moving balls to prevent tunneling
- **Physics Layers**: Proper configuration to optimize collision checks

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

### 3.3 Core Scripts

#### 3.3.1 `AudioManager.cs`
Manages all audio in the pachinko game including sound effects and music.

#### 3.3.2 `BallPoolManager.cs`
Manages the object pool for ball reuse:

#### 3.3.3 `Counter.cs`
Detects when balls enter scoring zones and updates score accordingly:

#### 3.3.4 `GameManager.cs`
Manages the overall game state with singleton pattern for easy access:

#### 3.3.5 `Song.cs`
Contains the data for each song in the game including the song name, intensity, and audio clips:

### 3.4 Physics Scripts

#### 3.4.1 `BallLauncher.cs`
Handles the ball launching mechanics using the object pool:

#### 3.4.2 `Bumper.cs`
Manages the bumpers physics properties for when pachinko balls bounce into them:

#### 3.4.3 `Spinner.cs`
Manages the spinner physics properties for when pachinko balls bounce into them:

### 3.5 Object Pooling Benefits

Object pooling provides several advantages for this project:

1. **Performance Optimization**: 
   - Eliminates garbage collection spikes from frequent instantiation/destruction
   - Reduces CPU overhead, especially important for mobile platforms
   - Maintains consistent framerate during gameplay

2. **Memory Management**:
   - Pre-allocates memory at the start, preventing memory fragmentation
   - Provides predictable memory usage patterns
   - Reduces overall memory allocations during gameplay

3. **Portfolio Value**:
   - Demonstrates understanding of common optimization techniques
   - Shows foresight in performance considerations
   - Illustrates knowledge of game development best practices

The object pooling system is implemented through the `BallPoolManager` class, which handles the creation, retrieval, and return of ball objects. This approach is especially valuable in a pachinko game where many balls are created and recycled throughout gameplay.

### 3.6 Materials and Shaders

#### 3.6.1 PBR Materials
The project will use Unity's Physically Based Rendering (PBR) materials for realistic appearance:
- **Metal Materials**: Brushed stainless steel for pins with proper metallic and smoothness values
- **Plastic Materials**: Colorful plastic materials for bumpers and other elements
- **Glass Material**: Transparent material with refraction for the front panel
- **Ball Material**: Shiny metallic material with high smoothness for the pachinko balls

#### 3.6.2 Custom Shaders
- **Neon Sign Shader**: For creating authentic pachinko machine signage with emissive properties
- **Flashing Light Shader**: For score indicators and special event feedback
- **Reflective Surface Shader**: For enhanced realism on the play field

### 3.7 Scene Structure

```
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
  |- Bumpers
  |- Gates
  |- Spinners
  |- Scoring Zones (each with Counter.cs)
  
- Lighting
  |- Key Light
  |- Fill Lights
  |- Accent Lights
  |- Score Zone Lights
  
- Effects
  |- Impact Particle Systems
  |- Ambient Particle System
  |- Score Celebration Effects
  
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
  |- AudioManager
```

## 4. Audio Design

### 4.1 Sound Effects
Realistic pachinko machine sounds with proper 3D spatial audio:
- **Ball Launch**: Mechanical spring sound with proper tension
- **Pin Hits**: Various metallic ping sounds of different pitches based on impact force
- **Bumper Hits**: Distinctive bouncy sound with electrical feedback
- **Scoring**: Celebratory jingles of different intensities based on score value
- **UI Sounds**: Subtle clicks and confirmations for button presses

### 4.2 Ambient Audio
- **Machine Hum**: Low background mechanical noise
- **Environment Ambience**: Optional arcade background sounds
- **Ball Rolling**: Physics-based rolling sounds that change based on surface
- **Mechanical Sounds**: Occasional clicks and whirs from the machine

### 4.3 Music
- **Background Music**: Optional light background music that doesn't overpower the gameplay sounds
- **Win Sequence**: More energetic music during high scoring events

## 5. Portfolio Showcase Elements

### 5.1 Technical Showcases
The following elements will highlight technical skills:

#### 5.1.1 Advanced Physics Implementation
- **Multi-Body Interactions**: Showcase how multiple balls interact realistically with pins, bumpers, and spinners
- **Custom Physics Parameters**: Demonstrate understanding of Unity's physics system through custom materials and force calculations
- **Stable Simulation**: Maintain consistent performance even with many objects through optimized code and object pooling
- **Interactive Elements**: Bumpers provide responsive force feedback, while spinners create unpredictable redirections

#### 5.1.2 Shader Expertise
- **Custom Visual Effects**: Implement unique shaders for machine elements
- **Realistic Materials**: Show mastery of PBR workflow
- **Optimization**: Properly balanced visual quality and performance
- **Glass Effects**: Realistic glass shader with proper reflections and transparency

#### 5.1.3 Systems Design
- **Clean Code Architecture**: Well-organized, commented code following best practices
- **Design Patterns**: Implementation of singleton, object pooling, and event-driven architecture
- **Performance Optimization**: Efficient code with proper physics layers, optimized memory management, and modern Unity API usage
- **Memory Management**: Demonstration of object pooling to reduce garbage collection
- **Unity Best Practices**: Using current non-deprecated methods like FindObjectsOfType with FindObjectsSortMode.None

### 5.2 Documentation
- **Development Blog**: Create accompanying posts documenting the development process
- **Technical Writeup**: Explain technical challenges and solutions
- **Video Demonstration**: Create a 1-2 minute overview video highlighting features

### 5.3 Extended Features (Time Permitting)
- **Ball Customization**: Allow different ball types with unique physics properties
- **Multiple Machines**: Create several machine designs with different layouts
- **Progressive Difficulty**: Implement increasing challenge as player scores

## 6. Implementation Plan

### 6.1 Development Phases

#### Phase 1: Core Structure and Physics (1-2 weeks)
- Set up 3D scene with cabinet model
- Implement basic physics for balls and pins
- Create basic Counter.cs functionality
- Implement ball launcher
- Set up object pooling system for balls

#### Phase 2: Interactive Elements (1 week)
- Implement Bumper.cs with physics and visual feedback
- Create Spinner.cs with rotation mechanics
- Add gates and ball guides
- Set up scoring zones with Counter.cs

#### Phase 3: Visual Polish (1-2 weeks)
- Add PBR materials to all objects
- Implement lighting system
- Create particle effects
- Design and implement UI
- Add glass shader effects

#### Phase 4: Audio Implementation (1 week)
- Integrate AudioManager.cs
- Add Ovani Music plugin 
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
- Culling masks to optimize rendering

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
- **Complete Project**: "Fully functional game with polished UI, sound, and gameplay"
