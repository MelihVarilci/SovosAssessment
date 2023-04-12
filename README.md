**SovosAssessment**

SovosAssessment, .NET Core 7 ile yazılmış bir RESTful web API'dir. MySQL veritabanı kullanılarak yazılmıştır. Bu proje, ORM (Object Relational Mapping) olarak Entity Framework, Mail servisi için Google Gmail SMTP ayarları, Unit of Work Design Pattern ve Repository Design Pattern kullanmaktadır.

**Gereksinimler**

Bu projeyi çalıştırmak için aşağıdaki gereksinimleri karşılamalısınız:

- .NET Core 7
- MySQL veritabanı
- Google Gmail SMTP ayarları
- İnternet bağlantısı

**Kurulum**

1. Öncelikle, projeyi klonlayın veya indirin:

```
git clone <https://github.com/MelihVarilci/SovosAssessment.git>
```

1. **appsettings.json** dosyasını açın ve veritabanı bağlantı dizesini, Gmail SMTP ayarlarını ve hangfire tetiklenmesi neticesinde mail gönderilecek mail sabitini güncelleyin:

```
"ConnectionStrings": {
"DefaultConnection": "server=localhost;database=SovosAssessment;user=root;password=123456"
},
```

```
"EmailSettings": {
"From": "example@gmail.com",
"Host": "smtp.gmail.com",
`"Port": 587,
"UserName": "example@gmail.com",
"Password": "yourpassword"
}
```

```
"MailAddressToSend": "example@gmail.com",
```

1. Veritabanı oluşturmak için Default Project: “SovosAssessment.Infrastructure” katmanını ayarlayın ve aşağıdaki komutları çalıştırın:

```
dotnet ef migrations add InitialMigration

dotnet ef database update
```
ya da
```
add-migration “InitialMigration”

update-database
```

1. Projeyi başlatmak için “SovosAssessment.WebAPI" katmanını Set as Startup Project olarak ayarlayın ve çalıştırın. Veya, Visual Studio kullanmıyorsanız aşağıdaki kodu çalıştırın.

```
dotnet run
```

**Kullanım**

SovosAssessment, aşağıdaki özellikleri sunmaktadır:

- CRUD işlemleri için RESTful API
- Fatura oluşturma, güncelleme, silme ve listeleme
- Yeni fatura kaydı olduğunda e-posta bildirimi gönderme

API'yi test etmek için, bir REST istemcisi kullanabilirsiniz. Örneğin, [Postman](https://www.postman.com/) kullanabilirsiniz.

**Katkıda Bulunma**

Eğer projede bir hata bulursanız veya önerileriniz varsa, lütfen yeni bir konu açın veya bir pull isteği gönderin.

