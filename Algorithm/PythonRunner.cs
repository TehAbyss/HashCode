namespace HashPhotoSlideshow.Algorithm
{
    using System.Diagnostics;
    using System.IO;
    using HashPhotoSlideshow.Model;

    public class PythonRunner : ISlideshowAlgorithm
    {
        public string ScriptPath { get; }
        public PythonRunner(string scriptPath)
        {
            ScriptPath = scriptPath;
        }

        public Slideshow GenerateSlideshow(PhotoCollection photoCollection)
        {
            string result = RunPython(ScriptPath);
            return new Slideshow(); 
        }

        public string RunPython(string pythonScriptPath)
        {
            string result = string.Empty;

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python.exe";
            start.Arguments = pythonScriptPath;
            start.UseShellExecute = true;
            start.RedirectStandardOutput = true;

            using(Process process = Process.Start(start))
            {
                using(StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}