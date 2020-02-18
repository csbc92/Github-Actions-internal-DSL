package dk.grp1.tanks.core.internal;

import com.badlogic.gdx.ApplicationListener;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.audio.Music;
import com.badlogic.gdx.audio.Sound;
import com.badlogic.gdx.backends.lwjgl.LwjglApplication;
import com.badlogic.gdx.backends.lwjgl.LwjglApplicationConfiguration;
import com.badlogic.gdx.graphics.*;
import com.badlogic.gdx.graphics.g2d.*;
import com.badlogic.gdx.graphics.glutils.ShapeRenderer;
import com.badlogic.gdx.math.EarClippingTriangulator;
import com.badlogic.gdx.utils.FloatArray;
import com.badlogic.gdx.utils.ShortArray;
import dk.grp1.tanks.common.data.*;
import dk.grp1.tanks.common.data.parts.*;
import dk.grp1.tanks.common.eventManager.IEventCallback;
import dk.grp1.tanks.common.eventManager.events.*;
import dk.grp1.tanks.common.services.*;
import dk.grp1.tanks.common.utils.Vector2D;
import dk.grp1.tanks.core.internal.GUI.*;
import dk.grp1.tanks.core.internal.managers.SoundAssetManager;
import dk.grp1.tanks.core.internal.managers.GameAssetManager;
import dk.grp1.tanks.core.internal.managers.GameInputProcessor;

import java.util.ArrayList;
import java.util.List;

import static com.badlogic.gdx.math.MathUtils.random;


public class Game implements ApplicationListener, IEventCallback {
    private final int WIDTH = 800;
    private final int HEIGHT = 600;

    private final boolean DEBUG = false;
    private GameState state;
    private ServiceLoader serviceLoader;
    private World world;
    private GameData gameData;
    private ShapeRenderer shapeRenderer;
    private OrthographicCamera camera;

    private List<IGuiProcessingService> drawImplementations;
    private SpriteBatch uiSpriteBatch;
    private SpriteBatch animationSpriteBatch;
    private SpriteBatch textureSpriteBatch;
    private IGUIEntityProcessingService winDrawer;


    //Variables for drawing the game map
    private Texture gameMapTexture;
    private PolygonSprite gameMapPolySprite;
    private PolygonSpriteBatch polySpriteBatch;
    private TextureRegion textureRegion;
    private Boolean shouldUpdateMap;

    private SoundAssetManager soundAssetManager;

    private List<AnimationWrapper> animationsToProcess;

    // Shake Map Config
    private float elapsed;
    private float duration;
    private float intensity;
    private float baseX;
    private float baseY;

    private GameAssetManager gameAssetManager;

    public Game(ServiceLoader serviceLoader) {
        this.serviceLoader = serviceLoader;
        gameAssetManager = new GameAssetManager();
        initGame();


        LwjglApplicationConfiguration cfg = new LwjglApplicationConfiguration();
        cfg.title = "Tanks";
        cfg.width = WIDTH;
        cfg.height = HEIGHT;
        cfg.useGL30 = false;
        cfg.resizable = true;
        new LwjglApplication(this, cfg);
    }

    public void create() {
        setupAssetManager();
        setupGame();
    }

    private void setupAssetManager() {
        this.soundAssetManager = new SoundAssetManager(Gdx.files.getLocalStoragePath());
        soundAssetManager.loadMusicAsset(this.getClass(), "tron.mp3");
        Music bgMusic = soundAssetManager.getMusicAsset(this.getClass(), "tron.mp3");
        bgMusic.setLooping(true);
        bgMusic.setVolume(0.05f);
        bgMusic.play();

    }

    /**
     * resets the game to begin a new game
     */
    private void restartGame() {
        for (IGuiProcessingService drawImplementation : drawImplementations) {
            drawImplementation.dispose();
        }
        winDrawer.dispose();
        initGame();
        setupGame();
        for (IGamePluginService plugin : serviceLoader.getGamePluginServices()
                ) {
            plugin.stop(world, gameData);

        }
        for (IGamePluginService plugin : serviceLoader.getGamePluginServices()
                ) {
            plugin.start(world, gameData);
        }


    }

