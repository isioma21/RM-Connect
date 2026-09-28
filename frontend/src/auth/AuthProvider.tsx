import { useEffect, useState, type ReactNode } from 'react'
import { Box, CircularProgress } from '@mui/material'
import { authApi } from '../api/auth'
import type { User } from '../api/types'
import { AuthContext } from './AuthContext'

/** Loads the logged-in user on start and shares it. */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    authApi.currentUser()
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setLoading(false))
  }, [])

  async function login(email: string, password: string) {
    const loggedIn = await authApi.login(email, password)
    setUser(loggedIn)
    return loggedIn
  }

  async function logout() {
    await authApi.logout().catch(() => {})
    setUser(null)
  }

  if (loading) {
    return (
      <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center' }}>
        <CircularProgress aria-label="Loading" />
      </Box>
    )
  }

  return <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>
}
