using System;
using System.Collections.Generic;
using System.Linq;
using CompanyName.ProjectName.Core.Extensions;

namespace CompanyName.ProjectName.Core.Models.Search
{
    public class SearchMutator<TItem, TSearch>
    {
        public List<SearchFieldMutator<TItem, TSearch>> SearchFieldMutators { get; } = new List<SearchFieldMutator<TItem, TSearch>>();

        public void AddCondition(Predicate<TSearch> condition, QueryMutator<TItem, TSearch> mutator)
        {
            SearchFieldMutators.Add(new SearchFieldMutator<TItem, TSearch>(condition, mutator));
        }

        public IQueryable<TItem> Apply(TSearch search, IQueryable<TItem> query)
        {
            if (search == null)
            {
                throw new ArgumentNullException(nameof(search));
            }

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            search.AllStringsToLower();

            foreach (var searchFieldMutator in SearchFieldMutators)
            {
                query = searchFieldMutator.Apply(search, query);
            }

            return query;
        }
    }
}
