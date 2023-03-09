﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace XRFAnalyzer.Models
{
    internal partial class Spectrum : ObservableObject
    {
        [ObservableProperty]
        private string _filePath;
        [ObservableProperty]
        private List<SpectrumPoint> _points;
        [ObservableProperty]
        private List<Peak> _peaks;
        [ObservableProperty]
        private SpectrumState _state;

        public Spectrum() 
        {
            Points = new List<SpectrumPoint>();
            Peaks = new List<Peak>();
            State = new SpectrumState();
        }

        /// <summary>
        /// Method TryParseMca attempts to parse a .mca file.
        /// </summary>
        public bool TryParseMca(string filePath, out string parsingResultMessage)
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

                this.Points = SpectrumPoint.GetPointsFromIntegers(data);
                foreach (Tuple<int,int> roi in rois) 
                {
                    this.AddPeak(Peak.ConvertFromRoi(this.Points, roi.Item1, roi.Item2));
                }

                this.FilePath = filePath;
                State.IsLoaded = true;
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
            this.Peaks.Add(peak);
        }

        public List<int> PeakHeights()
        {
            return this.Peaks.Select(x =>  x.HighestPoint.Count ).ToList();
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
