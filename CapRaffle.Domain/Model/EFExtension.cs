using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Data.Objects.DataClasses;
using System.Data;

namespace CapRaffle.Domain.Model
{
    public static class EFExtensions
    {
        public static void UpdateDetachedEntity<T>(this CapRaffleContext context, T entity, Func<T, int> GetIdDelegate) where T : EntityObject
        {
            var id = GetIdDelegate(entity);
            var ps = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            var entitySetName = context.DefaultContainerName + "." + ps.Pluralize(entity.GetType().Name);

            entity.EntityKey = context.CreateEntityKey(entitySetName, entity);

            var type = typeof(T);

            foreach (var keyMember in entity.EntityKey.EntityKeyValues)
            {
                var pInfo = type.GetProperty(keyMember.Key);
                pInfo.SetValue(entity, keyMember.Value, null);
            }


            context.Attach(entity);
            var ose = context.ObjectStateManager.GetObjectStateEntry(entity);

            ose.ChangeState(EntityState.Modified);

        }
    }
}
