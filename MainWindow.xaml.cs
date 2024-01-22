using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Windows.Input;

namespace MovieRatingApp
{
    // Główna klasa okna aplikacji.
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<Movie> allMovies;
        public Dictionary<string, int> userRatings = new Dictionary<string, int>();
        private Movie selectedMovie;

        // Konstruktor głównego okna.
        public MainWindow()
        {
            InitializeComponent();
            string tsvFilePath = "Data/movies.tsv";
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tsvFilePath);
            allMovies = LoadTsvFile(fullPath);
            clearableTextBoxInstance.txtInput.TextChanged += ClearableTextBox_TextChanged;
            userRatings = new Dictionary<string, int>();
        }

        // Obsługa zdarzenia zmiany tekstu.
        private void ClearableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            searchResultsListView.Visibility = string.IsNullOrWhiteSpace(textBox.Text) ? Visibility.Collapsed : Visibility.Visible;
        }

        // Ładowanie filmów z pliku TSV.
        public List<Movie> LoadTsvFile(string filePath)
        {
            var movies = new List<Movie>();
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1))
                {
                    var fields = line.Split('\t');
                    var movie = new Movie
                    {
                        TitleId = fields[0],
                        Title = fields[1],
                        Year = fields[5],
                        Genres = fields[7],
                        DirectorNames = fields[12],
                        RuntimeMinutes = fields[6],
                        AverageRating = fields[8]
                    };
                    movies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return movies;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Obsługa zdarzenia kliknięcia przycisku eksportu do XML.
        private void ExportToXmlButton_Click(object sender, RoutedEventArgs e)
        {
            // Jeśli nie ma ocen do wyeksportowania, wyświetl ostrzeżenie.
            if (userRatings.Count == 0)
            {
                MessageBox.Show("Nie ma ocen do wyeksportowania");
                return;
            }

            // Utwórz i konfiguruj okno dialogowe zapisu pliku.
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "XML file (*.xml)|*.xml",
                DefaultExt = "xml",
                FileName = "UserReviews"
            };

            // Wyświetl okno dialogowe zapisu pliku.
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Przekształć słownik ocen użytkowników na listę dla serializacji.
                    var keyValuePairsList = userRatings.Select(kv => new SerializableKeyValuePair { Key = kv.Key, Value = kv.Value }).ToList();
                    // Serializuj listę do pliku XML.
                    XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableKeyValuePair>));
                    using (FileStream stream = File.Create(saveFileDialog.FileName))
                    {
                        serializer.Serialize(stream, keyValuePairsList);
                    }
                    MessageBox.Show("Oceny zostały wyeksportowane poprawnie do XML file.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd przy eksportowaniu ocen!: {ex.Message}");
                }
            }
        }

        // Obsługa zdarzenia kliknięcia przycisku.
        private void ListViewItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var movie = button.DataContext as Movie;
                if (movie != null)
                {
                    selectedMovie = movie;
                    DisplayMovieDetails(movie);
                }
            }
        }

        private void DisplayMovieDetails(Movie movie)
        {
            movieTitleTextBlock.Text = $"Tytuł: {movie.Title}";
            movieYearTextBlock.Text = $"Rok: {movie.Year}";
            movieGenresTextBlock.Text = $"Gatunki: {movie.Genres}";
            movieDirectorNamesTextBlock.Text = $"Reżyser(zy): {movie.DirectorNames}";
            movieRuntimeMinutesTextBlock.Text = $"Czas trwania: {movie.RuntimeMinutes} minuty";
            movieAverageRatingTextBlock.Text = $"Średnia ocena na imdb: {movie.AverageRating}";
            movieProfile.Visibility = Visibility.Visible;
        }

        // Obsługa zdarzenia kliknięcia przycisku.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            movieProfile.Visibility = Visibility.Collapsed;
        }

        // Obsługa zdarzenia kliknięcia przycisku.
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = clearableTextBoxInstance.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchResults = allMovies.Where(movie => movie.Title.ToLower().Contains(searchText)).ToList();
                searchResultsListView.ItemsSource = searchResults;
            }
            else
            {
                searchResultsListView.ItemsSource = null;
            }
        }

        // Obsługa zdarzenia przesunięcia suwaka do oceny filmów.
        private void RatingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectedMovie == null || string.IsNullOrEmpty(selectedMovie.TitleId))
            {
                return;
            }

            var rating = (int)Math.Round(e.NewValue);

            if (userRatings.ContainsKey(selectedMovie.TitleId))
            {
                userRatings[selectedMovie.TitleId] = rating;
            }
            else
            {
                userRatings.Add(selectedMovie.TitleId, rating);
            }

            UpdateUserRatingsDisplay();
        }

        private void UpdateUserRatingsDisplay()
        {
            var ratingsDisplay = userRatings.Select(kv => $"{GetMovieTitleById(kv.Key)}: {kv.Value}").ToList();
            userRatingsListView.ItemsSource = ratingsDisplay;
        }

        private string GetMovieTitleById(string titleId)
        {
            var movie = allMovies.FirstOrDefault(m => m.TitleId == titleId);
            return movie?.Title ?? "Unknown";
        }

        private void ImportFromXmlButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "XML file (*.xml)|*.xml",
                DefaultExt = "xml"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableKeyValuePair>));
                    using (FileStream stream = File.OpenRead(openFileDialog.FileName))
                    {
                        var keyValuePairsList = (List<SerializableKeyValuePair>)serializer.Deserialize(stream);
                        userRatings.Clear();
                        foreach (var kvp in keyValuePairsList)
                        {
                            userRatings[kvp.Key] = kvp.Value;
                        }
                        UpdateUserRatingsDisplay();
                    }
                    MessageBox.Show("Oceny zostały zaimportowane poprawnie z XML file.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd przy importowaniu ocen!: {ex.Message}");
                }
            }
        }

        private void SubmitRatingButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMovie == null)
            {
                MessageBox.Show("Proszę wybrać film");
                return;
            }

            if (string.IsNullOrWhiteSpace(selectedMovie.TitleId))
            {
                MessageBox.Show("Wybrany film nie ma poprawnego identyfikatora");
                return;
            }

            int rating = (int)ratingSlider.Value;
            userRatings[selectedMovie.TitleId] = rating;
            UpdateUserRatingsDisplay();
            MessageBox.Show("Twoja ocena została dodana");
        }
    }

    // Klasa reprezentująca film.
    public class Movie
    {
        public string TitleId { get; set; }
        public string Title { get; set; }
        public string Tconst { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public string Year { get; set; }
        public string RuntimeMinutes { get; set; }
        public string Genres { get; set; }
        public string AverageRating { get; set; }
        public int NumVotes { get; set; }
        public string Directors { get; set; }
        public string Writers { get; set; }
        public string DirectorNames { get; set; }
        public string WriterNames { get; set; }
    }

    // Klasa pomocnicza do serializacji par klucz-wartość.
    public class SerializableKeyValuePair
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
}


