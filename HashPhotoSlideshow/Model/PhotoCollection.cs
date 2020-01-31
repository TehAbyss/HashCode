namespace HashPhotoSlideshow.Model
{
    using System.Collections.Generic;

    public class PhotoCollection : List<Photo>
    {
        public PhotoCollection()
        {
        }

        public PhotoCollection(IEnumerable<Photo> photos)
        {
            AddRange(photos);
        }
    }
}
