# Identity — отложени открития от ревюто (2026-10-01)

Тези точки са отложени за по-късно. API точките от ревюто са изпълнени отделно.
Легенда: 🔴 бъг / липсваща функционалност · 🟡 архитектура / добра практика · 🟢 стил / четимост.

## ▶ Докъде сме стигнали (обновено 2026-10-08)

**Как работим:** една точка наведнъж. Claude прави промяната, build-ва, пуска тестовете, **stage-ва** само кода (никога `docs/`), дава commit message и **чака**. Commit-ва потребителят. За нетривиални промени първо има план и одобрение. Отметките `[x]` тук ги слага Claude след всяка точка.

**Последен commit:** `226162d fix(services): reactivate trainer profile and find it by UserId`. **Application и Infrastructure са приключени.** Миграциите при стартиране са отложени, а inbox-ът няма да се прави. Сега сме на Domain.

**Stage-нато, чака commit:** `User` реферира ролите по Id (`UserRole`), с празна миграция `UserRolesAsOwnedCollection`. Следва 🟡 domain events, заедно с точката „`AddUserRoles` с Trainer“.

**Commit 4, `POST /api/auth/resend-confirmation`: направен (`af52763`).**
- Тяло `{ email }`, винаги връща **204**. Domain метод `User.RegenerateEmailConfirmationToken()` е no-op за потвърден имейл. Публикува `EmailConfirmationResendRequestedEvent`, който се обработва от `SendEmailConfirmationConsumer`.
- **Rate limiting (решено):** вграденият ASP.NET Core rate limiter, fixed window по IP, 3 заявки на 15 минути, при превишаване връща 429. Политиката е `RateLimitPolicies.ResendEmailConfirmation` в `Identity.Api/Extensions`. Броячът е в паметта, така че всяка реплика брои отделно. Cooldown по имейл е отложен, виж точката за `TimeProvider` в Domain.

**После:** Infrastructure → Domain → Общо.

**Висящи неща извън кода:**
- Тестови данни от проверката на Commit 3 в локалната среда. Потребителят не е казал дали да се изтрият:
  - потребител `claude-test-1791124041@example.com` в локалната `Body4uHUB.Identity` (на `DESKTOP-7N41TO3\MSSQLSERVER01`);
  - 1 съобщение в опашката `SendEmailConfirmation_error` в локалния RabbitMQ.
- Azure: да се добави pipeline променлива `App-FrontendUrl`, иначе deploy-ът спира при стартиране (`ValidateOnStart`).
- Frontend:
  - страница `/confirm-email?token=...&email=...`, която вика `POST /api/auth/confirm-email` с `{ email, token }`;
  - бутон „Изпрати отново“ (след login → 403 или при изтекъл токен), който вика `POST /api/auth/resend-confirmation` с `{ email }` и при 429 показва „Опитай по-късно“;
  - Register вече връща `UserDto` **без** `accessToken`, затова след регистрация се показва „Провери имейла си“;
  - ако има interceptor за 401 → logout, да изключва `/api/auth/login`;
  - админ панелът вече **не** задава ролята Trainer преди `POST /users/trainers`, защото командата я задава сама. Ако потребителят вече е треньор, връща 409.

**Взети решения (важат занапред):**
- **Options pattern:** inject-ваните класове получават `IOptions<T>`, никога `IConfiguration`. Виж точката „Options pattern“ в Infrastructure.
- **Primary constructor + `private readonly _field`** за handler-и, service-и, repository-та и consumer-и. Виж 🟢 точката в Application.
- **Без user enumeration** в auth endpoint-ите: еднакъв статус и **еднакво тяло** на отговора. При confirm-email и липсващият потребител хвърля `InvalidUserException`, за да мине през `CustomExceptionHandler` като невалидния токен.
- **`User.ConfirmEmail(token)` не нулира токена.** Повторно извикване със същия токен е no-op, а с друг токен е 422. Иначе потвърдените акаунти се различаваха от непознатите.
- **Линкът за потвърждение** е `{App:FrontendUrl}/confirm-email?token=...&email=...` и сочи към frontend страница, не към Api-то.
- **`SendEmailConfirmationConsumer` е без inbox и така остава** (at-least-once; възможен е безвреден дубликат на имейла със същия токен). Inbox-ът не би го предотвратил, виж точката за inbox в Infrastructure.

