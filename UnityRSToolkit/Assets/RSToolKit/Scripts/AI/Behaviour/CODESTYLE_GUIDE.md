```
 __   __      __   ___                   __        __     ___  __   ___  ___
|__) /__`    |__) |__  |__|  /\  \  / | /  \ |  | |__)     |  |__) |__  |__
|  \ .__/    |__) |___ |  | /~~\  \/  | \__/ \__/ |  \     |  |  \ |___ |___
```
# Code Style Gude

## Regions
- **Behaviour Structs :** Holds the structs used for the Behaviours
- **Init Behaviours :** Holds the methods that initializes Behaviour Nodes
- **Behaviour Logic :** Holds the Functions used by Behaviour Nodes

## Node Variables
### Decleration
- All node variables for a specific behaviour are grouped into structs.
- There is a master struct that holds the root node of the behaviour tree and the struct references of the behaviours.

``` csharp

```

### Naming

#### Actions

- Variable names start with **Do**

For example:
``` csharp
BehaviourAction DoSeekEnemy;
```
### Conditions

- Variable names start with either **Is** or **Has** depending on the situation

``` csharp
BehaviourCondition IsWithinSight;
BehaviourCondition HasFlag;
```

