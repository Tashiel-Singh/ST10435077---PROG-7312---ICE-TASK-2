using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GameScriptManager.Data;
using GameScriptManager.Models;
using GameScriptManager.Views;

namespace GameScriptManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// Coordinates the Linked List, Dual Display Areas, Sorting Engine, and Visualizers.
/// </summary>
public partial class MainWindow : Window
{
    private StoryLinkedList _scriptList = new();
    private Node? _currentNode;
    private readonly DispatcherTimer _autoPlayTimer = new();
    private bool _isAutoPlaying;

    public class VisualNodeItem
    {
        public Node Node { get; set; } = null!;
        public Brush BgColor { get; set; } = Brushes.Transparent;
        public Brush BorderColor { get; set; } = Brushes.Transparent;
        public Brush TextColor { get; set; } = Brushes.White;
        public Visibility ArrowVisibility { get; set; } = Visibility.Visible;
    }

    public MainWindow()
    {
        InitializeComponent();

        _autoPlayTimer.Interval = TimeSpan.FromSeconds(2.5);
        _autoPlayTimer.Tick += AutoPlayTimer_Tick;

        Loaded += OnWindowLoaded;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        // Requirement 2 & 5: Populate linked list with sample data and sort on startup
        LoadSampleData(sortImmediately: true);
    }

    /// <summary>
    /// Loads the 12 unordered script paragraphs into the linked list.
    /// </summary>
    private void LoadSampleData(bool sortImmediately)
    {
        StopAutoPlay();
        _scriptList = SampleScriptData.PopulateSampleScript();

        if (sortImmediately)
        {
            var result = _scriptList.Sort(GetSelectedAlgorithm());
            TxtMetrics.Text = $"Sort Time: {result.ElapsedMilliseconds:F2}ms | Comparisons: {result.Comparisons} | Swaps: {result.SwapsOrMoves}";
            TxtStatusMessage.Text = $"Loaded & Sorted {result.Algorithm} ({_scriptList.Count} scenes).";
        }
        else
        {
            TxtMetrics.Text = "Unordered Raw Script Loaded";
            TxtStatusMessage.Text = "Loaded disordered sample script from specification (12 scenes).";
        }

        _currentNode = _scriptList.Head;
        RefreshAllDisplays();
    }

    /// <summary>
    /// Refreshes Display Area 1, Display Area 2, the bottom visualizer chain, and indicators.
    /// </summary>
    private void RefreshAllDisplays()
    {
        UpdateDisplayArea1();
        UpdateDisplayArea2();
        UpdateNodeVisualizerChain();
        UpdateSortBadge();
    }

    #region Display Area 1 (Full Script)

