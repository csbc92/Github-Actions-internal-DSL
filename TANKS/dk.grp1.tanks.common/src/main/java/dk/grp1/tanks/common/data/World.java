package dk.grp1.tanks.common.data;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

/**
 * Created by danie on 12-03-2018.
 */
public class World   {
    private final Map<String, Entity> entityMap = new ConcurrentHashMap<>();
    private GameMap gameMap;

    /**
     * Gets the game map
     * @return The game map as type GameMap
     */
    public GameMap getGameMap() {
        return gameMap;
    }

    /**
     * Sets the game map.
     * @param gameMap
     */
    public void setGameMap(GameMap gameMap) {
        this.gameMap = gameMap;
    }

    /**
     * Adds an entity to the world
     * @param entity
     * @return
     */
    public String addEntity(Entity entity) {
        entityMap.put(entity.getID(), entity);
        return entity.getID();
    }

    public void removeEntity(String entityID) {
        entityMap.remove(entityID);
    }
    /**
     * Removes an entity to the world
     * @param entity
     * @return
     */
    public void removeEntity(Entity entity) {
        entityMap.remove(entity.getID());
    }

    /**
     * Gets all entities in the world
     * @return
     */
    public Collection<Entity> getEntities() {
        return entityMap.values();
    }

    /**
     * Gets all entities of a given type in the world
     * @param entityTypes
     * @param <E>
     * @return
     */
    public <E extends Entity> List<Entity> getEntities(Class<E>... entityTypes) {
        List<Entity> r = new ArrayList<>();
        for (Entity e : getEntities()) {
            for (Class<E> entityType : entityTypes) {
                if (entityType.isInstance(e)) {
                    r.add(e);
                }
            }
        }
        return r;
    }

    /**
     * Gets an entity in the world based on it's id
     * @param ID
     * @return
     */
    public Entity getEntity(String ID) {
        return entityMap.get(ID);
    }
}
