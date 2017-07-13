using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpInjector {
    public class ServiceLocator : IServiceLocator {
        private Dictionary<Type, Type> _typeRegistrations;
        private Dictionary<Type, object> _instanceRegistrations;
        private Dictionary<Type, Func<object>> _delegateRegistrations;

        private static IServiceLocator _instance;

        public static IServiceLocator Default {
            get => _instance ?? (_instance = new ServiceLocator());
            set => _instance = value;
        }

        public ServiceLocator() {
            _typeRegistrations = new Dictionary<Type, Type>();
            _instanceRegistrations = new Dictionary<Type, object>();
            _delegateRegistrations = new Dictionary<Type, Func<object>>();
        }

        public void Register<T>(Type type, bool overrideIfAlreadyRegistered = false) {
            EnsureRegistrationRule(typeof(T), overrideIfAlreadyRegistered);
            _typeRegistrations.Add(typeof(T), type);
        }

        private void EnsureRegistrationRule(Type typeKey, bool forceRegister) {
            if (!IsRegistered(typeKey)) {
                return;
            }
            if (forceRegister) {
                Unregister(typeKey);
                return;
            }
            throw new DuplicateRegistrationException(typeKey, "Only one registration per type is allowed");
        }

        private void Unregister(Type type) {
            _instanceRegistrations.Remove(type);
            _typeRegistrations.Remove(type);
            _delegateRegistrations.Remove(type);
        }

        public void Register<T, TClass>(bool overrideIfAlreadyRegistered)
            where T : class
            where TClass : T {
            Register<T>(typeof(TClass), overrideIfAlreadyRegistered);
        }

        public void Register<T>(T instance, bool overrideIfAlreadyRegistered = false) {
            EnsureRegistrationRule(typeof(T), overrideIfAlreadyRegistered);
            _instanceRegistrations.Add(typeof(T), instance);
        }

        public void Register<T>(Func<object> functionToCreateObject, bool overrideIfAlreadyRegistered = false) where T : class {
            EnsureRegistrationRule(typeof(T), overrideIfAlreadyRegistered);
            _delegateRegistrations.Add(typeof(T), functionToCreateObject);
        }

        public TIntf Resolve<TIntf>() where TIntf : class {
            return Resolve(typeof(TIntf)) as TIntf;
        }

        public object Resolve(Type type) {
            if (_delegateRegistrations.ContainsKey(type)) {
                return _delegateRegistrations[type].Invoke();
            }
            if (!_typeRegistrations.ContainsKey(type)) {
                if (_instanceRegistrations.ContainsKey(type)) {
                    return _instanceRegistrations[type];
                }
                EnsureConcreteClass(type);
                _typeRegistrations.Add(type, type);
            }
            var createdType = _typeRegistrations[type];

            ConstructorInfo mostSpecificConstructor = GetMostSpecificConstructor(createdType);

            var constructorParameters = new List<object>();
            foreach (var a in mostSpecificConstructor.GetParameters()) {
                constructorParameters.Add(Resolve(a.ParameterType));
            }
            return Activator.CreateInstance(createdType, constructorParameters.ToArray());
        }

        private void EnsureConcreteClass(Type type) {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsAbstract || typeInfo.IsInterface) {
                throw new NotSupportedException($"Cannot find registration for type {type.FullName}.");
            }
        }

        private static ConstructorInfo GetMostSpecificConstructor(Type type) {
            var constructors = type.GetTypeInfo().DeclaredConstructors;
            ConstructorInfo mostSpecificConstructor = null;
            foreach (var c in constructors) {
                if (mostSpecificConstructor == null ||
                    mostSpecificConstructor.GetParameters().Length < c.GetParameters().Length) {
                    mostSpecificConstructor = c;
                }
            }
            return mostSpecificConstructor;
        }

        public bool IsRegistered(Type typeKey) {
            return _instanceRegistrations.ContainsKey(typeKey) || 
                   _typeRegistrations.ContainsKey(typeKey) || 
                   _delegateRegistrations.ContainsKey(typeKey);
        }
    }
}