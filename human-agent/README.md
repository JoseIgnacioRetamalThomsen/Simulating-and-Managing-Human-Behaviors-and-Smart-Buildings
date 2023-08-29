# Hyper Building Human Agent

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

There are three distinct agent types. For optimal results, run one agent per program instance:

- **HumanBaseAgent**: This agent simply navigates to a predefined (x,y) position on the grid.
- **DiscoverAgent**: Discovers the building layout and constructs a graph of it in its beliefs.
- **MoveAgent**: Initially explores the building. Post exploration, it can relocate to a specific room within the building. Meanwhile, `MoveAgent1` is pre-equipped with building knowledge, allowing it to instantly navigate to a room.

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