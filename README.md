# System Summery

Made using the [Unity](https://unity.com/) game engine 

## Frameworks/Libraries: 

- [Zenject](https://github.com/modesttree/Zenject#readme) for dependency injection
- [NSubstitute](https://github.com/nsubstitute/NSubstitute#readme)  used to mock Monobehavior objects that cannot be instantiated 
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html) For Unit Testing
- [Newtonsoft Json](https://www.newtonsoft.com/json) For deserializing JSON objects

## Core Systems

- [MapManager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/MapManager.cs) was built over Unity's 2d TileMap system to extend it's functionality and link TileData to positions in the world. It is used by other partts of the system to modify and query the tiles on the map. 

- [Pathfinder](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/PathFinder.cs) (not fully refactored) is used to query the path through level and check how build/demolish actions would affect the current path.

- [PathCalculator](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/PathCalculator.cs) was created to address repeating code in the several variations of path preview calculations while refactoring PathFinder.

- [GUIManager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/GUIController.cs) is responsible for displaying game information to the user. It heavily relies on events to decouple it from game logic and implements a basic state machine to keep track of GUI/Game state.

- [GameManager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/GameManager.cs) is responsible for controlling the game state and implements a basic state machine. 

- [WaveManager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/WaveManager.cs) handles spawning enemy waves and also uses a state machine to keep track of wave state

## Custom input system

Typically Unity developers poll mouse actions and key presses in the update method of every Monobehavior object that deals with user input. 
This system however has dedicated classes that poll for input and fire events that other classes respond to.

- [InputHandler](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/InputHandler.cs) polls for a set of configurable hotkeys
- [MouseManager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/MouseManager.cs) fires events when user hovers over a new tile, on left/right mouse up/down, when gameobjects are clicked, and when IToolTipable objects are hovered/unhovered

## Object Pooling

GameObjects that would be frequently created and destroyed are instead recycled to boost performance in the [ObjectPool](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/ObjectPool.cs) class.

The same applies to the [ParticlePool](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/ParticlePool.cs) class

## Build System

Build functionality has been split up into three major components 

- [Build Validator ](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/BuildValidator.cs) which figures out if the conditions for building/demolishing something at a given position are valid or not
- [Hover Manager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/BuildValidator.cs) highlights tile under cursor in green or red while in build/demolsih mode indicating the validitity of the action to the user
- [Build Manager](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/BuildManager.cs) - Responsible for actually building/demolishing things

## Towers and Projectiles

- [Towers](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Tower.cs) target IEffectable objects within range based on user criteria and fire projectiles at them
- [Projectiles ](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Projectile.cs) contain a reference to a EffectGroup object which gets applied to the target on hit

## Status/Effect System

- [EffectGroup](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Effects/EffectGroup.cs) is an object that contains a list of Effects and is stored in the [effects.json](https://github.com/Ayy753/TD_Game/blob/master/Assets/Resources/effects.json) file, and deserialized using [EffectParser](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Managers/EffectParserJSON.cs) 

- [Effects](https://github.com/Ayy753/TD_Game/tree/master/Assets/Scripts/Effects) are composited implementations of the [IEffect](https://github.com/Ayy753/TD_Game/tree/master/Assets/Scripts/Interfaces/Effects) interface and it's derivitives 

- The [Status](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Components/Status.cs) class defines how Effects are applied to [IEffectable](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Interfaces/IEffectable.cs) objects. 

- They are initialized with a "static" [CharacterData](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Components/CharacterData.cs) object reference that defines the base stats of all objects of it's kind (as opposed to creating duplicate variables for each instance)

- [Stat](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Stat.cs) object's value field consist of a base and modification value that gets clamped between a minimum and maximum value. For the sake of performance this value field is only recalculated when the stat gets modified

- [Stats](https://github.com/Ayy753/TD_Game/blob/master/Assets/Scripts/Stats.cs) override the abstract Stat class, adding additional functionality and constraints.

The Resistence class is a generic override of Stat that is used to define each resist type
