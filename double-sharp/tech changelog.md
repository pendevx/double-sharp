# Tech Changelog

## 2024-09-15

### Changes

Moving the backend server off of Docker due to FILESTREAM reasons:

After recently moving off of the Windows file system and into SQL Server to store audio files, using the FILESTREAM technology with the SqlFileStream API to access those files, even though the database is running directly on a Windows OS, but the backend, instantiating the SqlFileStream objects, is running in a Linux container, where SqlFileStream is not supported on Linux, same as FILESTREAM isn't supported on Linux.

The frontend server and the database schema update tool may continue using Docker.

Considerations:

1. Use IIS to host the backend on the windows server?
2. Set up another CI/CD system for the backend server?
3. Should the db schema update tool still run on Docker?

### Tech stack:

-   Frontend:

    -   Main framework/library: **React**
    -   CSS: **TailwindCSS**
    -   Bundler: **Vite**

-   Backend:

    -   Framework: **ASP.NET Core**
    -   Logging: **Seq**
    -   ORM: **Entity Framework Core**

-   Database:

    -   **MS SQL Server** (with FILESTREAM)
    -   Schema update tool

-   Deployment:
    -   Webapp: **nginx** to forward /api requests to backend server
    -   Backend: TBC (IIS?)

## 2024-09-14

### Tech stack:

-   Frontend:

    -   Main framework/library: **React**
    -   CSS: **TailwindCSS**
    -   Bundler: **Vite**

-   Backend:

    -   Framework: **ASP.NET Core**
    -   Logging: **Seq**
    -   ORM: **Entity Framework Core**

-   Database:

    -   **MS SQL Server** (with FILESTREAM)
    -   Schema update tool

-   Deployment:
    -   Webapp: **nginx** to forward /api requests to backend server
    -   Backend: **Docker** with compose stack
