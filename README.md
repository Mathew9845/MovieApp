# MovieRatingApp

## Description
MovieRatingApp is a WPF (Windows Presentation Foundation) desktop application that allows users to browse and rate movies. It has been developed using .NET 8.0.

## Features
- Search for movies based on a TSV file.
- Display movie details.
- Add movie ratings.
- Export and import user ratings to/from an XML file.

## Prerequisites
- .NET 8.0
- Visual Studio 2022 (recommended for .NET 8.0)

## Installation and Configuration
1. Open the solution in Visual Studio 2022.
2. Ensure you have .NET 8.0 installed.
3. Compile the project using the "Build" option.

## Project Structure
- `MainWindow.xaml`: The main user interface of the application.
- `MainWindow.xaml.cs`: The logic of the user interface.
- `ClearableTextBox.xaml`: A user control with a clearable text field.
- `ClearableTextBox.xaml.cs`: Logic for the `ClearableTextBox` control.
- `Movie.cs`: Definition of the Movie class representing a single movie.
- `Data\final_data_with_names.tsv`: Sample movie data in TSV format.
- `Data\background.png`: Image file used as the application background.

## Execution
To run the application, click the "Start" button in Visual Studio.

## Usage
- In the search field, type the title of the movie you want to rate and click the search button.
- Click on a movie to view its details and add a rating.
- Use the buttons at the top of the user rating panel to export or import ratings to/from an XML file.
- The application is designed for a default window size of 1200x800 pixels.

## Movie Data and Data Processing Script

### Data Source
The data used in the `MovieRatingApp` application comes from the IMDb (Internet Movie Database) dataset, publicly available at: [https://datasets.imdbws.com/](https://datasets.imdbws.com/). The TSV files used by the script are `titleakas.tsv`, `titlebasics.tsv`, `titleratings.tsv`, `titlecrew.tsv`, and `namebasics.tsv`.

### Data Processing Script
The project includes a Python script (`process_data.py`) that processes and prepares movie data from IMDb. The script performs a series of operations on the listed TSV files to create a coherent and filtered dataset.

#### Note
- The TSV files are quite large, meaning that the processing may take a significant amount of time and require adequate system resources.

### Script Functions
- Merging data from different TSV files.
- Filtering movies based on availability in the US region, number of votes, and title type.
- Aggregating information about directors and writers.
- Exporting processed data to the `final_data_with_names.tsv` file.

### Data Customization
You can customize the script to change the filtering criteria (e.g., by changing the minimum number of votes or region). This allows you to generate a dataset that contains a larger number of movies or is tailored to other needs.

### Script Requirements
- Python 3.x
- `pandas` library
- The `process_data.py` script has been tested and is compatible with Python version 3.10.6. Ensure you have this version (or newer) installed to run the script correctly.

### Running Instructions
1. Install Python 3.10.6 and the `pandas` library.
2. Place the listed TSV files in the same directory as the script.
3. Run the script using the command `python process_data.py` in the terminal.

## Author
- Mathew9845.

