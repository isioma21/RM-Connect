import { Navigate, Route, Routes } from 'react-router-dom'
import { homePath, useAuth } from './auth/AuthContext.ts'
import { RequireRole } from './auth/RequireRole.tsx'
import { Layout } from './components/Layout.tsx'
import { LoginPage } from './pages/LoginPage.tsx'
import { RegisterPage } from './pages/RegisterPage.tsx'
import { BookAppointmentPage } from './pages/customer/BookAppointmentPage.tsx'
import { CustomerHomePage } from './pages/customer/CustomerHomePage.tsx'
import { ManagerHomePage } from './pages/manager/ManagerHomePage.tsx'
import { ManagerSchedulePage } from './pages/manager/ManagerSchedulePage.tsx'

export default function App() {
  const { user } = useAuth()

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<RequireRole role="Customer"><Layout /></RequireRole>}>
        <Route path="/customer" element={<CustomerHomePage />} />
        <Route path="/customer/book" element={<BookAppointmentPage />} />
      </Route>

      <Route element={<RequireRole role="RelationshipManager"><Layout /></RequireRole>}>
        <Route path="/manager" element={<ManagerHomePage />} />
        <Route path="/manager/schedule" element={<ManagerSchedulePage />} />
      </Route>

      <Route path="*" element={<Navigate to={user ? homePath(user) : '/login'} replace />} />
    </Routes>
  )
}
