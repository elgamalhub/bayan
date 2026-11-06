# Bayan · بيان

> Read. Think. Share.

Bayan is a bilingual (English / Arabic) publishing platform built with ASP.NET Core MVC. It gives writers a clean, distraction-free editor and gives readers a calm place to discover articles, follow topics, and join the discussion.

---

## Features

- **Articles** — create, edit, delete, search, and browse by category with pagination.
- **Built-in editor** — a lightweight `contenteditable` rich-text editor with headings, bold/italic, lists, quotes, links, and inline image uploads. No external editor service or API key required.
- **Comments** — signed-in readers can post, edit, and delete their own comments.
- **Likes** — toggle likes without reloading the page.
- **Profiles** — public author pages, avatars, bios, and social links.
- **Roles** — `Admin` and `User`, with owners and administrators able to manage articles.
- **Bilingual UI** — full English and Arabic interface with automatic RTL layout, direction-aware Bootstrap assets, and Arabic date formatting.
- **Dark mode** — persistent light/dark theme stored in `localStorage`.
- **Responsive design** — desktop, tablet, and mobile layouts.
- **Local assets only** — Bootstrap, jQuery, and jQuery Validation are served from `wwwroot/lib`; the application does not depend on a CDN.

## Tech stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core MVC (`net8.0`) |
| Data access | Entity Framework Core 9 · SQL Server |
| Authentication | ASP.NET Core Identity |
| Front end | Razor, Bootstrap 5 (local), vanilla JavaScript |
| Pagination | X.PagedList |
| Localization | Cookie-based request culture + in-app string catalogue |

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer
- SQL Server (LocalDB, Express, or a full instance)
- `dotnet-ef` 9.x for applying migrations:

```powershell
dotnet tool install --global dotnet-ef --version 9.*
```

## Getting started

1. **Clone the repository**

   ```powershell
   git clone https://github.com/<your-account>/<repository>.git
   cd <repository>
   ```

2. **Configure the database**

   Edit `appsettings.json` and set the `Default` connection string:

   ```json
   {
     "ConnectionStrings": {
       "Default": "Server=YOUR_SERVER;Database=StudyWithChatgpt;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
     }
   }
   ```

3. **Restore and apply migrations**

   ```powershell
   dotnet restore
   dotnet ef database update
   ```

   In the Development environment the application also applies pending migrations automatically at startup.

4. **Run**

   ```powershell
   dotnet run
   ```

   The default development URL is <http://localhost:5197>.

## Creating an administrator

Registration creates a normal `User` account. To promote an account, run the following against the application database (replace the username):

```sql
USE StudyWithChatgpt;

DECLARE @userId nvarchar(450) =
    (SELECT Id FROM AspNetUsers WHERE UserName = 'your-username');
DECLARE @roleId nvarchar(450) =
    (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');

IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles
               WHERE UserId = @userId AND RoleId = @roleId)
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@userId, @roleId);
```

Sign out and back in after changing the role.

## Project structure

```text
Controllers/          MVC controllers + JSON API endpoints
  Api/                Image upload and like endpoints
Models/               Entities and AppDbContext
ViewModels/           Form and page view models
Services/             Localization, validation attributes, image handling
Views/                Razor views, layouts, and shared partials
wwwroot/
  css/site.css        Design system, themes, responsive rules
  js/site.js          Navigation, dialogs, likes, favourites, toasts
  js/editor.js        Built-in rich-text editor
  lib/                Local Bootstrap, jQuery, and validation libraries
Migrations/           EF Core migrations
```

## Application endpoints

### Pages

| Route | Description |
| --- | --- |
| `/` | Home page |
| `/BlogPost` | Article index, search, and category filtering |
| `/BlogPost/Details/{id}` | Article page with comments |
| `/BlogPost/Create`, `/BlogPost/Edit/{id}` | Built-in editor |
| `/BlogPost/MyPosts` | Current user's articles |
| `/User/Login`, `/User/Signup` | Authentication |
| `/User/PublicProfile?userName={username}` | Public author profile |
| `/Home/Privacy` | Privacy policy |

### JSON API

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/uploads/image` | Upload an editor image (authenticated, max 5 MB) |
| `POST` | `/api/posts/{id}/like` | Toggle a like (authenticated) |

Both endpoints require an authenticated cookie session and a valid antiforgery token.

## Localization

The interface is available in English (`en`) and Arabic (`ar`). The selected culture is stored in the standard ASP.NET Core culture cookie and drives:

- UI copy from `Services/AppLocalizer.cs`
- `dir="rtl"` and the RTL Bootstrap stylesheet for Arabic
- Arabic-aware date formatting

Switch languages with the `EN / ع` control in the header, or directly:

```text
/language/set?culture=ar&returnUrl=/
```

## Editor notes

The editor is implemented in `wwwroot/js/editor.js` and posts an HTML fragment to the normal article form. Images are uploaded to `wwwroot/PostImages`; user avatars go to `wwwroot/UserImages`. Allowed image types and size limits are validated by `Services/ImageService.cs`.

## Database notes

- Existing data is preserved by the migrations; no destructive reset is required.
- Unique indexes prevent duplicate likes and duplicate category names.
- Comments optionally reference a user account; legacy comments without an account are still displayed.
- Deleting an account does not cascade-delete comments, avoiding SQL Server's multiple-cascade-path restriction.

## Security notes

- Passwords are hashed by ASP.NET Core Identity.
- All state-changing requests use antiforgery validation.
- Uploaded files are checked by extension, content type, and size.
- Use a dedicated SQL login and a strong administrator password in production, and keep connection strings out of public repositories.
