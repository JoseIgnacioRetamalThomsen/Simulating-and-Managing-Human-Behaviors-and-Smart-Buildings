# hyper building lights service

To execute the code, ensure the environment from [this link](https://github.com/JoseIgnacioRetamalThomsen/Simulating-and-Managing-Human-Behaviors-and-Smart-Buildings/releases/download/0.0.1/simulation-win64.zip) is up and running.

## Requirements

- **Java**: JDK 8 or later
- **Maven**: [Download and Install](https://maven.apache.org/download.cgi)

## Execution

Once the environment is operational, run the following command from the root project folder:

```bash
mvn compile astra:deploy
```


## Agent Types

Control the lighting system in the simulation. We assign a room agent to each room. 
There's a server agent that distributes sensor events to the room agents through a queue agent. 
The room agent monitors human agents in the room and adjusts the light (turning it on or off) 
accordingly.


## Folder Structure

<pre>
.
├── src/                     # Source files
│   ├── main/
│   │   ├── java/            # Java source files
│   │   └── astra/           # ASTRA source files
├── .gitignore               # List of files and folders ignored by git
├── pom.xml                  # Maven project object model file
└── README.md                # This file
</pre>

