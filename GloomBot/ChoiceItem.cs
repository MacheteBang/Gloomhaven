using System;
namespace GloomBot
{
    public class ChoiceItem : IComparable<ChoiceItem>
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public int CompareTo(ChoiceItem other)
        {
            return string.Compare(this.Title, other.Title);
        }
    }
}
