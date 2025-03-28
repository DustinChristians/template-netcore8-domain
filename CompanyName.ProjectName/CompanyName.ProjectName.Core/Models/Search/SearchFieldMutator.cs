using System;
using System.Linq;

namespace CompanyName.ProjectName.Core.Models.Search
{
    public delegate IQueryable<TItem> QueryMutator<TItem, TSearch>(IQueryable<TItem> items, TSearch search);

    public class SearchFieldMutator<TItem, TSearch>
    {
        public SearchFieldMutator(Predicate<TSearch> condition, QueryMutator<TItem, TSearch> mutator)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Mutator = mutator ?? throw new ArgumentNullException(nameof(mutator));
        }

        public Predicate<TSearch> Condition { get; }
        public QueryMutator<TItem, TSearch> Mutator { get; }

        public IQueryable<TItem> Apply(TSearch search, IQueryable<TItem> query) =>
            Condition(search) ? Mutator(query, search) : query;
    }
}