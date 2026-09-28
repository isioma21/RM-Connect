import { api } from './client'
import type { Session } from './types'

export const sessionsApi = {
  mine: () => api<Session[]>('GET', '/sessions'),
  signOut: (id: string) => api<void>('DELETE', `/sessions/${id}`),
  signOutOthers: () => api<void>('POST', '/sessions/revoke-others'),
}