## Application

- [x] 🔴 **Липсва потвърждаване на имейл.** `LoginCommandHandler` връща 403 при непотвърден имейл, но няма `ConfirmEmailCommand`/endpoint; линкът в `RegisterCommandHandler` сочи към несъществуващ `/Auth/ConfirmEmail`. `EmailConfirmationTokenExpiry` не се проверява никъде.
- [x] 🔴 **`Task.Run` в `RegisterCommandHandler`.** Захваща scoped `IEmailService` след края на заявката; изключенията вътре се губят (try/catch покрива само синхронната част). Решение: `UserRegisteredEvent` през `IEventBus` (outbox) + consumer, който праща имейла.
- [x] 🔴 **Login връща 404 при грешни креденшъли** (`Result.ResourceNotFound(InvalidCredentials)`) — трябва `Result.Unauthorized`.
- [x] 🔴 **`RegisterCommandValidator` не форматира `FirstNameLength`/`LastNameLength`** — клиентът вижда `{0}`/`{1}`. `EditUserCommandValidator` го прави правилно.
- [x] 🟡 **JWT authentication/authorization се регистрират в Application** (`AddJwtAuthentication`, `AddAuthorizationPolicies`) → да се преместят в Api. Да се махнат `Microsoft.AspNetCore.Authentication.JwtBearer` и `Swashbuckle` от `Application.csproj`. `IHttpContextAccessor` в `RegisterCommandHandler` → base URL от конфигурация (`App:FrontendUrl`).
- [x] 🟡 **Дублирано мапване `User` → `UserDto`** в `Login` и `Register` (в `Register` липсват `Roles`).
- [x] 🟡 **`GetAllRolesQuery` връща `IEnumerable<RoleDto>`** вместо `Result<...>`.
- [x] 🟡 **Мъртъв код:** проверката `string.IsNullOrWhiteSpace(newPasswordHash)` → `ResourceNotFound` в `ChangePassword`; `try/catch` около хеширането в `Register`.
- [x] 🟡 **N+1 в `AddUserRolesCommandHandler`** — `FindByIdsAsync(ids)` вместо заявка за всяка роля; излишен `.Must(roles => roles.Any())` във валидатора.
- [x] 🟡 **`IJwtTokenService.ValidateToken` не се ползва** — да се махне.
- [x] 🟢 **Primary constructors + `readonly` fields** за всички handler-и. **Решено:** primary constructor и `private readonly` field с `_` префикс, инициализиран от параметъра. Пазят се `readonly` и `_`, а отпада тялото на конструктора с присвояванията. В методите се ползва само `_field`; ако по невнимание се ползва параметърът, компилаторът дава CS9124. Образец: `ConfirmEmailCommandHandler`. Важи и за новия код (handler-и, service-и, repository-та, consumer-и), но не и за domain моделите.
  ```csharp
  internal sealed class XCommandHandler(
      IUserRepository userRepository,
      IUnitOfWork unitOfWork)
      : IRequestHandler<XCommand, Result>
  {
      private readonly IUserRepository _userRepository = userRepository;
      private readonly IUnitOfWork _unitOfWork = unitOfWork;
  ```
- [x] 🟢 **DTO-тата като records** (`UserDto`, `RoleDto`, `AuthResponseDto`) — консистентно с `GetAllUsersResponse`.
- [x] 🟢 **`EditUserCommand(Guid Id, ...)` → `UserId`**, както при останалите команди.
- [x] 🔴 **Липсва повторно изпращане на имейла за потвърждение.** Токенът изтича след 24 часа и после потребителят остава заключен (login → 403). → Commit 4, виж „Докъде сме стигнали“.

