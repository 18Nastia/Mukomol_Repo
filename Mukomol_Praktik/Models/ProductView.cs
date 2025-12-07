public class ProductView
{
    public string Name { get; set; }
    public string Brand { get; set; }
    public string Gost { get; set; }
    public string Gluten { get; set; }
    public string Idk { get; set; }
    public string White { get; set; }
    public int? Packaging { get; set; }
    public string ImagePath { get; set; }
    public string Type { get; set; } // Flour / PastaPackaged / PastaBulk

    /// <summary>
    /// Показывать ли блок "Вес:"
    /// </summary>
    public bool IsPackaged => Type == "PastaPackaged";
}
