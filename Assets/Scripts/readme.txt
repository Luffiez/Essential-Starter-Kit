# Essential Starter Kit – System Overview

Essential Starter Kit provides a set of foundational systems for Unity projects, including scene management, audio management, and global game state access. 
This package is designed to help you quickly add robust, reusable functionality to your Unity games, enabling rapid prototyping and clean project structure. 
With custom editors and flexible components, you can easily manage scenes, play and control audio, and centralize your game’s core logic.


## Game

- The `Game` class is a MonoBehaviour singleton that acts as the entry point for your project.
- It provides static access to core systems like `Game.SceneManager` and `Game.Settings`.
- Usage:
  - Access global systems via `Game.SceneManager` and `Game.Settings`.
  - Place the Game object in your initial scene.

---

## Audio System

- The audio system is managed by the `AudioManager` singleton.
- Audio settings are configured via the `AudioSettings` ScriptableObject, which references two `AudioLibrary` assets: one for BGM and one for SFX.
- Audio clips are described by `AudioClipInfo` objects, which store the clip, name, and channel.
- Usage:
  - Use `AudioTrigger` components to play BGM or SFX by selecting a clip from the libraries in the inspector.
  - The `AudioTriggerEditor` provides a dropdown to select BGM/SFX and the desired clip.
  - The system ensures BGM clips are always set to the BGM channel and SFX clips to the SFX channel.
  - To stop BGM, select the `[STOP BGM]` option in the AudioTrigger.
  - Played BGM or SFX via code by calling 'Game.Audio.Play()'
  - Stop BGM via code by calling 'Game.Audio.StopBgm()'

---

## SceneManager System

- The `SceneManager` class manages scene loading and unloading.
- Scenes are defined in a `ScenesSO` ScriptableObject, which lists all available scenes and their properties.
- Usage:
  - Use 'SceneLoader' components to load scenes by selecting a 'SceneInfo' from the 'ScenesSO' list in the inspector.
  - Use 'SceneUnloader' components to unload scenes by selecting a 'SceneInfo' from the 'ScenesSO' list in the inspector and calling 'UnloadScene'.
  - Use `Game.SceneManager.LoadScene(SceneInfo)` to load a scene (single or additive).
  - Use `Game.SceneManager.UnloadScene(SceneInfo)` to unload a scene.
  - The system tracks loaded scenes and prevents duplicate loading.
  - Scene selection and management can be extended via custom editor tools.

---

How to Use

1. Assign your `AudioSettings` and `ScenesSO` assets in the 'SettingsSO' in the 'game' monobehaviour.
2. Use the provided custom editors to configure scenes and audio clips.
3. Add `AudioTrigger` and scene management components to your GameObjects as needed.
4. Acces manager classes through the 'Game' class
