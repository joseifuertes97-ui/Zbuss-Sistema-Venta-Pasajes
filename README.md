# Zbuss - Sistema web de venta de pasajes

Proyecto web académico orientado a la gestión y venta de pasajes de bus. La solución incluye una aplicación web en ASP.NET Core MVC, una Web API y una base de datos SQL Server.

El sistema permite administrar buses, asientos, viajes, pasajeros, usuarios y roles, además de registrar ventas y reservas de pasajes.

## Funcionalidades principales

- Registro e inicio de sesión de usuarios.
- Autenticación mediante cookies.
- Gestión de usuarios y roles.
- Administración de buses y sus asientos.
- Registro y mantenimiento de viajes.
- Búsqueda y filtrado de viajes.
- Registro de pasajeros.
- Reserva y venta de pasajes.
- Consulta de reservas y ventas canceladas.
- Cálculo de subtotal, IGV y total de la venta.
- Envío de comprobante por correo electrónico.
- Web API para consultar y administrar información del sistema.

## Tecnologías utilizadas

- C#
- ASP.NET Core MVC 3.1
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Razor Views
- Bootstrap
- jQuery
- HTML y CSS

## Estructura del proyecto

```text
Zbuss-GitHub/
├── Zbuss_Proyect/             # Aplicación web ASP.NET Core MVC
│   ├── Controllers/
│   ├── Dao/
│   ├── Models/
│   ├── ViewModel/
│   ├── Views/
│   └── wwwroot/
├── Zbuss_ProyectWebApi/       # API REST del proyecto
│   ├── Controllers/
│   ├── Models/
│   └── ViewModels/
├── database/                  # Scripts de SQL Server
├── Zbuss_Proyect.sln          # Solución de Visual Studio
└── README.md
```

## Base de datos

El proyecto trabaja con la base de datos:

```text
bd_VENTAS_ZBUSS1
```

Entre las tablas principales se encuentran:

- `tb_ASIENTOS_BUS`
- `tb_BUS`
- `tb_DETALLE_VENTA`
- `tb_DETALLE_VIAJE`
- `tb_HORAS_VIAJE`
- `tb_PASAJERO`
- `tb_ROLES`
- `tb_USUARIOS`

Los scripts necesarios se encuentran en la carpeta `database`.

## Configuración de la conexión

Por seguridad, las credenciales originales de SQL Server fueron retiradas de esta versión para GitHub.

Los proyectos incluyen una conexión local de ejemplo en `appsettings.json`:

```json
"ConnectionStrings": {
  "Zbuss_ProyectDBConn": "Server=localhost;Database=bd_VENTAS_ZBUSS1;Trusted_Connection=True;"
}
```

Cambia esa cadena según el nombre de tu servidor y el método de autenticación que utilices en SQL Server.

## Configuración del correo

El proyecto contiene una función para enviar el comprobante de compra mediante SMTP de Gmail. La contraseña de aplicación original fue retirada antes de publicar el código.

En `Zbuss_Proyect/Controllers/PagoController.cs` encontrarás:

```csharp
const string fromPassword = "CONFIGURA_TU_APP_PASSWORD";
```

Para probar el envío de correos debes usar tus propias credenciales de prueba. No publiques contraseñas ni claves reales en GitHub.

## Cómo ejecutar el proyecto

1. Clona el repositorio.
2. Abre `Zbuss_Proyect.sln` en Visual Studio.
3. Restaura los paquetes NuGet.
4. Crea la base de datos ejecutando `database/script_database_zbuss.sql` en SQL Server Management Studio.
5. Revisa la cadena `Zbuss_ProyectDBConn` de ambos archivos `appsettings.json`.
6. Establece `Zbuss_Proyect` como proyecto de inicio para ejecutar la aplicación web.
7. Si deseas probar la API, ejecuta también `Zbuss_ProyectWebApi`.

También puedes restaurar las dependencias desde terminal:

```bash
dotnet restore
```

## Nota sobre la versión de .NET

El proyecto fue desarrollado con **.NET Core 3.1**. Si vas a ejecutarlo tal como fue creado, necesitarás un entorno compatible. Para un proyecto nuevo o una actualización futura, es recomendable migrarlo a una versión moderna de .NET.

## Objetivo del proyecto

Este proyecto fue realizado con fines académicos para aplicar conocimientos de desarrollo web con C#, patrón MVC, Entity Framework, SQL Server, APIs REST y operaciones CRUD en un sistema de venta de pasajes.

## Repositorio

Versión preparada para portafolio y GitHub, eliminando archivos generados por Visual Studio y credenciales locales que no deben publicarse.
