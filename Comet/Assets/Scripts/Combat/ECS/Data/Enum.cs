namespace comet.combat
{
    public enum EGridType
    {
        Ground,
        Wall,
        Water,
        Grass,
        Max,
    }

    public enum ETerrainTextureType
    {
        None,
        Ground,
        Grass,
        DirtRough,
        Blight,
    }

    public enum ETerrainHeightCtrlType
    {
        Higher,
        Lower,
        
        None,
    }

    public enum ETerrainTextureLayer
    {
        BaseLayer,
        DecoratorLayer,
        
        Max,
    }
}