    private void initGame() {
        this.world = new World();
        this.gameData = new GameData();
        this.drawImplementations = new ArrayList<>();
        this.animationsToProcess = new ArrayList<>();
        state = GameState.running;

        gameData.getEventManager().register(ExplosionAnimationEvent.class, this);
        gameData.getEventManager().register(SoundEvent.class, this);
        gameData.getEventManager().register(ShakeEvent.class, this);
        gameData.getEventManager().register(GameMapChangedEvent.class, this);


    }

    private void setupGame() {

        setupMapDrawingConfig();


        Gdx.input.setInputProcessor(new GameInputProcessor(gameData));

        camera = new OrthographicCamera(gameData.getGameWidth(), gameData.getGameHeight());
        baseX = camera.viewportWidth / 2f;
        baseY = camera.viewportHeight / 2f;
        camera.position.set(baseX, baseY, 0);
        camera.update();

        this.shapeRenderer = new ShapeRenderer();

        uiSpriteBatch = new SpriteBatch();
        uiSpriteBatch.setProjectionMatrix(camera.combined);

        animationSpriteBatch = new SpriteBatch();
        animationSpriteBatch.setProjectionMatrix(camera.combined);

        textureSpriteBatch = new SpriteBatch();
        textureSpriteBatch.setProjectionMatrix(camera.combined);

        winDrawer = new WinScreenGUI();

        drawImplementations.add(new HealthBarGUI());
        drawImplementations.add(new OnScreenText());
        drawImplementations.add(new WeaponGUI());
        drawImplementations.add(new FirepowerBarGUI());

    }

    private void setupMapDrawingConfig() {
        polySpriteBatch = new PolygonSpriteBatch();

        String path = "mapTexture.png";

        gameMapTexture = gameAssetManager.checkGetTexture(this.getClass(), path);

        textureRegion = new TextureRegion(gameMapTexture);

    }

    public void resize(int i, int i1) {

    }

    public void render() {
        Gdx.gl.glClearColor(0, 0, 0, 1);
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
        gameData.setDelta(Gdx.graphics.getDeltaTime());

        shakeCamera();
        switch (state) {

            case running:
                //shakeCamera();
                update();
                drawBackGround();
                draw();
                break;
            case notRunning:
                drawBackGround();
                drawWinScreen();
                if (gameData.getKeys().isPressed(GameKeys.RESTART)) {
                    restartGame();
                }
        }


    }

    private void playSounds(SoundEvent soundEvent) {

        if (soundEvent.getPath() == null) {
            return;
        }

        soundAssetManager.loadSoundAsset(soundEvent.getSource().getClass(), soundEvent.getPath());
        Sound sound = soundAssetManager.getSoundAsset(soundEvent.getSource().getClass(), soundEvent.getPath());

        if (sound != null) {
            float volume = 1.0f;
            sound.play(volume);
        }
    }

    private void shakeCamera() {
        // Return back to the original position each time before calling shake update.
// We don't update the batch here since we're only using the position for calculating shake.
        camera.position.x = baseX;
        camera.position.y = baseY;
        camera.zoom = 1;

// Update the shake position, then the camera.
        cameraShakeUpdate(gameData.getDelta(), camera);
        camera.update();
    }

    private void drawWinScreen() {


        uiSpriteBatch.setProjectionMatrix(camera.combined);
        winDrawer.drawEntity(gameData.getTurnManager().getRoundWinner(world), gameData, uiSpriteBatch);

    }


    private void drawBackGround() {
        String path = "background.png";

        SpriteBatch spriteBatch = new SpriteBatch();
        spriteBatch.begin();
        spriteBatch.setProjectionMatrix(camera.combined);
        Texture t = gameAssetManager.checkGetTexture(this.getClass(), path);

        spriteBatch.draw(t, 0, 0, gameData.getGameWidth(), gameData.getGameHeight());
        spriteBatch.end();
        spriteBatch.dispose();
    }

    @Override
    public void processEvent(Event event) {

        if (event instanceof ExplosionAnimationEvent) {
            processExplosionAnimationEvent(event);
        } else if (event instanceof SoundEvent) {
            playSounds((SoundEvent) event);
        } else if (event instanceof ShakeEvent) {
            processShakeEvent(event);
        } else if (event instanceof GameMapChangedEvent) {
            shouldUpdateMap = true;
        }

    }

    private void processShakeEvent(Event event) {
        ShakeEvent shakeEvent = (ShakeEvent) event;
        shakeConfig(shakeEvent.getIntensity(), 1000f);
    }

