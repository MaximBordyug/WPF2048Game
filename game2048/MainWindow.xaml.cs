using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace game2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    

    public partial class MainWindow : Window
    {
        Dictionary<string, Style> styles = new Dictionary<string, Style>();
        int Score = 0;
        int BestScore = 0;
        int[,] Matrix = new int[4, 4];
        Style zeroStyle = new Style();
        int[,] elementPolya = new int[4, 4];
        Random rnd = new Random();
        public MainWindow()
        {
            
            InitializeComponent();
           
zeroStyle.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Color.FromArgb(255, 255, 178, 102)) });
             initializeStyles();
            AddRandomTile();
            AddRandomTile();
            UpdateGrid();
            BestScoreTextBlock.Text = "Best: " + BestScore;

           
        }

        private void initializeStyles()
        {
            int calc = 2;
            
            while (calc <= 2048) {
                Style first = new Style();
                first.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Color.FromArgb((byte)rnd.Next(100, 255), (byte)rnd.Next(0, 255), 255, (byte)rnd.Next(0, 255)))});
               // first.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
                
                styles.Add(calc.ToString(),first);
                calc *= 2;
            }
        }
        private void UpdateGrid()
        {
            ButtonsGrid.Children.Clear();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var textBlock = new TextBlock();
                    textBlock.Text = Matrix[row, col].ToString();

                    if (styles.TryGetValue(Matrix[row, col].ToString(), out Style textStyle))
                    {
                        
                        textBlock.Background = textStyle.Setters
                            .OfType<Setter>()
                            .Where(s => s.Property == Control.BackgroundProperty)
                            .Select(s => s.Value)
                            .OfType<SolidColorBrush>()
                            .FirstOrDefault();
                    }
                    else 
                    {
                        textBlock.Background = zeroStyle.Setters
                    .OfType<Setter>()
                   .Where(s => s.Property == Control.BackgroundProperty)
                   .Select(s => s.Value)
                   .OfType<SolidColorBrush>()
                   .FirstOrDefault();
                    }
                    textBlock.Padding = new Thickness(60);
                    ButtonsGrid.Children.Add(textBlock);
                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, col);
                    ScoreTextBlock.Text = "Score: " + Score;
                    if(BestScore < Score)
                    {
                        BestScore = Score;
                        BestScoreTextBlock.Text = "Best: " + BestScore;
                    }
                    
                }
            }
        }
        private void AddRandomTile()
        {
            List<Tuple<int, int>> emptySpots = new List<Tuple<int, int>>();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (Matrix[row, col] == 0)
                    {
                        emptySpots.Add(new Tuple<int, int>(row, col));
                    }
                }
            }

            if (emptySpots.Count > 0)
            {
                Tuple<int, int> randomSpot = emptySpots[rnd.Next(emptySpots.Count)];
                Matrix[randomSpot.Item1, randomSpot.Item2] = rnd.Next(1, 11) <= 9 ? 2 : 4;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Game Over! Would you like to try again?", "Game Over", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    NewGame();
                }
                else if (result == MessageBoxResult.No)
                {
                    Close();
                }
            }
        }

        private void New_Game_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }
        private void NewGame()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Matrix[row, col] = 0;
                }
            }

            Score = 0;

            AddRandomTile();
            AddRandomTile();

            UpdateGrid();
        }
        private void KeyDownClick(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 3; i++) 
                        {
                            if (Matrix[i, j] != 0)
                            {
                                for (int i1 = i + 1; i1 < 4; i1++)  
                                {
                                    if (Matrix[i, j] == Matrix[i1, j])
                                    {
                                        Matrix[i, j] += Matrix[i1, j];
                                        Score += Matrix[i, j];
                                        Matrix[i1, j] = 0;
                                        break;
                                    }
                                    else if (Matrix[i1, j] != 0)
                                    {
                                        break;  
                                    }
                                }
                            }
                        }
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 3; i++) 
                        {
                            if (Matrix[i, j] == 0)
                            {
                                for (int i1 = i + 1; i1 < 4; i1++) 
                                {
                                    if (Matrix[i1, j] != 0)
                                    {
                                        Matrix[i, j] = Matrix[i1, j];
                                        Matrix[i1, j] = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    AddRandomTile();
                    UpdateGrid();
                    break;
                case Key.Down:
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 3; i > 0; i--)
                        {
                            if (Matrix[i, j] != 0)
                            {
                                for (int i1 = i - 1; i1 >= 0; i1--)
                                {
                                    if (Matrix[i, j] == Matrix[i1, j])
                                    {
                                        Matrix[i, j] += Matrix[i1, j];
                                        Score += Matrix[i, j];
                                        Matrix[i1, j] = 0;
                                        break;
                                    }
                                    else if (Matrix[i1, j] != 0)
                                    {
                                        break; 
                                    }
                                }
                            }
                        }
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 3; i > 0; i--)
                        {
                            if (Matrix[i, j] == 0)
                            {
                                for (int i1 = i - 1; i1 >= 0; i1--)
                                {
                                    if (Matrix[i1, j] != 0)
                                    {
                                        Matrix[i, j] = Matrix[i1, j];
                                        Matrix[i1, j] = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    AddRandomTile();
                    UpdateGrid();
                    break;
                case Key.Left:
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (Matrix[i, j] != 0)
                            {
                                for (int j1 = j + 1; j1 < 4; j1++)
                                {
                                    if (Matrix[i, j] == Matrix[i, j1])
                                    {
                                        Matrix[i, j] += Matrix[i, j1];
                                        Score += Matrix[i, j];
                                        Matrix[i, j1] = 0;
                                        break;
                                    }
                                    else if (Matrix[i, j1] != 0)
                                    {
                                        break; 
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (Matrix[i, j] == 0)
                            {
                                for (int j1 = j + 1; j1 < 4; j1++)
                                {
                                    if (Matrix[i, j1] != 0)
                                    {
                                        Matrix[i, j] = Matrix[i, j1];
                                        Matrix[i, j1] = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    AddRandomTile();
                    UpdateGrid();
                    break;
                case Key.Right:
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 3; j > 0; j--) 
                        {
                            if (Matrix[i, j] != 0)
                            {
                                for (int j1 = j - 1; j1 >= 0; j1--)  
                                {
                                    if (Matrix[i, j] == Matrix[i, j1])
                                    {
                                        Matrix[i, j] += Matrix[i, j1];
                                        Score += Matrix[i, j];
                                        Matrix[i, j1] = 0;
                                        break;
                                    }
                                    else if (Matrix[i, j1] != 0)
                                    {
                                        break;  
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 3; j > 0; j--) 
                        {
                            if (Matrix[i, j] == 0)
                            {
                                for (int j1 = j - 1; j1 >= 0; j1--)  
                                {
                                    if (Matrix[i, j1] != 0)
                                    {
                                        Matrix[i, j] = Matrix[i, j1];
                                        Matrix[i, j1] = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    AddRandomTile();
                    UpdateGrid();
                    break;
            }
        }

       
    }
}
