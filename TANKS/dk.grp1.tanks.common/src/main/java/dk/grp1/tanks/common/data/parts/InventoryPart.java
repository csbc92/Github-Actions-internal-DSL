package dk.grp1.tanks.common.data.parts;

import dk.grp1.tanks.common.data.Entity;
import dk.grp1.tanks.common.data.GameData;
import dk.grp1.tanks.common.data.World;
import dk.grp1.tanks.common.services.IWeapon;
import dk.grp1.tanks.common.services.IWeaponListener;

import java.util.*;

public class InventoryPart implements IEntityPart, IWeaponListener {

    private List<IWeapon> weapons;
    private Map<IWeapon, Integer> weaponAmmo;
    private IWeapon currentWeapon;

    public InventoryPart(){

    }

    public InventoryPart(List<IWeapon> weapons) {
        if (weapons == null) {
            this.weapons = new ArrayList<>();
        } else {
            this.weapons = weapons;
        }
        weaponAmmo = new HashMap<>();
    }

    @Override
    public void processPart(Entity entity, GameData gameData, World world) {
        if (currentWeapon == null && !weapons.isEmpty()) {
             currentWeapon = weapons.get(0);
        }
    }

    /**
     * sets the current weapon to the given weapon
     * @param weapon
     */
    public void setCurrentWeapon(IWeapon weapon) {
        this.currentWeapon = weapon;
    }

    /**
     * sets the weapon to the index given
     * @param i
     */
    public void setCurrentWeapon(int i) {
        this.currentWeapon = weapons.get(i);
    }

    public IWeapon getCurrentWeapon() {
        return this.currentWeapon;
    }

    public void decreaseAmmo() {
        Integer currentWeaponAmmo = weaponAmmo.get(currentWeapon);
        currentWeaponAmmo--;
        weaponAmmo.put(currentWeapon, currentWeaponAmmo);

        if (currentWeaponAmmo <= 0) {
            for (IWeapon weapon : weapons) {
                if (weaponAmmo.get(weapon) > 0) {
                    currentWeapon = weapon;
                    return;
                }
            }
        }
    }

    public List<IWeapon> getWeapons(){
        return this.weapons;
    }


    /**
     * sets current weapon to the next in the list
     * @return
     */
    public IWeapon nextWeapon() {
        if (weapons.size() != 0) {
            int i = weapons.indexOf(this.currentWeapon);
            i++;

            i = Math.floorMod(i, weapons.size());

            if (!weapons.isEmpty()) {
                this.currentWeapon = weapons.get(i);
            } else {
                this.currentWeapon = null;
            }

            return this.currentWeapon;
        }
        this.currentWeapon = null;
        return this.currentWeapon;
    }


    /**
     * sets the current weapon to previous in the list
     * @return
     */
    public IWeapon previousWeapon() {
        if (weapons.size() != 0) {
            int i = weapons.indexOf(this.currentWeapon);
            i--;

            i = Math.floorMod(i, weapons.size());

            if (i == -1) {
                this.currentWeapon = null;
            } else {
                this.currentWeapon = weapons.get(i);
            }

            return this.currentWeapon;
        }
        this.currentWeapon = null;
        return this.currentWeapon;
    }

    /**
     * add weapon to List of weapons
     * @param weapon
     */
    private void addWeapon(IWeapon weapon) {
        if (!weapons.contains(weapon)) {
            this.weapons.add(weapon);
        }
    }

    /**
     * remove weapon from List of weapons
     * @param weapon
     */
    private void removeWeapon(IWeapon weapon) {
        this.weapons.remove(weapon);
        nextWeapon();
    }

    @Override
    public void weaponAdded(IWeapon weapon, Object source) {
        addWeapon(weapon);
    }

    @Override
    public void weaponRemoved(IWeapon weapon, Object source) {
        removeWeapon(weapon);
    }
}
