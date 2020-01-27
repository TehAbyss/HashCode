namespace HashPhotoSlideshow.Algorithm
{
    using System.Diagnostics;
    using System.IO;
    using HashPhotoSlideshow.Model;

    public class PythonRunner : ISlideshowAlgorithm, IInternalAlgorithm
    {
        public string ScriptPath { get; }

        public PythonRunner(string scriptPath)
        {
            ScriptPath = scriptPath;

            if (!File.Exists(ScriptPath))
            {
                throw new FileNotFoundException($"{ScriptPath} file does not exist.");
            }
        }

        public Slideshow GenerateSlideshow(PhotoCollection photoCollection)
        {
            string standardOutput = Run();
            return new Slideshow();
        }

        private string Run()
        {
            string standardOutput = string.Empty;

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python.exe";
            start.Arguments = ScriptPath;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    standardOutput = reader.ReadToEnd();
                }
            }

            return standardOutput;
        }
    }
}