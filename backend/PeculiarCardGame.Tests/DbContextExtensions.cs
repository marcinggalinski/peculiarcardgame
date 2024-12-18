﻿using Microsoft.EntityFrameworkCore;

namespace PeculiarCardGame.Tests
{
    internal static class DbContextExtensions
    {
        /// <summary>
        /// Sets up <c>DbContext</c> for test by adding provided entities and clearing them from the change tracking.
        /// </summary>
        public static void SetupTest<T>(this DbContext dbContext, Action<T> cleanupCallback, params T[] entities)
            where T : class
        {
            foreach (var entity in entities)
            {
                dbContext.Add(entity);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();
                cleanupCallback(entity);
            }
        }

        /// <summary>
        /// Sets up <c>DbContext</c> for test by adding provided entities and clearing them from the change tracking.
        /// </summary>
        public static void SetupTest<T>(this DbContext dbContext, params T[] entities)
            where T : class => SetupTest(dbContext, _ => { }, entities);
    }
}
