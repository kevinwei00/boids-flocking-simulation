# Boids Flocking Simulation

**Author:** Kevin Wei
**Created:** 3/13/2019

### *Click image to watch the Youtube video*

[![Boids Flocking Simulation](boids-poster.jpg)](https://www.youtube.com/watch?v=0SKBGBOVqho)

## Details
Boids is a program that I wrote in Unity to simulate flocking behavior in 2D. It is part of a series of tech that I built for my top-down shooter to handle swarming enemies. There are many flocking demonstrations out there, but I added my own spin to the algorithm for my game AI by integrating finite state machines, smoother avoidance, and configurable weights for fine-tuning.

The flocking behavior follows the same 3 rules of cohesion, alignment, and separation; though I expanded on separation by adding an orthogonal vector so boids that cannot get close enough to the target will continue circling clockwise on the perimeter. I wanted to give the impression of a horde of horsemen surrounding their hapless target.

Each Boid FSM contains 3 states: *Halt*, *Seek*, and *Retreat*. The boids change color based on their current state for clearer emphasis in this demonstration. I used Unity’s ScriptableObjects to create configurations for different enemy types. This allowed me to easily fine-tune the look and feel of the AI movement patterns.
