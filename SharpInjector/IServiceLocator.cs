using System;

namespace SharpInjector {
    public interface IServiceLocator {
        TIntf Resolve<TIntf>() where TIntf : class;
        object Resolve(Type type);
        void Register<T, TClass>(bool overrideIfAlreadyRegistered = false)
            where T : class
            where TClass : T;
        void Register<T>(T instance, bool overrideIfAlreadyRegistered = false);
        void Register<T>(Type type, bool overrideIfAlreadyRegistered = false);
        void Register<T>(Func<object> functionToCreateObject, bool overrideIfAlreadyRegistered = false) where T : class;
        bool IsRegistered(Type typeKey);
    }
}