    private void update() {
        for (IEntityProcessingService processingService : serviceLoader.getEntityProcessingServices()) {
            processingService.process(world, gameData);
        }
        for (INonEntityProcessingService iNonEntityProcessingService : serviceLoader.getNonEntityProcessingServices()) {
            iNonEntityProcessingService.process(world, gameData);
        }

        for (IPostEntityProcessingService postEntityProcessingService : serviceLoader.getPostEntityProcessingServices()) {

            postEntityProcessingService.postProcess(world, gameData);
        }

        if (gameData.getTurnManager() != null) {
            if (gameData.getTurnManager().isRoundOver(world)) {
                state = GameState.notRunning;
            }
        }

        gameData.getKeys().update();

    }

    private void processExplosionAnimationEvent(Event event) {
        ExplosionAnimationEvent explosionAnimationEvent = (ExplosionAnimationEvent) event;
        ExplosionTexturePart explosionTexturePart = explosionAnimationEvent.getExplosionTexturePart();
        AnimationWrapper animationWrapper = new AnimationWrapper(explosionAnimationEvent.getPointOfExplosion(),
                explosionTexturePart.getSrcPath(),
                explosionAnimationEvent.getSource(),
                explosionTexturePart.getFrameCols(),
                explosionTexturePart.getFrameRows(),
                explosionAnimationEvent.getExplosionRadius()
        );
        animationsToProcess.add(animationWrapper);

    }

    private void renderGameMap() {
        shapeRenderer.setColor(Color.RED);

        GameMap gameMap = world.getGameMap();
        if (gameMap == null) {

            return;
        }

        if (!DEBUG) {

            if (gameMapPolySprite == null || shouldUpdateMap == true) {
                gameMapPolySprite = new PolygonSprite(convertGameMapToPolyRegion(gameMap));
                shouldUpdateMap = false;
            }

            polySpriteBatch.begin();
            polySpriteBatch.setProjectionMatrix(camera.combined);
            gameMapPolySprite.draw(polySpriteBatch);
            polySpriteBatch.end();
        } else {
            shapeRenderer.setProjectionMatrix(camera.combined);
            shapeRenderer.setColor(Color.RED);
            shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
            List<Vector2D> vertices = gameMap.getVertices(0, gameData.getGameWidth(), 1024);
            for (int i = 0, j = vertices.size() - 1;
                 i < vertices.size(); j = i++) {
                Vector2D vector1 = vertices.get(i);
                Vector2D vector2 = vertices.get(j);
                shapeRenderer.line(vector1.getX(), vector1.getY(), vector2.getX(), vector2.getY());
            }
            shapeRenderer.end();
        }
    }

    private PolygonRegion convertGameMapToPolyRegion(GameMap gameMap) {
        FloatArray vertices = new FloatArray(gameMap.getVerticesAsFloats(0, gameData.getGameWidth(), 1024));
        EarClippingTriangulator triangulator = new EarClippingTriangulator();
        ShortArray triangleIndices = triangulator.computeTriangles(vertices);
        PolygonRegion polygonRegion = new PolygonRegion(textureRegion, vertices.toArray(), triangleIndices.toArray());
        return polygonRegion;
    }


    private void draw() {
        renderGameMap();
        //drawShapes();
        drawTextures();
        drawAnimation();
        drawUI();
    }

    private void drawUI() {
        uiSpriteBatch.setProjectionMatrix(camera.combined);
        for (IGuiProcessingService gui : drawImplementations) {
            gui.draw(world, gameData, uiSpriteBatch);
        }
    }


    public void shakeConfig(float intensity, float duration) {
        this.elapsed = 0;
        this.duration = duration / 1000f;
        this.intensity = intensity;
    }

    public void cameraShakeUpdate(float delta, OrthographicCamera camera) {

        // Only shake when required.
        if (elapsed < duration) {

            // Calculate the amount of shake based on how long it has been shaking already
            camera.zoom = 0.99f;
            float currentPower = intensity * camera.zoom * ((duration - elapsed) / duration);
            float x = (random.nextFloat() - 0.5f) * currentPower;
            float y = (random.nextFloat() - 0.5f) * currentPower;
            camera.translate(-x, -y);

            // Increase the elapsed time by the delta provided.
            elapsed += delta;
        }
    }


