using Xunit;

namespace SharpInjector.Tests
{
    public class ServiceLocatorTests {

        public ServiceLocatorTests() {
            ServiceLocator.Default = new ServiceLocator();
        }

        [Fact]
        public void Can_register_function() {
            ServiceLocator.Default.Register<IFoo>(() => new Foo());
            Assert.IsAssignableFrom<Foo>(ServiceLocator.Default.Resolve<IFoo>());
        }

        [Fact]
        public void Can_register_singleton() {
            ServiceLocator.Default.Register<IFoo>(new Foo());
            Assert.IsAssignableFrom<Foo>(ServiceLocator.Default.Resolve<IFoo>());
        }

        [Fact]
        public void Can_resolve_concrete_class() {
            Assert.IsAssignableFrom<Foo>(ServiceLocator.Default.Resolve<Foo>());
        }

        [Fact]
        public void Can_resolve_concrete_class_with_dependencies() {
            ServiceLocator.Default.Register<IFoo>(new Foo());
            Assert.IsAssignableFrom<Bar>(ServiceLocator.Default.Resolve<Bar>());
        }

        public interface IFoo {

        }

        public class Foo : IFoo {

        }

        public class Bar {
            public Bar(IFoo foo) {

            }
        }
    }
}
