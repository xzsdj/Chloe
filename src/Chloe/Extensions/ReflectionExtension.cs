﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.InternalExtensions
{
    public static class ReflectionExtension
    {
        public static Type GetMemberType(this MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).PropertyType;
            if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).FieldType;

            throw new ArgumentException();
        }

        public static void SetMemberValue(this MemberInfo propertyOrField, object obj, object value)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                ((PropertyInfo)propertyOrField).SetValue(obj, value, null);
            else if (propertyOrField.MemberType == MemberTypes.Field)
                ((FieldInfo)propertyOrField).SetValue(obj, value);
            else
                throw new ArgumentException();
        }
        public static object GetMemberValue(this MemberInfo propertyOrField, object obj)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).GetValue(obj, null);
            else if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).GetValue(obj);

            throw new ArgumentException();
        }

        public static MemberInfo AsReflectedMemberOf(this MemberInfo propertyOrField, Type type)
        {
            if (propertyOrField.ReflectedType != type)
            {
                MemberInfo tempMember = null;
                if (propertyOrField.MemberType == MemberTypes.Property)
                {
                    tempMember = type.GetProperty(propertyOrField.Name);
                }
                else if (propertyOrField.MemberType == MemberTypes.Field)
                {
                    tempMember = type.GetField(propertyOrField.Name);
                }

                if (tempMember != null)
                    propertyOrField = tempMember;
            }

            return propertyOrField;
        }
        public static bool IsNullable(this Type type)
        {
            Type underlyingType;
            return IsNullable(type, out underlyingType);
        }
        public static bool IsNullable(this Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }
        public static Type GetUnderlyingType(this Type type)
        {
            Type underlyingType;
            if (!IsNullable(type, out underlyingType))
                underlyingType = type;

            return underlyingType;
        }
        public static bool IsAnonymousType(this Type type)
        {
            string typeName = type.Name;
            return typeName.Contains("<>") && typeName.Contains("__") && typeName.Contains("AnonymousType");
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }

#if NETFX
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo member)
        {
            return member.GetCustomAttributes<Attribute>();
        }
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member)
        {
            return member.GetCustomAttributes<TAttribute>().FirstOrDefault();
        }
        public static bool IsDefined(this MemberInfo member, Type attributeType)
        {
            return member.IsDefined(attributeType, false);
        }
#endif
    }
}
