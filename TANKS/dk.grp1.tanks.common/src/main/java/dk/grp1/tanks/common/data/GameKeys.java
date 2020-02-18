package dk.grp1.tanks.common.data;

public class GameKeys {

    private static boolean[] keys;
    private static boolean[] pkeys;

    public static final int UP = 0;
    public static final int LEFT = 1;
    public static final int DOWN = 2;
    public static final int RIGHT = 3;
    public static final int ENTER = 4;
    public static final int ESCAPE = 5;
    public static final int SPACE = 6;
    public static final int SHIFT = 7;
    public static final int W = 8;
    public static final int A = 9;
    public static final int S = 10;
    public static final int D = 11;
    public static final int N_1 = 12;
    public static final int N_2 = 13;
    public static final int RESTART = 14;

    // The total number of keys in use. This needs to be updated if new keys are added.
    private static final int NUM_KEYS = 15;

    public GameKeys() {
        keys = new boolean[NUM_KEYS];
        pkeys = new boolean[NUM_KEYS];
    }

    /**
     * Update the keys pressed
     */
    public void update() {
        for (int i = 0; i < NUM_KEYS; i++) {
            pkeys[i] = keys[i];
        }
    }

    /**
     * Set if a key is pressed or not
     * @param k
     * @param b
     */
    public void setKey(int k, boolean b) {
        keys[k] = b;
    }

    /**
     * returns if a key is down
     * @param k
     * @return
     */
    public boolean isDown(int k) {
        return keys[k];
    }

    /**
     * returns if a key has been pressed.
     * @param k
     * @return
     */
    public boolean isPressed(int k) {
        return keys[k] && !pkeys[k];
    }

}
