namespace eVision.eCrypt.KeyGenerator.Helpers.Extensions
{
    using System.Collections.Generic;

    internal static class CollectionExtensions
    {
        public static void ReplaceAllWith<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            collection.Clear();
            foreach (var newItem in newItems)
            {
                collection.Add(newItem);
            }
        }
    }
}
