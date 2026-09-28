import { api } from './client'
import type { Appointment, BookAppointmentRequest } from './types'

export const appointmentsApi = {
  mine: () => api<Appointment[]>('GET', '/appointments'),
  freeSlots: (date: string) => api<string[]>('GET', `/appointments/slots?date=${date}`),
  book: (request: BookAppointmentRequest) => api<Appointment>('POST', '/appointments', request),
  cancel: (id: string) => api<Appointment>('POST', `/appointments/${id}/cancel`),
  complete: (id: string) => api<Appointment>('POST', `/appointments/${id}/complete`),
}
