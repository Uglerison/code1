 Controle Financeiro

Aplicação web para controle financeiro pessoal desenvolvida com ASP.NET Core (backend) e Node.js (frontend).

## 🚀 Tecnologias

- **Backend:**
  - ASP.NET Core
  - Entity Framework Core
  - SQLite
  - Swagger

- **Frontend:**
  - Node.js
  - Express.js
  - HTML5
  - CSS3 (Design minimalista com tons pastéis)
  - JavaScript vanilla

## 📦 Estrutura do Projeto

```plaintext
├── backend/               # API em ASP.NET Core
│   └── FinanceControl.API/
│       ├── Controllers/
│       ├── Models/
│       ├── Data/
│       └── ...
└── frontend/             # Aplicação Node.js
    ├── public/
    │   ├── css/
    │   ├── js/
    │   └── index.html
    └── server.js
```

## 🛠️ Como Executar

### Backend

```bash
cd backend/FinanceControl.API
dotnet restore
dotnet run
```

O backend estará disponível em `http://localhost:5000`

### Frontend

```bash
cd frontend
npm install
npm start
```

O frontend estará disponível em `http://localhost:3000`

## 📋 Funcionalidades

- Cadastro de gastos
- Listagem de gastos
- Interface responsiva e intuitiva
- Design minimalista em tons pastéis de azul

## 🔜 Próximas Funcionalidades

- [ ] Categorização de gastos
- [ ] Filtros e busca
- [ ] Gráficos e relatórios
- [ ] Autenticação de usuários
- [ ] Edição e exclusão de gastos