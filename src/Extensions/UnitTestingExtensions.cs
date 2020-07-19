using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace SubSonic.Extensions.Test
{
    using src;
    using Data.Builders;
    using Extensions.Test.MockDbClient.Syntax;
    using Linq;
    using Schema;
    using System.Globalization;
    using System.Collections;
    using System.Reflection;

    public static partial class SubSonicTestExtensions
    {
        public static string GetSql<TEntity>(this ICollection<TEntity> entities)
        {
            if (entities is null)
            {
                return "";
            }

            if (entities.IsSubSonicQuerable())
            {
                return ((IQueryable<TEntity>)entities).Expression.ToString();
            }

            return "";
        }

        public static string GetSql<TEntity>(this IEnumerable<TEntity> entities)
        {
            if (entities is null)
            {
                return "";
            }

            if (entities.IsSubSonicQuerable())
            {
                return ((IQueryable<TEntity>)entities).Expression.ToString();
            }

            return "";
        }

        public static string GetSql<TEntity>(this IQueryable<TEntity> entities)
        {
            if (entities is null)
            {
                return "";
            }

            if (entities.IsSubSonicQuerable())
            {
                return (entities).Expression.ToString();
            }

            return "";
        }

        public static TType GetValue<TType>(this DbParameter parameter)
        {
            if (!(parameter is null))
            {
                Type type = typeof(TType);

                if (parameter.Value != DBNull.Value)
                {
                    return (TType)Convert.ChangeType(parameter.Value, type, CultureInfo.CurrentCulture);
                }
                else if (parameter.SourceColumnNullMapping)
                {
                    return (TType)Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(type));
                }
                else if (type.IsNullableType())
                {
                    return Activator.CreateInstance<TType>();
                }
                else
                {
#if NETSTANDARD2_0
                    return default(TType);
#elif NETSTANDARD2_1
                    return default;
#endif
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(parameter));
            }
        }

        public static TType GetValue<TType>(this object value)
        {
            if (value is DBNull)
            {
#if NETSTANDARD2_0
                return default(TType);
#elif NETSTANDARD2_1
                return default;
#endif
            }
            else if (value is TType result)
            {
                return result;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static DataTable ToDataTable(this IEnumerable source, Type entityType, IDbEntityModel model)
        {
            if (source is null)
            {
                throw Error.ArgumentNull(nameof(entityType));
            }

            if (entityType is null)
            {
                throw Error.ArgumentNull(nameof(entityType));
            }

            using (DataTableBuilder builder = new DataTableBuilder(model?.Name ?? entityType.Name))
            {
                foreach (PropertyInfo property in entityType.GetProperties())
                {
                    if (model != null && model[property.Name].EntityPropertyType != DbEntityPropertyType.Value)
                    {
                        continue;
                    }

                    builder.AddColumn(
                        model?[property.Name].Name ?? property.Name, 
                        property.PropertyType);
                }

                return builder.DataTable;
            }
        }

        public static DataTable ToDataTable<TEntity>(this IEnumerable<TEntity> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Type entityType = typeof(TEntity);
            SubSonicContext.DbModel.TryGetEntityModel(entityType, out IDbEntityModel model);

            if (entityType.IsClass)
            { 
                using (DataTableBuilder builder = new DataTableBuilder(source.ToDataTable(entityType, model)))
                {
                    foreach (TEntity entity in source)
                    {
                        DataRow row = builder.CreateRow();

                        foreach (PropertyInfo property in entityType.GetProperties())
                        {
                            if (model != null && model[property.Name].EntityPropertyType != DbEntityPropertyType.Value)
                            {
                                continue;
                            }

                            row[model?[property.Name].Name ?? property.Name] = entityType
                                    .GetProperty(property.Name)
                                    .GetValue(entity) ?? DBNull.Value;
                        }

                        builder.AddRow(row);
                    }

                    return builder.DataTable;
                }
            }
            else
            {
                using (DataTableBuilder builder = new DataTableBuilder())
                {
                    builder.AddColumn("", typeof(TEntity));

                    foreach (TEntity entity in source)
                    {
                        DataRow row = builder.CreateRow();

                        row[0] = entity;

                        builder.AddRow(row);
                    }

                    return builder.DataTable;
                }
            }
        }

        public static void AddCommandBehavior(this DbProviderFactory factory, string command, Func<DbCommand, object> result)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("", nameof(command));
            }

            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (factory is SubSonicMockDbClient db)
            {
                db.AddBehavior(new MockCommandBehavior()
                    .When((cmd) =>
                        cmd.CommandText == command)
                    .ReturnsData(result));
            }
        }

        public static void RecievedCommand(this DbProviderFactory factory, string command)
        {
            if (factory is SubSonicMockDbClient db)
            {
                if (db.RecievedBehavior(command) == 0)
                {
                    throw new InvalidOperationException(SubSonicExtenstionTestErrors.DbCommandNotRecieved.Format(command));
                }
            }
        }

        public static int RecievedCommandCount(this DbProviderFactory factory, string command)
        {
            if (factory is SubSonicMockDbClient db)
            {
                return db.RecievedBehavior(command);
            }

#if NETSTANDARD2_0
            return default(int);
#elif NETSTANDARD2_1
            return default;
#endif
        }

        public static void AddCommandBehavior<TEntity>(this DbProviderFactory factory, string command, IEnumerable<TEntity> entities)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("", nameof(command));
            }

            if (entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            if (factory is SubSonicMockDbClient db)
            {
                db.AddBehavior(new MockCommandBehavior()
                    .When((cmd) => 
                        cmd.CommandText == command)
                    .ReturnsData(entities.ToDataTable()));
            }
        }

        public static AlteredState<TSource, TActual> AlteredState<TSource, TActual>(this TSource source, object state)
            where TSource : class
            where TActual : class, TSource, new()
        {
            return new AlteredState<TSource, TActual>(source, state);
        }

        public static void UpdateProviders(this SubSonicContext dbContext, string dbProviderInvariantName, string sqlQueryProviderInvariantName = null)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (string.IsNullOrEmpty(dbProviderInvariantName))
            {
                throw new ArgumentException("", nameof(dbProviderInvariantName));
            }

            sqlQueryProviderInvariantName = sqlQueryProviderInvariantName ?? dbProviderInvariantName;

            if (!DbProviderFactories.GetProviderInvariantNames().Any(provider => provider.Equals(dbProviderInvariantName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ProviderInvariantNameNotRegisteredException(dbProviderInvariantName, typeof(DbProviderFactories).Name);
            }

            dbContext.Options.SetDbProviderInvariantName(dbProviderInvariantName);
            dbContext.Options.SetSqlQueryProviderInvariantName(sqlQueryProviderInvariantName);
        }
        public static void UpdateConnectionString(this SubSonicContext dbContext, Action<DbConnectionStringBuilder, SubSonicContextOptions> config)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            dbContext.GetConnectionString = (builder, options) =>
            {
                config(builder, options);

                return builder.ConnectionString;
            };
        }
    }
}
