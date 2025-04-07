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
- **Custom Editor Extensions**: Intuitive setup and visualization for bumpers, score pockets, and spinners
- **Ball Award System**: Scoring pockets have a chance to award extra balls based on pocket type, with higher-tier pockets offering better rewards
- **High Score Tracking**: Track both point-based high scores and a separate balls earned high score with celebration effects for new records
- **PBR Materials**: Realistic material appearance
- **Main Menu UI**: Intuitive main menu with How To Play instructions and Credits panel to acknowledge contributors

## 🚀 Installation

### Prerequisites

- Unity 6000.0.041f1 or newer
- Basic understanding of Unity's interface

### Setup

1. Clone this repository: `git clone https://github.com/yourusername/unity-pachinko.git`
2. Open the project in Unity
3. Open the main scene in `Assets/Scenes/PachinkoMachine.unity`
4. Press Play to test the game in the editor

## 🎯 How to Play

1. **Launch Balls**: Hold and release the left mouse button or spacebar to launch balls (hold longer for more power)
2. **Toggle View**: Press 'V' to switch between glass-on and glass-off views
3. **Objective**: Score points by getting balls into the scoring pockets
4. **Game End**: The game ends when you run out of balls

A detailed how-to-play guide is also available in the main menu!

## 🛠️ Technical Implementation

### Core Systems

- **Ball Physics**: Custom-tuned physics materials ensure realistic ball behavior
- **Object Pooling**: Efficient ball management through advanced object pooling
- **Interactive Elements**: Component-based architecture for bumpers, spinners, and scoring zones
- **Audio Management**: Comprehensive audio system with spatial sound and adaptive music
- **Ball Award System**: Configurable chance-based mechanism that awards extra balls when scoring in pockets
  - Low Score pockets (blue): Awards 1 extra ball
  - Medium Score pockets (purple): Awards 2-4 extra balls
  - High Score pockets (yellow): Awards 5-9 extra balls
  - Jackpot pockets (red): Awards 10-19 extra balls plus bonuses based on current score
- **Achievement System**: Track and reward player accomplishments including both score-based and balls earned high scores with celebratory visual effects
- **UI System**: Modular panel system for menus with smooth transitions and consistent navigation

### Code Architecture

The project follows clean code principles with a component-based architecture:

```
- Core/
  |- GameManager.cs        # Central game state management, high score tracking
  |- BallPoolManager.cs    # Object pooling implementation  
  |- Jukebox.cs            # Background music system
  |- Song.cs               # Song configuration for Jukebox
  |- SFXManager.cs         # Sound effect management
  |- Player.cs             # Player input handling
  |- StuckBallTracker.cs   # Tracks and handles balls that get stuck

- Prefabs/
  |- Ball.cs               # Ball behavior and properties
  |- BallLauncher.cs       # Ball launching mechanism
  |- Bumper.cs             # Interactive bumper elements
  |- Cabinet.cs            # Cabinet-related functionality
  |- CollectionTray.cs     # Ball collection management
  |- Pin.cs                # Interactive pin elements
  |- ScorePocket.cs        # Scoring zone implementation
  |- Spinner.cs            # Rotating field elements
  |- BumperEditor.cs       # Custom editor for bumpers
  |- ScorePocketEditor.cs  # Custom editor for score pockets
  |- SpinnerEditor.cs      # Custom editor for spinners
  
- UI/
  |- PowerMeterUI.cs       # Visual launch power indicator
  |- CooldownBarUI.cs      # Cooldown bar UI
  |- GameOverPanel.cs      # Game over screen with high score displays
  |- OptionsPanel.cs       # Options and pause menu
  |- OptionsButton.cs      # Button for options menu
  |- MainMenuManager.cs    # Main menu management
  |- CreditsPanel.cs       # Credits display panel
  |- HowToPlayPanel.cs     # How to play instructions panel
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
5. **Custom Editor Extensions**: Intuitive setup and visualization for bumpers, score pockets, and spinners
6. **PBR Materials**: Realistic material appearance
7. **Modular UI Architecture**: Panel-based UI system with reusable components and consistent navigation

## 🔍 Future Enhancements

Potential areas for expansion:
- Play tray that visually shows balls being fed into the launcher
- Upgrade the Score Pockets and Drain Tray to have more interesting animations
- Multiple machine layouts with varying difficulty
- Custom ball types with unique physics properties
- Advanced scoring systems with multipliers and combos
- Mobile touch controls for broader platform support
- VR implementation for immersive gameplay
- Build out the Shop Menu with custom ball skins and upgrades
- Add a Tutorial Mode to explain the game to new players

## 🧰 Built With

- **Unity 6**: Core game engine
- **Universal Render Pipeline**: Enhanced visual fidelity
- **TextMeshPro**: High-quality text rendering
- **PBR Materials**: Realistic material appearance

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Based on Unity's "Counting Prototype" tutorial
- Inspired by traditional Japanese pachinko machines
- Audio Support From Ovani Sound and Voice Acting by Hannah Weeks
- Special thanks to Pachitalk forums, Pachinkoman, KeppyLabs mentors and the Unity community :shipit: