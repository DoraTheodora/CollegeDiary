using Autofac;
using CD.Helper;

namespace CD.Droid
{
	public class DroidModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<FirebaseAuthenticator>().As<IFirebaseAuthenticator>().SingleInstance();
			builder.RegisterType<FirebaseRegister>().As<IFirebaseRegister>().SingleInstance();
		}
	}
}