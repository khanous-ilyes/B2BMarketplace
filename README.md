<div align="center">
  <h1>🚀 B2B Marketplace Platform</h1>
  <p><strong>A Modern, Multi-Tenant B2B E-Commerce & Supplier Portal</strong></p>

  [![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
  [![React](https://img.shields.io/badge/React-18-61DAFB?style=for-the-badge&logo=react&logoColor=black)](https://reactjs.org/)
  [![PostgreSQL](https://img.shields.io/badge/PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
  [![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
</div>

<br/>

> [!WARNING]
> **Source Code Status:**
> To protect proprietary business logic, the repository implementations have been removed from this public showcase. The architecture, entities, interfaces, and controllers remain visible to demonstrate code quality and structure. **The full source code with enterprise logic is available for purchase/licensing.**

---

## 🌟 Key Features

### 🏢 Supplier Portal
- **Dashboard Analytics**: Real-time insights into product performance and orders.
- **Product Management Engine**: Advanced tier-based limit validation (e.g., max 10 free products, auto-upgrades).
- **Media Hosting**: Secure Azure / Supabase integration for seamless image uploads.

### 🛒 Client Marketplace
- **B2B Procurement**: Streamlined buying flows for enterprise clients.
- **Smart Search & Filters**: Efficient product discovery across categories.

### 🔐 Security & Architecture
- **Clean Architecture**: Strictly layered .NET backend (Controllers -> Services -> Repositories -> Entities).
- **Firebase Auth**: Robust login flows with Google Sign-in and OTP email validation.
- **JWT Authorization**: Fine-grained role-based access control (Admin, Supplier, Client).

### 🏗️ System Architecture

```mermaid
graph TD
    subgraph Frontend [React Vite SPA]
        A[Supplier Dashboard]
        B[Client Marketplace]
        C[Admin Panel]
    end

    subgraph API [ASP.NET Core Web API]
        D[Controllers/Endpoints]
        E[Services / Business Logic]
        F[Repositories / Data Access]
    end

    subgraph Database & Cloud
        G[(PostgreSQL)]
        H[Firebase Auth]
        I[Azure / Supabase Storage]
    end

    A -->|HTTPS / REST| D
    B -->|HTTPS / REST| D
    C -->|HTTPS / REST| D

    D --> E
    E --> F
    F --> G

    E -->|Verify Token| H
    E -->|Upload Media| I
```


---

## 🛠️ Technology Stack

**Backend:**
- C# / .NET 8 Web API
- Entity Framework Core
- PostgreSQL

**Frontend:**
- React (Vite)
- Tailwind CSS
- Axios

**Cloud & Infrastructure:**
- Firebase (Authentication)
- Supabase / Azure (Blob Storage)
- MailKit / SMTP

---

## 💼 Commercial Licensing & Full Source Code

This project is a premium enterprise-grade solution. If you are interested in purchasing the full source code (with all business logic implementations), hiring me to customize this platform for your business, or discussing job opportunities, please get in touch:

📫 **Contact me on GitHub or via email (Please check my GitHub profile for contact details).**

---

<div align="center">
  <p><i>Designed & Developed with ❤️</i></p>
</div>
