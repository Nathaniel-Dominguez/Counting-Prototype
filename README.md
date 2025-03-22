# 🎮 Unity 3D Pachinko Game

A visually stunning and physically accurate 3D pachinko machine simulation built in Unity, showcasing expertise in Unity's 3D physics system, realistic materials, lighting, and game mechanics.

![Pachinko Game](https://via.placeholder.com/800x400?text=Pachinko+Game+Screenshot)

## 📖 Overview

This project evolved from Unity's "Counting Prototype" tutorial into a full-featured 3D pachinko simulation. It demonstrates advanced physics interactions, custom material properties, object pooling, and a comprehensive audio management system - all essential skills for game development.

### Key Features

- **Physics-Based Gameplay**: Experience realistic ball movement with precise collision detection using Unity's 3D physics system
- **Dual Camera System**: Toggle between a realistic glass-panel view and a clear gameplay view
- **Interactive Elements**: Encounter dynamic bumpers, spinners, and scoring pockets that respond to ball impacts
- **Visual Effects**: Enjoy particle effects, dynamic lighting, and material animations that provide satisfying feedback
- **Audio Design**: Immerse yourself in spatial audio with adaptive music intensity based on gameplay
- **Optimized Performance**: Play smoothly thanks to efficient object pooling and physics layer management

## 🚀 Installation

### Prerequisites

- Unity 2022.3 LTS or newer
- Basic understanding of Unity's interface

### Setup

1. Clone this repository: `git clone https://github.com/yourusername/unity-pachinko.git`
2. Open the project in Unity
3. Open the main scene in `Assets/Scenes/PachinkoMachine.unity`
4. Press Play to test the game in the editor

## 🎯 How to Play

1. **Launch Balls**: Hold and release the spacebar to launch balls (hold longer for more power)
2. **Toggle View**: Press 'V' to switch between glass-on and glass-off views
3. **Objective**: Score points by getting balls into the scoring pockets
4. **Game End**: The game ends when you run out of balls

## 🛠️ Technical Implementation

### Core Systems

- **Ball Physics**: Custom-tuned physics materials ensure realistic ball behavior
- **Object Pooling**: Efficient ball management through advanced object pooling
- **Interactive Elements**: Component-based architecture for bumpers, spinners, and scoring zones
- **Audio Management**: Comprehensive audio system with spatial sound and adaptive music

### Code Architecture

The project follows clean code principles with a component-based architecture:

```
- Jukebox.cs           # Background music system (stays playing when scene is loaded)
- Managers/
  |- GameManager.cs       # Central game state management
  |- GameConfig.cs      # Game configuration and settings
  |- BallPoolManager.cs   # Object pooling implementation
  |- SFXManager.cs        # Sound effect management
  
- Interactive/
  |- Ball.cs              # Ball behavior and properties
  |- Bumper.cs            # Interactive bumper elements
  |- Spinner.cs           # Rotating field elements
  |- ScorePocket.cs       # Scoring zone implementation
  |- CollectionTray.cs    # Ball collection management
  
- Player/
  |- Player.cs            # Player input handling
  |- BallLauncher.cs      # Ball launching mechanism
  
- UI/
  |- UIManager.cs         # User interface management
  |- PowerMeter.cs        # Visual launch power indicator
```

### Custom Editor Extensions

The project includes several custom editor extensions to enhance workflow:
- **BumperEditor**: Intuitive setup and visualization for bumper elements
- **ScorePocketEditor**: Quick configuration of different scoring pocket types
- **SpinnerEditor**: Advanced control over spinner behavior and physics

## 💡 Development Insights

This project demonstrates several key game development concepts:

1. **Physics Mastery**: Implementing realistic physics interactions while maintaining performance
2. **Component Design**: Creating reusable, modular components with clear responsibilities
3. **Visual Feedback**: Connecting game events to visual and audio feedback systems
4. **Performance Optimization**: Balancing visual fidelity with runtime performance

## 🔍 Future Enhancements

Potential areas for expansion:
- Multiple machine layouts with varying difficulty
- Custom ball types with unique physics properties
- Advanced scoring systems with multipliers and combos
- Mobile touch controls for broader platform support
- VR implementation for immersive gameplay

## 🧰 Built With

- **Unity 6**: Core game engine
- **Universal Render Pipeline**: Enhanced visual fidelity
- **TextMeshPro**: High-quality text rendering
- **ProBuilder**: Rapid prototyping and modeling

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Based on Unity's "Counting Prototype" tutorial
- Inspired by traditional Japanese pachinko machines
- Special thanks to KeppyLabs mentors and the Unity community