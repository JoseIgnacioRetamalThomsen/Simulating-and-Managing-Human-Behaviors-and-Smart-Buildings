package com.ucd.hyperbuilding;

import astra.core.ActionParam;
import astra.core.Agent;
import astra.core.Module;
import astra.formula.Predicate;
import astra.term.ListTerm;
import astra.term.Primitive;
import astra.term.Term;
import com.google.gson.Gson;
import com.ucd.hyperbuilding.data.*;

import java.util.*;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.function.Function;
import java.util.stream.Collectors;

public class ResponseParser extends Module implements AgentEnhancer {
    private final Gson gson = new Gson();

    private boolean isDebug = true;

    @ACTION
    public boolean tryFindNodeNotInActualViewButInPossibleMove(String movesJson, ActionParam<Integer> x,
                                                               ActionParam<Integer> y) {
        AgentView view = gson.fromJson(movesJson, AgentView.class);
        List<GridNodeData> posibleMoves = view.getPossibleMoves();
        List<GridNodeData> fullLocationView = view.getFullGridView();
        Set<GridNodeData> fullLocationViewSet = new HashSet<>(fullLocationView);
        Set<GridNodeData> nodeNotInLocationButInPosibbleMove = posibleMoves.stream()
                .filter(node -> !fullLocationViewSet.contains(node))
                .filter(node -> node.getSection().equals("transit"))
                .collect(Collectors.toSet());
//        System.out.println("There is a node=" + nodeNotInLocationButInPosibbleMove.size());
//        System.out.println(Arrays.toString(nodeNotInLocationButInPosibbleMove.toArray()));
        if (nodeNotInLocationButInPosibbleMove.size() > 0) {
            GridNodeData node = nodeNotInLocationButInPosibbleMove.stream().findFirst().get();
            x.set(node.getX());
            y.set(node.getY());
        } else {
            x.set(-999);
            y.set(-999);
        }
        return true;
    }

    @TERM
    public String getLocation(String movesJson) {
        AgentView view = gson.fromJson(movesJson, AgentView.class);
        String actualBuildingSection = view.getAgentPosition().getSection();
        return actualBuildingSection;
    }


    @ACTION
    public boolean registerAgent(String agentThingP) {
        log("Parsing new Agent Thing.");
        Thing agentThing = gson.fromJson(agentThingP, Thing.class);
        String viewUrl = agentThing.properties.stream()
                .filter(p -> p.Description.contains("Agent view"))
                .flatMap(p -> p.Forms.stream())
                .map(f -> f.Href)
                .findFirst()
                .get();
        String moveUrl = agentThing.actions.stream()
                .filter(action -> action.Description.contains("Move to position"))
                .flatMap(action -> action.Forms.stream())
                .map(form -> form.Href)
                .findFirst()
                .get();
        String statusUR = agentThing.properties.stream()
                .filter(p -> p.Description.contains("Agent status"))
                .flatMap(action -> action.Forms.stream())
                .map(form -> form.Href)
                .findFirst()
                .get();
        String actionUrl = agentThing.actions.stream()
                .filter(p -> p.Description.contains("Perform action"))
                .flatMap(action -> action.Forms.stream())
                .map(form -> form.Href)
                .findFirst()
                .get();
        log("Agent processed, viewUrl={}, moveUrl={}, staturUrl={}, actionUrl={}", viewUrl, moveUrl, statusUR,
                actionUrl);
        setBelief("getViewUrl", viewUrl);
        setBelief("moveUrl", moveUrl);
        setBelief("statusUrl", statusUR);
        setBelief("actionUrl", actionUrl);
        log("Parsing agent completed, agentBeliefs={}", Arrays.toString(agent.beliefs().beliefs().toArray()));
        return true;
    }

