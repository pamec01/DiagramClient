using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DiagramClient
{
    public class ReflectionTools
    {
        public string AnalyzeAssembly(Type assemblyType)
        {
            Assembly assembly = assemblyType.Assembly;
            StringBuilder umlBuilder = new StringBuilder();
            StringBuilder redundantBuilder = new StringBuilder();

            HashSet<string> processedRelationships = new HashSet<string>();
            List<string> inheritedRelationships = new List<string>();
            List<string> redundantRelationships = new List<string>();

            foreach (Type type in assembly.GetTypes())
            {
                // Ignorování anonymních tříd a systémových typů
                if (type.IsGenericType || type.Name.Contains("<"))
                    continue;

                string typeKind = GetTypeKind(type);
                umlBuilder.AppendLine($"[{typeKind}{type.Name}");

                // Přidání atributů
                var properties = type.GetProperties();
                if (properties.Length > 0)
                {
                    umlBuilder.AppendLine("|"); // oddělovač pro vlastnosti
                    foreach (var prop in properties)
                    {
                        umlBuilder.AppendLine($"{prop.Name}: {prop.PropertyType.Name}");
                    }
                }
                // Přidání metod
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methods.Length > 0)
                {
                    umlBuilder.AppendLine("|"); // oddělovač pro metody
                    foreach (var method in methods)
                    {
                        umlBuilder.AppendLine($"{method.Name}()");
                    }
                }

                umlBuilder.AppendLine("]"); // konec definice třídy

                // Přidání základních vztahů (dědičnost)
                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    string baseRelationship = $"[{type.BaseType.Name}]<:-[{type.Name}]";
                    umlBuilder.AppendLine(baseRelationship);
                    processedRelationships.Add(baseRelationship);
                    inheritedRelationships.Add(baseRelationship);
                }

                foreach (var interfaceType in type.GetInterfaces())
                {
                    string interfaceRelationship = $"[{interfaceType.Name}]<:-[{type.Name}]";
                    umlBuilder.AppendLine(interfaceRelationship);
                    processedRelationships.Add(interfaceRelationship);
                    inheritedRelationships.Add(interfaceRelationship);
                }

                // Získání vztahů mezi třídami a jejich vlastnostmi (asociace)
                foreach (var property in type.GetProperties())
                {
                    var propertyType = property.PropertyType;
                    Type relatedType = null;

                    if (propertyType.IsArray)
                    {
                        relatedType = propertyType.GetElementType();
                    }
                    else if (propertyType.IsGenericType && propertyType.GetInterfaces().Any(x => x.Name.Contains("IEnumerable")))
                    {
                        relatedType = propertyType.GetGenericArguments().First();
                    }
                    else if (!propertyType.IsPrimitive && propertyType.Namespace == type.Namespace)
                    {
                        relatedType = propertyType;
                    }

                    if (relatedType != null && relatedType.Namespace == type.Namespace)
                    {
                        string relationship = $"[{type.Name}]->[{relatedType.Name}]";

                        if (!processedRelationships.Contains(relationship))
                        {
                            // Zkontrolujeme, jestli není vazba zděděná
                            if (IsRelationshipInherited(type, relatedType))
                            {
                                redundantRelationships.Add(relationship); // Přidání do redundantních vazeb
                            }
                            else
                            {
                                umlBuilder.AppendLine(relationship);
                                processedRelationships.Add(relationship);
                            }
                        }
                    }
                }
            }

            // Připojení zděděných a redundantních vazeb na konec výstupu
            //umlBuilder.AppendLine("\n// Inherited Relationships:");
            foreach (var inheritedRelationship in inheritedRelationships)
            {
                //umlBuilder.AppendLine(inheritedRelationship);
            }

            //umlBuilder.AppendLine("\n// Redundant Relationships:");
            foreach (var redundantRelationship in redundantRelationships)
            {
                Console.Write("redundant: " + redundantRelationship);  
                //redundantBuilder.AppendLine(redundantRelationship);
            }

            return umlBuilder.ToString(); // Odstranění \r pro správný formát
        }

        private static string GetTypeKind(Type type)
        {
            if (type.IsInterface)
            {
                return "<interface>";
            }
            else if (type.IsAbstract && type.IsClass)
            {
                return "<abstract>";
            }
            else if (type.IsEnum)
            {
                return "<enum>";
            }
            else
            {
                return "<class>";
            }
        }

        // Kontrola, jestli je vztah zděděný (např. RemoteStudent -> Address, pokud dědí přes Student)
        private bool IsRelationshipInherited(Type derivedType, Type targetType)
        {
            Type current = derivedType.BaseType;
            while (current != null && current != typeof(object))
            {
                // Kontrola vlastností rodičů pro vztahy
                foreach (var property in current.GetProperties())
                {
                    var propertyType = property.PropertyType;
                    Type propertyRelatedType = null;

                    if (propertyType.IsArray)
                    {
                        propertyRelatedType = propertyType.GetElementType();
                    }
                    else if (propertyType.IsGenericType && propertyType.GetInterfaces().Any(x => x.Name.Contains("IEnumerable")))
                    {
                        propertyRelatedType = propertyType.GetGenericArguments().First();
                    }
                    else if (!propertyType.IsPrimitive && propertyType.Namespace == targetType.Namespace)
                    {
                        propertyRelatedType = propertyType;
                    }

                    if (propertyRelatedType == targetType)
                    {
                        return true;
                    }
                }
                current = current.BaseType;
            }
            return false;
        }
    }
}
