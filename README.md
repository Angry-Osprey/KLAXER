# KLAXER

A small portrait brick breaker for mobile, built in Unity.

## Why this project exists

This is my first attempt at expanding what I can do with Claude. Until now I'd mostly used it in a chat window: ask a question, copy some code, paste it in. Here I wanted to go further and learn to use **skills** and **command-line tools**, so Claude can work directly inside a real project rather than just talking about it.

The game is deliberately simple. It's the practice ground; the real subject is the workflow.

## How it's being built

- **Claude Code** (in the Claude desktop app), working directly in this repository.
- **Skills.** Packaged instructions Claude loads for a specific kind of task. The main one here is Unity's `unity-cli` skill, which teaches Claude how to control a running Unity Editor.
- **The Unity CLI (`unity`).** Together with the Pipeline package (`com.unity.pipeline`), it lets Claude send commands to the open Editor (`unity command ...`). Claude uses it to create prefabs, wire up components, save scenes, enter Play mode, simulate input and take screenshots, instead of hand-editing Unity's scene files.
- **git, Git LFS and GitHub** for version control. `main` holds stable milestones; all new work happens on `dev`.

### What I've tried so far

- Had Claude build a first version of the game, then review and refactor its own design choices.
- Drove the Unity Editor entirely from the command line: building a brick prefab, replacing 35 hand-placed bricks with a runtime grid, converting the UI to TextMeshPro, and play-testing with simulated mouse and keyboard input.
- Set up branches, Git LFS and pushes to GitHub along the way.
- Built Android APKs from the command line and tested them on my phone. The first one showed only the score and lives on a grey screen. Claude traced it through the build report and the Editor log to Unity reusing stale cached build data, which left out the shaders that put the camera image on screen. A clean build fixed it.
- Added new features on the `dev` branch: a 200-brick wall and falling power-ups, tested in the Editor by stepping the physics from the command line.

### Where it's at

The latest work (200 bricks and power-ups) is on `dev` and still needs a test on a real phone. `main` holds the last stable milestone.

## The game

Break all 200 bricks (a 10 × 20 wall) without losing your 3 lives. Higher rows are worth more (50 points at the top down to 10 at the bottom).

| Action | Touch / mouse | Keyboard |
|---|---|---|
| Move the paddle | Drag | Left/Right arrows or A/D |
| Launch the ball | Lift your finger / release the button | Space |
| Restart after the round ends | Tap | Any key |

Where the ball hits the paddle sets its angle: the edges send it off sharply, the centre sends it straight up.

### Power-ups

Each broken brick has a 12% chance of dropping a power-up. Catch it with the paddle to use it.

| Pickup | Effect |
|---|---|
| **W** | Wider paddle for 10 seconds |
| **M** | Multi-ball: two extra balls (up to 8 in play). Losing an extra ball is free; only the last one costs a life |
| **S** | Slower balls for 10 seconds |
| **+** | Extra life |

Catching a timed power-up again restarts its timer rather than stacking. Drop chance, odds, durations and strengths are all set in the Inspector (on the `PowerUps` and `GameManager` objects).

## Opening the project

1. Install [Git LFS](https://git-lfs.com) before cloning; sprites and fonts are stored in LFS.
2. Open the folder in Unity Hub with **Unity 6000.6.2f1** (Unity 6.6).
3. Open `Assets/Scenes/BrickBreaker.unity` and press Play.
4. Set the Game view to a portrait resolution (for example 1080×1920) to see it as intended.

Built with the Universal Render Pipeline, the Input System and TextMeshPro.

**Building for Android:** always do a clean build, which ignores Unity's cached build data. An incremental build once produced an APK that only showed the HUD on a grey screen (see above). The app installs as `com.klaxer.brickbreaker`.

## Code layout

| Script | Job |
|---|---|
| `GameManager` | The rules and flow: launching, score, lives, power-up effects, win/lose, restart |
| `BrickGrid` | Builds the brick wall at runtime, sized to the screen |
| `Brick` | Reports when the ball breaks it |
| `PowerUpDropper` | Rolls for a power-up when a brick breaks and drops it |
| `PowerUp` | A falling pickup; reports when the paddle catches it |
| `BallController` | Ball movement and paddle bounce angles |
| `PaddleController` | Moves the paddle from input |
| `ArenaBounds` | Fits the walls and the bottom "death zone" to the camera |
| `DeathZone` | Reports a lost ball |
| `Hud` | Score, lives and game-over text |
| `GameInput` | The one place input is read |

Scripts live in `Assets/Scripts`, the brick and power-up prefabs in `Assets/Prefabs`, and sprites in `Assets/Art`.
