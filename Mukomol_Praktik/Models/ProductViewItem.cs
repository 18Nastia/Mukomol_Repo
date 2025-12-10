namespace Mukomol_Praktik.Pages
{
    public class ProductViewItem
    {
        public string Name { get; set; }
        public string Gost { get; set; }
        public string Gluten { get; set; }
        public string White { get; set; }
        public string Brand { get; set; }
        public string Packaging { get; set; }  // теперь есть public set
        public string ImagePath { get; set; }
        public string Type { get; set; }

        // Удобное свойство для XAML: показывает блок веса только для фасованных макарон
        public bool IsPackaged => Type == "PastaPackaged";
    }
}
