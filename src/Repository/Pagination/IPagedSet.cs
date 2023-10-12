﻿using System.Collections.Generic;
using System.Linq;

namespace Radical.Servitizing.Repository.Pagination
{
    public interface IPagedSet<T>
    {
        bool HasNextPage { get; }

        bool HasPreviousPage { get; }

        int IndexFrom { get; }

        IList<T> Items { get; }

        int PageIndex { get; }

        int PageSize { get; }

        int TotalCount { get; }

        int TotalPages { get; }
    }

}
