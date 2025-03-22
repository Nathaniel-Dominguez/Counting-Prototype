# Pachinko Prefab Implementation Guide
This guide shows how to create and configure the prefabs in Unity

## Part 1: Ball Prefab
The foundation of gameplay - a simple sphere with physics properties

1. Create a sphere primitive (GameObject > 3D Object > Sphere)
2. Set its scale to (0.5, 0.5, 0.5) for proper pachinko ball size 
3. Add the following components:

### Rigidbody Component Settings
- Mass: 1
- Drag: 0.05
- Angular Drag: 0.05
- Use Gravity: true
- Is Kinematic: false
- Interpolate: Interpolate
- Collision Detection: Continuous Dynamic
- Constraints: Freeze Position Z, Freeze Rotation X, Freeze Rotation Y

### Custom Physics Material
- Dynamic Friction: 0.1
- Static Friction: 0.1
- Bounciness: 0.8
- Friction Combine: Minimum
- Bounce Combine: Maximum

### Trail Renderer
- Time: 0.2
- Width: 0.1 to 0
- Material: Create a new material with "Particles/Standard Unlit"
- Color: White with alpha fade

Add Tag "Ball" to GameObject

## Part 2: Basic Pin Prefab
Simple obstacles that balls bounce off

1. Create a cylinder primitive (GameObject > 3D Object > Cylinder)
2. Set its scale to (0.2, 1.5, 0.2) for thin pin shape
3. Rotate to stand upright
4. Add the following components:

### CapsuleCollider Settings
- Center: (0, 0, 0)
- Radius: 0.1
- Height: 1.5
- Direction: Y-Axis

### Custom Physics Material
- Dynamic Friction: 0.2
- Static Friction: 0.2
- Bounciness: 0.7
- Friction Combine: Average
- Bounce Combine: Maximum

### Metal Material
- Metallic: 0.9
- Smoothness: 0.8
- Base Color: Silver (#C0C0C0)

## Part 3: Bumper Prefab
Interactive elements that provide strong repulsion

1. Create a cylinder primitive (GameObject > 3D Object > Cylinder)
2. Set its scale to (1, 0.5, 1) for flat bumper shape
3. Add the following components:

### SphereCollider Settings
- Center: (0, 0, 0)
- Radius: 0.5
- Is Trigger: false

### Bumper Script Settings
- Bounce Force: 5.0
- Min Impact Velocity: 2.0
- Cooldown Time: 0.1

### Light Component
- Type: Point
- Color: Matching the bumper color
- Range: 2
- Intensity: 0.5 (will be modified by script)

### Particle System for Impact
- Duration: 0.2
- Looping: false
- Start Lifetime: 0.5
- Start Speed: 3
- Start Size: 0.1
- Shape: Sphere
- Emission: 20 particles on collision

### Emission Material
- Base Color: Red, Blue, Yellow, or Green
- Emission: Same color but brighter
- Metallic: 0.3
- Smoothness: 0.7

## Part 4: Spinner Prefab
Rotates to create unpredictable ball trajectories

1. Create an empty GameObject
2. Add a child GameObject with a custom mesh (triangle or arrow)
3. Add the following components to the parent:

### Spinner Script Settings
- Rotation Speed: 120.0
- Rotation Axis: (0, 0, 1) for Z-axis rotation
- Randomize Direction: true
- Direction Change Interval: 3.0
- Spinner Friction: 0.5
- Ball Spin Torque: 2.0
- Alternatively use the Spinner Config options
- Fast spinner - 500 speed
- Standard spinner - 360 speed
- Heavy spinner - 200 speed

### Rigidbody Settings
- Mass: 1
- Drag: 0.05
- Angular Drag: 0.05
- Use Gravity: false
- Is Kinematic: true
- Interpolate: Interpolate
- Collision Detection: Continuous Dynamic
- Constraints: Freeze Position X, Freeze Position Y, Freeze Position Z, Freeze Rotation X, Freeze Rotation Y

Add appropriate collider to match the visual shape

## Part 5: Scoring Zone Prefab
Areas that award points when balls enter

1. Create a cylinder primitive (GameObject > 3D Object > Cylinder)
2. Scale and modify to create a recessed area
3. Add the following components:

### BoxCollider/SphereCollider
- Is Trigger: true

### Counter Script Settings
- Point Value: (100, 300, or 1000 depending on difficulty)
- Ball Tag: "Ball"
- Score Delay: 0.2

### Light Component
- Type: Point
- Color: Matching the zone color
- Range: 2
- Intensity: 0 (will be modified by script)

### ParticleSystem for Celebration
- Duration: 1
- Looping: false
- Shape: Cone
- Emission: 50 particles on scoring
- Color: Gradient from zone color to bright white

## Part 6: Cabinet Frame Prefab
The outer structure of the pachinko machine

1. Create empty GameObject "Cabinet"
2. Add child cubes and shape into a frame
3. Structure recommended hierarchy: