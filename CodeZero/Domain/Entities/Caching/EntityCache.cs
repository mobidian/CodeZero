﻿//  <copyright file="EntityCache.cs" project="CodeZero" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System.Threading.Tasks;
using CodeZero.Domain.Repositories;
using CodeZero.Events.Bus.Entities;
using CodeZero.Events.Bus.Handlers;
using CodeZero.ObjectMapping;
using CodeZero.Runtime.Caching;

namespace CodeZero.Domain.Entities.Caching
{
    public class EntityCache<TEntity, TCacheItem> :
        EntityCache<TEntity, TCacheItem, int>,
        IEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>
    {
        public EntityCache(
            ICacheManager cacheManager,
            IRepository<TEntity, int> repository,
            string cacheName = null)
            : base(
                cacheManager,
                repository,
                cacheName)
        {
        }
    }

    public class EntityCache<TEntity, TCacheItem, TPrimaryKey> :
        IEventHandler<EntityChangedEventData<TEntity>>, IEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public TCacheItem this[TPrimaryKey id]
        {
            get { return Get(id); }
        }

        public string CacheName { get; private set; }

        public ITypedCache<TPrimaryKey, TCacheItem> InternalCache
        {
            get
            {
                return CacheManager.GetCache<TPrimaryKey, TCacheItem>(CacheName);
            }
        }

        public IObjectMapper ObjectMapper { get; set; }

        protected ICacheManager CacheManager { get; private set; }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }

        public EntityCache(
            ICacheManager cacheManager, 
            IRepository<TEntity, TPrimaryKey> repository, 
            string cacheName = null)
        {
            Repository = repository;
            CacheManager = cacheManager;
            CacheName = cacheName ?? GenerateDefaultCacheName();
            ObjectMapper = NullObjectMapper.Instance;
        }

        public virtual TCacheItem Get(TPrimaryKey id)
        {
            return InternalCache.Get(id, () => GetCacheItemFromDataSource(id));
        }

        public virtual Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            return InternalCache.GetAsync(id, () => GetCacheItemFromDataSourceAsync(id));
        }

        public virtual void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            InternalCache.Remove(eventData.Entity.Id);
        }

        protected virtual TCacheItem GetCacheItemFromDataSource(TPrimaryKey id)
        {
            return MapToCacheItem(GetEntityFromDataSource(id));
        }

        protected virtual async Task<TCacheItem> GetCacheItemFromDataSourceAsync(TPrimaryKey id)
        {
            return MapToCacheItem(await GetEntityFromDataSourceAsync(id));
        }

        protected virtual TEntity GetEntityFromDataSource(TPrimaryKey id)
        {
            return Repository.FirstOrDefault(id);
        }

        protected virtual Task<TEntity> GetEntityFromDataSourceAsync(TPrimaryKey id)
        {
            return Repository.FirstOrDefaultAsync(id);
        }

        protected virtual TCacheItem MapToCacheItem(TEntity entity)
        {
            if (ObjectMapper is NullObjectMapper)
            {
                throw new CodeZeroException(
                    string.Format(
                        "MapToCacheItem method should be overrided or IObjectMapper should be implemented in order to map {0} to {1}",
                        typeof (TEntity),
                        typeof (TCacheItem)
                        )
                    );
            }

            return ObjectMapper.Map<TCacheItem>(entity);
        }

        protected virtual string GenerateDefaultCacheName()
        {
            return GetType().FullName;
        }

        public override string ToString()
        {
            return string.Format("EntityCache {0}", CacheName);
        }
    }
}