    private void drawShapes() {
        for (Entity entity : world.getEntities()) {
            shapeRenderer.setProjectionMatrix(camera.combined);
            shapeRenderer.setColor(1, 1, 1, 1);

            CirclePart cp = entity.getPart(CirclePart.class);
            PositionPart pos = entity.getPart(PositionPart.class);
            if (cp != null) {
                shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
                shapeRenderer.circle(pos.getX(), pos.getY(), cp.getRadius());
                shapeRenderer.end();
            }

            //Draws the cannon if it exists
            CannonPart cannonPart = entity.getPart(CannonPart.class);

            if (cannonPart != null) {

                shapeRenderer.begin(ShapeRenderer.ShapeType.Line);
                shapeRenderer.polygon(Vector2D.getVerticesAsFloatArray(cannonPart.getVertices()));
                shapeRenderer.end();
            }
        }
    }

    private void drawAnimation() {
        List<AnimationWrapper> animationsToRemove = new ArrayList<>();
        animationSpriteBatch.setProjectionMatrix(camera.combined);
        animationSpriteBatch.begin();
        for (AnimationWrapper animationWrapper : animationsToProcess) {
            animationWrapper.updateStateTime(gameData.getDelta());
            Animation animation = gameAssetManager.checkGetAnimation(animationWrapper);
            Vector2D pointOfExplosion = animationWrapper.getPosition();
            TextureRegion currentFrame = animation.getKeyFrame(animationWrapper.getStateTime(), false);
            float animationScale = 6f;
            animationSpriteBatch.draw(currentFrame,
                    pointOfExplosion.getX() - (animationWrapper.getExplosionRadius() * (animationScale / 2)),
                    pointOfExplosion.getY() - (animationWrapper.getExplosionRadius() * (animationScale / 2)),
                    animationWrapper.getExplosionRadius() * animationScale,
                    animationWrapper.getExplosionRadius() * animationScale);
            if (animation.isAnimationFinished(animationWrapper.getStateTime())) {
                animationsToRemove.add(animationWrapper);

            }
        }
        for (AnimationWrapper animationWrapper : animationsToRemove) {
            animationsToProcess.remove(animationWrapper);
        }

        animationSpriteBatch.end();
    }

    private void drawTextures() {
        textureSpriteBatch.setProjectionMatrix(camera.combined);
        textureSpriteBatch.begin();


        for (Entity e : world.getEntities()) {
            TexturePart tp = e.getPart(TexturePart.class);
            PositionPart pp = e.getPart(PositionPart.class);
            CirclePart cp = e.getPart(CirclePart.class);


            if (tp != null && pp != null && cp != null) {
                Texture texture = gameAssetManager.checkGetTexture(e.getClass(), tp.getSrcPath());
                //Sprite sprite = new Sprite(texture);
                textureSpriteBatch.draw(texture, pp.getX() - cp.getRadius(), pp.getY() - cp.getRadius(), cp.getRadius() * 2, cp.getRadius() * 2);
            }

            CannonPart cannonPart = e.getPart(CannonPart.class);
            if (cannonPart != null && pp != null && cp != null) {
                TextureRegion textureRegion = new TextureRegion(gameAssetManager.checkGetTexture(e.getClass(), cannonPart.getTexturePath()));
                drawCannon(textureSpriteBatch, textureRegion, cannonPart);
            }

        }
        textureSpriteBatch.end();
    }

    private void drawCannon(SpriteBatch spriteBatch, TextureRegion textureRegion, CannonPart cannonPart) {
        float x = cannonPart.getVertices()[1].getX();
        float y = cannonPart.getVertices()[1].getY();
        float originX = 0; //(cannonPart.getVertices()[0].getX()-cannonPart.getVertices()[1].getX())/2;
        float originY = 0; //(cannonPart.getVertices()[0].getY()-cannonPart.getVertices()[1].getY())/2;
        float width = cannonPart.getWidth();
        float height = cannonPart.getLength();
        float scaleX = 1; // scale is 100%. 50% is equal to 0.5
        float scaleY = 1; // scale is 100%
        float rotation = (float) Math.toDegrees(cannonPart.getDirection()) - 90;
        spriteBatch.draw(textureRegion, x, y, originX, originY, width, height, scaleX, scaleY, rotation);
    }

    public void pause() {

    }

    public void resume() {

    }

    public void dispose() {
        textureSpriteBatch.dispose();
        animationSpriteBatch.dispose();
        uiSpriteBatch.dispose();
    }

    public World getWorld() {
        return world;
    }

    public GameData getGameData() {
        return gameData;
    }


}