    @ACTION
    public boolean processMoves(String movesJson) {
        //  log("Process moves movesJson={}", movesJson);
        log("Process moves");
        AgentView view = gson.fromJson(movesJson, AgentView.class);
        setBelief("actualPosition_x_y", view.getAgentPosition().getX(), view.getAgentPosition().getY());
        Map<String, Predicate> locationsAgentBelief = getLocationsNodesInAgentBeliefs();
        String actualBuildingSection = view.getAgentPosition().getSection();
        log("Agent belief actual building section={}", actualBuildingSection);
        boolean isNewLocation = isNewLocation(actualBuildingSection, locationsAgentBelief);
        if (isNewLocation && !actualBuildingSection.equals("transit")) {
            log("New location and not transit");
            updateLocationBelief(actualBuildingSection);
            Map<String, Integer> locationToIdMap = new HashMap<>();
            locationsAgentBelief.forEach((k, v) -> {
                locationToIdMap.put(k, Integer.parseInt(v.getTerm(0).toString()));
            });
            log("Location to id map created from agent beliefs, locationToIdMap={}",
                    printMap(locationToIdMap));
            Predicate locationPredicateAgentBeliefs = locationsAgentBelief.get(actualBuildingSection);
            log("Location predicate in agent beliefs={}", locationPredicateAgentBeliefs != null ?
                    locationPredicateAgentBeliefs.toString() : "");
            AtomicInteger idCounter = new AtomicInteger(getLocationIdCounterValueAndRemoveBelif());
            log("Counter created count={}", "" + idCounter.get());
            int actualLocationId = locationToIdMap.getOrDefault(actualBuildingSection, idCounter.getAndIncrement());
            locationToIdMap.putIfAbsent(actualBuildingSection, actualLocationId);
            log("location to id map updated with actual location={}", printMap(locationToIdMap));
            BuildingNode actualLocation = new BuildingNode(actualLocationId,
                    actualBuildingSection, new ArrayList<>(), new ArrayList<>());
            Set<String> neighboursLocationsNodesSet = getNeighboursLocationsNodesList(view);
            log("Got neighbours locations={}", Arrays.toString(neighboursLocationsNodesSet.toArray()));
            //assign ids
            neighboursLocationsNodesSet.forEach(section -> {
                if (locationToIdMap.get(section) != null) {
                    return;
                }
                int sectionId = locationToIdMap.getOrDefault(section, idCounter.getAndIncrement());
                locationToIdMap.putIfAbsent(section, sectionId);
            });
            log("Id assigned to new location, locationToIdMap={}", printMap(locationToIdMap));
            List<BuildingNode> neighborsLocationNodesList = neighboursLocationsNodesSet.stream()
                    .map(st -> new BuildingNode(locationToIdMap.get(st), st, new ArrayList<>(), new ArrayList<>()))
                    .collect(Collectors.toList());
            log("neighborsLocationNodesList={}", Arrays.toString(neighborsLocationNodesList.toArray()));
            List<Primitive> nearLocationsIdList = neighborsLocationNodesList.stream()
                    .map(bn -> bn.id)
                    .map(Primitive::newPrimitive)
                    .collect(Collectors.toList());
            log("nearLocationsIdList={}", Arrays.toString(nearLocationsIdList.toArray()));
            List<AgentAction> actionsThatAgentCanPerformInCurrentSection = view.getFullGridView().stream()
                    .flatMap(l -> l.getActions().stream()
                            .map(as -> new AgentAction(as, l.getX(), l.getY())))
                    .collect(Collectors.toList());
            List<Predicate> actionONFullViewTermList = actionsThatAgentCanPerformInCurrentSection.stream()
                    .map(action -> new Predicate("action_location_actionName_x_y",
                            new Term[]{Primitive.newPrimitive(actualBuildingSection),
                                    Primitive.newPrimitive(action.name),
                                    Primitive.newPrimitive(action.x),
                                    Primitive.newPrimitive(action.y)}))
                    .collect(Collectors.toList());
            log("Available action in current section={}", Arrays.toString(actionONFullViewTermList.toArray()));
            Predicate actualLocationPredicate = new Predicate("buildingNode_id_label_neightboursId_actions",
                    new Term[]{Primitive.newPrimitive((int) actualLocation.id),
                            Primitive.newPrimitive((String) actualBuildingSection),
                            ListTerm.toListTerm(nearLocationsIdList),
                            ListTerm.toListTerm(actionONFullViewTermList)});
            log("New actual location predicate created actualLocationPredicate={}",
                    actualLocationPredicate.toString());
            if (locationPredicateAgentBeliefs != null) {
                log("Removing actual location Belief={}", locationPredicateAgentBeliefs.toString());
                agent.beliefs().dropBelief(locationPredicateAgentBeliefs);
            }
            agent.beliefs().addBelief(actualLocationPredicate);

            List<Predicate> newLocationThatAreNotInAgentBeliefs = neighborsLocationNodesList.stream()
                    .filter(node -> locationsAgentBelief.get(node.label) == null)
                    .map(node -> new Predicate("buildingNode_id_label_neightboursId_actions",
                            new Term[]{Primitive.newPrimitive((int) node.id),
                                    Primitive.newPrimitive((String) node.label),
                                    ListTerm.toListTerm(new ArrayList<>()),
                                    ListTerm.toListTerm(new ArrayList<>())}))
                    .collect(Collectors.toList());
            log("Adding new location ={}",
                    Arrays.toString(newLocationThatAreNotInAgentBeliefs.toArray()));
            newLocationThatAreNotInAgentBeliefs.stream()
                    .forEach(l -> agent.beliefs().addBelief(l));

            Predicate counterPred = new Predicate("locationIdCounter",
                    new Term[]{Primitive.newPrimitive(idCounter.get())});
            log("Adding counter counterPred={}", counterPred.toString());
            agent.beliefs().addBelief(counterPred);
        } else if (actualBuildingSection.equals("transit")) {
            log("Agent in edge node(transition).");
            updateLocationBelief(actualBuildingSection);
            Optional<Predicate> actualLocationOptional = agent.beliefs().beliefs().stream()
                    .filter(formula -> formula instanceof Predicate)
                    .map(formula -> (Predicate) formula)
                    .filter(predicate -> predicate.predicate().equals("actualLocation"))
                    .findFirst();
            if (actualLocationOptional.isPresent()) {
                log("location got={}", actualLocationOptional.get().toString());
            } else {
                updateLocationBelief(actualBuildingSection);
            }
        }

        log("Actions on position=" + Arrays.toString(view.getAgentPosition().getActions().toArray()));
        ArrayList<Primitive> actionsThatAreNotMoveToArea = view.getAgentPosition().getActions().stream()
                .filter(actionString -> !actionString.isEmpty())
                .filter(as -> !as.contains("pathto"))
                .map(Primitive::newPrimitive)
                .collect(Collectors.toCollection(ArrayList::new));

        actionsThatAreNotMoveToArea.forEach(actionPrimitive -> {
            Predicate actionsListTem = new Predicate("actionAvailable",
                    new Term[]{actionPrimitive});
            agent.beliefs().addBelief(actionsListTem);
        });
        addAllPosiblesMovesInActualSpotToAgentBelifs(view);
        log("Finish process moves");
        return true;
    }


