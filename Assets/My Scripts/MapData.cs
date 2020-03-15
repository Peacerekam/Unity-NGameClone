using System;

[Serializable]
public class MapData
{
    public string name;
    public string author;
    public string tilesData;
    public string propsData;
    public string highscore;

    public MapData(string newName, string newAuthor, string newTilesData, string newPropsData)
    {
        name = newName;
        author = newAuthor;
        tilesData = newTilesData;
        propsData = newPropsData;
    }

    public void NewHighscore(string newHighscore)
    {
        highscore = newHighscore;
    }
}