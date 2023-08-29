# hyper building doors control service

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

This project consists of a service that controls the turnstile systems in the simulations. It listens for card events and sensor events from each turnstile. The system assigns a turnstile agent to each turnstile. There's also a server agent that listens for the events and distributes them to the respective turnstile agents through the queue agent. A database agent is utilized to check the card ID. The turnstile agent opens and closes the door using PUT requests.
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