    private void UpdateDisplayArea1()
    {
        string filter = TxtSearchScript?.Text.Trim() ?? string.Empty;
        var allNodes = _scriptList.ToList();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            allNodes = allNodes.FindAll(n =>
                n.StoryNumber.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                n.StoryText.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        ItemsFullScript.ItemsSource = allNodes;

        // Word count & stats
        int totalWords = 0;
        foreach (var node in _scriptList)
        {
            if (!string.IsNullOrWhiteSpace(node.StoryText))
            {
                totalWords += node.StoryText.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }

        TxtFullScriptStats.Text = $"{_scriptList.Count} paragraphs | {totalWords} words | ~{Math.Max(1, (int)Math.Ceiling(totalWords / 150.0))} min read";
    }

    private void OnScriptSearchChanged(object sender, TextChangedEventArgs e)
    {
        if (TxtSearchPlaceholder != null)
        {
            TxtSearchPlaceholder.Visibility = string.IsNullOrEmpty(TxtSearchScript.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
        if (BtnClearSearch != null)
        {
            BtnClearSearch.Visibility = string.IsNullOrEmpty(TxtSearchScript.Text) ? Visibility.Collapsed : Visibility.Visible;
        }

        UpdateDisplayArea1();
    }

    private void OnClearSearchClick(object sender, RoutedEventArgs e)
    {
        TxtSearchScript.Text = string.Empty;
        TxtSearchScript.Focus();
    }

    private void OnJumpToNodeClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Node node)
        {
            _currentNode = node;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();

            // If in Full View, also ensure we toggle to split or line view if requested
            if (RbFullView.IsChecked == true)
            {
                RbLineView.IsChecked = true;
            }
        }
    }

    private void OnCopyScriptClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string fullStory = _scriptList.GetCombinedScriptText(includeNumbers: true);
            Clipboard.SetText(fullStory);
            MessageBox.Show("Story script copied to clipboard successfully!", "Script Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to copy to clipboard: {ex.Message}", "Clipboard Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    #endregion

    #region Display Area 2 (One-Line at a Time)

    private void UpdateDisplayArea2()
    {
        if (_scriptList.IsEmpty || _currentNode == null)
        {
            TxtIndexIndicator.Text = "SCENE 0 OF 0";
            TxtCardChapterBadge.Text = "EMPTY SCRIPT";
            TxtSingleLineStory.Text = "No story scenes currently loaded. Click 'Load Sample Script' to begin.";
            TxtPrevPointer.Text = "◀ Prev: [None]";
            TxtCurrentPointer.Text = "● Active: [None]";
            TxtNextPointer.Text = "Next: [None] ▶";
            BtnPrevious.IsEnabled = false;
            BtnNext.IsEnabled = false;
            StoryProgressBar.Value = 0;
            return;
        }

        int currentIndex = _scriptList.IndexOf(_currentNode);
        int displayIndex = currentIndex >= 0 ? currentIndex + 1 : 1;

        TxtIndexIndicator.Text = $"SCENE {displayIndex} OF {_scriptList.Count}";
        TxtCardChapterBadge.Text = $"CHAPTER {_currentNode.StoryNumber}";
        TxtSingleLineStory.Text = _currentNode.StoryText;

        StoryProgressBar.Minimum = 1;
        StoryProgressBar.Maximum = Math.Max(1, _scriptList.Count);
        StoryProgressBar.Value = displayIndex;

        // Update Doubly Linked List Pointer Indicators
        string prevLabel = _currentNode.Previous != null ? $"[{_currentNode.Previous.StoryNumber}]" : "[None - Head]";
        string nextLabel = _currentNode.Next != null ? $"[{_currentNode.Next.StoryNumber}]" : "[None - Tail]";

        TxtPrevPointer.Text = $"◀ Prev: {prevLabel}";
        TxtCurrentPointer.Text = $"● Active: [{_currentNode.StoryNumber}]";
        TxtNextPointer.Text = $"Next: {nextLabel} ▶";

        // Navigation button states
        BtnPrevious.IsEnabled = _currentNode.Previous != null;
        BtnNext.IsEnabled = _currentNode.Next != null;
    }

    private void OnPreviousClick(object sender, RoutedEventArgs e)
    {
        if (_currentNode?.Previous != null)
        {
            _currentNode = _currentNode.Previous;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }
    }

    private void OnNextClick(object sender, RoutedEventArgs e)
    {
        if (_currentNode?.Next != null)
        {
            _currentNode = _currentNode.Next;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }
        else if (_isAutoPlaying)
        {
            StopAutoPlay();
        }
    }

    private void OnFirstClick(object sender, RoutedEventArgs e)
    {
        if (_scriptList.Head != null)
        {
            _currentNode = _scriptList.Head;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }
    }

    private void OnLastClick(object sender, RoutedEventArgs e)
    {
        if (_scriptList.Tail != null)
        {
            _currentNode = _scriptList.Tail;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }
    }

    private void OnAutoPlayClick(object sender, RoutedEventArgs e)
    {
        if (_isAutoPlaying)
        {
            StopAutoPlay();
        }
        else
        {
            StartAutoPlay();
        }
    }

    private void StartAutoPlay()
    {
        if (_scriptList.IsEmpty) return;

        // If at end, loop to head
        if (_currentNode?.Next == null)
        {
            _currentNode = _scriptList.Head;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }

        _isAutoPlaying = true;
        BtnAutoPlay.Content = "⏸ Pause Auto-Tour";
        BtnAutoPlay.Background = new SolidColorBrush(Color.FromRgb(163, 113, 247)); // Purple
        _autoPlayTimer.Start();
        TxtStatusMessage.Text = "Auto-Tour active: Advancing every 2.5 seconds.";
    }

    private void StopAutoPlay()
    {
        _isAutoPlaying = false;
        _autoPlayTimer.Stop();
        if (BtnAutoPlay != null)
        {
            BtnAutoPlay.Content = "▶️ Auto-Tour (Story Slide)";
            BtnAutoPlay.Background = new SolidColorBrush(Color.FromRgb(33, 38, 45));
        }
    }

    private void AutoPlayTimer_Tick(object? sender, EventArgs e)
    {
        if (_currentNode?.Next != null)
        {
            _currentNode = _currentNode.Next;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }
        else
        {
            // Completed tour
            StopAutoPlay();
            TxtStatusMessage.Text = "Auto-Tour completed! Reached the conclusion of Cybersphere.";
        }
    }

    #endregion

    #region Linked List Visualizer Chain

    private void UpdateNodeVisualizerChain()
    {
        var items = new List<VisualNodeItem>();
        var current = _scriptList.Head;

        while (current != null)
        {
            bool isActive = ReferenceEquals(current, _currentNode);

            Brush bg = isActive
                ? new SolidColorBrush(Color.FromArgb(200, 31, 111, 235)) // Glowing Blue
                : new SolidColorBrush(Color.FromRgb(33, 38, 45));

            Brush border = isActive
                ? new SolidColorBrush(Color.FromRgb(0, 210, 255)) // Neon Cyan Glow
                : new SolidColorBrush(Color.FromRgb(48, 54, 61));

            Brush text = isActive
                ? Brushes.White
                : new SolidColorBrush(Color.FromRgb(201, 209, 217));

            items.Add(new VisualNodeItem
            {
                Node = current,
                BgColor = bg,
                BorderColor = border,
                TextColor = text,
                ArrowVisibility = current.Next != null ? Visibility.Visible : Visibility.Collapsed
            });

            current = current.Next;
        }

        ItemsNodeChain.ItemsSource = items;

        string headStr = _scriptList.Head != null ? $"[{_scriptList.Head.StoryNumber}]" : "null";
        string tailStr = _scriptList.Tail != null ? $"[{_scriptList.Tail.StoryNumber}]" : "null";
        TxtListPointersSummary.Text = $"Head: {headStr} | Tail: {tailStr} | Count: {_scriptList.Count}";
    }

    private void OnVisualNodeClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is Node node)
        {
            _currentNode = node;
            UpdateDisplayArea2();
            UpdateNodeVisualizerChain();
        }
    }

    private void UpdateSortBadge()
    {
        bool isSorted = _scriptList.IsSorted();
        if (isSorted)
        {
            BadgeSortState.Background = new SolidColorBrush(Color.FromRgb(46, 160, 67)); // Green
            TxtSortStateBadge.Text = "ORDERED (1..N)";
        }
        else
        {
            BadgeSortState.Background = new SolidColorBrush(Color.FromRgb(218, 54, 51)); // Red
            TxtSortStateBadge.Text = "DISORDERED / RAW";
        }
    }

    #endregion

    #region Toolbar Commands (Sort, Shuffle, Add, Clear)

    private SortingAlgorithm GetSelectedAlgorithm()
    {
        if (CmbAlgorithm?.SelectedItem is ComboBoxItem item &&
            item.Tag is string tagStr &&
            Enum.TryParse<SortingAlgorithm>(tagStr, out var algo))
        {
            return algo;
        }
        return SortingAlgorithm.InsertionSort;
    }

    private void OnSortClick(object sender, RoutedEventArgs e)
    {
        StopAutoPlay();

        if (_scriptList.IsEmpty)
        {
            MessageBox.Show("The linked list is currently empty. Load sample data first.", "Empty List", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var algo = GetSelectedAlgorithm();
        var result = _scriptList.Sort(algo);

        _currentNode = _scriptList.Head;
        RefreshAllDisplays();

        TxtMetrics.Text = $"Sort Time: {result.ElapsedMilliseconds:F3}ms | Comparisons: {result.Comparisons} | Swaps/Moves: {result.SwapsOrMoves}";
        TxtStatusMessage.Text = $"Successfully sorted linked list using {result.Algorithm} ({result.Comparisons} comparisons, {result.SwapsOrMoves} moves).";
    }

    private void OnLoadSampleClick(object sender, RoutedEventArgs e)
    {
        LoadSampleData(sortImmediately: false);
    }

    private void OnShuffleClick(object sender, RoutedEventArgs e)
    {
        StopAutoPlay();

        if (_scriptList.Count <= 1)
        {
            MessageBox.Show("Need at least 2 nodes to shuffle.", "Cannot Shuffle", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // Shuffle existing nodes into a disordered sequence to test sorting
        var list = _scriptList.ToList();
        var rng = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int k = rng.Next(i + 1);
            (list[i].StoryNumber, list[k].StoryNumber) = (list[k].StoryNumber, list[i].StoryNumber);
            (list[i].StoryText, list[k].StoryText) = (list[k].StoryText, list[i].StoryText);
        }

        _currentNode = _scriptList.Head;
        RefreshAllDisplays();
        TxtStatusMessage.Text = "Scrambled linked list nodes into disordered sequence. Ready to test sorting!";
        TxtMetrics.Text = "Nodes Shuffled (Disordered)";
    }

    private void OnAddSceneClick(object sender, RoutedEventArgs e)
    {
        int nextSuggested = _scriptList.IsEmpty ? 1 : (_scriptList.Tail?.StoryNumber ?? 0) + 1;
        var dialog = new AddNodeDialog(nextSuggested)
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true)
        {
            Node newNode;
            if (dialog.Prepend)
            {
                newNode = _scriptList.AddFirst(dialog.StoryNumber, dialog.StoryText);
            }
            else
            {
                newNode = _scriptList.Add(dialog.StoryNumber, dialog.StoryText);
            }

            _currentNode = newNode;
            RefreshAllDisplays();
            TxtStatusMessage.Text = $"Added Scene #{newNode.StoryNumber} to the Linked List.";
        }
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        var res = MessageBox.Show("Are you sure you want to clear all nodes from the linked list?", "Clear List", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res == MessageBoxResult.Yes)
        {
            StopAutoPlay();
            _scriptList.Clear();
            _currentNode = null;
            RefreshAllDisplays();
            TxtStatusMessage.Text = "Linked list cleared.";
            TxtMetrics.Text = "List Empty";
        }
    }

    #endregion

    #region View Mode Switching (Smooth Responsive Layout)

    private void OnViewModeChanged(object sender, RoutedEventArgs e)
    {
        if (ColDisplayArea1 == null || ColDisplayArea2 == null || ColSplitter == null) return;

        if (RbFullView.IsChecked == true)
        {
            // Show only Display Area 1 (Full View)
            ColDisplayArea1.Width = new GridLength(1, GridUnitType.Star);
            ColDisplayArea2.Width = new GridLength(0);
            ColSplitter.Width = new GridLength(0);
            CardDisplayArea1.Visibility = Visibility.Visible;
            CardDisplayArea2.Visibility = Visibility.Collapsed;
            MainSplitter.Visibility = Visibility.Collapsed;
        }
        else if (RbLineView.IsChecked == true)
        {
            // Show only Display Area 2 (One-line Navigation View)
            ColDisplayArea1.Width = new GridLength(0);
            ColDisplayArea2.Width = new GridLength(1, GridUnitType.Star);
            ColSplitter.Width = new GridLength(0);
            CardDisplayArea1.Visibility = Visibility.Collapsed;
            CardDisplayArea2.Visibility = Visibility.Visible;
            MainSplitter.Visibility = Visibility.Collapsed;
        }
        else
        {
            // Side-by-side Dual View
            ColDisplayArea1.Width = new GridLength(1.1, GridUnitType.Star);
            ColDisplayArea2.Width = new GridLength(1, GridUnitType.Star);
            ColSplitter.Width = new GridLength(12);
            CardDisplayArea1.Visibility = Visibility.Visible;
            CardDisplayArea2.Visibility = Visibility.Visible;
            MainSplitter.Visibility = Visibility.Visible;
        }
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        // Keyboard shortcuts for line-by-line navigation
        if (e.Key == Key.Left)
        {
            OnPreviousClick(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Right)
        {
            OnNextClick(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Home)
        {
            OnFirstClick(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.End)
        {
            OnLastClick(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Space && Keyboard.FocusedElement is not TextBox)
        {
            OnAutoPlayClick(sender, e);
            e.Handled = true;
        }
    }

    #endregion
}