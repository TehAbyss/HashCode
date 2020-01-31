namespace HashPhotoSlideshow.Model
{
    public class Slide
    {
        public PhotoCollection Photos = new PhotoCollection();

        public Slide()
        {
        }

        public Slide(Photo photo)
        {
            Photos.Add(photo);
        }
    }
}
