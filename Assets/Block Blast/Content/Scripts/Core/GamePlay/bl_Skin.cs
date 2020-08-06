using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bl_Skin : MonoBehaviour {

    public int number;

}


[System.Serializable]
public class Skin {
    
    public bool isSelected;
    public bool isPurchased;
    public int price;
    public int availableLevel;

    public Skin (bool isSelected, bool isPurchased, int price, int availableLevel) {

        this.price = price;
        this.availableLevel = availableLevel;
        this.isSelected = isSelected;
        this.isPurchased = isPurchased;

    }

}


[System.Serializable]

public class AllSkins {
    public Skin[] skins;

    public AllSkins(Skin[] skins) {
        this.skins = skins;
    }

    public Skin[] getSkins() {
        return this.skins;
    }
}