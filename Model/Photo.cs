namespace HashPhotoSlideshow
{
    using System.Collections.Generic;

    public class Photo
    {
        public int Id;
        public Orientation Orientation;
        public List<string> Tags = new List<string>();
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
}