## Infrastructure

- [x] 🔴 **Несъвпадащи версии на MassTransit:** `MassTransit.EntityFrameworkCore` 8.4.1 срещу `MassTransit`/`MassTransit.RabbitMQ` 8.5.7. **Решено:** всичко на 8.4.1 (и в Services), защото `MassTransit.EntityFrameworkCore` 8.5.x изисква EF Core 9.
- [x] 🔴 **`ModifiedAt` не се обновява** при промяна само на owned `ContactInfo` или само на ролите. Филтърът да е по `IModifiableEntity`; за предпочитане `SaveChangesInterceptor`, който проверява и `entry.References`. **Решено:** `ModifiedAtInterceptor` в `Shared.Infrastructure/Persistence`, регистриран и в трите услуги (Content и Services имаха същия пропуск при `OwnsMany`). Промяната върви **нагоре**: всеки Added/Modified/Deleted owned entry или many-to-many join entry маркира своя owner. Така се хващат и премахнати owned елементи, които `CollectionEntry.IsModified` не вижда. Shadow skip navigation-ите се пропускат, затова при `user.Roles.Add(role)` не се маркира `Role`. **Ново поведение:** коментар/пост/ревю обновява `ModifiedAt` и на aggregate root-а (`Article`, `ForumTopic`, `TrainerProfile`).
- [x] 🔴 **`DbInitializer`:** NRE, ако `SeedData:UserRoles` липсва; fallback-ът `?? "SomeRandomPassword"` за admin паролата трябва да хвърля, а не да seed-ва с позната парола. **Решено:** `SeedDataSettings` (`Identity.Infrastructure/Settings`) с `[Required]` за всички полета и без fallback-и, регистриран в `AddInfrastructure`. Допълнително `.Validate(...)` отхвърля незаменени `#{...}#` token-и в email-а и паролата (липсваща pipeline променлива). `DbInitializer` взима `.Value` в полето, затова грешката идва при resolve, **преди** миграцията. Проверено чрез стартиране на Api-то.
- [x] 🟡 **Options pattern** (`JwtSettings`, `MassTransitSettings`, `SeedDataSettings`) с `ValidateDataAnnotations().ValidateOnStart()` вместо ръчно `IConfiguration`/`int.Parse` в `JwtTokenService`, `PasswordHasherService`, `DbInitializer`, `AddMassTransitWithRabbitMq`, `AddJwtAuthentication`. Образец: `AppSettings` (`App:FrontendUrl`) в `Identity.Application/Settings`.
  - **Правило:** класовете, които се inject-ват (handler-и, service-и, health check-ове), получават `IOptions<T>`, никога `IConfiguration`. Кодът за регистрация в composition root-а (`Add*` extension-ите) също чете през bound options, ако стойността трябва да се валидира. `GetConnectionString(...)` в регистрацията остава.
  - `DatabaseReadinessHealthCheck` остава с `GetConnectionString`, това е стандартният начин в .NET.
  - **Решено:**
    - `JwtSettings`, `MassTransitSettings` и `EmailSettings` са в `Shared.Infrastructure/Settings`, единственият Shared проект, до който стигат Identity/Services.Infrastructure и `Shared.Api`. `PasswordHashingSettings` (`Security:Hashing`, `WorkFactor` 10–31) е в `Identity.Infrastructure/Settings`.
    - Регистрацията е с `services.AddValidatedOptions<T>(configuration, T.SectionName)` (`Shared.Infrastructure/Extensions/OptionsExtensions.cs`).
    - `[NoPipelineToken]` (`Shared.Infrastructure/Settings`) отхвърля незаменени `#{...}#` token-и. Сложен е на всички полета с token в `appsettings.json`.
    - **Валидацията е само lazy** (при resolve или при старта), никога при регистрацията. Причината: `dotnet ef` build-ва host-а без `appsettings.Local.json`, т.е. с token-ите. JWT bearer се конфигурира с `AddOptions<JwtBearerOptions>().Configure<IOptions<JwtSettings>>`, а RabbitMQ взима settings-ите от `context` в `UsingRabbitMq`. Само outbox `QueryDelay`/`QueryMessageLimit` се четат при регистрацията с `Get<T>()` без валидация, но същата секция се валидира от `ValidateOnStart` преди bus-ът да тръгне.
    - `MassTransitSettings.Port` (default 5672) се ползва и от `cfg.Host`, и от health check-а. `EmailSettings.EnableSsl` вече се чете от конфигурацията (беше хардкоднат `true`).
    - Services също ползва `MassTransitSettings`, заради общия health check.
- [x] 🟡 **`EventBus`** логва „Event published“, а с `UseBusOutbox()` съобщението само се записва в outbox; `PublishAsync` няма `CancellationToken` (засяга `IEventBus` в Shared). **Решено:**
  - `IEventBus.PublishAsync(@event, cancellationToken)` е **без default** стойност, за да не може token-ът тихо да се изпусне;
  - логът е „Event {EventType} {EventId} added to the outbox“;
  - `try/catch` + `LogError` + `throw` е махнат, защото грешката се логваше втори път от `CustomExceptionHandler`. **Решение на потребителя: без компромиси тук, т.е. без log-and-rethrow.**
- [x] 🟡 **`UserConfiguration` max lengths** (100 за имената, 50 за ролята) не съвпадат с домейна (`MaxNameLength = 20`) → константи от `ModelConstants`. Индексът `IX_Members_Email` → `IX_Users_Email`. **Изисква миграция.** **Решено (`3622247`):**
  - `EF` конфигурациите ползват `MaxNameLength`, `MaxEmailLength`, `MaxPhoneNumberLength` и `MaxRoleNameLength` от `ModelConstants`;
  - `ContactInfo` и `Role` проверяват дължината и в домейна, за да не стигне твърде дълга стойност до `SaveChanges` (500);
  - миграцията е `AlignUserAndRoleMaxLengths`;
  - за ролята имената са с префикс `Role`, защото `User.cs` import-ва static и `UserConstants`, и `RoleConstants`.
- [x] 🟡 **Race condition при регистрация** — unique violation → `DbUpdateException` → 500; да се хваща и да връща 409. **Решено:**
  - `UnitOfWork` в Identity превежда `SqlException` 2601/2627 в `UniqueConstraintViolationException` (`Body4uHUB.Shared/Exceptions`, обикновен `Exception`, а не domain).
  - `RegisterCommandHandler` го хваща и връща `Result.Conflict(UserEmailExists)`, същото като при предварителната проверка.
  - Не е мапнато глобално в `CustomExceptionHandler`, защото само handler-ът знае какво означава нарушението.
  - Content и Services не са пипани, там още няма unique сценарий.
- [ ] ⏸ 🟡 **Миграции при стартиране** — за production с повече от една реплика да са отделна стъпка (migration bundle в Azure Pipelines). **Отложено (2026-10-12):**
  - Причина: никъде няма паралелни реплики. Container App е с `max-replicas 1`, а при IIS има по един сайт за среда и app pool-ът се спира преди deploy.
  - Кога да се върнем: когато `max-replicas > 1`.
  - Какво включва тогава: и трите услуги; connection string за pipeline-а, включително за Container App пътя; решение къде остава seed-ът от `DbInitializer`.
- [x] ✖ 🟡 **Inbox за consumer-ите в Identity** (по желание). **Няма да се прави (2026-10-12):**
  - Inbox записът се commit-ва в DB транзакцията след `Consume`, а SMTP не е част от нея. Затова срив между SMTP и commit пак дава дубликат.
  - Inbox-ът покрива само тесния прозорец между commit и ack.
  - Consumer-ът не пише в базата, а дубликатът носи същия токен, т.е. е безвреден.
  - Inbox си струва за consumer-и, които променят данни.
  - Ако някога се включи, трябва да се махне и `DisableInboxCleanupService()`, иначе `InboxState` расте без край.
  - Оригиналната бележка: `SendEmailConfirmationConsumer` е без inbox, затова при срив между SMTP и ack е възможен дубликат на имейла. Включва се с `AddConfigureEndpointsCallback(... UseEntityFrameworkOutbox<IdentityDbContext>(context))`. Същата идея като transactional inbox-а в CasinoBroker.
- [x] 🟢 Излишен `MigrationsAssembly(...)` в `AddDbContext`. Махнат е, защото по подразбиране това е assembly-то на `IdentityDbContext`, където са миграциите.
- [x] 🟢 Безсмислен `when (ex is not StackOverflowException)` в `PasswordHasherService`. Остава само `OutOfMemoryException`, защото `StackOverflowException` не може да се хване.
- [x] 🟢 `sealed` + primary constructors за Infrastructure класовете:
  - repository-тата, `UnitOfWork` и `IdentityDbContext` (той подава `options` директно на `DbContext(options)`, без поле);
  - EF конфигурациите и settings класовете.

## Domain

- [x] 🔴 **`DeleteTrainerCommand` не маха Trainer ролята** — липсва `user.RemoveRole(trainerRole)`. *(Да се потвърди дали е умишлено.)* **Не е умишлено.** Изтрит треньор запазваше `TrainerOrAdmin` достъпа в Content и Services, а endpoint за махане на роля няма. Решено заедно със следващата точка.
- [x] ❓ **Потокът при `CreateTrainer`** — `EnsureIsTrainer` хвърля, ако потребителят още няма ролята, т.е. първо `POST /users/{id}/roles`, после `POST /users/trainers`. Да се реши дали е умишлено. **Решено: ролята Trainer и треньорският акаунт вървят заедно.**
  - `User.AssignTrainerRole(role)` / `RevokeTrainerRole(role)` заменят `EnsureIsTrainer`. Правилото е инвариант на aggregate-а, затова е в `User`, а не в handler-ите.
  - `CreateTrainer` сам задава ролята. Ако потребителят вече е треньор, handler-ът проверява `user.IsTrainer` и връща **409** `UserAlreadyTrainer`, а domain методът хвърля при същото условие като защита.
  - `DeleteTrainer` маха ролята, а за потребител без нея връща 422 `UserNotInRole`.
- [ ] 🟡 **`POST /users/{id}/roles` още може да зададе Trainer без профил.** Да се реши при точката за domain events: `AddUserRoles` да отказва Trainer или ролите да минават само през trainer командите.
- [x] 🔴 **(Services) Не може да се стане треньор отново.** `TrainerAccountCreatedEventConsumer` само логва „already exists“ за деактивиран профил. Трябва да го активира отново. Services няма тестове. **Решено:**
  - Неактивен профил се активира отново с `TrainerProfile.Reactivate(bio, years)`. Запазва услугите, ревютата и рейтинга, а bio-то и годините опит идват от новото събитие.
  - Активен профил означава повторна доставка и само се логва.
  - **Открит и оправен бъг:** `TrainerAccountDeletedEventConsumer` търсеше с `GetByIdAsync(message.UserId)`, т.е. по Id на профила, затова деактивирането никога не е работило.
  - Сега и двата consumer-а ползват `GetByUserIdAsync`. `ExistsByUserId` е махнат.
- [x] 🟡 **`User` държи `Role` обекти, а `Role` е `AggregateRoot`** — в DDD aggregate-ите се реферират по Id. Варианти: `Role` да не е aggregate root или `User` да пази `UserRole(RoleId)`. **Решено: `User` пази `UserRole(RoleId)`, а `Role` остава aggregate root**, защото има собствен живот (seed, `GetAllRoles`, сочат я много потребители).
  - `UserRole : ValueObject` (`Identity.Domain/ValueObjects`) е мапнат с `OwnsMany` към същата таблица `UserRoles`. Миграцията `UserRolesAsOwnedCollection` е празна, промени се само snapshot-ът.
  - `AddRole`/`RemoveRole`/`AssignTrainerRole`/`RevokeTrainerRole` още приемат `Role`, т.е. подаването като аргумент е наред, но пазят само `Id`. Така остава и проверката на името за Trainer.
  - `IsTrainer` е заменен с `HasRole(Guid roleId)`, защото `User` вече не знае имената на ролите.
  - Login зарежда ролите с `FindByIdsAsync` за JWT-то и DTO-то (една заявка повече). `ToDto(user, roles)` приема ролите отвън.
- [ ] 🟡 **Domain events не се ползват** — `user.AssignTrainerRole()` / `RevokeTrainerRole()` да вдигат domain event, dispatcher в `SaveChanges` → integration event през outbox.
- [x] 🟡 **Токенът за потвърждение на имейл** се генерира в handler-а; целият цикъл да е в `User` (`ConfirmEmail(token)` да проверява токена и срока).
- [ ] 🟡 **Нормализиране на имейл** (`Trim().ToLowerInvariant()`) в `ContactInfo.Create`.
- [ ] 🟢 **`ModelConstants`:** класовете да са `static`; `LastNameLength` е хардкоднат („2 and 20“); няма константа за 200 при `EmailLength`.
- [ ] 🟢 **`InvalidCredentialsException` не се ползва.**
- [ ] 🟢 **Изтичането на токена не е тествано**, защото `User` ползва `DateTime.UtcNow`. Вариант: `TimeProvider` (.NET 8), подаван към методите, които зависят от времето. След това може да се добави cooldown по имейл в `RegenerateEmailConfirmationToken`, например без нов токен, ако текущият е издаден преди по-малко от 2 минути. Тогава ограничението ще пази и пощата на жертвата, а не само ще брои по IP.
- [ ] 🟢 **EF конструкторите** `private User() : base(Guid.NewGuid())` → `: base()`.
- [ ] 🟢 **File-scoped namespaces**; уеднаквяване на позицията на `using` (`EditUserCommandValidator`, `ContactInfo`, `InvalidContactInfoException` ги имат вътре в namespace-а).

## Общо за solution-а

- [ ] 🟡 **`Directory.Build.props` + `Directory.Packages.props`** (central package management) — общи `TargetFramework`/`Nullable`/`ImplicitUsings` и еднакви версии на пакетите.
- [ ] 🟡 **Включване на `Nullable`** — голяма промяна, по проект.
- [ ] 🟢 **Application тестовете** покриват само `Register` и `ConfirmEmail`.
- [ ] 🟢 **`SomethingWentWrong`** в `Shared.Domain/Constants/ModelConstants.cs` вече не се ползва никъде.
- [ ] 🟡 **`AdminOnly` има различни роли в различните услуги:** Identity приема `"Administrator", "Admin"`, Content и Services приемат само `"Administrator"`; `IsAdmin()` в `Shared.Api` проверява и двете. Имената на ролите (`Administrator`, `Admin`, `Trainer`) са литерали, разпръснати из кода, и да се изнесат като константи до `AuthorizationPolicies`.
- [ ] 🟡 **Content и Services регистрират JWT в Application** (`AddJwtAuthentication` с `IConfiguration`), както беше в Identity. Да се изнесе общ `AddJwtAuthentication` в `Shared.Api` (с `JwtSettings` options), който да ползват и трите Api-та. `JwtSettings` вече е в `Shared.Infrastructure/Settings`. Образец за lazy конфигурацията е `AddJwtAuthentication` в `Identity.Api/Extensions/ServiceCollectionExtensions.cs`.
- [ ] 🔴 **Неуспешен старт връща exit code 0.** `catch` в `Program.cs` на трите Api-та само логва `Log.Fatal`, затова IIS/pipeline-ът не разбира, че процесът е паднал. Поправка: `Environment.ExitCode = 1;` в `catch`. Открито при проверката на `SeedDataSettings`.
- [ ] 🟡 **Content и Services имат същия `#if DEBUG` проблем** с DataProtection и CORS, който беше оправен в Identity.
