using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;

namespace test
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int count = 0;
        Sudoku sudokuPuzzle;
        int[,] puzzle, backup, solution;
        int[,] loadedPuzzle = new int[9, 9];
        int[,] checkRead = new int[9, 9];
        string difficulty;
        string textName = " ";
        public MainWindow()
        {
            InitializeComponent();
            difficulty = "Easy";
            
            Label heading = new Label();

            heading.Content = "Difficulty: " + difficulty;
            heading.FontSize = 26;
            heading.Margin = new Thickness(0, 0, 0, 0);
            heading.FontFamily = new FontFamily("Courier");
            heading.Name = "Heading";
            sudokuGrid.Children.Add(heading);

            sudokuGrid.RowDefinitions.Add(new RowDefinition());
            sudokuGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumnSpan(heading, 7);
            Grid.SetRowSpan(heading, 1);
            Grid.SetRow(heading, 1);
            Grid.SetColumn(heading, 1);

            Menu menu = new Menu();
            menu.Name = "menu";
            sudokuGrid.Children.Add(menu);
            menu.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Grid.SetRow(menu, 0);
            Grid.SetColumnSpan(menu, 18);

            MenuItem file = new MenuItem();
            file.Header = "_File";
            menu.Items.Add(file);

            MenuItem newItem = new MenuItem();
            newItem.Header = "_New Game";
            newItem.Click += (sender, args) => generatePuzzle(difficulty);
            file.Items.Add(newItem);

            MenuItem easy = new MenuItem();
            easy.Header = "_Easy";
            easy.IsCheckable = true;
            easy.Name = "easy";
            easy.Click += (sender, args) => { difficulty = "Easy"; generatePuzzle(difficulty); };
            newItem.Items.Add(easy);

            MenuItem medium = new MenuItem();
            medium.Header = "_Medium";
            medium.IsCheckable = true;
            medium.Name = "medium";
            medium.Click += (sender, args) => { difficulty = "Medium"; generatePuzzle(difficulty); };
            newItem.Items.Add(medium);

            MenuItem hard = new MenuItem();
            hard.Header = "_Hard";
            hard.IsCheckable = true;
            hard.Name = "hard";
            hard.Click += (sender, args) => { difficulty = "Hard"; generatePuzzle(difficulty); };
            newItem.Items.Add(hard);

            MenuItem loadItem = new MenuItem();
            loadItem.Header = "_Load Game...";
            loadItem.Click += (sender, args) => loadGame();
            file.Items.Add(loadItem);

            MenuItem saveItem = new MenuItem();
            saveItem.Header = "_Save Game...";
            saveItem.Click += (sender, args) => saveGame();
            file.Items.Add(saveItem);

            file.Items.Add(new Separator());

            MenuItem exit = new MenuItem();
            exit.Header = "E_xit";
            exit.Click += (sender, args) => this.Close();
            file.Items.Add(exit);




            generatePuzzle(difficulty);

            for (int x = 0; x < 10; x++)
            {
                sudokuGrid.RowDefinitions.Add(new RowDefinition());
                sudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int y = 0; y < 7; y++)
            {
                sudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(2, 5, 10, 15);
                    border.Background = Brushes.Black;
                    sudokuGrid.Children.Add(border);

                    Grid.SetColumn(border, j + 1);
                    Grid.SetRow(border, i + 2);
                    var thickness = new Thickness();

                    TextBox text = new TextBox();
                    text.Name = "text" + count.ToString();
                    if (puzzle[i, j] != 0)
                    {
                        text.Text = puzzle[i, j].ToString();
                        text.IsReadOnly = true;
                        text.Foreground = Brushes.Black;
                        text.Background = Brushes.AliceBlue;
                        text.GotFocus += (sender, args) => textName = text.Name;
                        text.TextChanged += Text_TextChanged;
                        text.TextChanged += checkAnswer;
                    }
                    else
                    {

                        text.Foreground = Brushes.Blue;
                        text.PreviewTextInput += TextBox_PreviewTextInput;
                        text.GotFocus += (sender, args) => textName = text.Name;
                        text.TextChanged += Text_TextChanged;
                        text.TextChanged += checkAnswer;
                    }
                    if (i % 3 == 0)
                    {
                        thickness.Top = 2;
                    }
                    else if (i == 8)
                    {
                        thickness.Bottom = 2;
                    }
                    if (j % 3 == 0)
                    {
                        thickness.Left = 2;
                    }
                    else if (j == 8)
                    {
                        thickness.Right = 2;
                    }

                    text.Margin = thickness;

                    Grid.SetColumn(text, j + 1);
                    Grid.SetRow(text, i + 2);
                    sudokuGrid.Children.Add(text);
                    count++;
                }
            }

            string[] buttons = { "New Game", "Save Game", "Load Game" };
            int column = 11;
            int buttonNum = 1;
            foreach (string button in buttons)
            {
                Button sideButton = new Button();
                sideButton.Content = button;
                sideButton.FontSize = 16;
                sideButton.Name = "button" + buttonNum.ToString();
                sideButton.Click += Button_Click;
                sudokuGrid.Children.Add(sideButton);

                buttonNum++;

                Grid.SetRow(sideButton, 2);
                Grid.SetColumn(sideButton, column);

                column = column + 2;

                Grid.SetColumnSpan(sideButton, 2);
            }
            for (int l = 1; l < 10; l++)
            {
                Button numButton = new Button();
                numButton.Content = l;
                numButton.FontSize = 26;
                numButton.Name = "button" + buttonNum.ToString();
                numButton.Click += Button_Click;
                sudokuGrid.Children.Add(numButton);
                Grid.SetRow(numButton, 4 + (l - 1) / 3);
                Grid.SetColumn(numButton, 11 + ((l - 1) % 3) * 2);
                Grid.SetColumnSpan(numButton, 2);
                ++buttonNum;
            }
            Button clearButton = new Button();
            clearButton.Content = "Clear";
            clearButton.FontSize = 20;
            clearButton.Name = "button" + buttonNum.ToString();
            clearButton.Click += Button_Click;
            sudokuGrid.Children.Add(clearButton);
            Grid.SetRow(clearButton, 7);
            Grid.SetColumn(clearButton, 11);
            Grid.SetColumnSpan(clearButton, 6);
        }

        private void checkAnswer(object sender, TextChangedEventArgs e)
        {
            bool isCorrect = false;
            int i = 0;
            int j = 0;
            int counter = 0;
            foreach (var text in sudokuGrid.Children)
            {
                if (text is TextBox)
                {
                    if (counter > 80)
                    {
                        break;
                    }
                    isCorrect = ((TextBox)text).Text == solution[i, j].ToString();
                    if (!isCorrect)
                    {
                        return;
                    }

                    counter++;
                    i = counter / 9;
                    j = counter % 9;
                }
            }
            if (isCorrect)
            {
                MessageBoxButton messageButton = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Puzzle Solved!" + Environment.NewLine + "Would you like to start a new game?", "Success!", messageButton, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    generatePuzzle(difficulty);
                }
            }
        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            int counter = 0;
            int i = 0;
            int j = 0;
            foreach (var textButton in sudokuGrid.Children)
            {
                if (textButton is TextBox)
                {
                    puzzle[i, j] = ((TextBox)textButton).Text == "" || ((TextBox)textButton).Text == " " ? 0 : Int32.Parse(((TextBox)textButton).Text);
                    counter++;
                    i = counter / 9;
                    j = counter % 9;
                    ((TextBox)textButton).Foreground = Brushes.Blue;
                    if (((TextBox)textButton).IsReadOnly)
                    {
                        ((TextBox)textButton).Foreground = Brushes.Black;
                    }
                }
            }
            foreach (var textButton in sudokuGrid.Children)
            {
                if (textButton is TextBox)
                {
                    
                    int column = Grid.GetColumn((TextBox)textButton);
                    int row = Grid.GetRow((TextBox)textButton);
                    foreach (var text in sudokuGrid.Children)
                    {
                        if (text is TextBox)
                        {
                            if (text != textButton)
                            {
                                
                                if (Grid.GetColumn((TextBox)text) == column)
                                {
                                    if (((TextBox)text).Text == ((TextBox)textButton).Text)
                                    {
                                        ((TextBox)textButton).Foreground = Brushes.Red;
                                        ((TextBox)text).Foreground = Brushes.Red;
                                    }
                                }
                                if (Grid.GetRow((TextBox)text) == row)
                                {
                                    if (((TextBox)text).Text == ((TextBox)textButton).Text)
                                    {
                                        ((TextBox)textButton).Foreground = Brushes.Red;
                                        ((TextBox)text).Foreground = Brushes.Red;
                                    }
                                }
                            }
                        }
                    }

                    if (row < 5)
                    {
                        if (column < 4)
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) < 5 && Grid.GetColumn((TextBox)text2) < 4)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        else if (column < 7)
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) < 5 && Grid.GetColumn((TextBox)text2) < 7 && Grid.GetColumn((TextBox)text2) >= 4)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) < 5 && Grid.GetColumn((TextBox)text2) >= 7)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (row >= 5 && row < 8)
                    {
                        if (column < 4)
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) >= 5 && Grid.GetRow((TextBox)text2) < 8 && Grid.GetColumn((TextBox)text2) < 4)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (column < 7)
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) >= 5 && Grid.GetRow((TextBox)text2) < 8 && Grid.GetColumn((TextBox)text2) < 7 && Grid.GetColumn((TextBox)text2) >= 4)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) >= 5 && Grid.GetRow((TextBox)text2) < 8 && Grid.GetColumn((TextBox)text2) >= 7)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (column < 4)
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) >= 8 && Grid.GetColumn((TextBox)text2) < 4)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (column < 7)
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) >= 8 && Grid.GetColumn((TextBox)text2) < 7 && Grid.GetColumn((TextBox)text2) >= 4)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var text2 in sudokuGrid.Children)
                            {
                                if (text2 is TextBox)
                                {
                                    if (text2 != textButton)
                                    {
                                        if (Grid.GetRow((TextBox)text2) >= 8 && Grid.GetColumn((TextBox)text2) >= 7)
                                        {
                                            if (((TextBox)text2).Text == ((TextBox)textButton).Text)
                                            {
                                                ((TextBox)textButton).Foreground = Brushes.Red;
                                                ((TextBox)text2).Foreground = Brushes.Red;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void generatePuzzle(string difficulty)
        {
            int level = 4;
            if (difficulty == "Hard")
            {
                level = 4;
            }
            else if (difficulty == "Medium")
            {
                level = 5;
            }
            else if (difficulty == "Easy")
            {
                level = 6;
            }
            sudokuPuzzle = new Sudoku(level);
            puzzle = (int[,])sudokuPuzzle.puzzle.Clone();
            backup = (int[,])sudokuPuzzle.puzzle.Clone();
            solution = (int[,])sudokuPuzzle.solution.Clone();
            loadPuzzle();
        }
        private void loadPuzzle()
        {
            int x, i, j;
            x = 0;
            foreach (var text in sudokuGrid.Children)
            {
                if (text is TextBox)
                {
                    i = x / 9;
                    j = x % 9;
                    ((TextBox)text).Text = "";
                    ((TextBox)text).Foreground = Brushes.Blue;
                    ((TextBox)text).Background = Brushes.White;
                    ((TextBox)text).PreviewTextInput += TextBox_PreviewTextInput;
                    ((TextBox)text).IsReadOnly = false;
                    if (backup[i, j] != 0)
                    {
                        ((TextBox)text).Text = backup[i, j].ToString();
                        ((TextBox)text).Foreground = Brushes.Black;
                        ((TextBox)text).Background = Brushes.AliceBlue;
                        ((TextBox)text).IsReadOnly = true;
                    }

                    x++;
                }

            }
            ((Label)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "Heading")).Content = "Difficulty: " + difficulty;
            if (difficulty == "Easy")
            {
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "medium")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "hard")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "easy")).IsChecked = true;
            }
            else if (difficulty == "Medium")
            {
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "easy")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "hard")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "medium")).IsChecked = true;
            }
            else if (difficulty == "Hard")
            {
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "medium")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "easy")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "hard")).IsChecked = true;
            }
        }

        private void loadSavedPuzzle()
        {
            int x, i, j;
            x = 0;
            foreach (var text in sudokuGrid.Children)
            {
                if (text is TextBox)
                {
                    i = x / 9;
                    j = x % 9;
                    ((TextBox)text).Text = "";
                    ((TextBox)text).Foreground = Brushes.Blue;
                    ((TextBox)text).Background = Brushes.White;
                    ((TextBox)text).PreviewTextInput += TextBox_PreviewTextInput;
                    ((TextBox)text).IsReadOnly = false;
                    if (loadedPuzzle[i, j] != 0 && checkRead[i, j] == 0)
                    {
                        ((TextBox)text).Text = loadedPuzzle[i, j].ToString();
                    }
                        if (loadedPuzzle[i, j] != 0 && checkRead[i, j] == 1)
                    {
                        ((TextBox)text).Text = loadedPuzzle[i, j].ToString();
                        ((TextBox)text).Foreground = Brushes.Black;
                        ((TextBox)text).Background = Brushes.AliceBlue;
                        ((TextBox)text).IsReadOnly = true;
                    } else
                    {
                        
                    }

                    x++;
                }

            }
            ((Label)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "Heading")).Content = "Difficulty: " + difficulty;
            if (difficulty == "Easy")
            {
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "medium")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "hard")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "easy")).IsChecked = true;
            }
            else if (difficulty == "Medium")
            {
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "easy")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "hard")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "medium")).IsChecked = true;
            }
            else if (difficulty == "Hard")
            {
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "medium")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "easy")).IsChecked = false;
                ((MenuItem)LogicalTreeHelper.FindLogicalNode(sudokuGrid, "hard")).IsChecked = true;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^1-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "button1")
            {
                generatePuzzle(difficulty);
            }
            if (button.Name == "button2")
            {
                saveGame();
            }
            if (button.Name == "button3")
            {
                loadGame();
            }
            if (button.Name == "button4" ||
                button.Name == "button5" ||
                button.Name == "button6" ||
                button.Name == "button7" ||
                button.Name == "button8" ||
                button.Name == "button9")
            {
                int number = ((int)Char.GetNumericValue(button.Name[6])) - 3;
                if (textName != " ")
                {
                    ((TextBox)LogicalTreeHelper.FindLogicalNode(sudokuGrid, textName)).Text = number.ToString();
                }

            }
            if (button.Name == "button10" ||
                button.Name == "button11" ||
                button.Name == "button12")
            {
                int number = Int32.Parse(button.Name.Substring(6)) - 3;
                if (textName != " ")
                {
                    ((TextBox)LogicalTreeHelper.FindLogicalNode(sudokuGrid, textName)).Text = number.ToString();
                }
            }
            if (button.Name == "button13")
            {
                loadPuzzle();
            }
        }
        private void saveGame()
        {
            string saveString = "";
            foreach (int number in puzzle)
            {
                saveString += number.ToString();
            }
            saveString += Environment.NewLine;
            foreach (int number in backup)
            {
                saveString += number.ToString();
            }
            saveString += Environment.NewLine;
            foreach (int number in solution)
            {
                saveString += number.ToString();
            }
            saveString += Environment.NewLine;
            if (difficulty == "Easy")
            {
                saveString += 1.ToString();
            } else if (difficulty == "Medium")
            {
                saveString += 2.ToString();
            } else
            {
                saveString += 3.ToString();
            }
            saveString += Environment.NewLine;
            foreach (var textbox in sudokuGrid.Children)
            {
                if (textbox is TextBox)
                {
                    saveString += ((TextBox)textbox).IsReadOnly ? 1.ToString() : 0.ToString();
                }
                
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "SudokuSave.txt";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            save.Title = "Save Game";
            save.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            save.FilterIndex = 2;
            save.RestoreDirectory = true;
            if (save.ShowDialog() == true)
            {
                File.WriteAllText(save.FileName, saveString);
            }
        }

        private void loadGame()
        {
            OpenFileDialog load = new OpenFileDialog();
            load.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            load.Title = "Load Game";
            load.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            load.FilterIndex = 2;
            load.RestoreDirectory = true;
            if (load.ShowDialog() == true)
            {
                string[] filelines = File.ReadAllLines(load.FileName);
                int counter = 0;
                int i = 0;
                int j = 0;
                for (int number = 0; number < 81; number++)
                {
                    loadedPuzzle[i, j] = ((int)Char.GetNumericValue(filelines[0][number]));
                    counter++;
                    i = counter / 9;
                    j = counter % 9;
                }
                counter = 0;
                i = 0;
                j = 0;
                for (int number = 0; number < 81; number++)
                {
                    backup[i, j] = ((int)Char.GetNumericValue(filelines[1][number]));
                    counter++;
                    i = counter / 9;
                    j = counter % 9;
                }
                counter = 0;
                i = 0;
                j = 0;
                for (int number = 0; number < 81; number++)
                {
                    solution[i, j] = ((int)Char.GetNumericValue(filelines[2][number]));
                    counter++;
                    i = counter / 9;
                    j = counter % 9;
                }
                if (filelines[3] == "1")
                {
                    difficulty = "Easy";
                } else if (filelines[3] == "2")
                {
                    difficulty = "Medium";
                } else
                {
                    difficulty = "Hard";
                }
                counter = 0;
                i = 0;
                j = 0;
                for (int number = 0; number < 81; number++)
                {
                    checkRead[i, j] = ((int)Char.GetNumericValue(filelines[4][number]));
                    counter++;
                    i = counter / 9;
                    j = counter % 9;
                }

                loadSavedPuzzle();
            }
        }
    }

}
