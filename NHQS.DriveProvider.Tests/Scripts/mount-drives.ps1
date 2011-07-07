ls orders*.dll,people*.dll | convert-path | import-module;
import-module ./NHibernate.DriveProvider.dll;

new-psdrive -psp NHQS -root '' -name Orders -domain Orders.Domain.Order -config ./nhqs.driveprovider.tests.dll.config
new-psdrive -psp NHQS -root '' -name People -domain People.Domain.Person -config ./nhqs.driveprovider.tests.dll.config


