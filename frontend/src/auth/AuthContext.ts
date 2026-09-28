import { createContext, useContext } from 'react'
import type { User } from '../api/types'

export type AuthContextValue = {
  user: User | null
  login: (email: string, password: string) => Promise<User>
  logout: () => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)

/** Current user plus login and logout. */
export function useAuth() {
  const auth = useContext(AuthContext)
  if (!auth) throw new Error('useAuth must be used inside AuthProvider')
  return auth
}

/** Home page for the user's role. */
export function homePath(user: User) {
  return user.role === 'Customer' ? '/customer' : '/manager'
}
