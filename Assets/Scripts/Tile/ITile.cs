using System;

public interface ITile
{
    public void OnSelect();
    public void SetCoordinates(int x, int y);
    public Tuple<int, int> GetCoordinates();
}