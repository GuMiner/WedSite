namespace WedSite.Pages
{
    public class PartyCardModel
    {
        public PartyCardModel(string image, string name)
        {
            Image = image;
            Name = name;
        }

        public string Image { get; set; }

        public string Name { get; set; }
    }
}
