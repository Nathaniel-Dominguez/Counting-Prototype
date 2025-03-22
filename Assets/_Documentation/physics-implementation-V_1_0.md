# Detailed Physics Implementation
This guide shows how to implement the physics interactions for the key game components

## Part 1: Ball Physics Setup
Creating realistic and consistent pachinko ball behavior

### Ball PhysicsMaterial Setup
Create a new Physics Material asset:
- Name: "BallPhysicsMaterial"
- Dynamic Friction: 0.1
- Static Friction: 0.1  
- Bounciness: 0.8
- Friction Combine: Minimum
- Bounce Combine: Maximum

### Ball Rigidbody Settings
- Mass: 1 (keep consistent for predictable physics)
- Drag: 0.05 (slight air resistance)
- Angular Drag: 0.05 (allows for some spinning)
- Use Gravity: true
- Is Kinematic: false (enable physics)
- Interpolate: Interpolate (smoother motion between frames)
- Collision Detection: Continuous Dynamic (prevents tunneling through objects)
- Constraints:
  - Freeze Position Z (keep ball in 2D plane)
  - Freeze Rotation X (optional, prevents rolling out of plane)
  - Freeze Rotation Y (optional, prevents rolling out of plane)

### Ball Collider Setup
SphereCollider:
- Center: (0, 0, 0)
- Radius: 0.25 (make sure this matches visual radius)
- Material: BallPhysicsMaterial

## Part 2: Bumper Physics and Behavior
Implementing the key interactive element of the pachinko machine

### Bumper Collider Setup
SphereCollider or CylinderCollider:
- Center: (0, 0, 0)
- Radius: 0.5 (for sphere) or Radius/Height for cylinder
- Material: Create a new Physics Material with:
  - Dynamic Friction: 0.3
  - Static Friction: 0.3
  - Bounciness: 0.5

### Bumper Rigidbody Settings
- Mass: 1 (keep consistent for predictable physics)
- Drag: 0.05 (slight air resistance)
- Angular Drag: 0.05 (allows for some spinning)
- Use Gravity: false (no gravity effect)
- Is Kinematic: true (keep stationary)
- Interpolate: Interpolate (smooth motion between frames)
- Collision Detection: Continuous Dynamic (prevents tunneling through objects)
- Constraints:
  - Freeze Position X (keep in place)
  - Freeze Position Y (keep in place)
  - Freeze Position Z (keep in place)

### Bumper Collision Detection
- Use OnCollisionEnter for basic collision detection
- Use OnTriggerEnter for more precise detection (e.g., for bumpers)

### Bumper Force Application
- Use Rigidbody.AddForce to apply a directional force to the ball

### Spinner Physics Implementation
Creating an element that redirects balls in unpredictable ways

### Spinner Collider Setup
- Use a SphereCollider or CylinderCollider
- Center: (0, 0, 0)
- Radius: 0.5 (for sphere) or Radius/Height for cylinder
- Material: Create a new Physics Material with:
  - Dynamic Friction: 0.3
  - Static Friction: 0.3
  - Bounciness: 0.5

### Spinner Rigidbody Settings
- Mass: 1 (keep consistent for predictable physics)
- Drag: 0.05 (slight air resistance)
- Angular Drag: 0.05 (allows for some spinning)
- Use Gravity: false (no gravity effect)
- Is Kinematic: true (keep stationary)
- Interpolate: Interpolate (smooth motion between frames)
- Collision Detection: Continuous Dynamic (prevents tunneling through objects)
- Constraints:
  - Freeze Position X (keep in place)
  - Freeze Position Y (keep in place)
  - Freeze Position Z (keep in place)

### Spinner Force Application
- Use Rigidbody.AddForce to apply a directional force to the ball

### Spinner Collision Detection
- Use OnCollisionEnter for basic collision detection
- Use OnTriggerEnter for more precise detection (e.g., for bumpers)

### Spinner Force Application
- Use Rigidbody.AddForce to apply a directional force to the ball