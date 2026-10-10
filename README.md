# AlRahal

AlRahal is a 2D arcade flight game built with Unity. Guide a red bird through moving stone arches, clear openings to score, and beat your best result in Easy or Hard mode.

## Play

1. Open the project in Unity **6000.6.4f1**.
2. Open `Assets/Scenes/AlRahal.unity` and press **Play**.
3. Use **Select Level** to choose Easy or Hard, then press **Begin Journey** on the menu.
4. Press **Space**, left click, or tap to flap. Press **Escape** or the on-screen pause button to pause.

A run ends if the bird hits an arch or leaves the vertical play area. **Try Again** restarts the selected level. The best score for each level is saved locally with Unity PlayerPrefs.

## Levels

| Level | Bird movement | Arch movement | Opening |
| --- | --- | --- | --- |
| Easy | Gentler flap and gravity | Slower | Wider |
| Hard | Faster flap and fall | Faster | Narrower |

The game gradually increases arch speed and narrows openings as the score rises, up to a capped difficulty. The bird's wing animation plays when the player flaps and returns to a glide afterward.

## Project structure

- `Assets/Scenes/AlRahal.unity` contains the playable scene and menus.
- `Assets/Scripts/Runtime/` contains movement, scoring, spawning, audio, background, and UI logic.
- `Assets/Prefabs/PillarPair.prefab` contains the obstacle pair.
- `Assets/Sprites/` contains the artwork used by the game.
- `Assets/Settings/`, `Packages/`, and `ProjectSettings/` contain Unity configuration.

Keep `.meta` files with their matching Unity assets. Generated folders such as `Library/`, `Temp/`, `Logs/`, and `UserSettings/` are excluded from Git.

## Build

Open **File > Build Profiles** in Unity, install support for your target platform if needed, and build with `Assets/Scenes/AlRahal.unity` enabled. Test both levels, scoring, collision, audio, pause, retry, and menu navigation in the built game.
