namespace WedSite.Pages
{
    public class PartyCardModel
    {
        public PartyCardModel(string image, string name, bool extraWide = false)
        {
            Image = image;
            Name = name;
            ExtraWide = extraWide;
        }

        public string Image { get; set; }

        public string Name { get; set; }

        public bool ExtraWide { get; set; }
    }
}
