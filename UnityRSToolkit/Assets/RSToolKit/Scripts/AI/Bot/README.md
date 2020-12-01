```
 __   __      __   __  ___ 
|__) /__`    |__) /  \  |  
|  \ .__/    |__) \__/  |  
```
RS Bot is an AI system for NPC type characters.

`Bot` classes are designed to be modular to make it easy to add / remove features from your character.

The logic for *Bot* code is seperate into three types:

`Bot` class and children which carry the core logic of the bot.
`BotPart` components that are attached to the `Bot` Game Object.
`BotLogic` are classes that are used by `Bot` and `BotPart` classes.


## Features

### Bot

The `Bot` class has the core tools when it comes with interacting with other `GameObject`s.

#### BotLocomotive

A child of `Bot`, this class holds the core functionality when it comes to locomotion.

#### BotGround

A child of `BotLocomotive` which handles all locomotion on a NavMesh.

#### BotFlyable

A child of `BotLocomotive` for bots that move on a NavMesh but also can fly by using physics.

### BotPart

#### BotPartVision

It is a helper class that makes handles the "Vision" of the bot with functionality like handling the FOV.

#### BotPartWander

Has the core functionality for a BotLocomotive to wander around.

##### BotPartWanderFlying

A child of `BotPartWander` which caters for wandering whislt flying.

##### BotPartWanderNavMesh

A child of `BotPartWander` which caters for wandering whislt on a NavMesh.


#### BotPartWanderManager

Is a manager class that that activates and runs BotPartWander code depending on the Bot locomotion state.
