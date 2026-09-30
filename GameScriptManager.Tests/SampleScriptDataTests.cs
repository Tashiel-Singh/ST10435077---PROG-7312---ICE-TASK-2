using GameScriptManager.Data;
using GameScriptManager.Models;
using Xunit;

namespace GameScriptManager.Tests;

public class SampleScriptDataTests
{
    [Fact]
    public void PopulateSampleScript_LoadsAllTwelveItems()
    {
        var list = SampleScriptData.PopulateSampleScript();

        Assert.Equal(12, list.Count);
        Assert.False(list.IsSorted());
    }

    [Fact]
    public void RawData_MatchesImageSpecification()
    {
        var raw = SampleScriptData.RawUnorderedData;

        Assert.Equal(12, raw.Count);

        // Verify sequence starts with 3 and ends with 8 as shown in sample script image
        Assert.Equal(3, raw[0].Number);
        Assert.Contains("Alex gains experience points", raw[0].Text);

        Assert.Equal(12, raw[1].Number);
        Assert.Contains("The tale of Alex, the IT student-turned-digital-legend", raw[1].Text);

        Assert.Equal(4, raw[2].Number);
        Assert.Contains("The Firewall Fortress looms ahead", raw[2].Text);

        Assert.Equal(2, raw[3].Number);
        Assert.Contains("Armed with a trusty keyboard and a digital sword", raw[3].Text);

        Assert.Equal(1, raw[4].Number);
        Assert.Contains("In the virtual realm of Cybersphere", raw[4].Text);

        Assert.Equal(7, raw[5].Number);
        Assert.Contains("Syntax Sentinels", raw[5].Text);

        Assert.Equal(10, raw[6].Number);
        Assert.Contains("Malware Marauders", raw[6].Text);

        Assert.Equal(11, raw[7].Number);
        Assert.Contains("Celebrated as a digital hero", raw[7].Text);

        Assert.Equal(9, raw[8].Number);
        Assert.Contains("In a climactic battle", raw[8].Text);

        Assert.Equal(5, raw[9].Number);
        Assert.Contains("virtual reality riddle", raw[9].Text);

        Assert.Equal(6, raw[10].Number);
        Assert.Contains("Repository of the Ancients", raw[10].Text);

        Assert.Equal(8, raw[11].Number);
        Assert.Contains("nemesis in the form of the Malware Marauders", raw[11].Text);
    }

    [Fact]
    public void SortedStory_FormsCompleteCoherentNarrative()
    {
        var list = SampleScriptData.PopulateSampleScript();
        list.Sort(SortingAlgorithm.InsertionSort);

        var first = list.GetNodeAt(0);
        var last = list.GetNodeAt(11);

        Assert.NotNull(first);
        Assert.Equal(1, first.StoryNumber);
        Assert.StartsWith("In the virtual realm of Cybersphere", first.StoryText);

        Assert.NotNull(last);
        Assert.Equal(12, last.StoryNumber);
        Assert.StartsWith("The tale of Alex", last.StoryText);
    }
}
