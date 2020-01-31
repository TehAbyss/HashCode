namespace HashPhotoSlideshow.Model
{
    using System.Collections.Generic;

    public class Photo
    {
        public int Id;
        public Orientation Orientation;
        public List<string> Tags = new List<string>();
    }
}
