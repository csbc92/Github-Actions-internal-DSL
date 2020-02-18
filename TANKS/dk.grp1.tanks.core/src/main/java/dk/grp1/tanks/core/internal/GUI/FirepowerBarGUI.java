package dk.grp1.tanks.core.internal.GUI;

import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.Pixmap;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import dk.grp1.tanks.common.data.Entity;
import dk.grp1.tanks.common.data.GameData;
import dk.grp1.tanks.common.data.GameKeys;
import dk.grp1.tanks.common.data.World;
import dk.grp1.tanks.common.data.parts.CannonPart;

public class FirepowerBarGUI implements IGuiProcessingService {
    private Texture firepowerTexture;
    private Pixmap pixmap;
    private float barHeight = 23.5f;
    private float barWidth = 5;
    private float xOffSet = 20;
    private float yOffSet = -5;

    /**
     * Draws a Fire power bar next to each entity that has a CannonPart
     * @param world current game world and entities
     * @param gameData
     * @param spriteBatch spriteBatch with camera set
     */
    @Override
    public void draw(World world, GameData gameData, SpriteBatch spriteBatch) {
        for (Entity entity : world.getEntities()) {
            CannonPart cannonPart = entity.getPart(CannonPart.class);
            if (cannonPart != null && gameData.getKeys().isDown(GameKeys.SPACE)) {
                firepowerBar(entity, cannonPart, spriteBatch);
            }
        }

    }

    @Override
    public void dispose() {

    }

    /**
     * Draw the firepower bar.
     * @param entity
     * @param cannonPart
     * @param batch
     */
    private void firepowerBar(Entity entity, CannonPart cannonPart, SpriteBatch batch) {
        pixmap = new Pixmap(1, 1, Pixmap.Format.RGBA8888);
        pixmap.setColor(Color.CYAN);
        pixmap.fill();
        firepowerTexture = new Texture(pixmap);
        batch.begin();
        batch.draw(firepowerTexture, cannonPart.getJointX() + xOffSet, cannonPart.getJointY() + yOffSet, barWidth
                , barHeight * cannonPart.getFirepower() / cannonPart.getMaxFirepower());
        batch.end();
        pixmap.dispose();
        firepowerTexture.dispose();
    }

}
