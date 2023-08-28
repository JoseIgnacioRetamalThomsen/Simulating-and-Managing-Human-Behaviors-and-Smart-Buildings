package com.ucd.hyperbuilding.data;

import java.util.List;

public class BuildingNode {
    public int id;
    public String label;
    public List<Integer> neighboursIds;
    public List<String> actions;

    public BuildingNode(int id, String label, List<Integer> neighboursIds, List<String> actions) {
        this.id = id;
        this.label = label;
        this.neighboursIds = neighboursIds;
        this.actions = actions;
    }

    @Override
    public String toString() {
        return "BuildingNode{" +
                "id=" + id +
                ", label='" + label + '\'' +
                ", neighboursIds=" + neighboursIds +
                ", actions=" + actions +
                '}';
    }
}
