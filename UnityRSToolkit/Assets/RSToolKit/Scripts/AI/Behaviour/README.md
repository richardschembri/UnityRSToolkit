# RSToolkit Behaviour AI
Started as reimplementation of [NPBehave](https://github.com/meniku/NPBehave) in order to learn how behaviour trees work but morphed into it's own beast.


## Behaviour Manager
Is the component to be added to the GameObject of which you wish to give behaviours. 

----------

## Nodes

### Task
#### Action
A method or function that is run. Multiple types of actions are available.
* A single frame action that always returns successful
* A single frame action that can succeed or fail
* A multiple frame action that has multiple **ActionResult**s:
    - **BLOCKED:** Action is not yet ready to run.
    - **PROGRESS:** Action is in progress.
    - **SUCCESS:** Action finished successfully.
    - **FAILED:** Action finished unsuccessfully.
    With the option to monitor the action with **ActionRequest**:
        - **START:** First tick the action or BLOCKED was returned in the last tick.
        - **UPDATE:** PROGRESS returned in the last tick.
        - **CANCEL:** the action should be canceled and return SUCCESS or FAILED.
#### Wait
Waits for a number of seconds before returning true.

#### Wait Until Stopped
Waits until stopped.

### Decorators
Nodes that handle a child node.

#### Condition
Execute child node if the given condition returns true.

#### Cooldown
Runs the child node immediately, but only if last execution wasn't at least past cooldownTime.

#### After Timeout
Runs the given Child Node. If the decoratee finishes sucessfully before the limit time is reached the decorator will wait until the limit is reached and then stop the execution with the result of the Child Node. If the Child Node stops with fail before the limit time is reached, the decorator will immediately stop.

#### Before Timeout
Run the given decoratee. If the decoratee doesn't finish within the limit, the execution fails.

#### Inverter
Inverts the child node's result.

#### Random
Runs the child node with the given probability chance between 0 and 1.

#### Repeater
Repeatedly runs it's child node.

#### Result Overrider
Overrides the child node's result.

#### Service
Run the given service function along with the child node every tick/interval.

#### Wait For Condition
Delay execution of the child node until the condition is true.

#### Blackboard
Blackboards serve a shared memory between multiple Behaviour Trees.

##### Blackboard Condition
Execute the child node only if the Blackboard's key matches the op / value condition. If stopsOnChange is not NONE, the node will observe the Blackboard for changes and stop execution of running nodes based on the stopsOnChange stops rules.

##### Blackboard Query
BlackboardCondition allows to check only one key, BlackboardQuery will observe multiple blackboard keys and evaluate the given query function as soon as one of the value's changes, allowing you to do arbitrary queries on the blackboard. 

### Composites
Parent nodes that manage multiple child nodes.

#### Selector
Runs children sequentially until one succeeds (succeeds if one of the children succeeds).

#### Sequence
Runs children sequentially until one fails and fail (succeeds if none of the children fail).

#### Parallel
Runs child nodes in parallel with stop conditions.
* **ONE_CHILD:** Stops when 1 child stops.
* **ALL_CHILDREN:** Stops when all children stop.

----------

## Silent Mode
You have the ability to Start/RequestStop/ForceStop nodes with a **silent**
parameter so that the behaviour nodes are executed manually instead of automatically. This is useful for synching up AIs that are in a network enviroment where
the network host can switch over to other peers.

It may also be used for a controlled way to see a behaviour run for testing
purposes.
