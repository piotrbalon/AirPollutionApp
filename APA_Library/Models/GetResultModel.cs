namespace APA_Library.Models
{
    public struct MetaData
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Found { get; set; }
    }

    public class GetResultModel<T>
    {
        public MetaData Meta { get; set; }
        public T Results { get; set; }
    }
}
