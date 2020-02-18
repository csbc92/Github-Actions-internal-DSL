package dk.grp1.tanks.weapon.HomingMissile.internal.AI;

import dk.grp1.tanks.common.utils.Vector2D;

import java.util.List;

public interface ITreeSearch {
    /**
     * Searches for a path to a goal state
     * @return The path as a list of nodes, or null if no path was found
     */
    public List<Node> search();

    /**
     * Searches for a path to a goal state.
     * @return the path as a list of coordinates
     */
    public List<Vector2D> searchPoints();
}
