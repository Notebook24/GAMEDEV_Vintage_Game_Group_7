The ghost has a component GhostAI which handles the ai
its in Assets/Scripts/Entities/GhostAI

NEEDS:
- a NavMeshAgent on the ghost
- a baked nav surface in the scene (see below)

1 REQUIRED SERIALIZED FIELD:
Transform playerTransform; just put the player here

2 EVENTS:
onPlayerCaught - when the ghost gets within 1 unit of the player (only while aggressive stalking or chasing). fires EVERY FRAME the player is in range so make your game over logic ok with being called a bunch
onStateChanged - when the monster state changes it also passes the new state

4 PUBLIC METHODS:
increaseAgression() - increases aggression by 1
Startle() - scares the ghost, what it does depends on the state (see STATES)
getCurrentState() - returns the current state
isMoving() - true if the ghost is actually moving


HOW TO USE THE GHOST
1. bake a nav surface (IMPORTANT)
    - make an empty gameobject, add a NavMeshSurface component (under AI)
    - Collect Objects should be All Game Objects (or the maze has to be a child of it)
    - hit Bake. redo it if you change the maze
2. Instantiate prefab
3. increase aggression through the game (could be via pellets or time). ghost will stay in RunningAway until aggression is 3
4. use startle as a mechanism to scare the monster away
5. use onPlayerCaught to do game over logic
6. use onStateChanged if you need to do logic when the ghost changes state (audio, ui, etc)
7. after catching the player you must destroy the ghost and reinstantiate there is no reset method

TESTING:
GhostAITester (its in Assets/Scripts/TestTools/GhostAITester) lets you test the ghost without a real game
- put it in the heirarchy
- put ghost inside the serialized field
- right click = increaseAgression()
- left click = Startle()
- you need to right click 3 times before the ghost will leave RunningAway

STATES:
Normal flow is RunningAway -> NonAggressiveStalking -> AggresiveStalking
It only goes into the chase if you startle it enough

RunningAway
- runs as far from the player as it can, player cant interact with it here
- starting state, and where it goes after being startled
- leaves after runAwayDurration seconds (needs aggression 3+)
- startle does nothing here

NonAggressiveStalking
- follows the player's trail from behind, stops when its within NonAggressiveStalkDistance
- does not attack
- switches to AggresiveStalking after a while (shorter as aggression goes up)
- also switches early if the player walks up to it (inside half of NonAggressiveStalkDistance)
- startle sends it to RunningAway

AggresiveStalking
- runs straight at the player, no timer it just keeps coming
- startle sends it to RunningAway

ChasingStarted
- triggered by startling 4 times while stalking (first 3 make it run away)
- freezes for chaseStartedWarning seconds with its light on, then starts chasing
- startle does nothing here

Chasing
- a seperate faster chase
- startle does NOT make it run away, it freezes the ghost for 3 seconds (startling while frozen does nothing)
- startle it 4 times and it gives up, turns the light off and runs away


TWEAKABLE STUFF:
timeBetweenPathFinding - how often it recalculates path
slowStalkSpeed / fastStalkSpeed - non aggressive / aggressive speed
runningAwaySpeed, runningAwayDistance, runAwayDurration - running away stuff
NonAggressiveStalkingBaseDuration - how long it stalks before attacking (at low aggression)
NonAggressiveStalkDistance - how far behind the player it stays
playerTrailGap - how far apart the trail points are
chaseStartedWarning - how long it freezes before the chase
chasingSpeed - MAKE SURE THIS IS FASTER THAN THE PLAYER
