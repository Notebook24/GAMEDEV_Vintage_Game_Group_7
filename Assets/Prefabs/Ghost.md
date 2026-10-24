The ghost has a component GhostAI which handles the ai
its in Assets/Scripts/Entities

it has:
1 REQUIRED SERIALIZED FIELD:
Transform playerTransform; just put the player here

2 EVENTS:
onPlayerCaught - when the player collides with the ghost
onStateChanged - when the monster state changes it also passes the state

2 PUBLIC METHODS:
increaseAgression() - increases aggression
Startle() - will cause the ghost to run away when on the stalking state and freeze in the chasing state


HOW TO USE THE GHOST 
1. Instantiate prefab
2. after catching the player you must destroy the ghost and reinstantiate there is no reset method
3. increase aggression through the game (could be via pellets or time)
4. use startle as a mechanism to scare the monster away
5. use onPlayerCaught to do game over logic
6. use onStateChanged if you need to do logic when the ghost changes state

STATES:

        RunningAway - will try to be as far away from the player as possible, actively runs away from the player, the player should not be able to interact with the ghost here
        NonAggressiveStalking - follows the player close behind but does not attack or move close to the player
        AggresiveStalking - targets and attacks the player, startling will cause it to run away
        ChasingStarted - triggered by startling 3 times, will freeze for a certain amount of seconds before starting chase
        Chasing - a seperate faster chase sequence, startling at this point will NOT cause it to run away but momentarily stun the ghost. Startle it 3 times and it will run away
