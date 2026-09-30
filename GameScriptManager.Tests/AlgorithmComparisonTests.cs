using GameScriptManager.Data;
using GameScriptManager.Models;
using Xunit;

namespace GameScriptManager.Tests;

public class AlgorithmComparisonTests
{
    [Fact]
    public void AllAlgorithms_ProduceIdenticalSortedOutput()
    {
        var algos = new[]
        {
            SortingAlgorithm.InsertionSort,
            SortingAlgorithm.BubbleSort,
            SortingAlgorithm.MergeSort
        };

        var results = new List<List<int>>();

        foreach (var algo in algos)
        {
            var list = SampleScriptData.PopulateSampleScript();
            list.Sort(algo);

            var numbers = new List<int>();
            foreach (var node in list)
            {
                numbers.Add(node.StoryNumber);
            }
            results.Add(numbers);
        }

        // All 3 algorithms must produce exact sequence 1..12
        var expected = Enumerable.Range(1, 12).ToList();
        foreach (var result in results)
        {
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void Sort_HandlesDuplicateKeys()
    {
        var list = new StoryLinkedList();
        list.Add(5, "Five A");
        list.Add(2, "Two");
        list.Add(5, "Five B");
        list.Add(1, "One");

        list.Sort(SortingAlgorithm.InsertionSort);

        Assert.True(list.IsSorted());
        Assert.Equal(4, list.Count);
        Assert.Equal(1, list.GetNodeAt(0)!.StoryNumber);
        Assert.Equal(2, list.GetNodeAt(1)!.StoryNumber);
        Assert.Equal(5, list.GetNodeAt(2)!.StoryNumber);
        Assert.Equal(5, list.GetNodeAt(3)!.StoryNumber);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(16)]
    [InlineData(50)]
    public void MergeSort_HandlesVariousListSizes(int size)
    {
        var list = new StoryLinkedList();
        var rng = new Random(42);
        var values = Enumerable.Range(1, size).OrderBy(_ => rng.Next()).ToList();

        foreach (var v in values)
        {
            list.Add(v, $"Scene {v}");
        }

        list.Sort(SortingAlgorithm.MergeSort);

        Assert.True(list.IsSorted());
        Assert.Equal(size, list.Count);

        // Verify pointers
        var curr = list.Head;
        int count = 0;
        Node? prev = null;
        while (curr != null)
        {
            Assert.Same(prev, curr.Previous);
            prev = curr;
            curr = curr.Next;
            count++;
        }
        Assert.Equal(size, count);
        Assert.Same(list.Tail, prev);
    }

    [Fact]
    public void Sort_TracksMetricsCorrectly()
    {
        var list = SampleScriptData.PopulateSampleScript();
        var result = list.Sort(SortingAlgorithm.InsertionSort);

        Assert.True(result.Comparisons > 0, "Comparisons should be tracked.");
        Assert.True(result.SwapsOrMoves > 0, "Moves should be tracked.");
        Assert.False(result.WasAlreadySorted, "Sample data was not pre-sorted.");
    }
}
