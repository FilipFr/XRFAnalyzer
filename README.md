# XRFAnalyzer

XRFAnalyzer is an easy-to-use, free and open source software used for analysis of x-ray fluorescence spectra. The software was developed as a part of a diploma thesis called *Spectrum processing for X-ray fluorescence analysis*, which was written at FEI STU in Bratislava. XRFAnalyzer reimplements and extends the functionality of an older software called [XRayAnalyzer](https://github.com/irelevant25/XRayAnalyzer "XRayAnalyzer").

### Download link

The latest version of XRFAnalyzer was developed for Windows 10 (64 bit).  The GUI should work well on display resolution 1366x768 and higher.  

[Click here to download.](https://github.com/FilipFr/XRFAnalyzer/raw/master/XRFAnalyzer/XRFAnalyzer.zip "Click here to download.")

The software does not require installation, simply unzip tha package and run the *XRFAnalyzer.exe* file in the top level folder.

### Advantages of XRFAnalyzer over similar software
- Simplicity and ease-of-use.
- Analysis is fast - spectrum processing and qualitative analysis are partially automated.
- Algorithms used are fully transparent.
- The software is developed with extensibility in mind.

### Features
Spectra processing:
- .mca file support (.mca file format is used by AMPTEK software for spectra persistence)
- manual and automatic peak addition
- manual peak removal
- spectra can be calibrated by manually adding calibration points or by loading a .mca file that already contains some calibration points (if previously loaded spectrum was calibrated during the current run of the application and the currently loaded spectrum contains no calibration data, the calibration persists and can be used for the currently loaded spectrum)
- background approximation and removal using the [ZhangFit algorithm](https://github.com/StatguyUser/BaselineRemoval "ZhangFit algorithm")
- sum peaks detection and removal

Qualitative analysis uses the following ordered list of priorities (from highest to lowest, the region of interest in which lines are considered around the peak center is given by detector efficiency):
1. A line defined by the user during calibration is preferred.
2. If an element was already identified in the spectrum, it's line is preferred.
3. Lines are preferred by the order of their probability (for example KL3 over KM3).
4. A line closest to the peak center is preferred.

Quantitative analysis uses the fundamental parameter method (only standardless analysis is currently possible). The user has the option to provide the value of energy of the primary radiation in keV. Alternatively, the user can load a primary spectrum for a more precise quantification.

### How to use the software

[Click here to watch the video demonstration on youtube](https://youtu.be/YPiHeNg0FEo "Click here to watch the video demonstration on youtube")
### Links to similar software
- [Peakaboo](https://www.peakaboo.org/ "Peakaboo")
- [NIST DTSA-II](https://www.cstl.nist.gov/div837/837.02/epq/dtsa2/index.html "NIST DTSA-II")
- [Larch](https://xraypy.github.io/xraylarch/ "Larch")
