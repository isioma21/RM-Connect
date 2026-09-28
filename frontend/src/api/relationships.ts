import { api } from './client'
import type { Relationship, User } from './types'

export const relationshipsApi = {
  managers: () => api<User[]>('GET', '/relationships/managers'),
  current: () => api<Relationship | undefined>('GET', '/relationships/current'),
  request: (managerId: string) => api<Relationship>('POST', '/relationships', { managerId }),
  end: (id: string) => api<Relationship>('POST', `/relationships/${id}/end`),

  // Relationship manager
  forManager: () => api<Relationship[]>('GET', '/relationships'),
  accept: (id: string) => api<Relationship>('POST', `/relationships/${id}/accept`),
  decline: (id: string) => api<Relationship>('POST', `/relationships/${id}/decline`),
}
