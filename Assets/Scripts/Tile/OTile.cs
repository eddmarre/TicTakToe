using System;
using UnityEngine;

public class OTile : MonoBehaviour, ITile
{
    private Tuple<int, int> _coords;

    public void OnSelect()
    {
    }

    public void SetCoordinates(int x, int y) => _coords = new Tuple<int, int>(x, y);

    public Tuple<int, int> GetCoordinates() => _coords;
}