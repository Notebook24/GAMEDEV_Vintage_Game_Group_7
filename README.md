# PAC: After Dark

*A first-person Pac-Man horror survival game built in Unity (3D).*

## Group Members

**Group 7**

| Name | Role |
|------|------|
| Joramm Dela Torre | SWE 1: Core State & Architecture (GameManager, UI) |
| Alain Zuriel Marcos | SWE 2: Player Controller & Survival Mechanics |
| Nykko Vanguardia | SWE 3: AI Logic (Ghosts) & Camera Flash |
| VL "Kei" Saguin | SWE 4: Level Engineering & Interactions (Maze, Lighting, Dots) |

## Description

PAC: After Dark is a 3D first-person horror survival game that reimagines Pac-Man from the inside of the maze. The player wanders a pitch-black maze with nothing but a flashlight, collecting every scattered dot while Ghosts relentlessly hunt them down. There are no weapons and no combat, so survival depends on evasion, spotting Ghosts early with the flashlight beam, and managing a limited sprint stamina bar to sprint away when they get close. Collect all the dots to win, but if a Ghost touches you, it's game over.

## Why We Chose This Combination

We combined pac-man's gameplay of collecting dots while ghosts chase you with first-person horror because both aspects are built on thrill and tension. we thought that moving in the maze in the dark and seeing it through the player' own eyes turns that tension into fear, since you can only see what your flashlight lights up, while all the ghosts are trying to find and chase you! Pac-Man's simple rules also split cleanly into separate systems (movement, ghost AI, collectibles, game state), which suited our team of four and our one-week timeline.

## Controls

| Input | Action |
|-------|--------|
| **W / A / S / D** | Move forward / left / backward / right |
| **Mouse** | Look around (up/down camera tilt is clamped to -90/90 degrees) |
| **Left Mouse Button** | Camera Flash |
| **Left Shift** (hold) | Sprint (drains stamina, increases speed by 1.8x) |

**Camera:** your main weapon against the ghosts. Aim towards the ghost and click the left mouse button to flash your light, this will force them to run away. It has a battery and each use drains it, be careful!

**Stamina:** sprinting drains the stamina bar in the bottom-left corner. It regenerates after a short delay when you stop sprinting. If it hits zero you can't sprint again until it recovers at least 10% of the maximum stamina (stamina is measured from 0 to 100).

## Objective

- **Win:** collect all the dots in the maze.
- **Lose:** a Ghost catches you.

## Built With

- Unity 6 (6000.6)
- C#
