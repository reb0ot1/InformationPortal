using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CovidInformationPortal.Services.Utilities
{
    public class PropertyHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyHelper[]> Cache
            = new ConcurrentDictionary<Type, PropertyHelper[]>();

        private static MethodInfo CallInnerDelegateMethod
            = typeof(PropertyHelper).GetMethod(nameof(CreateDelegate), BindingFlags.NonPublic | BindingFlags.Static);

        private static MethodInfo CallInnerDelegateMethod2
            = typeof(PropertyHelper).GetMethod(nameof(CreateSetDelegate), BindingFlags.NonPublic | BindingFlags.Static);

        public string Name { get; set; }

        public Type PropertyType { get; set; }

        public Func<object, object> GetValue { get; set; }

        public Action<object, object> SetValue { get; set; }

        public static PropertyHelper[] GetProperties(Type objectType)
         => Cache.GetOrAdd(objectType, _ => objectType
           .GetProperties()
           .Select(s =>
           {
               var getMethod = s.GetMethod;
               var setMethod = s.SetMethod;
               var declaringClass = s.DeclaringType;
               var typeOfResult = s.PropertyType;

               var getMethodDelegateType = typeof(Func<,>).MakeGenericType(declaringClass, typeOfResult);
               var setMethodDelegateType = typeof(Action<,>).MakeGenericType(declaringClass, typeOfResult);
               var getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);
               var setMethodDelegate = setMethod.CreateDelegate(setMethodDelegateType);

               var callInnerGenericMethod = CallInnerDelegateMethod.MakeGenericMethod(declaringClass, typeOfResult);
               var callInnerGenericMethod2 = CallInnerDelegateMethod2.MakeGenericMethod(declaringClass, typeOfResult);

               Func<object, object> result = (Func<object, object>)callInnerGenericMethod.Invoke(null, new[] { getMethodDelegate });
               Action<object, object> result2 = (Action<object, object>)callInnerGenericMethod2.Invoke(null, new[] { setMethodDelegate });

               return new PropertyHelper
               {
                   Name = s.Name,
                   GetValue = result,
                   PropertyType = typeOfResult,
                   SetValue = result2
               };
           })
           .ToArray()
            );

        private static Func<object, object> CreateDelegate<TSource, TValue>(Func<TSource, TValue> deleg)
            => instance => deleg((TSource)instance);

        private static Action<object, object> CreateSetDelegate<TSourse, TValue>(Action<TSourse, TValue> deleg)
            => (instance, value) => deleg((TSourse)instance, (TValue)value);
    }
}
