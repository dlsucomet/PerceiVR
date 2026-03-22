

# PerceiVR


## Overview of Thesis

PerceiVR: The Space Between is a Unity-based VR prototype for the Meta Quest 2. It is designed to help educators understand the classroom experiences of autistic students. The project uses interactive narrative scenes based on real accounts from autistic individuals, with input from experts and online ASD communities.

The application is built in Unity and uses the XR Interaction Toolkit, Meta Movement SDK, and OVR Framework for VR input and interaction. Users interact with the system using the Quest controllers for actions like grabbing, poking, and self-soothing gestures. The project includes custom scripts for these interactions, as well as dynamic audio and visual effects. 

**Proponents:** Cloma, Shania Francine T. | Jugno, Chiara Louise C. | Roman, Isaac Nathan | Siazon, Jalene Graciella T.  
**Adviser:** Jordan Aiko Deja  
**Co-Adviser:** Christian Terrence Esguerra

## Project File Structure Overview

```
PerceiVR/
│
├── Assets/                       # Main Unity assets (scenes, prefabs, scripts, resources, etc.)
│   ├── InteractionSDK/           # Interaction SDK assets
│   ├── MetaXR/                   # Meta XR integration assets
│   ├── Misc/                     # Miscellaneous assets
│   ├── Oculus/                   # Oculus integration assets
│   ├── Plugins/                  # Plugins for Unity
│   ├── Resources/                # Unity Resources folder
│   ├── Samples/                  # Sample assets
│   ├── Settings/                 # Project settings assets
│   ├── The Space Between/        # Main assets for the 'The Space Between' VR prototype
│   │   ├── Animations/           # Animation clips and controllers for characters and scenes
│   │   ├── Art/                  # 2D/3D art assets, including environments, materials, skyboxes, textures, and UI
│   │   ├── Audio/                # All audio assets (sound effects, music, ambient sounds, voice clips)
│   │   ├── Characters/           # 3D character models and related assets
│   │   ├── Prefabs/              # Prefabricated objects for reuse (UI, camera rigs, etc.)
│   │   ├── Scenes/               # Unity scene files and scene-specific assets
│   │   ├── Scripts/              # C# scripts for gameplay, interaction, and narrative logic
│   │   ├── Signals/              # Custom Unity signal assets for event-driven logic
│   │   └── Timelines/            # Unity Timeline assets for sequencing events and cutscenes
│   ├── VR Assets/                # VR-specific assets
│   ├── XR/                       # XR-related assets
│   ├── XRI/                      # XR Interaction Toolkit assets
│   └── ...                       # Animations, signals, materials, terrains, etc.
│
├── GeneratedAssets_deleted/      # Deleted/generated assets (backup or temp)
│   └── ...
│
├── Packages/                     # Unity package manifest and lock files
│   ├── manifest.json
│   └── packages-lock.json
│
├── ProjectSettings/              # Unity project settings files
│   └── ...
│
├── The Space Between_BurstDebugInformation_DoNotShip/  # Burst compiler debug info
│   └── tempburstlibs/
│
├── README.md                     # Project documentation
├── UpgradeLog.htm                # Unity upgrade log
├── UpgradeLog2.htm               # Unity upgrade log
└── ...                           # Other project files
```

---
This overview summarizes the main folders and their purposes in the PerceiVR Unity project. For more details, see the contents of each folder.

