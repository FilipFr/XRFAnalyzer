using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace XRFAnalyzer.Models
{
    internal class Spectrum
    {
        public List<int> Counts { get; set; }
        public List<Tuple<int, int>> Peaks { get; set; }
        public Dictionary<int, double> CalibrationPoints { get; set; }

        public Spectrum() 
        {
            Counts = new List<int>();
            Peaks = new List<Tuple<int, int>>();
            CalibrationPoints = new Dictionary<int, double>();
        }


        /// <summary>
        /// Method TryParseMca attempts to parse a .mca file.
        /// </summary>
        public bool TryParseMca(string filePath, out string parsingResultMessage)
        {
            try 
            {
                Spectrum newSpectrum = new Spectrum();
                IEnumerable<string> lines = File.ReadLines(filePath);

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
                            case "<<CALIBRATION>>":
                                {
                                    if (line.StartsWith("LABEL"))
                                    {
                                        break;
                                    }
                                    string[] line_parts = line.Split(' ');
                                    if (!newSpectrum.CalibrationPoints.ContainsKey(Int32.Parse(line_parts[0]))) 
                                    { 
                                        newSpectrum.CalibrationPoints[Int32.Parse(line_parts[0])] = Double.Parse(line_parts[1]);
                                        break;
                                    }
                                    break;
                                }
                            case "<<ROI>>":
                                {
                                    string[] line_parts = line.Split(' ');
                                    newSpectrum.Peaks.Add(new Tuple<int, int>(Int32.Parse(line_parts[0]), Int32.Parse(line_parts[1])));
                                    break;
                                }
                            case "<<DATA>>":
                                {
                                    newSpectrum.Counts.Add(Int32.Parse(line));
                                    break;
                                }
                        } 
                    }
                }
                this.Counts = newSpectrum.Counts;
                this.Peaks = newSpectrum.Peaks;
                this.CalibrationPoints = newSpectrum.CalibrationPoints;
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
