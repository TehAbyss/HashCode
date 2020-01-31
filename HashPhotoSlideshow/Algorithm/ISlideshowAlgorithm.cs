namespace HashPhotoSlideshow.Algorithm
{
    using HashPhotoSlideshow.Model;

    public interface ISlideshowAlgorithm
    {
        Slideshow GenerateSlideshow(PhotoCollection photoCollection);
    }
}