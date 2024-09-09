using System.Collections.Generic;

[System.Serializable]
public class GameStateData
{
    public int redBlockCount;
    public int greenBlockCount;
    public int blueBlockCount;
    public List<BlockData> blocks = new List<BlockData>();
}
