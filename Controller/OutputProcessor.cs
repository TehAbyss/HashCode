namespace HashPhotoSlideshow.Controller
{
	using System.IO;
	using System.Text;

    public class OutputProcessor
    {
    	private string slideshowOutputFileName = $"slideshow_{Date.now()}";
    	private Slideshow Slideshow {get;}

    	public OutputProcessor(Slideshow slideshow) {
    		Slideshow = slideshow;
    	}

    	public void ProcessSlideShow() {
    		using (var fs = File.Create(slideshowOutputFileName)) {
    			using (var sr = new StreamWriter(fs)) {
	    			sr.WriteLine(Slideshow.size().ToString());

	    			foreach (var slide in Slideshow) {
	    				foreach (var photo in slide) {
	    					sr.Write($"{photo.Id.ToString()} ");
	    				}
	    				sr.WriteLine();
	    			}
    			}
    		}
    	}
    }
}