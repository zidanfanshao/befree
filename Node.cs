namespace Befree
{
    public abstract class Node
    {
        public string Name { get; set; }
        public abstract object ToClashProxy();
    }
}
