namespace GameScriptManager.Models;

/// <summary>
/// Specifies the sorting algorithm to apply on the linked list.
/// </summary>
public enum SortingAlgorithm
{
    /// <summary>
    /// Insertion Sort: Efficient for linked lists, inserting each node into sorted position.
    /// Time Complexity: Best O(N), Average/Worst O(N^2). Space: O(1).
    /// </summary>
    InsertionSort,

    /// <summary>
    /// Bubble Sort: Iteratively compares and swaps adjacent node values.
    /// Time Complexity: Best O(N), Average/Worst O(N^2). Space: O(1).
    /// </summary>
    BubbleSort,

    /// <summary>
    /// Merge Sort: Divide-and-conquer algorithm splitting and merging sublists.
    /// Time Complexity: Best/Average/Worst O(N log N). Space: O(log N) stack.
    /// </summary>
    MergeSort
}
