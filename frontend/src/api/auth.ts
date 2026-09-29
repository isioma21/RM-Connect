import { api } from './client'
import type { ChangePasswordRequest, RegisterCustomerRequest, RegisterManagerRequest, User } from './types'

export const authApi = {
  currentUser: () => api<User>('GET', '/auth/current-user'),
  login: (email: string, password: string) => api<User>('POST', '/auth/login', { email, password }),
  changePassword: (request: ChangePasswordRequest) => api<void>('POST', '/auth/change-password', request),
  logout: () => api<void>('POST', '/auth/logout'),
  registerCustomer: (request: RegisterCustomerRequest) => api<User>('POST', '/auth/register/customer', request),
  registerManager: (request: RegisterManagerRequest) => api<User>('POST', '/auth/register/manager', request),
}
