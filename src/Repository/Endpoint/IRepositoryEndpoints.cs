﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Radical.Series;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Repository.Endpoint
{
    public interface IRepositoryEndpoints : ISeries<IRepositoryEndpoint>
    {
        IRepositoryEndpoint this[DbContext context] { get; set; }
        IRepositoryEndpoint this[string contextName] { get; set; }
        IRepositoryEndpoint this[Type contextType] { get; set; }

        IRepositoryEndpoint<TContext> Add<TContext>(IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext;
        IRepositoryEndpoint Get(Type contextType);
        IRepositoryEndpoint<TContext> Get<TContext>() where TContext : DbContext;
        ulong GetKey(IRepositoryEndpoint item);
        IRepositoryEndpoint New(Type contextType, DbContextOptions options);
        IRepositoryEndpoint<TContext> New<TContext>(DbContextOptions<TContext> options) where TContext : DataStoreContext;
        IRepositoryEndpoint<TContext> Put<TContext>(IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext;
        bool Remove<TContext>() where TContext : DbContext;
        bool TryAdd(Type contextType, IRepositoryEndpoint repoSource);
        bool TryAdd<TContext>(IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext;
        bool TryGet(Type contextType, out IRepositoryEndpoint repoSource);
        bool TryGet<TContext>(out IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext;
    }
}