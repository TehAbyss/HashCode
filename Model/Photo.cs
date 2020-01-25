namespace HashPhotoSlideshow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    public class Photo
    {
        public int Id;
        public Orientation Orientation;
        public List<string> Tags;

        
    }

    public enum Orientation
        {
            Horizontal,
            Vertical
        }
}