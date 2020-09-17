using CD.Helper;
using Autofac;

namespace CD.iOS
{
	public class IOSModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<FirebaseAuthenticator>().As<IFirebaseAuthenticator>().SingleInstance();
		}
	}
}