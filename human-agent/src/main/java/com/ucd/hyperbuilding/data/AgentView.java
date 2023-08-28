package com.ucd.hyperbuilding.data;

import java.util.List;

public class AgentView {
    private GridNodeData agentPosition;
    private List<GridNodeData> possibleMoves;
    private List<GridNodeData> fullGridView;

    public GridNodeData getAgentPosition() {
        return agentPosition;
    }

    public void setAgentPosition(GridNodeData agentPosition) {
        this.agentPosition = agentPosition;
    }

    public List<GridNodeData> getPossibleMoves() {
        return possibleMoves;
    }

    public void setPossibleMoves(List<GridNodeData> possibleMoves) {
        this.possibleMoves = possibleMoves;
    }

    public List<GridNodeData> getFullGridView() {
        return fullGridView;
    }

    public void setFullGridView(List<GridNodeData> fullGridView) {
        this.fullGridView = fullGridView;
    }

}
