# hyper building human agent

To run the code, it is first required that the environment from xxx be running.
After enviroment is up and running , execute the command below from root project folder:
'''mvn compile astra:deploy'''

There are three types of agents implemented; it's more effective to run one agent for each instance of the program.

- The HumanBaseAgent simply moves to a hardcoded (x,y) location on the grid.
- The DiscoverAgent discovers the building and creates a graph of it in its beliefs.
- The MoveAgent first discovers the building, and afterward, it can move to a room within the building. 
Additionally, MoveAgent1 already possesses knowledge of the building, so it can directly move to a room. 
