package com.ucd.hyperbuilding;

import astra.core.ActionParam;
import astra.core.Agent;
import astra.core.Module;
import astra.formula.Formula;
import astra.formula.Predicate;
import astra.term.ListTerm;

import java.util.*;
import java.util.function.Function;
import java.util.stream.Collectors;

public class Helper extends Module implements AgentEnhancer {

    @TERM
    public String getLocationFromAction(Formula actionFormula) {
        Predicate predicate = (Predicate) actionFormula;
        return predicate.termAt(0).toString().replace("\"", "");
    }
//agent.beliefs().beliefs().stream()
//                    .filter(formula -> formula instanceof Predicate)
//            .map(formula -> (Predicate) formula)
//            .filter(predicate -> predicate.predicate().equals("actualLocation"))
//            .findFirst();

    @TERM
    public String getActionNameWithOutPath(Formula actionFormula) {
        Predicate predicate = (Predicate) actionFormula;
        return predicate.termAt(1).toString().replace("\"", "").
                substring("pathto_".length());
    }

    @ACTION
    public boolean getRandoActionFromList(ListTerm terms, ActionParam<Integer> x, ActionParam<Integer> y,
                                          ActionParam<String> location, String locationToAvoid) {
        List<List<String>> pathtoTerms = terms.stream()
                .map(t -> extractValues(t.toString()))
                .filter(t -> t.get(1).toString().contains("pathto_"))
                .collect(Collectors.toList());
        if (pathtoTerms.size() > 0) {
            List<String> toReturn = null;
            do {
                Collections.shuffle(pathtoTerms);
                toReturn = pathtoTerms.get(0);
            } while (toReturn.get(1) == locationToAvoid);
            x.set(Integer.parseInt(toReturn.get(2).toString()));
            y.set(Integer.parseInt(toReturn.get(3).toString()));
            location.set(toReturn.get(1).substring("pathto_".length()));
            return true;
        }
        x.set(-999);
        y.set(-999);
        return true;
    }

    @ACTION
    public boolean getNearNodeWithLowerAngleToTarget(int targetX, int targetY, int actualX, int actualY,
                                                     ActionParam<Integer> x,
                                                     ActionParam<Integer> y) {
//        List<GridPoint> nearNodes = agent.beliefs().beliefs().stream()
//                .filter(formula -> formula instanceof Predicate)
//                .map(formula -> (Predicate) formula)
//                .filter(predicate -> predicate.predicate().equals("move_x_y_isFree_location_haveAction_actions"))
//                .map(p -> new GridPoint(Integer.parseInt(p.termAt(0).toString()),
//                        Integer.parseInt(p.termAt(0).toString())))
//                .collect(Collectors.toList());
//
//        HashMap<Double, GridPoint> angleToGridPointMap = new HashMap<Double, GridPoint>();
//        nearNodes.forEach(g -> {
//            double angle = calculateAngle(targetX - actualX, targetY - actualY,
//                    g.x - actualX, g.y -actualY);
//            angleToGridPointMap.put(angle,g);
//        });
        Map<Double, GridPoint> angleToGridPointMap =
                agent.beliefs().beliefs().stream()
                        .filter(formula -> formula instanceof Predicate)
                        .map(formula -> (Predicate) formula)
                        .filter(predicate -> predicate.predicate().equals("move_x_y_isFree_location_haveAction_actions"))
                        .filter(predicate -> Boolean.parseBoolean(predicate.termAt(2).toString()))
                        .map(p -> new GridPoint(Integer.parseInt(p.termAt(0).toString()),
                                Integer.parseInt(p.termAt(1).toString())))
                        .collect(Collectors.toMap(
                                g -> calculateAngle(targetX - actualX, targetY - actualY, g.x - actualX, g.y - actualY),
                                Function.identity(),
                                (existing, replacement) -> existing
                        ));
        double minAngle = Collections.min(angleToGridPointMap.keySet());
        GridPoint gridPointWithLowestAngle = angleToGridPointMap.get(minAngle);
        x.set(gridPointWithLowestAngle.x);
        y.set(gridPointWithLowestAngle.y);
        return true;
    }

    private double calculateAngle(int x1, int y1, int x2, int y2) {
        double dotProduct = x1 * x2 + y1 * y2;
        double magnitude1 = Math.sqrt(x1 * x1 + y1 * y1);
        double magnitude2 = Math.sqrt(x2 * x2 + y2 * y2);

        double cosAngle = dotProduct / (magnitude1 * magnitude2);
        double angleRadians = Math.acos(cosAngle);

        return Math.toDegrees(angleRadians);
    }


    @ACTION
    public boolean getRandoActionFromList(ListTerm terms, ActionParam<Integer> x,
                                          ActionParam<Integer> y, ActionParam<String> location) {

        List<List<String>> pathtoTerms = terms.stream()
                .map(t -> extractValues(t.toString()))
                .filter(t -> t.get(1).toString().contains("pathto_"))
                .collect(Collectors.toList());
        if (pathtoTerms.size() > 0) {
            Collections.shuffle(pathtoTerms);
            List<String> toReturn = pathtoTerms.get(0);
            x.set(Integer.parseInt(toReturn.get(2).toString()));
            y.set(Integer.parseInt(toReturn.get(3).toString()));
            location.set(toReturn.get(1).substring("pathto_".length()));
            return true;
        }
        x.set(-999);
        y.set(-999);
        return true;

    }

    private static List<String> extractValues(String input) {
        List<String> values = new ArrayList<>();
        int startIndex = input.indexOf("(");
        int endIndex = input.lastIndexOf(")");
        if (startIndex != -1 && endIndex != -1) {
            String content = input.substring(startIndex + 1, endIndex);
            String[] tokens = content.split(",");
            for (String token : tokens) {
                String trimmed = token.trim();
                if (trimmed.startsWith("\"")) {
                    values.add(trimmed.substring(1, trimmed.length() - 1)); // Remove quotes
                } else {
                    values.add(trimmed);
                }
            }
        }
        return values;
    }

    @TERM
    public String getActionNameFromAction(Formula actionFormula) {
        Predicate predicate = (Predicate) actionFormula;
        return predicate.termAt(1).toString().replace("\"", "");
    }

    @TERM
    public int getXFromAction(Formula actionFormula) {
        Predicate predicate = (Predicate) actionFormula;
        return Integer.parseInt(predicate.termAt(2).toString());
    }

    @TERM
    public int getYFromAction(Formula actionFormula) {
        Predicate predicate = (Predicate) actionFormula;
        return Integer.parseInt(predicate.termAt(3).toString());
    }

    @Override
    public Agent getAgent() {
        return agent;
    }

    private static class GridPoint {
        public int x;
        public int y;

        public GridPoint(int x, int y) {
            this.x = x;
            this.y = y;
        }

        @Override
        public boolean equals(Object o) {
            if (this == o) return true;
            if (o == null || getClass() != o.getClass()) return false;
            GridPoint gridPoint = (GridPoint) o;
            return x == gridPoint.x && y == gridPoint.y;
        }

        @Override
        public int hashCode() {
            return Objects.hash(x, y);
        }
    }
}
