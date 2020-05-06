using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quadrante {
    public Sprite spirte;
    public Vector2 largura;
    public Vector2 altura;

    public Quadrante(Sprite spr, Vector2 larg, Vector2 alt)
    {
        this.spirte = spr;
        this.largura = larg;
        this.altura = alt;
    }
}


