﻿using Domain.Services;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Entities;
using Domain.Validations;

namespace DataAccess.Services
{
    public class BaseService : IBaseService
    {
        private readonly ApiContext Context;
        public BaseService(ApiContext context)
        {
            Context = context;
        }

        public virtual T GetWithKeys<T>(object[] keys, IEnumerable<string> navigations = null, IEnumerable<string> collections = null) where T : class
        {
            var entity = Context.Set<T>().Find(keys);

            if (entity != null)
            {
                (navigations ?? new string[] { }).Aggregate(entity, (e, navegation) =>
                {
                    Context.Entry<T>(e).Reference(navegation).Load();
                    return e;
                });

                (collections ?? new string[] { }).Aggregate(entity, (e, collection) =>
                {
                    Context.Entry<T>(e).Collection(collection).Load();
                    return e;
                });
            }
            return entity ?? new NullEntityFactory().GetNullObject<T>();
        }

        public virtual IQueryable<T> GetAll<T>(string included = "", bool readOnly = false) where T : class
        {
            return Query<T>(e => true, readOnly, included);
        }

        public virtual IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate = null, bool readOnly = false, string included = "") where T : class
        {
            var query = included
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(Context.Set<T>() as IQueryable<T>, (set, navigation) => set.Include<T>(navigation))
                .Where(predicate ?? (e => true));

            return readOnly ? query.AsNoTracking() : query;
        }

        public virtual T Insert<T>(T entity) where T : class
        {
            if (entity is BaseEntity baseEntity && !baseEntity.IsValid)
            {
                return entity;
            }
            return Context.Set<T>().Add(entity).Entity;
        }

        public virtual IEnumerable<T> InsertMany<T>(IEnumerable<T> entities) where T : class
        {
            if (entities.Any(entity => entity is BaseEntity baseEntity && !baseEntity.IsValid))
            {
                return entities;
            }
            Context.Set<T>().AddRange(entities);

            return entities;
        }

        public virtual T Remove<T>(T entity) where T : class
        {
            entity = Context.Set<T>().Remove(entity).Entity;
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.ValidationResult =new  NoContentValidationResult();
            }
            return entity;
        }

        public virtual T Remove<T>(params object[] keys) where T : class
        {
            var entity = Context.Set<T>().Remove(GetWithKeys<T>(keys)).Entity;
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.ValidationResult = new NoContentValidationResult();
            }
            return entity;
        }

        public virtual IEnumerable<T> RemoveMany<T>(IEnumerable<T> entities) where T : class
        {
            Context.Set<T>().RemoveRange(entities);
            foreach(var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.ValidationResult = new NoContentValidationResult();
                }
            }
            return entities;
        }

        public virtual IEnumerable<T> RemoveMany<T>(IEnumerable<object[]> keysList) where T : class
        {
            foreach (var keys in keysList)
            {
                var entity = Remove<T>(keys);
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.ValidationResult = new NoContentValidationResult();
                }
                yield return entity;
            }
        }
    }
}