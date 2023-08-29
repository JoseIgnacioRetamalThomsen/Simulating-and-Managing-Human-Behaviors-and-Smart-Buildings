# HyperBuilding

Simulate and manage human behaviors in smart buildings.

## Demo Videos

- **Using environment with Postman:** [Watch here](https://vimeo.com/858805518?share=copy)

- **Deploying services to interact with the environment:** [Watch here](https://vimeo.com/858806427?share=copy)

## Running the Project

### Environment

#### Steps:

- Download the [build](https://github.com/JoseIgnacioRetamalThomsen/Simulating-and-Managing-Human-Behaviors-and-Smart-Buildings/releases/download/0.0.1/simulation-win64.zip).
- Extract the `.zip` file.
- In the extracted folder, execute `AgentSim2023.exe`.

### Services 

Clone the project repository:
`git clone https://github.com/JoseIgnacioRetamalThomsen/Simulating-and-Managing-Human-Behaviors-and-Smart-Buildings/tree/main`

#### Doors service

```bash
cd doors-service
mvn compile astra:deploy
```

#### Rooms service

```bash
cd rooms-service
mvn compile astra:deploy
```

#### Human agent

```bash
cd human-agent
mvn compile astra:deploy
```