    @Override
    public Agent getAgent() {
        return agent;
    }

    private static <K, V> String printMap(Map<K, V> map) {
        String result = map.entrySet().stream()
                .map(entry -> "Key: " + entry.getKey().toString() + ", Value: " + entry.getValue().toString())
                .collect(Collectors.joining(", "));
        return result;
    }

    private void updateLocationBelief(String actualBuildingSection) {
        log("Updating location beliefs.");
        Optional<Predicate> actualLocationOptional = agent.beliefs().beliefs().stream()
                .filter(formula -> formula instanceof Predicate)
                .map(formula -> (Predicate) formula)
                .filter(predicate -> predicate.predicate().equals("actualLocation"))
                .findFirst();
        if (actualLocationOptional.isPresent()) {

            Predicate predicateToRemove = actualLocationOptional.get();
            log("Location present in agent belief, removing it if is not actual" +
                    "predicateToRemove={}", predicateToRemove.toString());
            if (predicateToRemove.toString().equals(actualBuildingSection)) {
                return;
            }
            log("Removing");
            agent.beliefs().dropBelief(predicateToRemove);

        } else {
            log("Not actual location found in agent beliefs.");
        }

        log("Adding actual location actualBuildingSection={}", actualBuildingSection);
        agent.beliefs().addBelief(new Predicate("actualLocation",
                new Term[]{Primitive.newPrimitive(actualBuildingSection)}));
    }

