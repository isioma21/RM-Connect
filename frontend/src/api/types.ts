export type Role = 'Customer' | 'RelationshipManager'

export type User = {
  id: string
  firstName: string
  lastName: string
  email: string
  phone: string | null
  branch: string | null
  role: Role
}

export type RelationshipStatus = 'Pending' | 'Active' | 'Declined' | 'Ended'

export type Relationship = {
  id: string
  status: RelationshipStatus
  requestedAt: string
  respondedAt: string | null
  endedAt: string | null
  endReason: string | null
  customer: User
  manager: User
}

export type AppointmentChannel = 'Call' | 'BranchVisit'
export type AppointmentStatus = 'Booked' | 'Cancelled' | 'Completed'

export type Appointment = {
  id: string
  startsAt: string
  channel: AppointmentChannel
  reason: string
  status: AppointmentStatus
  cancellationReason: string | null
  customer: User
  manager: User
}

export type BookAppointmentRequest = {
  startsAt: string
  channel: AppointmentChannel
  reason: string
}

export type ChangePasswordRequest = {
  currentPassword: string
  newPassword: string
}

export type RegisterCustomerRequest = {
  firstName: string
  lastName: string
  email: string
  phone: string
  password: string
}

export type RegisterManagerRequest = {
  firstName: string
  lastName: string
  workEmail: string
  branch: string
  password: string
}

export type Session = {
  id: string
  device: string
  ipAddress: string
  signedInAt: string
  lastSeenAt: string
  isCurrent: boolean
}
