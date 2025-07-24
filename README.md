Pokemon Web API
Pokemon Web API, bir Pokemon veritabanı yönetim sistemi ve yorum/inceleme platformudur. API, Pokemon'ları, kategorileri, incelemeleri ve sahipleri yönetmeye yönelik bir dizi endpoint sağlar. Bu API, web ve mobil uygulamalar gibi istemciler için backend hizmeti sunmaktadır.

Proje Özeti
Bu proje, ASP.NET Core kullanılarak geliştirilmiş bir RESTful Web API'dir. API, Pokemon, Category, Owner, Review gibi kaynakları yönetmek için gerekli tüm CRUD (Create, Read, Update, Delete) işlemlerini sağlar. API, kullanıcıların Pokemon incelemeleri yapmalarını, Pokemon'ları ve kategorileri yönetmelerini ve sahip bilgilerini tutmalarını sağlar.

Kullanılan Teknolojiler
ASP.NET Core 5.0+: Web API'yi oluşturmak için kullanılan modern bir framework.

Entity Framework Core: Veritabanı işlemleri için kullanılan ORM (Object Relational Mapper) aracı.

SQL Server: Veritabanı yönetim sistemi olarak kullanıldı.

Swagger: API'nin otomatik dokümantasyonu ve test edilmesi için kullanılan araç.

AutoMapper: Nesneler arasında veri transferi için kullanılan araç (DTO - Data Transfer Object ile model dönüşümü).

Proje Özellikleri
Pokemon Yönetimi:

Pokemon'lar eklenebilir, güncellenebilir, silinebilir ve listelenebilir.

Pokemon'lara ait kategoriler ve incelemelerle ilişkiler kurulabilir.

Kategori Yönetimi:

Pokemon'lar için kategoriler eklenebilir ve güncellenebilir.

Kategorilerle ilgili CRUD işlemleri sağlanır.

İnceleme Yönetimi:

Pokemon'lar için kullanıcılar tarafından yapılmış yorumlar ve değerlendirmeler eklenebilir.

İncelemelere dair başlık, yorum ve puan gibi veriler yönetilebilir.

Sahip Yönetimi:

Her Pokemon'un sahibi belirli bir ülke ve kullanıcıya bağlanabilir.

Sahip bilgileri yönetilebilir ve ilgili Pokemon'lar listelenebilir.

API Testi ve Dokümantasyonu:

Swagger kullanılarak API'nin kolayca test edilmesi sağlanmıştır. Swagger UI üzerinden API uç noktaları test edilebilir.

Kullanıcı Yönergeleri
API'nin Başlatılması:
Proje, ASP.NET Core üzerinde çalıştırılabilir. Başlangıç için uygulamayı Visual Studio veya .NET CLI üzerinden çalıştırabilirsiniz.

Veritabanı Yapılandırması:
Entity Framework Core kullanılarak veritabanı işlemleri gerçekleştirilir. Veritabanını güncellemek için dotnet ef database update komutunu kullanarak migrasyonları uygulayabilirsiniz.

API Testi:
API'yi test etmek için Swagger UI'yi kullanabilirsiniz. Swagger, tüm uç noktaları (endpoints) görsel olarak sunarak her birini test etmenizi sağlar.

Kullanılan Teknolojiler
ASP.NET Core: Web API'yi geliştirmek için kullanılan ana framework.

Entity Framework Core: Veritabanı işlemleri için ORM (Object Relational Mapping) aracı.

Swagger: API dokümantasyonu ve test için kullanıldı.