    private boolean isNewLocation(String location, Map<String, Predicate> locationsNodesInAgentBeliefs) {
        Predicate locationPredicate = locationsNodesInAgentBeliefs.get(location);
        if (locationPredicate == null) {
            return true;
        }

        ListTerm listTerm = (ListTerm) locationPredicate.getTerm(2);
        if (listTerm.size() == 0) {
            return true;
        }
        return false;

    }

    private void addActualPosition(AgentView view) {
        Predicate position = new Predicate("actualPosition_x_y",
                new Term[]{Primitive.newPrimitive((int) view.getAgentPosition().getX())
                        , Primitive.newPrimitive((int) view.getAgentPosition().getY())});
        log("Adding actual position, new position={}", position != null ? position.toString() :
                "");
        agent.beliefs().addBelief(position);
    }

    private static Set<String> getNeighboursLocationsNodesList(AgentView view) {
        Set<String> neighboursLocationsNodesList = view.getFullGridView().stream()
                .flatMap(locationNode -> locationNode.getActions().stream()
                        .filter(action -> action.contains("pathto"))
                        .limit(1))
                .map(str -> str.substring(7))
                .collect(Collectors.toSet());
        return neighboursLocationsNodesList;
    }

    private Map<String, Predicate> getLocationsNodesInAgentBeliefs() {
        Map<String, Predicate> locationsNodesInAgentBeliefs = agent.beliefs().store().beliefs().stream()
                .filter(f -> f instanceof Predicate)
                .map(f -> (Predicate) f)
                .filter(p -> p.predicate().equals("buildingNode_id_label_neightboursId_actions"))
                .filter(p -> !p.getTerm(1).toString().equals("\"\""))//fix for quotes bug pepe
                .collect(Collectors.toMap(p -> p.getTerm(1).toString().replace("\"", ""),
                        Function.identity()));
        return locationsNodesInAgentBeliefs;
    }

    private int getLocationIdCounterValueAndRemoveBelif() {
        int idCounter = Integer.parseInt(agent.beliefs().beliefs().stream()
                .filter(f -> f instanceof Predicate)
                .map(f -> (Predicate) f)
                .filter(p -> p.predicate().equals("locationIdCounter"))
                .findFirst()
                .get().getTerm(0).toString());
        agent.beliefs().dropBelief(new Predicate("locationIdCounter"
                , new Term[]{Primitive.newPrimitive((int) idCounter)}));
        return idCounter;
    }

    private void addAllPosiblesMovesInActualSpotToAgentBelifs(AgentView view) {
        view.getPossibleMoves().forEach(posibleMove -> {
            ArrayList<Primitive> list = new ArrayList<>();
            posibleMove.getActions().forEach(a -> list.add(Primitive.newPrimitive(a)));
            agent.beliefs().addBelief(new Predicate("move_x_y_isFree_location_haveAction_actions", new Term[]{Primitive.newPrimitive((int) posibleMove.getX()),
                    Primitive.newPrimitive((int) posibleMove.getY()),
                    Primitive.newPrimitive((boolean) posibleMove.isFree()),
                    Primitive.newPrimitive((String) posibleMove.getSection()),
                    Primitive.newPrimitive(list.size() > 0), ListTerm.toListTerm(list)}));
        });
    }

    @TERM
    public double calculateAngle(int x1, int y1, int x2, int y2) {
        double dotProduct = x1 * x2 + y1 * y2;
        double magnitude1 = Math.sqrt(x1 * x1 + y1 * y1);
        double magnitude2 = Math.sqrt(x2 * x2 + y2 * y2);

        double cosAngle = dotProduct / (magnitude1 * magnitude2);
        double angleRadians = Math.acos(cosAngle);

        return Math.toDegrees(angleRadians);
    }


}

