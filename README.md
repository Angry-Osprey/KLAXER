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

## The game

Break all 35 bricks without losing your 3 lives. Higher rows are worth more (50 points at the top down to 10 at the bottom).

| Action | Touch / mouse | Keyboard |
|---|---|---|
| Move the paddle | Drag | Left/Right arrows or A/D |
| Launch the ball | Lift your finger / release the button | Space |
| Restart after the round ends | Tap | Any key |

Where the ball hits the paddle sets its angle: the edges send it off sharply, the centre sends it straight up.

## Opening the project

1. Install [Git LFS](https://git-lfs.com) before cloning; sprites and fonts are stored in LFS.
2. Open the folder in Unity Hub with **Unity 6000.6.2f1** (Unity 6.6).
3. Open `Assets/Scenes/BrickBreaker.unity` and press Play.
4. Set the Game view to a portrait resolution (for example 1080×1920) to see it as intended.

Built with the Universal Render Pipeline, the Input System and TextMeshPro.

## Code layout

| Script | Job |
|---|---|
| `GameManager` | The rules and flow: launching, score, lives, win/lose, restart |
| `BrickGrid` | Builds the brick wall at runtime, sized to the screen |
| `Brick` | Reports when the ball breaks it |
| `BallController` | Ball movement and paddle bounce angles |
| `PaddleController` | Moves the paddle from input |
| `ArenaBounds` | Fits the walls and the bottom "death zone" to the camera |
| `DeathZone` | Reports a lost ball |
| `Hud` | Score, lives and game-over text |
| `GameInput` | The one place input is read |

Scripts live in `Assets/Scripts`, the brick prefab in `Assets/Prefabs`, and sprites in `Assets/Art`.
