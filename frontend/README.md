# RM Connect – frontend

React + TypeScript (Vite) with MUI and React Router.

## Run it

1. Start the API (from the repo root): `dotnet run --project src/RmConnect.Api --launch-profile http`
2. Start the frontend:

   ```
   cd frontend
   npm install
   npm run dev
   ```

3. Open http://localhost:5173

In development, Vite forwards `/api` calls to the API on `http://localhost:5126` (set `API_URL` to change it),
so the login cookie works without any CORS setup.

## Structure

- `src/api` – calls to the API (`client.ts` sends the login cookie and turns errors into `ApiError`)
- `src/auth` – the logged-in user (`useAuth`) and role-protected routes (`RequireRole`)
- `src/components` – shared pieces such as the top bar (`Layout`)
- `src/pages` – one file per screen
