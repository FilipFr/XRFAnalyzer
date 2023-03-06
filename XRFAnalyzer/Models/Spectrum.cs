using System;
using System.Collections.Generic;
using System.IO;


namespace XRFAnalyzer.Models
{
    internal class Spectrum
    {

        public List<SpectrumPoint> Points { get; set; }
        public Dictionary<int, Peak> Peaks { get; set; }

        public Spectrum() 
        {
            Points = new List<SpectrumPoint>();
            Peaks = new Dictionary<int, Peak>();
        }

        /// <summary>
        /// Method TryParseMca attempts to parse a .mca file.
        /// </summary>
        public static bool TryParseMca(string filePath, ref Spectrum spectrum, out string parsingResultMessage)
        { 
            try 
            {
                IEnumerable<string> lines = File.ReadLines(filePath);

                List<Tuple<int, int>> rois = new();
                List<int> data = new();
                string currentSection = "";

                foreach (string line in lines)
                {
                    if (line.StartsWith("<<") && line != "<<END>>") 
                    {
                        currentSection = line;
                    } 
                    else if (line == "<<END>>") 
                    {
                        currentSection = "";
                    }
                    else 
                    {
                        switch (currentSection)
                        {
                            case "<<ROI>>":
                                string[] line_parts = line.Split(' ');
                                rois.Add(new Tuple<int, int>(Int32.Parse(line_parts[0]), Int32.Parse(line_parts[1])));
                                break;
                            case "<<DATA>>":
                                data.Add(Int32.Parse(line));
                                break;
                        } 
                    }
                    
                }

                Spectrum newSpectrum = new();
                newSpectrum.Points = SpectrumPoint.GetPointsFromIntegers(data);
                foreach (Tuple<int,int> roi in rois) 
                {
                    newSpectrum.AddPeak(Peak.ConvertFromRoi(newSpectrum.Points, roi.Item1, roi.Item2));
                }
                spectrum = newSpectrum;

                parsingResultMessage = "Load successful";
                return true;
            } 
            catch (FileNotFoundException) 
            {
                parsingResultMessage = "File not found at path " + filePath;
                return false; 
            } 
            catch (IOException)
            {
                parsingResultMessage = "Unable to open file " + filePath;
                return false;
            }
        }

        public void AddPeak(Peak peak) 
        {
            int counter = 0;
            while (Peaks.ContainsKey(counter)) 
            { 
                counter++; 
            }
            this.Peaks.Add(counter, peak);
        }
        
        //private McaMetadata ?metadata;

        //private class McaMetadata
        //{
        //    Dictionary<string, string> ?header;
        //    Dictionary<string, string> ?config;
        //    Dictionary<string, string> ?status;
        //    Dictionary<string, string> ?bootFlags;
        //}
        
    }
}
