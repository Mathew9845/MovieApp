using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MovieRatingApp.View.UserControls
{

    public partial class ClearableTextBox : UserControl
    {
        public delegate void SearchEventHandler(object sender, string searchText);

        public event SearchEventHandler SearchClicked;

        public ClearableTextBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private string placeholder;

        public string Placeholder
        {
            get { return placeholder; }
            set { 
                placeholder = value; 
                tbPlaceholder.Text = placeholder;
            }
        }







        public void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                tbPlaceholder.Visibility = Visibility.Visible;
                
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Hidden;
            }


        }

    }
}
